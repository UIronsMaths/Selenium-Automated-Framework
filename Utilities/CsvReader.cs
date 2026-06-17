using CsvHelper;
using System.Globalization;
using System.IO;
using System.Collections.Generic;
using System.Linq;

public class CsvUserReader
{
    public static TestSettings settings = ConfigurationManager.Settings;

    public static List<UserData> ReadCsv(string file = "users.csv")
    {
        string fullPath = Path.Combine(settings.BaseDirectory, file);
        LogManager.Step(fullPath);

        using var reader = new StreamReader(fullPath);
        using var csv = new CsvReader(reader, CultureInfo.InvariantCulture);
        return csv.GetRecords<UserData>().ToList();
    }
}