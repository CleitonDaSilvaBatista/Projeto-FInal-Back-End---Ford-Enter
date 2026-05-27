using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace SistemaBancarioSprint3.Migrations
{
    public partial class AddCartoesERegrasSeguranca : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Contas_UsuarioId",
                table: "Contas");

            migrationBuilder.CreateTable(
                name: "Cartoes",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    NomeTitular = table.Column<string>(type: "character varying(120)", maxLength: 120, nullable: false),
                    Numero = table.Column<string>(type: "character varying(19)", maxLength: 19, nullable: false),
                    Validade = table.Column<string>(type: "character varying(5)", maxLength: 5, nullable: false),
                    Cvv = table.Column<string>(type: "character varying(3)", maxLength: 3, nullable: false),
                    Limite = table.Column<decimal>(type: "numeric(18,2)", precision: 18, scale: 2, nullable: false),
                    CriadoEm = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UsuarioId = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Cartoes", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Cartoes_Usuarios_UsuarioId",
                        column: x => x.UsuarioId,
                        principalTable: "Usuarios",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Contas_UsuarioId_Tipo",
                table: "Contas",
                columns: new[] { "UsuarioId", "Tipo" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Cartoes_Numero",
                table: "Cartoes",
                column: "Numero",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Cartoes_UsuarioId",
                table: "Cartoes",
                column: "UsuarioId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(name: "Cartoes");

            migrationBuilder.DropIndex(
                name: "IX_Contas_UsuarioId_Tipo",
                table: "Contas");

            migrationBuilder.CreateIndex(
                name: "IX_Contas_UsuarioId",
                table: "Contas",
                column: "UsuarioId");
        }
    }
}
