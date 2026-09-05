using Microsoft.EntityFrameworkCore;
using Solid.Api.Database;
using Solid.Api.Database.Entities;

namespace Solid.Api.Seeds;

/// <summary>
/// Test seed data. Run via: POST /api/admin/seed  (only in Development)
/// OR call DatabaseSeeder.SeedAsync(dbContext) from Program.cs in dev mode.
/// </summary>
public static class DatabaseSeeder
{
    public static async Task SeedAsync(SolidDbContext db)
    {
        await EnsureLookupDataAsync(db);

        // ── Instructors ──────────────────────────────────────────────
        User instructor1;
        if (!await db.Users.AnyAsync(u => u.MobileNumber == "+201000000001"))
        {
            instructor1 = new User
            {
                DisplayName = "Dr. Ahmed Sayed",
                MobileNumber = "+201000000001",
                Email = "instructor1@solid.test",
                Password = BCrypt.Net.BCrypt.HashPassword("Password123!", 12),
                Role = "instructor",
                PreferredLanguage = "ar",
                Bio = "متخصص في علاج الإدمان بخبرة 10 سنوات",
                Experience = "[\"علاج الإدمان\",\"الصحة النفسية\"]",
                Quote = "الأمل هو أول خطوة نحو الشفاء",
                IsActive = true,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };
            db.Users.Add(instructor1);
        }
        else
        {
            instructor1 = await db.Users.FirstAsync(u => u.MobileNumber == "+201000000001");
        }

        User instructor2;
        if (!await db.Users.AnyAsync(u => u.MobileNumber == "+201000000002"))
        {
            instructor2 = new User
            {
                DisplayName = "Dr. Sara Ibrahim",
                MobileNumber = "+201000000002",
                Email = "instructor2@solid.test",
                Password = BCrypt.Net.BCrypt.HashPassword("Password123!", 12),
                Role = "instructor",
                PreferredLanguage = "en",
                Bio = "Addiction recovery specialist with 8 years experience",
                IsActive = true,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };
            db.Users.Add(instructor2);
        }
        else
        {
            instructor2 = await db.Users.FirstAsync(u => u.MobileNumber == "+201000000002");
        }

        // ── Addict users ──────────────────────────────────────────────
        User addict1;
        if (!await db.Users.AnyAsync(u => u.MobileNumber == "+201111111111"))
        {
            addict1 = new User
            {
                DisplayName = "Mohamed Hassan",
                MobileNumber = "+201111111111",
                Email = "addict1@solid.test",
                Password = BCrypt.Net.BCrypt.HashPassword("Password123!", 12),
                Role = "addict",
                PreferredLanguage = "ar",
                IsActive = true,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };
            db.Users.Add(addict1);
        }
        else
        {
            addict1 = await db.Users.FirstAsync(u => u.MobileNumber == "+201111111111");
        }

        User addict2;
        if (!await db.Users.AnyAsync(u => u.MobileNumber == "+201111111112"))
        {
            addict2 = new User
            {
                DisplayName = "Ali Mahmoud",
                MobileNumber = "+201111111112",
                Email = "addict2@solid.test",
                Password = BCrypt.Net.BCrypt.HashPassword("Password123!", 12),
                Role = "addict",
                PreferredLanguage = "ar",
                IsActive = true,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };
            db.Users.Add(addict2);
        }
        else
        {
            addict2 = await db.Users.FirstAsync(u => u.MobileNumber == "+201111111112");
        }

        User addict3;
        if (!await db.Users.AnyAsync(u => u.MobileNumber == "+201111111113"))
        {
            addict3 = new User
            {
                DisplayName = "Omar Khaled",
                MobileNumber = "+201111111113",
                Email = "addict3@solid.test",
                Password = BCrypt.Net.BCrypt.HashPassword("Password123!", 12),
                Role = "addict",
                PreferredLanguage = "en",
                IsActive = true,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };
            db.Users.Add(addict3);
        }
        else
        {
            addict3 = await db.Users.FirstAsync(u => u.MobileNumber == "+201111111113");
        }

        await db.SaveChangesAsync();

        // ── Lookup value IDs resolution ──────────────────────────────
        var duration1To3 = (await db.LookupValues.FirstOrDefaultAsync(v => v.ValueKey == "1_3y"))?.Id ?? 3;
        var duration6To12 = (await db.LookupValues.FirstOrDefaultAsync(v => v.ValueKey == "6_12m"))?.Id ?? 2;
        var durationOver3 = (await db.LookupValues.FirstOrDefaultAsync(v => v.ValueKey == "over_3y"))?.Id ?? 4;

        var eduUniversity = (await db.LookupValues.FirstOrDefaultAsync(v => v.ValueKey == "university"))?.Id ?? 8;
        var eduSecondary = (await db.LookupValues.FirstOrDefaultAsync(v => v.ValueKey == "secondary"))?.Id ?? 7;
        var eduPostgrad = (await db.LookupValues.FirstOrDefaultAsync(v => v.ValueKey == "postgraduate"))?.Id ?? 9;

        var treatmentHospital = (await db.LookupValues.FirstOrDefaultAsync(v => v.ValueKey == "hospital"))?.Id ?? 10;
        var treatmentOutpatient = (await db.LookupValues.FirstOrDefaultAsync(v => v.ValueKey == "outpatient"))?.Id ?? 11;
        var treatmentSelf = (await db.LookupValues.FirstOrDefaultAsync(v => v.ValueKey == "self"))?.Id ?? 12;

        // ── Addiction profiles ────────────────────────────────────────
        if (!await db.AddictionProfiles.AnyAsync(p => p.UserId == addict1.Id))
        {
            db.AddictionProfiles.Add(new AddictionProfile
            {
                UserId = addict1.Id,
                AddictionDurationId = duration1To3,
                EducationLevelId = eduUniversity,
                HadPriorTreatment = true,
                AddictionReason = "ضغوط العمل والمشاكل الأسرية",
                DaysClean = 45,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            });
        }

        if (!await db.AddictionProfiles.AnyAsync(p => p.UserId == addict2.Id))
        {
            db.AddictionProfiles.Add(new AddictionProfile
            {
                UserId = addict2.Id,
                AddictionDurationId = duration6To12,
                EducationLevelId = eduSecondary,
                HadPriorTreatment = false,
                AddictionReason = "رفقاء السوء",
                DaysClean = 10,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            });
        }

        if (!await db.AddictionProfiles.AnyAsync(p => p.UserId == addict3.Id))
        {
            db.AddictionProfiles.Add(new AddictionProfile
            {
                UserId = addict3.Id,
                AddictionDurationId = durationOver3,
                EducationLevelId = eduPostgrad,
                HadPriorTreatment = true,
                AddictionReason = "Stress and anxiety",
                DaysClean = 120,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            });
        }

        // ── User substances ───────────────────────────────────────────
        if (!await db.UserSubstances.AnyAsync(us => us.UserId == addict1.Id))
        {
            db.UserSubstances.Add(new UserSubstance { UserId = addict1.Id, SubstanceId = 1 }); // Hash
            db.UserSubstances.Add(new UserSubstance { UserId = addict1.Id, SubstanceId = 2 }); // Bango
        }

        if (!await db.UserSubstances.AnyAsync(us => us.UserId == addict2.Id))
        {
            db.UserSubstances.Add(new UserSubstance { UserId = addict2.Id, SubstanceId = 5 }); // Tramadol
        }

        if (!await db.UserSubstances.AnyAsync(us => us.UserId == addict3.Id))
        {
            db.UserSubstances.Add(new UserSubstance { UserId = addict3.Id, SubstanceId = 8 }); // Cocaine
        }

        // ── User treatment types ──────────────────────────────────────
        if (!await db.UserTreatmentTypes.AnyAsync(ut => ut.UserId == addict1.Id))
        {
            db.UserTreatmentTypes.Add(new UserTreatmentType { UserId = addict1.Id, LookupValueId = treatmentHospital });
            db.UserTreatmentTypes.Add(new UserTreatmentType { UserId = addict1.Id, LookupValueId = treatmentOutpatient });
        }

        if (!await db.UserTreatmentTypes.AnyAsync(ut => ut.UserId == addict2.Id))
        {
            db.UserTreatmentTypes.Add(new UserTreatmentType { UserId = addict2.Id, LookupValueId = treatmentSelf });
        }

        await db.SaveChangesAsync();

        // ── Groups ────────────────────────────────────────────────────
        Group group1;
        if (!await db.Groups.AnyAsync(g => g.NameEn == "Depressants Recovery Group A"))
        {
            group1 = new Group
            {
                InstructorId = instructor1.Id,
                SubstanceCategoryId = 1, // Depressants
                GroupType = "mixed",
                Status = "active",
                NameAr = "مجموعة المثبطات أ",
                NameEn = "Depressants Recovery Group A",
                MinMembers = 3,
                MaxMembers = 15,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };
            db.Groups.Add(group1);
        }
        else
        {
            group1 = await db.Groups.FirstAsync(g => g.NameEn == "Depressants Recovery Group A");
        }

        Group group2;
        if (!await db.Groups.AnyAsync(g => g.NameEn == "Sedatives Recovery Group B"))
        {
            group2 = new Group
            {
                InstructorId = instructor2.Id,
                SubstanceCategoryId = 2, // Sedatives
                GroupType = "mixed",
                Status = "forming",
                NameAr = "مجموعة المهدئات ب",
                NameEn = "Sedatives Recovery Group B",
                MinMembers = 7,
                MaxMembers = 15,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };
            db.Groups.Add(group2);
        }
        else
        {
            group2 = await db.Groups.FirstAsync(g => g.NameEn == "Sedatives Recovery Group B");
        }

        await db.SaveChangesAsync();

        // ── Group members ─────────────────────────────────────────────
        if (!await db.GroupMembers.AnyAsync(m => m.GroupId == group1.Id && m.UserId == addict1.Id))
        {
            db.GroupMembers.Add(new GroupMember { GroupId = group1.Id, UserId = addict1.Id, JoinedAt = DateTime.UtcNow, IsActive = true });
        }

        if (!await db.GroupMembers.AnyAsync(m => m.GroupId == group2.Id && m.UserId == addict2.Id))
        {
            db.GroupMembers.Add(new GroupMember { GroupId = group2.Id, UserId = addict2.Id, JoinedAt = DateTime.UtcNow, IsActive = true });
        }

        await db.SaveChangesAsync();

        // ── Therapy sessions ──────────────────────────────────────────
        TherapySession session1;
        var roomName1 = "solid-seed-room-001";
        if (!await db.TherapySessions.AnyAsync(s => s.JitsiRoomName == roomName1))
        {
            session1 = new TherapySession
            {
                SubstanceCategoryId = group1.SubstanceCategoryId!.Value,
                InstructorId = instructor1.Id,
                SessionNumber = 1,
                SessionType = "group",
                Status = "scheduled",
                ScheduledAt = DateTime.UtcNow.AddDays(2),
                DurationMinutes = 45,
                JitsiRoomName = roomName1,
                SessionMetadata = "{\"title\":\"Session 1 - Introduction\"}",
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };
            db.TherapySessions.Add(session1);
        }
        else
        {
            session1 = await db.TherapySessions.FirstAsync(s => s.JitsiRoomName == roomName1);
        }

        TherapySession session2;
        var roomName2 = "solid-seed-room-002";
        if (!await db.TherapySessions.AnyAsync(s => s.JitsiRoomName == roomName2))
        {
            session2 = new TherapySession
            {
                SubstanceCategoryId = group1.SubstanceCategoryId!.Value,
                InstructorId = instructor1.Id,
                SessionNumber = 2,
                SessionType = "group",
                Status = "live",
                ScheduledAt = DateTime.UtcNow.AddHours(-1),
                StartedAt = DateTime.UtcNow.AddMinutes(-30),
                DurationMinutes = 45,
                JitsiRoomName = roomName2,
                SessionMetadata = "{\"title\":\"Session 2 - Coping Strategies\",\"max_participants\":10}",
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };
            db.TherapySessions.Add(session2);
        }
        else
        {
            session2 = await db.TherapySessions.FirstAsync(s => s.JitsiRoomName == roomName2);
        }

        TherapySession session3;
        var roomName3 = "solid-seed-room-003";
        if (!await db.TherapySessions.AnyAsync(s => s.JitsiRoomName == roomName3))
        {
            session3 = new TherapySession
            {
                SubstanceCategoryId = group1.SubstanceCategoryId!.Value,
                InstructorId = instructor1.Id,
                SessionNumber = 3,
                SessionType = "group",
                Status = "finished",
                ScheduledAt = DateTime.UtcNow.AddDays(-3),
                StartedAt = DateTime.UtcNow.AddDays(-3),
                EndedAt = DateTime.UtcNow.AddDays(-3).AddMinutes(45),
                DurationMinutes = 45,
                JitsiRoomName = roomName3,
                SessionMetadata = "{\"title\":\"Session 3 - Relapse Prevention\"}",
                CreatedAt = DateTime.UtcNow.AddDays(-4),
                UpdatedAt = DateTime.UtcNow.AddDays(-3)
            };
            db.TherapySessions.Add(session3);
        }
        else
        {
            session3 = await db.TherapySessions.FirstAsync(s => s.JitsiRoomName == roomName3);
        }

        await db.SaveChangesAsync();

        // ── Session attendances ───────────────────────────────────────
        if (!await db.SessionAttendances.AnyAsync(a => a.SessionId == session2.Id && a.UserId == addict1.Id))
        {
            db.SessionAttendances.Add(new SessionAttendance
            {
                SessionId = session2.Id,
                UserId = addict1.Id,
                JoinedAt = DateTime.UtcNow.AddMinutes(-20),
                WasPresent = true,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            });
        }

        if (!await db.SessionAttendances.AnyAsync(a => a.SessionId == session3.Id && a.UserId == addict1.Id))
        {
            db.SessionAttendances.Add(new SessionAttendance
            {
                SessionId = session3.Id,
                UserId = addict1.Id,
                JoinedAt = DateTime.UtcNow.AddDays(-3),
                LeftAt = DateTime.UtcNow.AddDays(-3).AddMinutes(45),
                WasPresent = true,
                Rating = 5,
                Comment = "جلسة رائعة ومفيدة جداً",
                CreatedAt = DateTime.UtcNow.AddDays(-3),
                UpdatedAt = DateTime.UtcNow.AddDays(-3)
            });
        }

        await db.SaveChangesAsync();

        // ── Payments ──────────────────────────────────────────────────
        if (!await db.Payments.AnyAsync(p => p.UserId == addict1.Id && p.SessionId == session3.Id))
        {
            db.Payments.Add(new Payment
            {
                UserId = addict1.Id,
                SessionId = session3.Id,
                Amount = 1200,
                Currency = "EGP",
                Status = "paid",
                Gateway = "manual",
                GatewayTransactionId = Guid.NewGuid().ToString(),
                PaidAt = DateTime.UtcNow.AddDays(-3),
                CreatedAt = DateTime.UtcNow.AddDays(-4),
                UpdatedAt = DateTime.UtcNow.AddDays(-3)
            });
        }

        if (!await db.Payments.AnyAsync(p => p.UserId == addict1.Id && p.SessionId == session1.Id))
        {
            db.Payments.Add(new Payment
            {
                UserId = addict1.Id,
                SessionId = session1.Id,
                Amount = 1200,
                Currency = "EGP",
                Status = "pending",
                Gateway = "manual",
                GatewayTransactionId = Guid.NewGuid().ToString(),
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            });
        }

        await db.SaveChangesAsync();

        // ── Payment methods ───────────────────────────────────────────
        if (!await db.PaymentMethods.AnyAsync(pm => pm.UserId == addict1.Id))
        {
            db.PaymentMethods.Add(new PaymentMethod
            {
                UserId = addict1.Id,
                CardHolder = "Mohamed Hassan",
                CardNumber = "4111111111111111",
                Expiry = "12/28",
                IsDefault = true,
                GatewayToken = "tok_test_123",
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            });
        }

        await db.SaveChangesAsync();

        // ── Recommendations ───────────────────────────────────────────
        if (!await db.Recommendations.AnyAsync(r => r.NameEn == "Cairo Recovery Center"))
        {
            db.Recommendations.Add(new Recommendation
            {
                SubstanceCategoryId = 1,
                Type = "clinic",
                NameAr = "مركز القاهرة للتعافي",
                NameEn = "Cairo Recovery Center",
                ContactInfo = "01234567890",
                Latitude = 30.0444m,
                Longitude = 31.2357m,
                IsActive = true,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            });

            db.Recommendations.Add(new Recommendation
            {
                SubstanceCategoryId = 2,
                Type = "hospital",
                NameAr = "مستشفى النصر للأمراض النفسية",
                NameEn = "Al-Nasr Psychiatric Hospital",
                ContactInfo = "01098765432",
                Latitude = 30.0626m,
                Longitude = 31.2497m,
                IsActive = true,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            });

            db.Recommendations.Add(new Recommendation
            {
                Type = "support_group",
                NameAr = "مجموعة دعم المدمنين المجهولين",
                NameEn = "Anonymous Addicts Support Group",
                ContactInfo = "01155443322",
                IsActive = true,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            });
        }

        await db.SaveChangesAsync();

        // ── Notifications ─────────────────────────────────────────────
        if (!await db.Notifications.AnyAsync(n => n.NotifiableId == addict1.Id))
        {
            db.Notifications.Add(new Notification
            {
                Type = "App\\Notifications\\SessionReminder",
                NotifiableType = "Modules\\User\\Models\\User",
                NotifiableId = addict1.Id,
                Data = "{\"title\":\"تذكير بالجلسة\",\"body\":\"لديك جلسة غداً الساعة 10 صباحاً\",\"type\":\"reminder\",\"icon\":\"calendar\"}",
                CreatedAt = DateTime.UtcNow.AddHours(-2),
                UpdatedAt = DateTime.UtcNow.AddHours(-2)
            });

            db.Notifications.Add(new Notification
            {
                Type = "App\\Notifications\\WelcomeNotification",
                NotifiableType = "Modules\\User\\Models\\User",
                NotifiableId = addict1.Id,
                Data = "{\"title\":\"مرحباً بك\",\"body\":\"تم تسجيل حسابك بنجاح\",\"type\":\"info\",\"icon\":\"bell\"}",
                ReadAt = DateTime.UtcNow.AddHours(-1),
                CreatedAt = DateTime.UtcNow.AddDays(-1),
                UpdatedAt = DateTime.UtcNow.AddHours(-1)
            });
        }

        await db.SaveChangesAsync();
    }

