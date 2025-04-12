using CsvHelper;
using CsvHelper.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Globalization;
using cub;
using cub.Entities;
using cub.Helpers;
using Microsoft.EntityFrameworkCore;

class Service
{
    static async Task Main(string[] args)
    {
        string filePath = "/Users/marianakhortiuk/.dotnet/work/sample-cab-data.csv";

        try
        {
            var records = ReadCsvFile(filePath);

            var dataTable = ConvertToDataTable(records);

            await SaveDataToDatabase(dataTable);

            await RemoveDuplicatesFromDatabase();

            Console.WriteLine("done");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"error: {ex.Message}");
        }
    }

    public static async Task RemoveDuplicatesFromDatabase()
    {
        using (var dbContext = new AppDbContext())
        {
            var trips = await dbContext.Trips.ToListAsync();

            var duplicates = trips
                .GroupBy(x => new { x.tpep_pickup_datetime, x.PULocationID, x.DOLocationID })
                .Where(g => g.Count() > 1)
                .SelectMany(g => g.Skip(1))
                .ToList();

            if (duplicates.Any())
            {
                await WriteDuplicatesToCsv(duplicates);
            }

            dbContext.Trips.RemoveRange(duplicates);

            await dbContext.SaveChangesAsync();

            Console.WriteLine("Duplicates removed");
        }
    }

    public static async Task WriteDuplicatesToCsv(List<Trip> duplicates)
    {
        string filePath = "duplicates.csv";

        using (var writer = new StreamWriter(filePath))
        using (var csv = new CsvWriter(writer, new CsvConfiguration(CultureInfo.InvariantCulture)))
        {
            csv.WriteHeader<Trip>();
            await csv.NextRecordAsync();

            await csv.WriteRecordsAsync(duplicates);

            Console.WriteLine("Duplicates was written to duplicates.csv file");
        }
    }

    public static IEnumerable<Trip> ReadCsvFile(string filePath)
    {
        var config = new CsvConfiguration(CultureInfo.InvariantCulture)
        {
            HasHeaderRecord = true,
            MissingFieldFound = null,
            BadDataFound = null,
            HeaderValidated = null,
            TrimOptions = TrimOptions.Trim,
            IgnoreBlankLines = true
        };

        using (var reader = new StreamReader(filePath))
        using (var csv = new CsvReader(reader, config))
        {
            csv.Context.RegisterClassMap<TripMap>();

            return csv.GetRecords<Trip>().ToList();
        }
    }

    public static DataTable ConvertToDataTable(IEnumerable<Trip> records)
    {
        var dataTable = new DataTable();

        dataTable.Columns.Add("tpep_pickup_datetime", typeof(DateTime));
        dataTable.Columns.Add("tpep_dropoff_datetime", typeof(DateTime));
        dataTable.Columns.Add("passenger_count", typeof(int));
        dataTable.Columns.Add("trip_distance", typeof(float));
        dataTable.Columns.Add("store_and_fwd_flag", typeof(StoreAndFwdFlag));
        dataTable.Columns.Add("PULocationID", typeof(int));
        dataTable.Columns.Add("DOLocationID", typeof(int));
        dataTable.Columns.Add("fare_amount", typeof(float));
        dataTable.Columns.Add("tip_amount", typeof(float));

        foreach (var record in records)
        {
            var row = dataTable.NewRow();
            row["tpep_pickup_datetime"] = record.tpep_pickup_datetime;
            row["tpep_dropoff_datetime"] = record.tpep_dropoff_datetime;
            row["passenger_count"] = record.passenger_count ?? (object)DBNull.Value;
            row["trip_distance"] = record.trip_distance;
            row["store_and_fwd_flag"] = record.store_and_fwd_flag.HasValue
                ? record.store_and_fwd_flag.Value
                : (object)DBNull.Value;
            row["PULocationID"] = record.PULocationID;
            row["DOLocationID"] = record.DOLocationID;
            row["fare_amount"] = record.fare_amount;
            row["tip_amount"] = record.tip_amount ?? (object)DBNull.Value;
            dataTable.Rows.Add(row);
        }

        return dataTable;
    }
    
    public static async Task SaveDataToDatabase(DataTable dataTable)
    {
        string connectionString =
            "Server=localhost,1433;Database=cab47;User ID=SA;Password=7MyStrongPass123;Encrypt=False;TrustServerCertificate=True;Connection Timeout=30;";

        using (var connection = new SqlConnection(connectionString))
        {
            await connection.OpenAsync();

            foreach (DataRow row in dataTable.Rows)
            {
                var pickupDateTime = (DateTime)row["tpep_pickup_datetime"];
                var dropoffDateTime = (DateTime)row["tpep_dropoff_datetime"];

                var utcPickupDateTime = TimeZoneInfo.ConvertTimeToUtc(
                    pickupDateTime,
                    TimeZoneInfo.FindSystemTimeZoneById("Eastern Standard Time"));

                var utcDropoffDateTime = TimeZoneInfo.ConvertTimeToUtc(
                    dropoffDateTime,
                    TimeZoneInfo.FindSystemTimeZoneById("Eastern Standard Time"));

                var command = new SqlCommand(
                    "INSERT INTO Trips (tpep_pickup_datetime, tpep_dropoff_datetime, passenger_count, trip_distance, store_and_fwd_flag, PULocationID, DOLocationID, fare_amount, tip_amount) " +
                    "VALUES (@pickup, @dropoff, @passengerCount, @tripDistance, @storeAndFwdFlag, @puLocationID, @doLocationID, @fareAmount, @tipAmount)",
                    connection);

                command.Parameters.AddWithValue("@pickup", utcPickupDateTime);
                command.Parameters.AddWithValue("@dropoff", utcDropoffDateTime);
                command.Parameters.AddWithValue("@passengerCount", row["passenger_count"]);
                command.Parameters.AddWithValue("@tripDistance", row["trip_distance"]);
                command.Parameters.AddWithValue("@storeAndFwdFlag", row["store_and_fwd_flag"]);
                command.Parameters.AddWithValue("@puLocationID", row["PULocationID"]);
                command.Parameters.AddWithValue("@doLocationID", row["DOLocationID"]);
                command.Parameters.AddWithValue("@fareAmount", row["fare_amount"]);
                command.Parameters.AddWithValue("@tipAmount", row["tip_amount"]);

                await command.ExecuteNonQueryAsync();
            }

            await connection.CloseAsync();
            Console.WriteLine("The data is successfully written to the database.");
        }
    }
}