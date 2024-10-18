using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HotelApi.Migrations
{
    /// <inheritdoc />
    public partial class AddReservationModel : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Acompanhante",
                table: "Clientes");

            migrationBuilder.DropColumn(
                name: "Apartamento",
                table: "Clientes");

            migrationBuilder.DropColumn(
                name: "DataEntrada",
                table: "Clientes");

            migrationBuilder.DropColumn(
                name: "DataSaida",
                table: "Clientes");

            migrationBuilder.DropColumn(
                name: "MeioTransporte",
                table: "Clientes");

            migrationBuilder.DropColumn(
                name: "MotivoViagem",
                table: "Clientes");

            migrationBuilder.DropColumn(
                name: "ProximoDestino",
                table: "Clientes");

            migrationBuilder.DropColumn(
                name: "ValorEstadia",
                table: "Clientes");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Acompanhante",
                table: "Clientes",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Apartamento",
                table: "Clientes",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "DataEntrada",
                table: "Clientes",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "DataSaida",
                table: "Clientes",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "MeioTransporte",
                table: "Clientes",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "MotivoViagem",
                table: "Clientes",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ProximoDestino",
                table: "Clientes",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "ValorEstadia",
                table: "Clientes",
                type: "numeric",
                nullable: true);
        }
    }
}
