namespace Solid.Api.Infrastructure.Auth;

public static class AuthRoles
{
    public const string Admin = "admin";

    public const string Instructor = "instructor";

    public static bool IsAdmin(this IAuthContext auth)
    {
        return string.Equals(auth.Role, Admin, StringComparison.OrdinalIgnoreCase);
    }

    public static bool IsInstructor(this IAuthContext auth)
    {
        return string.Equals(auth.Role, Instructor, StringComparison.OrdinalIgnoreCase);
    }

    public static bool IsAdminOrInstructor(this IAuthContext auth)
    {
        return auth.IsAdmin() || auth.IsInstructor();
    }
}
