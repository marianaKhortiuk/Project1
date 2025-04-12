using CsvHelper;
using CsvHelper.Configuration;
using cub.Entities;

namespace cub.Helpers;

public class CustomStoreAndFwdFlagConverter : CsvHelper.TypeConversion.DefaultTypeConverter
{
    public override object? ConvertFromString(string text, IReaderRow row, MemberMapData memberMapData)
    {
        return text?.ToUpper() switch
        {
            "Y" => StoreAndFwdFlag.Yes,
            "N" => StoreAndFwdFlag.No,
            _ => null
        };
    }
}