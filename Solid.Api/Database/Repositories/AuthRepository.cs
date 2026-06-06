using Microsoft.EntityFrameworkCore;
using Solid.Api.Common;
using Solid.Api.Database.Entities;

namespace Solid.Api.Database.Repositories;

public sealed class AuthRepository(SolidDbContext dbContext) : IAuthRepository
{
    public async Task<User?> FindUserByIdAsync(long userId)
    {
        return await dbContext.Users
            .AsNoTracking()
            .FirstOrDefaultAsync(user => user.Id == userId && user.DeletedAt == null);
    }

    public async Task<User?> FindUserByMobileAsync(string mobileNumber, bool onlyActive = false)
    {
        var candidates = PhoneNumberValidator.SearchCandidates(mobileNumber);
        var query = dbContext.Users
            .AsNoTracking()
            .Where(user => user.MobileNumber != null && candidates.Contains(user.MobileNumber) && user.DeletedAt == null);

        if (onlyActive)
        {
            query = query.Where(user => user.IsActive);
        }

        return await query.FirstOrDefaultAsync();
    }

    public async Task<User> CreateInactiveUserAsync(AuthUserCreate create)
    {
        var now = DateTime.UtcNow;
        var user = new User
        {
            DisplayName = create.DisplayName,
            MobileNumber = create.MobileNumber,
            Password = create.PasswordHash,
            Role = "addict",
            PreferredLanguage = create.PreferredLanguage,
            IsActive = false,
            CreatedAt = now,
            UpdatedAt = now
        };

        dbContext.Users.Add(user);
        await dbContext.SaveChangesAsync();

        await CompleteRegistrationDetailsAsync(user.Id, create);

        return (await FindUserByIdAsync(user.Id))!;
    }

    public async Task<bool> HasAddictionProfileAsync(long userId)
    {
        return await dbContext.AddictionProfiles.AnyAsync(profile => profile.UserId == userId);
    }

    public async Task CompleteRegistrationDetailsAsync(long userId, AuthUserCreate create)
    {
        var now = DateTime.UtcNow;

        if (!await HasAddictionProfileAsync(userId))
        {
            dbContext.AddictionProfiles.Add(new AddictionProfile
            {
                UserId = userId,
                AddictionDurationId = create.AddictionDurationId,
                EducationLevelId = create.EducationLevelId,
                HadPriorTreatment = create.HadPriorTreatment,
                AddictionReason = create.AddictionReason,
                DaysClean = create.DaysClean,
                CreatedAt = now,
                UpdatedAt = now
            });
        }

        var existingSubstanceIds = await dbContext.UserSubstances
            .Where(substance => substance.UserId == userId)
            .Select(substance => substance.SubstanceId)
            .ToListAsync();

        foreach (var substanceId in create.SubstanceIds.Distinct().Except(existingSubstanceIds))
        {
            dbContext.UserSubstances.Add(new UserSubstance
            {
                UserId = userId,
                SubstanceId = substanceId,
                CreatedAt = now
            });
        }

        var existingTreatmentTypeIds = await dbContext.UserTreatmentTypes
            .Where(treatmentType => treatmentType.UserId == userId)
            .Select(treatmentType => treatmentType.LookupValueId)
            .ToListAsync();

        foreach (var treatmentTypeId in create.TreatmentTypeIds.Distinct().Except(existingTreatmentTypeIds))
        {
            dbContext.UserTreatmentTypes.Add(new UserTreatmentType
            {
                UserId = userId,
                LookupValueId = treatmentTypeId
            });
        }

        await dbContext.SaveChangesAsync();
    }

    public async Task ActivateUserAsync(long userId)
    {
        var user = await dbContext.Users.FirstOrDefaultAsync(user => user.Id == userId);
        if (user is null)
        {
            return;
        }

        user.IsActive = true;
        user.UpdatedAt = DateTime.UtcNow;

        await dbContext.SaveChangesAsync();
    }

    public async Task RecordLoginAsync(long userId, string deviceId)
    {
        var now = DateTime.UtcNow;
        var user = await dbContext.Users.FirstOrDefaultAsync(user => user.Id == userId);
        if (user is null)
        {
            return;
        }

        user.ActiveDeviceId = deviceId;
        user.UpdatedAt = now;

        dbContext.DeviceSessions.Add(new DeviceSession
        {
            UserId = userId,
            DeviceId = deviceId,
            EventType = "login",
            CreatedAt = now
        });

        await dbContext.SaveChangesAsync();
    }

    public async Task ClearActiveDeviceAsync(long userId)
    {
        var user = await dbContext.Users.FirstOrDefaultAsync(user => user.Id == userId);
        if (user is null)
        {
            return;
        }

        user.ActiveDeviceId = null;
        user.UpdatedAt = DateTime.UtcNow;

        await dbContext.SaveChangesAsync();
    }

    public async Task UpdatePasswordAsync(long userId, string passwordHash)
    {
        var user = await dbContext.Users.FirstOrDefaultAsync(user => user.Id == userId);
        if (user is null)
        {
            return;
        }

        user.Password = passwordHash;
        user.UpdatedAt = DateTime.UtcNow;

        await dbContext.SaveChangesAsync();
    }

    public async Task DeactivateAccountAsync(long userId)
    {
        var user = await dbContext.Users.FirstOrDefaultAsync(user => user.Id == userId);
        if (user is null)
        {
            return;
        }

        var now = DateTime.UtcNow;
        user.FcmToken = null;
        user.ActiveDeviceId = null;
        user.IsActive = false;
        user.DeletedAt = now;
        user.UpdatedAt = now;

        await dbContext.SaveChangesAsync();
    }
}