    private static async Task EnsureLookupDataAsync(SolidDbContext db)
    {
        var seededAt = new DateTime(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc);

        if (!await db.LookupTypes.AnyAsync())
        {
            db.LookupTypes.AddRange(
                new LookupType { Key = "addiction_duration", LabelAr = "مدة الإدمان", LabelEn = "Addiction Duration", CreatedAt = seededAt, UpdatedAt = seededAt },
                new LookupType { Key = "education_level", LabelAr = "المستوى التعليمي", LabelEn = "Education Level", CreatedAt = seededAt, UpdatedAt = seededAt },
                new LookupType { Key = "treatment_type", LabelAr = "نوع العلاج", LabelEn = "Treatment Type", CreatedAt = seededAt, UpdatedAt = seededAt }
            );
            await db.SaveChangesAsync();
        }

        var durationType = await db.LookupTypes.FirstAsync(t => t.Key == "addiction_duration");
        var eduType = await db.LookupTypes.FirstAsync(t => t.Key == "education_level");
        var treatmentType = await db.LookupTypes.FirstAsync(t => t.Key == "treatment_type");

        if (!await db.LookupValues.AnyAsync())
        {
            db.LookupValues.AddRange(
                new LookupValue { LookupTypeId = durationType.Id, ValueKey = "less_6m", LabelAr = "أقل من 6 أشهر", LabelEn = "Less than 6 months", SortOrder = 1, IsActive = true, CreatedAt = seededAt, UpdatedAt = seededAt },
                new LookupValue { LookupTypeId = durationType.Id, ValueKey = "6_12m", LabelAr = "6 - 12 شهر", LabelEn = "6 - 12 months", SortOrder = 2, IsActive = true, CreatedAt = seededAt, UpdatedAt = seededAt },
                new LookupValue { LookupTypeId = durationType.Id, ValueKey = "1_3y", LabelAr = "1 - 3 سنوات", LabelEn = "1 - 3 years", SortOrder = 3, IsActive = true, CreatedAt = seededAt, UpdatedAt = seededAt },
                new LookupValue { LookupTypeId = durationType.Id, ValueKey = "over_3y", LabelAr = "أكثر من 3 سنوات", LabelEn = "Over 3 years", SortOrder = 4, IsActive = true, CreatedAt = seededAt, UpdatedAt = seededAt },
                new LookupValue { LookupTypeId = eduType.Id, ValueKey = "none", LabelAr = "بدون تعليم", LabelEn = "No Education", SortOrder = 1, IsActive = true, CreatedAt = seededAt, UpdatedAt = seededAt },
                new LookupValue { LookupTypeId = eduType.Id, ValueKey = "primary", LabelAr = "ابتدائي", LabelEn = "Primary", SortOrder = 2, IsActive = true, CreatedAt = seededAt, UpdatedAt = seededAt },
                new LookupValue { LookupTypeId = eduType.Id, ValueKey = "secondary", LabelAr = "ثانوي", LabelEn = "Secondary", SortOrder = 3, IsActive = true, CreatedAt = seededAt, UpdatedAt = seededAt },
                new LookupValue { LookupTypeId = eduType.Id, ValueKey = "university", LabelAr = "جامعي", LabelEn = "University", SortOrder = 4, IsActive = true, CreatedAt = seededAt, UpdatedAt = seededAt },
                new LookupValue { LookupTypeId = eduType.Id, ValueKey = "postgraduate", LabelAr = "دراسات عليا", LabelEn = "Postgraduate", SortOrder = 5, IsActive = true, CreatedAt = seededAt, UpdatedAt = seededAt },
                new LookupValue { LookupTypeId = treatmentType.Id, ValueKey = "hospital", LabelAr = "علاج في المستشفى", LabelEn = "Hospital Treatment", SortOrder = 1, IsActive = true, CreatedAt = seededAt, UpdatedAt = seededAt },
                new LookupValue { LookupTypeId = treatmentType.Id, ValueKey = "outpatient", LabelAr = "علاج خارجي", LabelEn = "Outpatient", SortOrder = 2, IsActive = true, CreatedAt = seededAt, UpdatedAt = seededAt },
                new LookupValue { LookupTypeId = treatmentType.Id, ValueKey = "self", LabelAr = "علاج ذاتي", LabelEn = "Self Treatment", SortOrder = 3, IsActive = true, CreatedAt = seededAt, UpdatedAt = seededAt },
                new LookupValue { LookupTypeId = treatmentType.Id, ValueKey = "religious", LabelAr = "علاج ديني", LabelEn = "Religious", SortOrder = 4, IsActive = true, CreatedAt = seededAt, UpdatedAt = seededAt }
            );
            await db.SaveChangesAsync();
        }

        if (!await db.SubstanceCategories.AnyAsync())
        {
            db.SubstanceCategories.AddRange(
                new SubstanceCategory { Slug = "depressants", NameAr = "المثبطات", NameEn = "Depressants", IsActive = true, SortOrder = 1, CreatedAt = seededAt, UpdatedAt = seededAt },
                new SubstanceCategory { Slug = "sedatives", NameAr = "المهدئات", NameEn = "Sedatives", IsActive = true, SortOrder = 2, CreatedAt = seededAt, UpdatedAt = seededAt },
                new SubstanceCategory { Slug = "stimulants", NameAr = "المنشطات", NameEn = "Stimulants", IsActive = true, SortOrder = 3, CreatedAt = seededAt, UpdatedAt = seededAt },
                new SubstanceCategory { Slug = "hallucinogens", NameAr = "المهلوسات", NameEn = "Hallucinogens", IsActive = true, SortOrder = 4, CreatedAt = seededAt, UpdatedAt = seededAt }
            );
            await db.SaveChangesAsync();
        }

        if (!await db.Substances.AnyAsync())
        {
            var depressants = await db.SubstanceCategories.FirstAsync(c => c.Slug == "depressants");
            var sedatives = await db.SubstanceCategories.FirstAsync(c => c.Slug == "sedatives");
            var stimulants = await db.SubstanceCategories.FirstAsync(c => c.Slug == "stimulants");
            var hallucinogens = await db.SubstanceCategories.FirstAsync(c => c.Slug == "hallucinogens");

            db.Substances.AddRange(
                new Substance { SubstanceCategoryId = depressants.Id, NameAr = "حشيش", NameEn = "Hash", IsActive = true, CreatedAt = seededAt, UpdatedAt = seededAt },
                new Substance { SubstanceCategoryId = depressants.Id, NameAr = "بانجو", NameEn = "Bango", IsActive = true, CreatedAt = seededAt, UpdatedAt = seededAt },
                new Substance { SubstanceCategoryId = depressants.Id, NameAr = "هيدرا", NameEn = "Hydra", IsActive = true, CreatedAt = seededAt, UpdatedAt = seededAt },
                new Substance { SubstanceCategoryId = sedatives.Id, NameAr = "أفيون", NameEn = "Opium", IsActive = true, CreatedAt = seededAt, UpdatedAt = seededAt },
                new Substance { SubstanceCategoryId = sedatives.Id, NameAr = "ترامادول", NameEn = "Tramadol", IsActive = true, CreatedAt = seededAt, UpdatedAt = seededAt },
                new Substance { SubstanceCategoryId = sedatives.Id, NameAr = "هيروين", NameEn = "Heroin", IsActive = true, CreatedAt = seededAt, UpdatedAt = seededAt },
                new Substance { SubstanceCategoryId = stimulants.Id, NameAr = "شابو", NameEn = "Shabu", IsActive = true, CreatedAt = seededAt, UpdatedAt = seededAt },
                new Substance { SubstanceCategoryId = stimulants.Id, NameAr = "كوكايين", NameEn = "Cocaine", IsActive = true, CreatedAt = seededAt, UpdatedAt = seededAt },
                new Substance { SubstanceCategoryId = stimulants.Id, NameAr = "إكستاسي", NameEn = "Ecstasy", IsActive = true, CreatedAt = seededAt, UpdatedAt = seededAt },
                new Substance { SubstanceCategoryId = hallucinogens.Id, NameAr = "إل إس دي", NameEn = "LSD", IsActive = true, CreatedAt = seededAt, UpdatedAt = seededAt },
                new Substance { SubstanceCategoryId = hallucinogens.Id, NameAr = "آيس", NameEn = "Ice (Crystal Meth)", IsActive = true, CreatedAt = seededAt, UpdatedAt = seededAt }
            );
            await db.SaveChangesAsync();
        }
    }
}
