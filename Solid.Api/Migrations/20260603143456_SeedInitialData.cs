using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace Solid.Api.Migrations
{
    /// <inheritdoc />
    public partial class SeedInitialData : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "lookup_types",
                columns: new[] { "id", "created_at", "key", "label_ar", "label_en", "updated_at" },
                values: new object[,]
                {
                    { 1L, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "addiction_duration", "مدة الإدمان", "Addiction Duration", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc) },
                    { 2L, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "education_level", "المستوى التعليمي", "Education Level", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc) },
                    { 3L, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "treatment_type", "نوع العلاج", "Treatment Type", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc) }
                });

            migrationBuilder.InsertData(
                table: "settings",
                columns: new[] { "id", "created_at", "group", "name", "payload", "updated_at" },
                values: new object[,]
                {
                    { 1L, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "general", "session_price", "1200", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc) },
                    { 2L, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "general", "group_min_members", "7", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc) },
                    { 3L, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "general", "group_max_members", "15", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc) },
                    { 4L, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "general", "session_duration_minutes", "15", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc) },
                    { 5L, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "general", "booking_cutoff_minutes", "15", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc) },
                    { 6L, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "general", "session_start_hour", "9", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc) },
                    { 7L, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "general", "session_days", "[\"sunday\",\"monday\",\"tuesday\",\"wednesday\"]", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc) },
                    { 8L, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "general", "auto_start_timeout_minutes", "1440", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc) },
                    { 9L, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "content", "privacy_policy", "\"\"", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc) },
                    { 10L, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "content", "terms_and_conditions", "\"\"", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc) }
                });

            migrationBuilder.InsertData(
                table: "substance_categories",
                columns: new[] { "id", "created_at", "is_active", "name_ar", "name_en", "slug", "sort_order", "updated_at" },
                values: new object[,]
                {
                    { 1L, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), true, "المثبطات", "Depressants", "depressants", (byte)1, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc) },
                    { 2L, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), true, "المهدئات", "Sedatives", "sedatives", (byte)2, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc) },
                    { 3L, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), true, "المنشطات", "Stimulants", "stimulants", (byte)3, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc) },
                    { 4L, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), true, "المهلوسات", "Hallucinogens", "hallucinogens", (byte)4, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc) }
                });

            migrationBuilder.InsertData(
                table: "lookup_values",
                columns: new[] { "id", "created_at", "is_active", "label_ar", "label_en", "lookup_type_id", "sort_order", "updated_at", "value_key" },
                values: new object[,]
                {
                    { 1L, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), true, "أقل من 6 أشهر", "Less than 6 months", 1L, (byte)1, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "less_6m" },
                    { 2L, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), true, "6 - 12 شهر", "6 - 12 months", 1L, (byte)2, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "6_12m" },
                    { 3L, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), true, "1 - 3 سنوات", "1 - 3 years", 1L, (byte)3, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "1_3y" },
                    { 4L, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), true, "أكثر من 3 سنوات", "Over 3 years", 1L, (byte)4, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "over_3y" },
                    { 5L, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), true, "بدون تعليم", "No Education", 2L, (byte)1, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "none" },
                    { 6L, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), true, "ابتدائي", "Primary", 2L, (byte)2, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "primary" },
                    { 7L, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), true, "ثانوي", "Secondary", 2L, (byte)3, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "secondary" },
                    { 8L, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), true, "جامعي", "University", 2L, (byte)4, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "university" },
                    { 9L, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), true, "دراسات عليا", "Postgraduate", 2L, (byte)5, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "postgraduate" },
                    { 10L, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), true, "علاج في المستشفى", "Hospital Treatment", 3L, (byte)1, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "hospital" },
                    { 11L, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), true, "علاج خارجي", "Outpatient", 3L, (byte)2, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "outpatient" },
                    { 12L, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), true, "علاج ذاتي", "Self Treatment", 3L, (byte)3, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "self" },
                    { 13L, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), true, "علاج ديني", "Religious", 3L, (byte)4, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "religious" }
                });

            migrationBuilder.InsertData(
                table: "substances",
                columns: new[] { "id", "created_at", "is_active", "name_ar", "name_en", "substance_category_id", "updated_at" },
                values: new object[,]
                {
                    { 1L, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), true, "حشيش", "Hash", 1L, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc) },
                    { 2L, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), true, "بانجو", "Bango", 1L, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc) },
                    { 3L, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), true, "هيدرا", "Hydra", 1L, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc) },
                    { 4L, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), true, "أفيون", "Opium", 2L, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc) },
                    { 5L, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), true, "ترامادول", "Tramadol", 2L, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc) },
                    { 6L, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), true, "هيروين", "Heroin", 2L, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc) },
                    { 7L, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), true, "شابو", "Shabu", 3L, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc) },
                    { 8L, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), true, "كوكايين", "Cocaine", 3L, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc) },
                    { 9L, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), true, "إكستاسي", "Ecstasy", 3L, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc) },
                    { 10L, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), true, "إل إس دي", "LSD", 4L, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc) },
                    { 11L, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), true, "آيس", "Ice (Crystal Meth)", 4L, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc) }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "lookup_values",
                keyColumn: "id",
                keyValue: 1L);

            migrationBuilder.DeleteData(
                table: "lookup_values",
                keyColumn: "id",
                keyValue: 2L);

            migrationBuilder.DeleteData(
                table: "lookup_values",
                keyColumn: "id",
                keyValue: 3L);

            migrationBuilder.DeleteData(
                table: "lookup_values",
                keyColumn: "id",
                keyValue: 4L);

            migrationBuilder.DeleteData(
                table: "lookup_values",
                keyColumn: "id",
                keyValue: 5L);

            migrationBuilder.DeleteData(
                table: "lookup_values",
                keyColumn: "id",
                keyValue: 6L);

            migrationBuilder.DeleteData(
                table: "lookup_values",
                keyColumn: "id",
                keyValue: 7L);

            migrationBuilder.DeleteData(
                table: "lookup_values",
                keyColumn: "id",
                keyValue: 8L);

            migrationBuilder.DeleteData(
                table: "lookup_values",
                keyColumn: "id",
                keyValue: 9L);

            migrationBuilder.DeleteData(
                table: "lookup_values",
                keyColumn: "id",
                keyValue: 10L);

            migrationBuilder.DeleteData(
                table: "lookup_values",
                keyColumn: "id",
                keyValue: 11L);

            migrationBuilder.DeleteData(
                table: "lookup_values",
                keyColumn: "id",
                keyValue: 12L);

            migrationBuilder.DeleteData(
                table: "lookup_values",
                keyColumn: "id",
                keyValue: 13L);

            migrationBuilder.DeleteData(
                table: "settings",
                keyColumn: "id",
                keyValue: 1L);

            migrationBuilder.DeleteData(
                table: "settings",
                keyColumn: "id",
                keyValue: 2L);

            migrationBuilder.DeleteData(
                table: "settings",
                keyColumn: "id",
                keyValue: 3L);

            migrationBuilder.DeleteData(
                table: "settings",
                keyColumn: "id",
                keyValue: 4L);

            migrationBuilder.DeleteData(
                table: "settings",
                keyColumn: "id",
                keyValue: 5L);

            migrationBuilder.DeleteData(
                table: "settings",
                keyColumn: "id",
                keyValue: 6L);

            migrationBuilder.DeleteData(
                table: "settings",
                keyColumn: "id",
                keyValue: 7L);

            migrationBuilder.DeleteData(
                table: "settings",
                keyColumn: "id",
                keyValue: 8L);

            migrationBuilder.DeleteData(
                table: "settings",
                keyColumn: "id",
                keyValue: 9L);

            migrationBuilder.DeleteData(
                table: "settings",
                keyColumn: "id",
                keyValue: 10L);

            migrationBuilder.DeleteData(
                table: "substances",
                keyColumn: "id",
                keyValue: 1L);

            migrationBuilder.DeleteData(
                table: "substances",
                keyColumn: "id",
                keyValue: 2L);

            migrationBuilder.DeleteData(
                table: "substances",
                keyColumn: "id",
                keyValue: 3L);

            migrationBuilder.DeleteData(
                table: "substances",
                keyColumn: "id",
                keyValue: 4L);

            migrationBuilder.DeleteData(
                table: "substances",
                keyColumn: "id",
                keyValue: 5L);

            migrationBuilder.DeleteData(
                table: "substances",
                keyColumn: "id",
                keyValue: 6L);

            migrationBuilder.DeleteData(
                table: "substances",
                keyColumn: "id",
                keyValue: 7L);

            migrationBuilder.DeleteData(
                table: "substances",
                keyColumn: "id",
                keyValue: 8L);

            migrationBuilder.DeleteData(
                table: "substances",
                keyColumn: "id",
                keyValue: 9L);

            migrationBuilder.DeleteData(
                table: "substances",
                keyColumn: "id",
                keyValue: 10L);

            migrationBuilder.DeleteData(
                table: "substances",
                keyColumn: "id",
                keyValue: 11L);

            migrationBuilder.DeleteData(
                table: "lookup_types",
                keyColumn: "id",
                keyValue: 1L);

            migrationBuilder.DeleteData(
                table: "lookup_types",
                keyColumn: "id",
                keyValue: 2L);

            migrationBuilder.DeleteData(
                table: "lookup_types",
                keyColumn: "id",
                keyValue: 3L);

            migrationBuilder.DeleteData(
                table: "substance_categories",
                keyColumn: "id",
                keyValue: 1L);

            migrationBuilder.DeleteData(
                table: "substance_categories",
                keyColumn: "id",
                keyValue: 2L);

            migrationBuilder.DeleteData(
                table: "substance_categories",
                keyColumn: "id",
                keyValue: 3L);

            migrationBuilder.DeleteData(
                table: "substance_categories",
                keyColumn: "id",
                keyValue: 4L);
        }
    }
}
