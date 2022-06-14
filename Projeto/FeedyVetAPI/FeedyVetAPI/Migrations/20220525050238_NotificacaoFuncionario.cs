using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace FeedyVetAPI.Migrations
{
    public partial class NotificacaoFuncionario : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "NotificacaoFuncionario",
                columns: table => new
                {
                    IdNotificacao = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    IdFuncionario = table.Column<int>(nullable: false),
                    Estado = table.Column<string>(nullable: false),
                    Descricao = table.Column<string>(nullable: true),
                    DataNotificacao = table.Column<DateTime>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_NotificacaoFuncionario", x => x.IdNotificacao);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "NotificacaoFuncionario");
        }
    }
}
