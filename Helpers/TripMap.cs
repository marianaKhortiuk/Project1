using CsvHelper.Configuration;

namespace cub.Helpers;

public class TripMap : ClassMap<Trip>
{
    public TripMap()
    {
        Map(m => m.tpep_pickup_datetime).Name("tpep_pickup_datetime");
        Map(m => m.tpep_dropoff_datetime).Name("tpep_dropoff_datetime");
        Map(m => m.passenger_count).Name("passenger_count");
        Map(m => m.trip_distance).Name("trip_distance");
        Map(m => m.store_and_fwd_flag).Name("store_and_fwd_flag").TypeConverter<CustomStoreAndFwdFlagConverter>();
        Map(m => m.PULocationID).Name("PULocationID");
        Map(m => m.DOLocationID).Name("DOLocationID");
        Map(m => m.fare_amount).Name("fare_amount");
        Map(m => m.tip_amount).Name("tip_amount");
    }
}
