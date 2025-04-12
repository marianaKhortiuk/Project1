using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace cub.Migrations
{
    /// <inheritdoc />
    public partial class YesOrNo : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"
    CREATE TABLE TempTrips
    (
        Id INT PRIMARY KEY,
        store_and_fwd_flag INT
    )
");

            migrationBuilder.Sql(@"
    INSERT INTO TempTrips (Id, store_and_fwd_flag)
    SELECT Id, store_and_fwd_flag
    FROM Trips
");

            migrationBuilder.AlterColumn<string>(
                name: "store_and_fwd_flag",
                table: "Trips",
                type: "VARCHAR(3)",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "INT"
            );

            migrationBuilder.Sql(@"
    UPDATE Trips
    SET store_and_fwd_flag = CASE
        WHEN TempTrips.store_and_fwd_flag = 1 THEN 'Yes'
        WHEN TempTrips.store_and_fwd_flag = 0 THEN 'No'
        ELSE Trips.store_and_fwd_flag
    END
    FROM Trips
    INNER JOIN TempTrips ON Trips.Id = TempTrips.Id
");

            migrationBuilder.Sql("DROP TABLE TempTrips");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<int>(
                name: "store_and_fwd_flag",
                table: "Trips",
                type: "INT",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "VARCHAR(3)"
            );
        }
    }
}
