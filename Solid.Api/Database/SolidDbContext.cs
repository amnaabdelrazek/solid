using Microsoft.EntityFrameworkCore;
using Solid.Api.Database.Entities;

namespace Solid.Api.Database;

public sealed class SolidDbContext(DbContextOptions<SolidDbContext> options) : DbContext(options)
{
    public DbSet<User> Users => Set<User>();

    public DbSet<PersonalAccessToken> PersonalAccessTokens => Set<PersonalAccessToken>();

    public DbSet<AddictionProfile> AddictionProfiles => Set<AddictionProfile>();

    public DbSet<UserSubstance> UserSubstances => Set<UserSubstance>();

    public DbSet<UserTreatmentType> UserTreatmentTypes => Set<UserTreatmentType>();

    public DbSet<DeviceSession> DeviceSessions => Set<DeviceSession>();

    public DbSet<LookupType> LookupTypes => Set<LookupType>();

    public DbSet<LookupValue> LookupValues => Set<LookupValue>();

    public DbSet<SubstanceCategory> SubstanceCategories => Set<SubstanceCategory>();

    public DbSet<Substance> Substances => Set<Substance>();

    public DbSet<Group> Groups => Set<Group>();

    public DbSet<GroupMember> GroupMembers => Set<GroupMember>();

    public DbSet<TherapySession> TherapySessions => Set<TherapySession>();

    public DbSet<SessionAttendance> SessionAttendances => Set<SessionAttendance>();

    public DbSet<Payment> Payments => Set<Payment>();

    public DbSet<PaymentMethod> PaymentMethods => Set<PaymentMethod>();

    public DbSet<Recommendation> Recommendations => Set<Recommendation>();

    public DbSet<Setting> Settings => Set<Setting>();

    public DbSet<Notification> Notifications => Set<Notification>();

    public DbSet<CacheEntry> Cache => Set<CacheEntry>();

