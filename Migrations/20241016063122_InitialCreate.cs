using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace HotelApi.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Clientes",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Nome = table.Column<string>(type: "text", nullable: false),
                    CPF = table.Column<string>(type: "text", nullable: false),
                    Profissao = table.Column<string>(type: "text", nullable: true),
                    Nacionalidade = table.Column<string>(type: "text", nullable: true),
                    DataNascimento = table.Column<string>(type: "text", nullable: true),
                    Sexo = table.Column<string>(type: "text", nullable: true),
                    RG = table.Column<string>(type: "text", nullable: true),
                    Residencia = table.Column<string>(type: "text", nullable: true),
                    CEP = table.Column<string>(type: "text", nullable: true),
                    Cidade = table.Column<string>(type: "text", nullable: true),
                    Pais = table.Column<string>(type: "text", nullable: true),
                    MotivoViagem = table.Column<string>(type: "text", nullable: true),
                    MeioTransporte = table.Column<string>(type: "text", nullable: true),
                    ProximoDestino = table.Column<string>(type: "text", nullable: true),
                    TelefoneResidencial = table.Column<string>(type: "text", nullable: true),
                    TelefoneComercial = table.Column<string>(type: "text", nullable: true),
                    DataEntrada = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    DataSaida = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    Acompanhante = table.Column<string>(type: "text", nullable: true),
                    ValorEstadia = table.Column<decimal>(type: "numeric", nullable: true),
                    Apartamento = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Clientes", x => x.Id);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Clientes");
        }
    }
}
