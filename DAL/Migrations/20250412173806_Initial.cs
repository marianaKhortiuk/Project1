using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace cub.Migrations
{
    /// <inheritdoc />
    public partial class Initial : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Trips",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    tpep_pickup_datetime = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    tpep_dropoff_datetime = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    passenger_count = table.Column<int>(type: "int", nullable: true),
                    trip_distance = table.Column<float>(type: "real", nullable: true),
                    store_and_fwd_flag = table.Column<int>(type: "int", nullable: true),
                    PULocationID = table.Column<int>(type: "int", nullable: true),
                    DOLocationID = table.Column<int>(type: "int", nullable: true),
                    fare_amount = table.Column<float>(type: "real", nullable: true),
                    tip_amount = table.Column<float>(type: "real", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Trips", x => x.Id);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Trips");
        }
    }
}