    public DbSet<CacheLock> CacheLocks => Set<CacheLock>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        ConfigureUsers(modelBuilder);
        ConfigureAuthTables(modelBuilder);
        ConfigureLookups(modelBuilder);
        ConfigureGroups(modelBuilder);
        ConfigureSessions(modelBuilder);
        ConfigurePayments(modelBuilder);
        ConfigureSettingsAndNotifications(modelBuilder);
        SeedInitialData(modelBuilder);
    }

    private static void ConfigureUsers(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<User>(entity =>
        {
            entity.ToTable("users");
            entity.HasKey(user => user.Id);
            entity.HasIndex(user => user.MobileNumber).IsUnique().HasFilter("[mobile_number] IS NOT NULL");
            entity.HasIndex(user => user.Email).IsUnique().HasFilter("[email] IS NOT NULL");
            entity.HasIndex(user => user.Username).IsUnique().HasFilter("[username] IS NOT NULL");

            entity.Property(user => user.Id).HasColumnName("id");
            entity.Property(user => user.DisplayName).HasColumnName("display_name").HasMaxLength(150);
            entity.Property(user => user.MobileNumber).HasColumnName("mobile_number").HasMaxLength(20);
            entity.Property(user => user.Email).HasColumnName("email").HasMaxLength(255);
            entity.Property(user => user.Username).HasColumnName("username").HasMaxLength(80);
            entity.Property(user => user.Password).HasColumnName("password");
            entity.Property(user => user.Role).HasColumnName("role").HasMaxLength(50);
            entity.Property(user => user.PreferredLanguage).HasColumnName("preferred_language").HasMaxLength(5).HasDefaultValue("ar");
            entity.Property(user => user.FcmToken).HasColumnName("fcm_token").HasMaxLength(512);
            entity.Property(user => user.ActiveDeviceId).HasColumnName("active_device_id").HasMaxLength(255);
            entity.Property(user => user.IsActive).HasColumnName("is_active").HasDefaultValue(true);
            entity.Property(user => user.EmailVerifiedAt).HasColumnName("email_verified_at");
            entity.Property(user => user.Bio).HasColumnName("bio");
            entity.Property(user => user.AvatarUrl).HasColumnName("avatar_url");
            entity.Property(user => user.Experience).HasColumnName("experience");
            entity.Property(user => user.Quote).HasColumnName("quote");
            entity.Property(user => user.RememberToken).HasColumnName("remember_token").HasMaxLength(100);
            entity.Property(user => user.DeletedAt).HasColumnName("deleted_at");
            entity.Property(user => user.CreatedAt).HasColumnName("created_at");
            entity.Property(user => user.UpdatedAt).HasColumnName("updated_at");
        });

        modelBuilder.Entity<AddictionProfile>(entity =>
        {
            entity.ToTable("addiction_profiles");
            entity.HasKey(profile => profile.Id);
            entity.HasIndex(profile => profile.UserId).IsUnique();

            entity.Property(profile => profile.Id).HasColumnName("id");
            entity.Property(profile => profile.UserId).HasColumnName("user_id");
            entity.Property(profile => profile.AddictionDurationId).HasColumnName("addiction_duration_id");
            entity.Property(profile => profile.EducationLevelId).HasColumnName("education_level_id");
            entity.Property(profile => profile.HadPriorTreatment).HasColumnName("had_prior_treatment").HasDefaultValue(false);
            entity.Property(profile => profile.AddictionReason).HasColumnName("addiction_reason");
            entity.Property(profile => profile.DaysClean).HasColumnName("days_clean");
            entity.Property(profile => profile.CreatedAt).HasColumnName("created_at");
            entity.Property(profile => profile.UpdatedAt).HasColumnName("updated_at");

            entity.HasOne(profile => profile.User)
                .WithOne(user => user.AddictionProfile)
                .HasForeignKey<AddictionProfile>(profile => profile.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasOne(profile => profile.AddictionDuration)
                .WithMany(value => value.AddictionDurationProfiles)
                .HasForeignKey(profile => profile.AddictionDurationId)
                .OnDelete(DeleteBehavior.NoAction);

            entity.HasOne(profile => profile.EducationLevel)
                .WithMany(value => value.EducationLevelProfiles)
                .HasForeignKey(profile => profile.EducationLevelId)
                .OnDelete(DeleteBehavior.NoAction);
        });

        modelBuilder.Entity<UserSubstance>(entity =>
        {
            entity.ToTable("user_substances");
            entity.HasKey(userSubstance => userSubstance.Id);
            entity.HasIndex(userSubstance => new { userSubstance.UserId, userSubstance.SubstanceId }).IsUnique();

            entity.Property(userSubstance => userSubstance.Id).HasColumnName("id");
            entity.Property(userSubstance => userSubstance.UserId).HasColumnName("user_id");
            entity.Property(userSubstance => userSubstance.SubstanceId).HasColumnName("substance_id");
            entity.Property(userSubstance => userSubstance.CreatedAt).HasColumnName("created_at").HasDefaultValueSql("SYSDATETIME()");

            entity.HasOne(userSubstance => userSubstance.User)
                .WithMany(user => user.UserSubstances)
                .HasForeignKey(userSubstance => userSubstance.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasOne(userSubstance => userSubstance.Substance)
                .WithMany(substance => substance.UserSubstances)
                .HasForeignKey(userSubstance => userSubstance.SubstanceId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        modelBuilder.Entity<UserTreatmentType>(entity =>
        {
            entity.ToTable("user_treatment_types");
            entity.HasKey(treatmentType => treatmentType.Id);
            entity.HasIndex(treatmentType => new { treatmentType.UserId, treatmentType.LookupValueId }).IsUnique();

            entity.Property(treatmentType => treatmentType.Id).HasColumnName("id");
            entity.Property(treatmentType => treatmentType.UserId).HasColumnName("user_id");
            entity.Property(treatmentType => treatmentType.LookupValueId).HasColumnName("lookup_value_id");

            entity.HasOne(treatmentType => treatmentType.User)
                .WithMany(user => user.UserTreatmentTypes)
                .HasForeignKey(treatmentType => treatmentType.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasOne(treatmentType => treatmentType.LookupValue)
                .WithMany()
                .HasForeignKey(treatmentType => treatmentType.LookupValueId)
                .OnDelete(DeleteBehavior.NoAction);
        });
    }

    private static void ConfigureAuthTables(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<PersonalAccessToken>(entity =>
        {
            entity.ToTable("personal_access_tokens");
            entity.HasKey(token => token.Id);
            entity.HasIndex(token => new { token.TokenableType, token.TokenableId });
            entity.HasIndex(token => token.Token).IsUnique();

            entity.Property(token => token.Id).HasColumnName("id");
            entity.Property(token => token.TokenableType).HasColumnName("tokenable_type");
            entity.Property(token => token.TokenableId).HasColumnName("tokenable_id");
            entity.Property(token => token.Name).HasColumnName("name");
            entity.Property(token => token.Token).HasColumnName("token").HasMaxLength(64);
            entity.Property(token => token.Abilities).HasColumnName("abilities");
            entity.Property(token => token.LastUsedAt).HasColumnName("last_used_at");
            entity.Property(token => token.ExpiresAt).HasColumnName("expires_at");
            entity.Property(token => token.CreatedAt).HasColumnName("created_at");
            entity.Property(token => token.UpdatedAt).HasColumnName("updated_at");
        });

        modelBuilder.Entity<DeviceSession>(entity =>
        {
            entity.ToTable("device_sessions");
            entity.HasKey(session => session.Id);
            entity.HasIndex(session => new { session.UserId, session.CreatedAt });

            entity.Property(session => session.Id).HasColumnName("id");
            entity.Property(session => session.UserId).HasColumnName("user_id");
            entity.Property(session => session.DeviceId).HasColumnName("device_id").HasMaxLength(255);
            entity.Property(session => session.DeviceInfo).HasColumnName("device_info");
            entity.Property(session => session.EventType).HasColumnName("event_type").HasMaxLength(50);
            entity.Property(session => session.SanctumTokenId).HasColumnName("sanctum_token_id");
            entity.Property(session => session.CreatedAt).HasColumnName("created_at").HasDefaultValueSql("SYSDATETIME()");

            entity.HasOne(session => session.User)
                .WithMany(user => user.DeviceSessions)
                .HasForeignKey(session => session.UserId)
                .OnDelete(DeleteBehavior.Cascade);
        });
    }

    private static void ConfigureLookups(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<LookupType>(entity =>
        {
            entity.ToTable("lookup_types");
            entity.HasKey(type => type.Id);
            entity.HasIndex(type => type.Key).IsUnique();

            entity.Property(type => type.Id).HasColumnName("id");
            entity.Property(type => type.Key).HasColumnName("key").HasMaxLength(80);
            entity.Property(type => type.LabelAr).HasColumnName("label_ar").HasMaxLength(150);
            entity.Property(type => type.LabelEn).HasColumnName("label_en").HasMaxLength(150);
            entity.Property(type => type.CreatedAt).HasColumnName("created_at");
            entity.Property(type => type.UpdatedAt).HasColumnName("updated_at");
        });

        modelBuilder.Entity<LookupValue>(entity =>
        {
            entity.ToTable("lookup_values");
            entity.HasKey(value => value.Id);

            entity.Property(value => value.Id).HasColumnName("id");
            entity.Property(value => value.LookupTypeId).HasColumnName("lookup_type_id");
            entity.Property(value => value.ValueKey).HasColumnName("value_key").HasMaxLength(80);
            entity.Property(value => value.LabelAr).HasColumnName("label_ar").HasMaxLength(200);
            entity.Property(value => value.LabelEn).HasColumnName("label_en").HasMaxLength(200);
            entity.Property(value => value.SortOrder).HasColumnName("sort_order").HasDefaultValue((byte)0);
            entity.Property(value => value.IsActive).HasColumnName("is_active").HasDefaultValue(true);
            entity.Property(value => value.CreatedAt).HasColumnName("created_at");
            entity.Property(value => value.UpdatedAt).HasColumnName("updated_at");

            entity.HasOne(value => value.LookupType)
                .WithMany(type => type.Values)
                .HasForeignKey(value => value.LookupTypeId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        modelBuilder.Entity<SubstanceCategory>(entity =>
        {
            entity.ToTable("substance_categories");
            entity.HasKey(category => category.Id);
            entity.HasIndex(category => category.Slug).IsUnique();

            entity.Property(category => category.Id).HasColumnName("id");
            entity.Property(category => category.NameAr).HasColumnName("name_ar").HasMaxLength(150);
            entity.Property(category => category.NameEn).HasColumnName("name_en").HasMaxLength(150);
            entity.Property(category => category.Slug).HasColumnName("slug").HasMaxLength(80);
            entity.Property(category => category.IsActive).HasColumnName("is_active").HasDefaultValue(true);
            entity.Property(category => category.SortOrder).HasColumnName("sort_order").HasDefaultValue((byte)0);
            entity.Property(category => category.CreatedAt).HasColumnName("created_at");
            entity.Property(category => category.UpdatedAt).HasColumnName("updated_at");
        });

        modelBuilder.Entity<Substance>(entity =>
        {
            entity.ToTable("substances");
            entity.HasKey(substance => substance.Id);

            entity.Property(substance => substance.Id).HasColumnName("id");
            entity.Property(substance => substance.SubstanceCategoryId).HasColumnName("substance_category_id");
            entity.Property(substance => substance.NameAr).HasColumnName("name_ar").HasMaxLength(100);
            entity.Property(substance => substance.NameEn).HasColumnName("name_en").HasMaxLength(100);
            entity.Property(substance => substance.IsActive).HasColumnName("is_active").HasDefaultValue(true);
            entity.Property(substance => substance.CreatedAt).HasColumnName("created_at");
            entity.Property(substance => substance.UpdatedAt).HasColumnName("updated_at");

            entity.HasOne(substance => substance.SubstanceCategory)
                .WithMany(category => category.Substances)
                .HasForeignKey(substance => substance.SubstanceCategoryId)
                .OnDelete(DeleteBehavior.Cascade);
        });
    }

    private static void ConfigureGroups(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Group>(entity =>
        {
            entity.ToTable("groups");
            entity.HasKey(group => group.Id);

            entity.Property(group => group.Id).HasColumnName("id");
            entity.Property(group => group.InstructorId).HasColumnName("instructor_id");
            entity.Property(group => group.SubstanceCategoryId).HasColumnName("substance_category_id");
            entity.Property(group => group.GroupType).HasColumnName("group_type").HasMaxLength(50);
            entity.Property(group => group.Status).HasColumnName("status").HasMaxLength(50);
            entity.Property(group => group.NameAr).HasColumnName("name_ar").HasMaxLength(200);
            entity.Property(group => group.NameEn).HasColumnName("name_en").HasMaxLength(200);
            entity.Property(group => group.MinMembers).HasColumnName("min_members").HasDefaultValue((byte)7);
            entity.Property(group => group.MaxMembers).HasColumnName("max_members").HasDefaultValue((byte)15);
            entity.Property(group => group.DeletedAt).HasColumnName("deleted_at");
            entity.Property(group => group.CreatedAt).HasColumnName("created_at");
            entity.Property(group => group.UpdatedAt).HasColumnName("updated_at");

            entity.HasOne(group => group.Instructor)
                .WithMany()
                .HasForeignKey(group => group.InstructorId)
                .OnDelete(DeleteBehavior.SetNull);

            entity.HasOne(group => group.SubstanceCategory)
                .WithMany(category => category.Groups)
                .HasForeignKey(group => group.SubstanceCategoryId)
                .OnDelete(DeleteBehavior.SetNull);
        });

        modelBuilder.Entity<GroupMember>(entity =>
        {
            entity.ToTable("group_members");
            entity.HasKey(member => member.Id);
            entity.HasIndex(member => new { member.GroupId, member.UserId }).IsUnique();

            entity.Property(member => member.Id).HasColumnName("id");
            entity.Property(member => member.GroupId).HasColumnName("group_id");
            entity.Property(member => member.UserId).HasColumnName("user_id");
            entity.Property(member => member.JoinedAt).HasColumnName("joined_at").HasDefaultValueSql("SYSDATETIME()");
            entity.Property(member => member.IsActive).HasColumnName("is_active").HasDefaultValue(true);

            entity.HasOne(member => member.Group)
                .WithMany(group => group.Members)
                .HasForeignKey(member => member.GroupId)
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasOne(member => member.User)
                .WithMany(user => user.GroupMembers)
                .HasForeignKey(member => member.UserId)
                .OnDelete(DeleteBehavior.Cascade);
        });
    }

    private static void ConfigureSessions(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<TherapySession>(entity =>
        {
            entity.ToTable("therapy_sessions");
            entity.HasKey(session => session.Id);
            entity.HasIndex(session => session.JitsiRoomName).IsUnique();

            entity.Property(session => session.Id).HasColumnName("id");
            entity.Property(session => session.GroupId).HasColumnName("group_id");
            entity.Property(session => session.InstructorId).HasColumnName("instructor_id");
            entity.Property(session => session.SessionNumber).HasColumnName("session_number");
            entity.Property(session => session.SessionType).HasColumnName("session_type").HasMaxLength(50);
            entity.Property(session => session.Status).HasColumnName("status").HasMaxLength(50);
            entity.Property(session => session.ScheduledAt).HasColumnName("scheduled_at");
            entity.Property(session => session.StartedAt).HasColumnName("started_at");
            entity.Property(session => session.EndedAt).HasColumnName("ended_at");
            entity.Property(session => session.DurationMinutes).HasColumnName("duration_minutes").HasDefaultValue((byte)45);
            entity.Property(session => session.JitsiRoomName).HasColumnName("jitsi_room_name").HasMaxLength(255);
            entity.Property(session => session.JitsiJwtIssuedAt).HasColumnName("jitsi_jwt_issued_at");
            entity.Property(session => session.SessionMetadata).HasColumnName("session_metadata");
            entity.Property(session => session.DeletedAt).HasColumnName("deleted_at");
            entity.Property(session => session.CreatedAt).HasColumnName("created_at");
            entity.Property(session => session.UpdatedAt).HasColumnName("updated_at");

            entity.HasOne(session => session.Group)
                .WithMany(group => group.Sessions)
                .HasForeignKey(session => session.GroupId)
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasOne(session => session.Instructor)
                .WithMany(user => user.InstructorSessions)
                .HasForeignKey(session => session.InstructorId)
                .OnDelete(DeleteBehavior.NoAction);
        });

        modelBuilder.Entity<SessionAttendance>(entity =>
        {
            entity.ToTable("session_attendances");
            entity.HasKey(attendance => attendance.Id);
            entity.HasIndex(attendance => new { attendance.SessionId, attendance.UserId }).IsUnique();

            entity.Property(attendance => attendance.Id).HasColumnName("id");
            entity.Property(attendance => attendance.SessionId).HasColumnName("session_id");
            entity.Property(attendance => attendance.UserId).HasColumnName("user_id");
            entity.Property(attendance => attendance.JoinedAt).HasColumnName("joined_at");
            entity.Property(attendance => attendance.LeftAt).HasColumnName("left_at");
            entity.Property(attendance => attendance.WasPresent).HasColumnName("was_present").HasDefaultValue(false);
            entity.Property(attendance => attendance.Rating).HasColumnName("rating");
            entity.Property(attendance => attendance.Comment).HasColumnName("comment");
            entity.Property(attendance => attendance.CreatedAt).HasColumnName("created_at");
            entity.Property(attendance => attendance.UpdatedAt).HasColumnName("updated_at");

            entity.HasOne(attendance => attendance.Session)
                .WithMany(session => session.Attendances)
                .HasForeignKey(attendance => attendance.SessionId)
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasOne(attendance => attendance.User)
                .WithMany(user => user.SessionAttendances)
                .HasForeignKey(attendance => attendance.UserId)
                .OnDelete(DeleteBehavior.NoAction);
        });
    }

    private static void ConfigurePayments(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Payment>(entity =>
        {
            entity.ToTable("payments");
            entity.HasKey(payment => payment.Id);
            entity.HasIndex(payment => payment.GatewayTransactionId).IsUnique().HasFilter("[gateway_transaction_id] IS NOT NULL");

            entity.Property(payment => payment.Id).HasColumnName("id");
            entity.Property(payment => payment.UserId).HasColumnName("user_id");
            entity.Property(payment => payment.SessionId).HasColumnName("session_id");
            entity.Property(payment => payment.Amount).HasColumnName("amount").HasPrecision(10, 2);
            entity.Property(payment => payment.Currency).HasColumnName("currency").HasMaxLength(5).HasDefaultValue("EGP");
            entity.Property(payment => payment.Status).HasColumnName("status").HasMaxLength(50);
            entity.Property(payment => payment.Gateway).HasColumnName("gateway").HasMaxLength(50);
            entity.Property(payment => payment.GatewayTransactionId).HasColumnName("gateway_transaction_id").HasMaxLength(255);
            entity.Property(payment => payment.GatewayResponse).HasColumnName("gateway_response");
            entity.Property(payment => payment.PaidAt).HasColumnName("paid_at");
            entity.Property(payment => payment.CreatedAt).HasColumnName("created_at");
            entity.Property(payment => payment.UpdatedAt).HasColumnName("updated_at");

            entity.HasOne(payment => payment.User)
                .WithMany(user => user.Payments)
                .HasForeignKey(payment => payment.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasOne(payment => payment.Session)
                .WithMany(session => session.Payments)
                .HasForeignKey(payment => payment.SessionId)
                .OnDelete(DeleteBehavior.SetNull);
        });

        modelBuilder.Entity<PaymentMethod>(entity =>
        {
            entity.ToTable("payment_methods");
            entity.HasKey(method => method.Id);

            entity.Property(method => method.Id).HasColumnName("id");
            entity.Property(method => method.UserId).HasColumnName("user_id");
            entity.Property(method => method.CardHolder).HasColumnName("card_holder").HasMaxLength(255);
            entity.Property(method => method.CardType).HasColumnName("card_type");
            entity.Property(method => method.CardNumber).HasColumnName("card_number");
            entity.Property(method => method.Expiry).HasColumnName("expiry");
            entity.Property(method => method.IsDefault).HasColumnName("is_default").HasDefaultValue(false);
            entity.Property(method => method.GatewayToken).HasColumnName("gateway_token");
            entity.Property(method => method.CreatedAt).HasColumnName("created_at");
            entity.Property(method => method.UpdatedAt).HasColumnName("updated_at");

            entity.HasOne(method => method.User)
                .WithMany(user => user.PaymentMethods)
                .HasForeignKey(method => method.UserId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        modelBuilder.Entity<Recommendation>(entity =>
        {
            entity.ToTable("recommendations");
            entity.HasKey(recommendation => recommendation.Id);

            entity.Property(recommendation => recommendation.Id).HasColumnName("id");
            entity.Property(recommendation => recommendation.SubstanceCategoryId).HasColumnName("substance_category_id");
            entity.Property(recommendation => recommendation.Type).HasColumnName("type").HasMaxLength(50);
            entity.Property(recommendation => recommendation.NameAr).HasColumnName("name_ar").HasMaxLength(255);
            entity.Property(recommendation => recommendation.NameEn).HasColumnName("name_en").HasMaxLength(255);
            entity.Property(recommendation => recommendation.ContactInfo).HasColumnName("contact_info").HasMaxLength(500);
            entity.Property(recommendation => recommendation.Latitude).HasColumnName("latitude").HasPrecision(10, 7);
            entity.Property(recommendation => recommendation.Longitude).HasColumnName("longitude").HasPrecision(10, 7);
            entity.Property(recommendation => recommendation.IsActive).HasColumnName("is_active").HasDefaultValue(true);
            entity.Property(recommendation => recommendation.CreatedAt).HasColumnName("created_at");
            entity.Property(recommendation => recommendation.UpdatedAt).HasColumnName("updated_at");

            entity.HasOne(recommendation => recommendation.SubstanceCategory)
                .WithMany(category => category.Recommendations)
                .HasForeignKey(recommendation => recommendation.SubstanceCategoryId)
                .OnDelete(DeleteBehavior.SetNull);
        });
    }

    private static void ConfigureSettingsAndNotifications(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Setting>(entity =>
        {
            entity.ToTable("settings");
            entity.HasKey(setting => setting.Id);
            entity.HasIndex(setting => new { setting.Group, setting.Name }).IsUnique();

            entity.Property(setting => setting.Id).HasColumnName("id");
            entity.Property(setting => setting.Group).HasColumnName("group");
            entity.Property(setting => setting.Name).HasColumnName("name");
            entity.Property(setting => setting.Locked).HasColumnName("locked").HasDefaultValue(false);
            entity.Property(setting => setting.Payload).HasColumnName("payload");
            entity.Property(setting => setting.CreatedAt).HasColumnName("created_at");
            entity.Property(setting => setting.UpdatedAt).HasColumnName("updated_at");
        });

        modelBuilder.Entity<Notification>(entity =>
        {
            entity.ToTable("notifications");
            entity.HasKey(notification => notification.Id);
            entity.HasIndex(notification => new { notification.NotifiableType, notification.NotifiableId });

            entity.Property(notification => notification.Id).HasColumnName("id");
            entity.Property(notification => notification.Type).HasColumnName("type");
            entity.Property(notification => notification.NotifiableType).HasColumnName("notifiable_type");
            entity.Property(notification => notification.NotifiableId).HasColumnName("notifiable_id");
            entity.Property(notification => notification.Data).HasColumnName("data");
            entity.Property(notification => notification.ReadAt).HasColumnName("read_at");
            entity.Property(notification => notification.CreatedAt).HasColumnName("created_at");
            entity.Property(notification => notification.UpdatedAt).HasColumnName("updated_at");
        });

        modelBuilder.Entity<CacheEntry>(entity =>
        {
            entity.ToTable("cache");
            entity.HasKey(cache => cache.Key);
            entity.HasIndex(cache => cache.Expiration);

            entity.Property(cache => cache.Key).HasColumnName("key");
            entity.Property(cache => cache.Value).HasColumnName("value");
            entity.Property(cache => cache.Expiration).HasColumnName("expiration");
        });

        modelBuilder.Entity<CacheLock>(entity =>
        {
            entity.ToTable("cache_locks");
            entity.HasKey(cacheLock => cacheLock.Key);
            entity.HasIndex(cacheLock => cacheLock.Expiration);

            entity.Property(cacheLock => cacheLock.Key).HasColumnName("key");
            entity.Property(cacheLock => cacheLock.Owner).HasColumnName("owner");
            entity.Property(cacheLock => cacheLock.Expiration).HasColumnName("expiration");
        });
    }

    private static void SeedInitialData(ModelBuilder modelBuilder)
    {
        var seededAt = new DateTime(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc);

        modelBuilder.Entity<LookupType>().HasData(
            new LookupType { Id = 1, Key = "addiction_duration", LabelAr = "مدة الإدمان", LabelEn = "Addiction Duration", CreatedAt = seededAt, UpdatedAt = seededAt },
            new LookupType { Id = 2, Key = "education_level", LabelAr = "المستوى التعليمي", LabelEn = "Education Level", CreatedAt = seededAt, UpdatedAt = seededAt },
            new LookupType { Id = 3, Key = "treatment_type", LabelAr = "نوع العلاج", LabelEn = "Treatment Type", CreatedAt = seededAt, UpdatedAt = seededAt });

        modelBuilder.Entity<LookupValue>().HasData(
            new LookupValue { Id = 1, LookupTypeId = 1, ValueKey = "less_6m", LabelAr = "أقل من 6 أشهر", LabelEn = "Less than 6 months", SortOrder = 1, IsActive = true, CreatedAt = seededAt, UpdatedAt = seededAt },
            new LookupValue { Id = 2, LookupTypeId = 1, ValueKey = "6_12m", LabelAr = "6 - 12 شهر", LabelEn = "6 - 12 months", SortOrder = 2, IsActive = true, CreatedAt = seededAt, UpdatedAt = seededAt },
            new LookupValue { Id = 3, LookupTypeId = 1, ValueKey = "1_3y", LabelAr = "1 - 3 سنوات", LabelEn = "1 - 3 years", SortOrder = 3, IsActive = true, CreatedAt = seededAt, UpdatedAt = seededAt },
            new LookupValue { Id = 4, LookupTypeId = 1, ValueKey = "over_3y", LabelAr = "أكثر من 3 سنوات", LabelEn = "Over 3 years", SortOrder = 4, IsActive = true, CreatedAt = seededAt, UpdatedAt = seededAt },
            new LookupValue { Id = 5, LookupTypeId = 2, ValueKey = "none", LabelAr = "بدون تعليم", LabelEn = "No Education", SortOrder = 1, IsActive = true, CreatedAt = seededAt, UpdatedAt = seededAt },
            new LookupValue { Id = 6, LookupTypeId = 2, ValueKey = "primary", LabelAr = "ابتدائي", LabelEn = "Primary", SortOrder = 2, IsActive = true, CreatedAt = seededAt, UpdatedAt = seededAt },
            new LookupValue { Id = 7, LookupTypeId = 2, ValueKey = "secondary", LabelAr = "ثانوي", LabelEn = "Secondary", SortOrder = 3, IsActive = true, CreatedAt = seededAt, UpdatedAt = seededAt },
            new LookupValue { Id = 8, LookupTypeId = 2, ValueKey = "university", LabelAr = "جامعي", LabelEn = "University", SortOrder = 4, IsActive = true, CreatedAt = seededAt, UpdatedAt = seededAt },
            new LookupValue { Id = 9, LookupTypeId = 2, ValueKey = "postgraduate", LabelAr = "دراسات عليا", LabelEn = "Postgraduate", SortOrder = 5, IsActive = true, CreatedAt = seededAt, UpdatedAt = seededAt },
            new LookupValue { Id = 10, LookupTypeId = 3, ValueKey = "hospital", LabelAr = "علاج في المستشفى", LabelEn = "Hospital Treatment", SortOrder = 1, IsActive = true, CreatedAt = seededAt, UpdatedAt = seededAt },
            new LookupValue { Id = 11, LookupTypeId = 3, ValueKey = "outpatient", LabelAr = "علاج خارجي", LabelEn = "Outpatient", SortOrder = 2, IsActive = true, CreatedAt = seededAt, UpdatedAt = seededAt },
            new LookupValue { Id = 12, LookupTypeId = 3, ValueKey = "self", LabelAr = "علاج ذاتي", LabelEn = "Self Treatment", SortOrder = 3, IsActive = true, CreatedAt = seededAt, UpdatedAt = seededAt },
            new LookupValue { Id = 13, LookupTypeId = 3, ValueKey = "religious", LabelAr = "علاج ديني", LabelEn = "Religious", SortOrder = 4, IsActive = true, CreatedAt = seededAt, UpdatedAt = seededAt });

        modelBuilder.Entity<SubstanceCategory>().HasData(
            new SubstanceCategory { Id = 1, Slug = "depressants", NameAr = "المثبطات", NameEn = "Depressants", IsActive = true, SortOrder = 1, CreatedAt = seededAt, UpdatedAt = seededAt },
            new SubstanceCategory { Id = 2, Slug = "sedatives", NameAr = "المهدئات", NameEn = "Sedatives", IsActive = true, SortOrder = 2, CreatedAt = seededAt, UpdatedAt = seededAt },
            new SubstanceCategory { Id = 3, Slug = "stimulants", NameAr = "المنشطات", NameEn = "Stimulants", IsActive = true, SortOrder = 3, CreatedAt = seededAt, UpdatedAt = seededAt },
            new SubstanceCategory { Id = 4, Slug = "hallucinogens", NameAr = "المهلوسات", NameEn = "Hallucinogens", IsActive = true, SortOrder = 4, CreatedAt = seededAt, UpdatedAt = seededAt });

        modelBuilder.Entity<Substance>().HasData(
            new Substance { Id = 1, SubstanceCategoryId = 1, NameAr = "حشيش", NameEn = "Hash", IsActive = true, CreatedAt = seededAt, UpdatedAt = seededAt },
            new Substance { Id = 2, SubstanceCategoryId = 1, NameAr = "بانجو", NameEn = "Bango", IsActive = true, CreatedAt = seededAt, UpdatedAt = seededAt },
            new Substance { Id = 3, SubstanceCategoryId = 1, NameAr = "هيدرا", NameEn = "Hydra", IsActive = true, CreatedAt = seededAt, UpdatedAt = seededAt },
            new Substance { Id = 4, SubstanceCategoryId = 2, NameAr = "أفيون", NameEn = "Opium", IsActive = true, CreatedAt = seededAt, UpdatedAt = seededAt },
            new Substance { Id = 5, SubstanceCategoryId = 2, NameAr = "ترامادول", NameEn = "Tramadol", IsActive = true, CreatedAt = seededAt, UpdatedAt = seededAt },
            new Substance { Id = 6, SubstanceCategoryId = 2, NameAr = "هيروين", NameEn = "Heroin", IsActive = true, CreatedAt = seededAt, UpdatedAt = seededAt },
            new Substance { Id = 7, SubstanceCategoryId = 3, NameAr = "شابو", NameEn = "Shabu", IsActive = true, CreatedAt = seededAt, UpdatedAt = seededAt },
            new Substance { Id = 8, SubstanceCategoryId = 3, NameAr = "كوكايين", NameEn = "Cocaine", IsActive = true, CreatedAt = seededAt, UpdatedAt = seededAt },
            new Substance { Id = 9, SubstanceCategoryId = 3, NameAr = "إكستاسي", NameEn = "Ecstasy", IsActive = true, CreatedAt = seededAt, UpdatedAt = seededAt },
            new Substance { Id = 10, SubstanceCategoryId = 4, NameAr = "إل إس دي", NameEn = "LSD", IsActive = true, CreatedAt = seededAt, UpdatedAt = seededAt },
            new Substance { Id = 11, SubstanceCategoryId = 4, NameAr = "آيس", NameEn = "Ice (Crystal Meth)", IsActive = true, CreatedAt = seededAt, UpdatedAt = seededAt });

        modelBuilder.Entity<Setting>().HasData(
            new Setting { Id = 1, Group = "general", Name = "session_price", Locked = false, Payload = "1200", CreatedAt = seededAt, UpdatedAt = seededAt },
            new Setting { Id = 2, Group = "general", Name = "group_min_members", Locked = false, Payload = "7", CreatedAt = seededAt, UpdatedAt = seededAt },
            new Setting { Id = 3, Group = "general", Name = "group_max_members", Locked = false, Payload = "15", CreatedAt = seededAt, UpdatedAt = seededAt },
            new Setting { Id = 4, Group = "general", Name = "session_duration_minutes", Locked = false, Payload = "15", CreatedAt = seededAt, UpdatedAt = seededAt },
            new Setting { Id = 5, Group = "general", Name = "booking_cutoff_minutes", Locked = false, Payload = "15", CreatedAt = seededAt, UpdatedAt = seededAt },
            new Setting { Id = 6, Group = "general", Name = "session_start_hour", Locked = false, Payload = "9", CreatedAt = seededAt, UpdatedAt = seededAt },
            new Setting { Id = 7, Group = "general", Name = "session_days", Locked = false, Payload = "[\"sunday\",\"monday\",\"tuesday\",\"wednesday\"]", CreatedAt = seededAt, UpdatedAt = seededAt },
            new Setting { Id = 8, Group = "general", Name = "auto_start_timeout_minutes", Locked = false, Payload = "1440", CreatedAt = seededAt, UpdatedAt = seededAt },
            new Setting { Id = 9, Group = "content", Name = "privacy_policy", Locked = false, Payload = "\"\"", CreatedAt = seededAt, UpdatedAt = seededAt },
            new Setting { Id = 10, Group = "content", Name = "terms_and_conditions", Locked = false, Payload = "\"\"", CreatedAt = seededAt, UpdatedAt = seededAt });
    }
}
