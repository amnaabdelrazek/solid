namespace Solid.Api.Infrastructure.Sms;

public interface ISmsSender
{
    Task SendAsync(string to, string message);
}
