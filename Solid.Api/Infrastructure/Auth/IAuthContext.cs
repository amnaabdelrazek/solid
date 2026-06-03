namespace Solid.Api.Infrastructure.Auth;

public interface IAuthContext
{
    long UserId { get; }

    string? Role { get; }
}
