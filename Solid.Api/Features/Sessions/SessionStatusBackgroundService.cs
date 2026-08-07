using Microsoft.EntityFrameworkCore;
using Solid.Api.Database;
using Solid.Api.Database.Entities;
using Solid.Api.Features.Notifications;
using Solid.Api.Database.Repositories;

namespace Solid.Api.Features.Sessions;

public class SessionStatusBackgroundService(
    IServiceProvider serviceProvider,
    ILogger<SessionStatusBackgroundService> logger) : BackgroundService
{
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        logger.LogInformation("SessionStatusBackgroundService started.");

        using var timer = new PeriodicTimer(TimeSpan.FromMinutes(1));

        while (!stoppingToken.IsCancellationRequested && await timer.WaitForNextTickAsync(stoppingToken))
        {
            try
            {
                await ProcessSessionsAsync(stoppingToken);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error occurred processing sessions auto-start/auto-end.");
            }
        }
    }

    private async Task ProcessSessionsAsync(CancellationToken stoppingToken)
    {
        using var scope = serviceProvider.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<SolidDbContext>();
        var notificationService = scope.ServiceProvider.GetRequiredService<INotificationService>();
        var sessionRepository = scope.ServiceProvider.GetRequiredService<ISessionRepository>();

        var now = DateTime.UtcNow;

        // 1. Auto-Start: find scheduled sessions whose ScheduledAt <= now, and transition to live
        var sessionsToStart = await dbContext.TherapySessions
            .Where(s => s.DeletedAt == null && s.Status == "scheduled" && s.ScheduledAt <= now)
            .ToListAsync(stoppingToken);

        if (sessionsToStart.Count > 0)
        {
            logger.LogInformation("Auto-starting {Count} sessions.", sessionsToStart.Count);
            foreach (var session in sessionsToStart)
            {
                session.Status = "live";
                session.StartedAt = now;
                session.UpdatedAt = now;

                try
                {
                    var recipients = await sessionRepository.UserIdsForSubstanceCategoryAsync(session.SubstanceCategoryId);

                    await notificationService.NotifyUsersAsync(
                        recipients,
                        "SessionStarted",
                        "Session started",
                        $"Session #{session.SessionNumber ?? session.Id} has started.",
                        "video",
                        new
                        {
                            session_id = session.Id,
                            substance_category_id = session.SubstanceCategoryId
                        });
                }
                catch (Exception ex)
                {
                    logger.LogError(ex, "Failed to send auto-start notifications for session {SessionId}", session.Id);
                }
            }
            await dbContext.SaveChangesAsync(stoppingToken);
        }

        // 2. Auto-End: find live sessions that have exceeded their duration
        var liveSessions = await dbContext.TherapySessions
            .Where(s => s.DeletedAt == null && s.Status == "live")
            .ToListAsync(stoppingToken);

        var sessionsToEnd = liveSessions
            .Where(s => s.StartedAt.HasValue && s.StartedAt.Value.AddMinutes(s.DurationMinutes) <= now)
            .ToList();

        if (sessionsToEnd.Count > 0)
        {
            logger.LogInformation("Auto-ending {Count} sessions.", sessionsToEnd.Count);
            foreach (var session in sessionsToEnd)
            {
                session.Status = "finished";
                session.EndedAt = session.StartedAt!.Value.AddMinutes(session.DurationMinutes);
                session.UpdatedAt = now;
            }
            await dbContext.SaveChangesAsync(stoppingToken);
        }
    }
}
