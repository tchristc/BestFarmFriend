using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BestFarmFriend.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddLocationTimezone : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // The Timezone column is already included in InitialCreate for new databases.
            // For databases that pre-date migrations, the column is added at startup in Program.cs
            // before MigrateAsync is called. This migration serves as a version marker only.
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
        }
    }
}

