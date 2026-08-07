using Solid.Api.Common;
using Solid.Api.Database.Repositories;
using Solid.Api.Features.Shared;
using Solid.Api.Features.Users;
using Solid.Api.Infrastructure.Auth;

namespace Solid.Api.Features.Upload;

public static class UploadSlice
{
    private static readonly string[] AllowedExtensions = [".jpg", ".jpeg", ".png", ".webp", ".gif"];
    private const long MaxFileSizeBytes = 5 * 1024 * 1024; // 5 MB

    public static IEndpointRouteBuilder MapUploadSlice(this IEndpointRouteBuilder api)
    {
        api.MapPost("/upload/image", UploadImage)
            .DisableAntiforgery();

        api.MapPost("/profile/avatar", UploadAvatar)
            .DisableAntiforgery();

        return api;
    }

    private static async Task<IResult> UploadImage(
        HttpRequest request,
        IWebHostEnvironment environment)
    {
        if (!request.HasFormContentType)
        {
            return ApiResponse.Fail("Expected multipart/form-data request.", StatusCodes.Status400BadRequest);
        }

        var form = await request.ReadFormAsync();
        var file = form.Files.GetFile("file") ?? form.Files.FirstOrDefault();

        if (file is null || file.Length == 0)
        {
            return ApiResponse.Fail("No image file provided.", StatusCodes.Status422UnprocessableEntity);
        }

        if (file.Length > MaxFileSizeBytes)
        {
            return ApiResponse.Fail("File size exceeds maximum allowed limit of 5MB.", StatusCodes.Status422UnprocessableEntity);
        }

        var extension = Path.GetExtension(file.FileName).ToLowerInvariant();
        if (!AllowedExtensions.Contains(extension))
        {
            return ApiResponse.Fail("Invalid image file format. Allowed formats: .jpg, .jpeg, .png, .webp, .gif", StatusCodes.Status422UnprocessableEntity);
        }

        var webRoot = environment.WebRootPath ?? Path.Combine(environment.ContentRootPath, "wwwroot");
        var uploadsFolder = Path.Combine(webRoot, "uploads", "images");
        Directory.CreateDirectory(uploadsFolder);

        var uniqueFileName = $"{Guid.NewGuid():N}{extension}";
        var filePath = Path.Combine(uploadsFolder, uniqueFileName);

        await using (var stream = new FileStream(filePath, FileMode.Create))
        {
            await file.CopyToAsync(stream);
        }

        var relativeUrl = $"/uploads/images/{uniqueFileName}";
        return ApiResponse.Ok(new { url = relativeUrl }, "Image uploaded successfully.");
    }

    private static async Task<IResult> UploadAvatar(
        IAuthContext auth,
        HttpRequest request,
        IWebHostEnvironment environment,
        IUserRepository userRepository)
    {
        if (!request.HasFormContentType)
        {
            return ApiResponse.Fail("Expected multipart/form-data request.", StatusCodes.Status400BadRequest);
        }

        var form = await request.ReadFormAsync();
        var file = form.Files.GetFile("file") ?? form.Files.GetFile("avatar") ?? form.Files.FirstOrDefault();

        if (file is null || file.Length == 0)
        {
            return ApiResponse.Fail("No avatar file provided.", StatusCodes.Status422UnprocessableEntity);
        }

        if (file.Length > MaxFileSizeBytes)
        {
            return ApiResponse.Fail("File size exceeds maximum allowed limit of 5MB.", StatusCodes.Status422UnprocessableEntity);
        }

        var extension = Path.GetExtension(file.FileName).ToLowerInvariant();
        if (!AllowedExtensions.Contains(extension))
        {
            return ApiResponse.Fail("Invalid image file format. Allowed formats: .jpg, .jpeg, .png, .webp, .gif", StatusCodes.Status422UnprocessableEntity);
        }

        var webRoot = environment.WebRootPath ?? Path.Combine(environment.ContentRootPath, "wwwroot");
        var uploadsFolder = Path.Combine(webRoot, "uploads", "avatars");
        Directory.CreateDirectory(uploadsFolder);

        var uniqueFileName = $"{Guid.NewGuid():N}{extension}";
        var filePath = Path.Combine(uploadsFolder, uniqueFileName);

        await using (var stream = new FileStream(filePath, FileMode.Create))
        {
            await file.CopyToAsync(stream);
        }

        var relativeUrl = $"/uploads/avatars/{uniqueFileName}";

        await userRepository.UpdateProfileAsync(
            auth.UserId,
            new ProfileUpdate(null, null, null, null, relativeUrl));

        var updatedUser = await userRepository.FindAsync(auth.UserId);

        return ApiResponse.Ok(new
        {
            avatar_url = relativeUrl,
            user = updatedUser is not null ? UserResource.From(updatedUser) : null
        }, "Avatar uploaded and profile updated successfully.");
    }
}
