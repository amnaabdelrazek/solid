using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Solid.Api.Migrations
{
    /// <inheritdoc />
    public partial class Init : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "cache",
                columns: table => new
                {
                    key = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    value = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    expiration = table.Column<long>(type: "bigint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_cache", x => x.key);
                });

            migrationBuilder.CreateTable(
                name: "cache_locks",
                columns: table => new
                {
                    key = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    owner = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    expiration = table.Column<long>(type: "bigint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_cache_locks", x => x.key);
                });

            migrationBuilder.CreateTable(
                name: "lookup_types",
                columns: table => new
                {
                    id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    key = table.Column<string>(type: "nvarchar(80)", maxLength: 80, nullable: false),
                    label_ar = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: false),
                    label_en = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: false),
                    created_at = table.Column<DateTime>(type: "datetime2", nullable: true),
                    updated_at = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_lookup_types", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "notifications",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    type = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    notifiable_type = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    notifiable_id = table.Column<long>(type: "bigint", nullable: false),
                    data = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    read_at = table.Column<DateTime>(type: "datetime2", nullable: true),
                    created_at = table.Column<DateTime>(type: "datetime2", nullable: true),
                    updated_at = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_notifications", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "personal_access_tokens",
                columns: table => new
                {
                    id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    tokenable_type = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    tokenable_id = table.Column<long>(type: "bigint", nullable: false),
                    name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    token = table.Column<string>(type: "nvarchar(64)", maxLength: 64, nullable: false),
                    abilities = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    last_used_at = table.Column<DateTime>(type: "datetime2", nullable: true),
                    expires_at = table.Column<DateTime>(type: "datetime2", nullable: true),
                    created_at = table.Column<DateTime>(type: "datetime2", nullable: true),
                    updated_at = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_personal_access_tokens", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "settings",
                columns: table => new
                {
                    id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    group = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    name = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    locked = table.Column<bool>(type: "bit", nullable: false, defaultValue: false),
                    payload = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    created_at = table.Column<DateTime>(type: "datetime2", nullable: true),
                    updated_at = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_settings", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "substance_categories",
                columns: table => new
                {
                    id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    name_ar = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: false),
                    name_en = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: false),
                    slug = table.Column<string>(type: "nvarchar(80)", maxLength: 80, nullable: false),
                    is_active = table.Column<bool>(type: "bit", nullable: false, defaultValue: true),
                    sort_order = table.Column<byte>(type: "tinyint", nullable: false, defaultValue: (byte)0),
                    created_at = table.Column<DateTime>(type: "datetime2", nullable: true),
                    updated_at = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_substance_categories", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "users",
                columns: table => new
                {
                    id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    display_name = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: false),
                    mobile_number = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true),
                    username = table.Column<string>(type: "nvarchar(80)", maxLength: 80, nullable: true),
                    password = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    role = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    preferred_language = table.Column<string>(type: "nvarchar(5)", maxLength: 5, nullable: false, defaultValue: "ar"),
                    fcm_token = table.Column<string>(type: "nvarchar(512)", maxLength: 512, nullable: true),
                    active_device_id = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    is_active = table.Column<bool>(type: "bit", nullable: false, defaultValue: true),
                    email_verified_at = table.Column<DateTime>(type: "datetime2", nullable: true),
                    bio = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    avatar_url = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    experience = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    quote = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    remember_token = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    deleted_at = table.Column<DateTime>(type: "datetime2", nullable: true),
                    created_at = table.Column<DateTime>(type: "datetime2", nullable: true),
                    updated_at = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_users", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "lookup_values",
                columns: table => new
                {
                    id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    lookup_type_id = table.Column<long>(type: "bigint", nullable: false),
                    value_key = table.Column<string>(type: "nvarchar(80)", maxLength: 80, nullable: false),
                    label_ar = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    label_en = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    sort_order = table.Column<byte>(type: "tinyint", nullable: false, defaultValue: (byte)0),
                    is_active = table.Column<bool>(type: "bit", nullable: false, defaultValue: true),
                    created_at = table.Column<DateTime>(type: "datetime2", nullable: true),
                    updated_at = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_lookup_values", x => x.id);
                    table.ForeignKey(
                        name: "FK_lookup_values_lookup_types_lookup_type_id",
                        column: x => x.lookup_type_id,
                        principalTable: "lookup_types",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "recommendations",
                columns: table => new
                {
                    id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    substance_category_id = table.Column<long>(type: "bigint", nullable: true),
                    type = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    name_ar = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    name_en = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    contact_info = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    latitude = table.Column<decimal>(type: "decimal(10,7)", precision: 10, scale: 7, nullable: true),
                    longitude = table.Column<decimal>(type: "decimal(10,7)", precision: 10, scale: 7, nullable: true),
                    is_active = table.Column<bool>(type: "bit", nullable: false, defaultValue: true),
                    created_at = table.Column<DateTime>(type: "datetime2", nullable: true),
                    updated_at = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_recommendations", x => x.id);
                    table.ForeignKey(
                        name: "FK_recommendations_substance_categories_substance_category_id",
                        column: x => x.substance_category_id,
                        principalTable: "substance_categories",
                        principalColumn: "id",
                        onDelete: ReferentialAction.SetNull);
                });

            migrationBuilder.CreateTable(
                name: "substances",
                columns: table => new
                {
                    id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    substance_category_id = table.Column<long>(type: "bigint", nullable: false),
                    name_ar = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    name_en = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    is_active = table.Column<bool>(type: "bit", nullable: false, defaultValue: true),
                    created_at = table.Column<DateTime>(type: "datetime2", nullable: true),
                    updated_at = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_substances", x => x.id);
                    table.ForeignKey(
                        name: "FK_substances_substance_categories_substance_category_id",
                        column: x => x.substance_category_id,
                        principalTable: "substance_categories",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "device_sessions",
                columns: table => new
                {
                    id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    user_id = table.Column<long>(type: "bigint", nullable: false),
                    device_id = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    device_info = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    event_type = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    sanctum_token_id = table.Column<long>(type: "bigint", nullable: true),
                    created_at = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "SYSDATETIME()")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_device_sessions", x => x.id);
                    table.ForeignKey(
                        name: "FK_device_sessions_users_user_id",
                        column: x => x.user_id,
                        principalTable: "users",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "groups",
                columns: table => new
                {
                    id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    instructor_id = table.Column<long>(type: "bigint", nullable: true),
                    substance_category_id = table.Column<long>(type: "bigint", nullable: true),
                    group_type = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    status = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    name_ar = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    name_en = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    min_members = table.Column<byte>(type: "tinyint", nullable: false, defaultValue: (byte)7),
                    max_members = table.Column<byte>(type: "tinyint", nullable: false, defaultValue: (byte)15),
                    deleted_at = table.Column<DateTime>(type: "datetime2", nullable: true),
                    created_at = table.Column<DateTime>(type: "datetime2", nullable: true),
                    updated_at = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_groups", x => x.id);
                    table.ForeignKey(
                        name: "FK_groups_substance_categories_substance_category_id",
                        column: x => x.substance_category_id,
                        principalTable: "substance_categories",
                        principalColumn: "id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_groups_users_instructor_id",
                        column: x => x.instructor_id,
                        principalTable: "users",
                        principalColumn: "id",
                        onDelete: ReferentialAction.SetNull);
                });

            migrationBuilder.CreateTable(
                name: "payment_methods",
                columns: table => new
                {
                    id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    user_id = table.Column<long>(type: "bigint", nullable: false),
                    card_type = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    card_number = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    expiry = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    is_default = table.Column<bool>(type: "bit", nullable: false, defaultValue: false),
                    gateway_token = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    created_at = table.Column<DateTime>(type: "datetime2", nullable: true),
                    updated_at = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_payment_methods", x => x.id);
                    table.ForeignKey(
                        name: "FK_payment_methods_users_user_id",
                        column: x => x.user_id,
                        principalTable: "users",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "addiction_profiles",
                columns: table => new
                {
                    id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    user_id = table.Column<long>(type: "bigint", nullable: false),
                    addiction_duration_id = table.Column<long>(type: "bigint", nullable: false),
                    education_level_id = table.Column<long>(type: "bigint", nullable: false),
                    had_prior_treatment = table.Column<bool>(type: "bit", nullable: false, defaultValue: false),
                    addiction_reason = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    days_clean = table.Column<int>(type: "int", nullable: true),
                    created_at = table.Column<DateTime>(type: "datetime2", nullable: true),
                    updated_at = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_addiction_profiles", x => x.id);
                    table.ForeignKey(
                        name: "FK_addiction_profiles_lookup_values_addiction_duration_id",
                        column: x => x.addiction_duration_id,
                        principalTable: "lookup_values",
                        principalColumn: "id");
                    table.ForeignKey(
                        name: "FK_addiction_profiles_lookup_values_education_level_id",
                        column: x => x.education_level_id,
                        principalTable: "lookup_values",
                        principalColumn: "id");
                    table.ForeignKey(
                        name: "FK_addiction_profiles_users_user_id",
                        column: x => x.user_id,
                        principalTable: "users",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "user_treatment_types",
                columns: table => new
                {
                    id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    user_id = table.Column<long>(type: "bigint", nullable: false),
                    lookup_value_id = table.Column<long>(type: "bigint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_user_treatment_types", x => x.id);
                    table.ForeignKey(
                        name: "FK_user_treatment_types_lookup_values_lookup_value_id",
                        column: x => x.lookup_value_id,
                        principalTable: "lookup_values",
                        principalColumn: "id");
                    table.ForeignKey(
                        name: "FK_user_treatment_types_users_user_id",
                        column: x => x.user_id,
                        principalTable: "users",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "user_substances",
                columns: table => new
                {
                    id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    user_id = table.Column<long>(type: "bigint", nullable: false),
                    substance_id = table.Column<long>(type: "bigint", nullable: false),
                    created_at = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "SYSDATETIME()")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_user_substances", x => x.id);
                    table.ForeignKey(
                        name: "FK_user_substances_substances_substance_id",
                        column: x => x.substance_id,
                        principalTable: "substances",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_user_substances_users_user_id",
                        column: x => x.user_id,
                        principalTable: "users",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "group_members",
                columns: table => new
                {
                    id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    group_id = table.Column<long>(type: "bigint", nullable: false),
                    user_id = table.Column<long>(type: "bigint", nullable: false),
                    joined_at = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "SYSDATETIME()"),
                    is_active = table.Column<bool>(type: "bit", nullable: false, defaultValue: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_group_members", x => x.id);
                    table.ForeignKey(
                        name: "FK_group_members_groups_group_id",
                        column: x => x.group_id,
                        principalTable: "groups",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_group_members_users_user_id",
                        column: x => x.user_id,
                        principalTable: "users",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "therapy_sessions",
                columns: table => new
                {
                    id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    group_id = table.Column<long>(type: "bigint", nullable: false),
                    instructor_id = table.Column<long>(type: "bigint", nullable: false),
                    session_number = table.Column<int>(type: "int", nullable: true),
                    session_type = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    status = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    scheduled_at = table.Column<DateTime>(type: "datetime2", nullable: false),
                    started_at = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ended_at = table.Column<DateTime>(type: "datetime2", nullable: true),
                    duration_minutes = table.Column<byte>(type: "tinyint", nullable: false, defaultValue: (byte)45),
                    jitsi_room_name = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    jitsi_jwt_issued_at = table.Column<DateTime>(type: "datetime2", nullable: true),
                    session_metadata = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    deleted_at = table.Column<DateTime>(type: "datetime2", nullable: true),
                    created_at = table.Column<DateTime>(type: "datetime2", nullable: true),
                    updated_at = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_therapy_sessions", x => x.id);
                    table.ForeignKey(
                        name: "FK_therapy_sessions_groups_group_id",
                        column: x => x.group_id,
                        principalTable: "groups",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_therapy_sessions_users_instructor_id",
                        column: x => x.instructor_id,
                        principalTable: "users",
                        principalColumn: "id");
                });

            migrationBuilder.CreateTable(
                name: "payments",
                columns: table => new
                {
                    id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    user_id = table.Column<long>(type: "bigint", nullable: false),
                    session_id = table.Column<long>(type: "bigint", nullable: true),
                    amount = table.Column<decimal>(type: "decimal(10,2)", precision: 10, scale: 2, nullable: false),
                    currency = table.Column<string>(type: "nvarchar(5)", maxLength: 5, nullable: false, defaultValue: "EGP"),
                    status = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    gateway = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    gateway_transaction_id = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    gateway_response = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    paid_at = table.Column<DateTime>(type: "datetime2", nullable: true),
                    created_at = table.Column<DateTime>(type: "datetime2", nullable: true),
                    updated_at = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_payments", x => x.id);
                    table.ForeignKey(
                        name: "FK_payments_therapy_sessions_session_id",
                        column: x => x.session_id,
                        principalTable: "therapy_sessions",
                        principalColumn: "id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_payments_users_user_id",
                        column: x => x.user_id,
                        principalTable: "users",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "session_attendances",
                columns: table => new
                {
                    id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    session_id = table.Column<long>(type: "bigint", nullable: false),
                    user_id = table.Column<long>(type: "bigint", nullable: false),
                    joined_at = table.Column<DateTime>(type: "datetime2", nullable: true),
                    left_at = table.Column<DateTime>(type: "datetime2", nullable: true),
                    was_present = table.Column<bool>(type: "bit", nullable: false, defaultValue: false),
                    rating = table.Column<byte>(type: "tinyint", nullable: true),
                    comment = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    created_at = table.Column<DateTime>(type: "datetime2", nullable: true),
                    updated_at = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_session_attendances", x => x.id);
                    table.ForeignKey(
                        name: "FK_session_attendances_therapy_sessions_session_id",
                        column: x => x.session_id,
                        principalTable: "therapy_sessions",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_session_attendances_users_user_id",
                        column: x => x.user_id,
                        principalTable: "users",
                        principalColumn: "id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_addiction_profiles_addiction_duration_id",
                table: "addiction_profiles",
                column: "addiction_duration_id");

            migrationBuilder.CreateIndex(
                name: "IX_addiction_profiles_education_level_id",
                table: "addiction_profiles",
                column: "education_level_id");

            migrationBuilder.CreateIndex(
                name: "IX_addiction_profiles_user_id",
                table: "addiction_profiles",
                column: "user_id",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_cache_expiration",
                table: "cache",
                column: "expiration");

            migrationBuilder.CreateIndex(
                name: "IX_cache_locks_expiration",
                table: "cache_locks",
                column: "expiration");

            migrationBuilder.CreateIndex(
                name: "IX_device_sessions_user_id_created_at",
                table: "device_sessions",
                columns: new[] { "user_id", "created_at" });

            migrationBuilder.CreateIndex(
                name: "IX_group_members_group_id_user_id",
                table: "group_members",
                columns: new[] { "group_id", "user_id" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_group_members_user_id",
                table: "group_members",
                column: "user_id");

            migrationBuilder.CreateIndex(
                name: "IX_groups_instructor_id",
                table: "groups",
                column: "instructor_id");

            migrationBuilder.CreateIndex(
                name: "IX_groups_substance_category_id",
                table: "groups",
                column: "substance_category_id");

            migrationBuilder.CreateIndex(
                name: "IX_lookup_types_key",
                table: "lookup_types",
                column: "key",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_lookup_values_lookup_type_id",
                table: "lookup_values",
                column: "lookup_type_id");

            migrationBuilder.CreateIndex(
                name: "IX_notifications_notifiable_type_notifiable_id",
                table: "notifications",
                columns: new[] { "notifiable_type", "notifiable_id" });

            migrationBuilder.CreateIndex(
                name: "IX_payment_methods_user_id",
                table: "payment_methods",
                column: "user_id");

            migrationBuilder.CreateIndex(
                name: "IX_payments_gateway_transaction_id",
                table: "payments",
                column: "gateway_transaction_id",
                unique: true,
                filter: "[gateway_transaction_id] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_payments_session_id",
                table: "payments",
                column: "session_id");

            migrationBuilder.CreateIndex(
                name: "IX_payments_user_id",
                table: "payments",
                column: "user_id");

            migrationBuilder.CreateIndex(
                name: "IX_personal_access_tokens_token",
                table: "personal_access_tokens",
                column: "token",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_personal_access_tokens_tokenable_type_tokenable_id",
                table: "personal_access_tokens",
                columns: new[] { "tokenable_type", "tokenable_id" });

            migrationBuilder.CreateIndex(
                name: "IX_recommendations_substance_category_id",
                table: "recommendations",
                column: "substance_category_id");

            migrationBuilder.CreateIndex(
                name: "IX_session_attendances_session_id_user_id",
                table: "session_attendances",
                columns: new[] { "session_id", "user_id" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_session_attendances_user_id",
                table: "session_attendances",
                column: "user_id");

            migrationBuilder.CreateIndex(
                name: "IX_settings_group_name",
                table: "settings",
                columns: new[] { "group", "name" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_substance_categories_slug",
                table: "substance_categories",
                column: "slug",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_substances_substance_category_id",
                table: "substances",
                column: "substance_category_id");

            migrationBuilder.CreateIndex(
                name: "IX_therapy_sessions_group_id",
                table: "therapy_sessions",
                column: "group_id");

            migrationBuilder.CreateIndex(
                name: "IX_therapy_sessions_instructor_id",
                table: "therapy_sessions",
                column: "instructor_id");

            migrationBuilder.CreateIndex(
                name: "IX_therapy_sessions_jitsi_room_name",
                table: "therapy_sessions",
                column: "jitsi_room_name",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_user_substances_substance_id",
                table: "user_substances",
                column: "substance_id");

            migrationBuilder.CreateIndex(
                name: "IX_user_substances_user_id_substance_id",
                table: "user_substances",
                columns: new[] { "user_id", "substance_id" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_user_treatment_types_lookup_value_id",
                table: "user_treatment_types",
                column: "lookup_value_id");

            migrationBuilder.CreateIndex(
                name: "IX_user_treatment_types_user_id_lookup_value_id",
                table: "user_treatment_types",
                columns: new[] { "user_id", "lookup_value_id" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_users_mobile_number",
                table: "users",
                column: "mobile_number",
                unique: true,
                filter: "[mobile_number] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_users_username",
                table: "users",
                column: "username",
                unique: true,
                filter: "[username] IS NOT NULL");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "addiction_profiles");

            migrationBuilder.DropTable(
                name: "cache");

            migrationBuilder.DropTable(
                name: "cache_locks");

            migrationBuilder.DropTable(
                name: "device_sessions");

            migrationBuilder.DropTable(
                name: "group_members");

            migrationBuilder.DropTable(
                name: "notifications");

            migrationBuilder.DropTable(
                name: "payment_methods");

            migrationBuilder.DropTable(
                name: "payments");

            migrationBuilder.DropTable(
                name: "personal_access_tokens");

            migrationBuilder.DropTable(
                name: "recommendations");

            migrationBuilder.DropTable(
                name: "session_attendances");

            migrationBuilder.DropTable(
                name: "settings");

            migrationBuilder.DropTable(
                name: "user_substances");

            migrationBuilder.DropTable(
                name: "user_treatment_types");

            migrationBuilder.DropTable(
                name: "therapy_sessions");

            migrationBuilder.DropTable(
                name: "substances");

            migrationBuilder.DropTable(
                name: "lookup_values");

            migrationBuilder.DropTable(
                name: "groups");

            migrationBuilder.DropTable(
                name: "lookup_types");

            migrationBuilder.DropTable(
                name: "substance_categories");

            migrationBuilder.DropTable(
                name: "users");
        }
    }
}
