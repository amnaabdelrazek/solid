namespace Solid.Api.Infrastructure.Auth;

public static class AuthRouteGroupExtensions
{
    public static RouteGroupBuilder RequireLaravelSanctum(this RouteGroupBuilder group)
    {
        group.RequireAuthorization();

        return group;
    }
}
