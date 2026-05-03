using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BiblioApp.Migrations
{
    /// <inheritdoc />
    public partial class AddEmailAndEcheanceToEmprunt : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // Add EmailEmprunteur column (default empty string for existing rows)
            migrationBuilder.AddColumn<string>(
                name: "EmailEmprunteur",
                table: "Emprunts",
                type: "nvarchar(256)",
                maxLength: 256,
                nullable: false,
                defaultValue: "");

            // Add DateEcheance column (default = DateEmprunt + 14 days for existing rows)
            migrationBuilder.AddColumn<DateTime>(
                name: "DateEcheance",
                table: "Emprunts",
                type: "datetime2",
                nullable: false,
                defaultValueSql: "DATEADD(day, 14, DateEmprunt)");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(name: "EmailEmprunteur", table: "Emprunts");
            migrationBuilder.DropColumn(name: "DateEcheance",    table: "Emprunts");
        }
    }
}
