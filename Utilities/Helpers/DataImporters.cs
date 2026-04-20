using CsvHelper;
using CsvHelper.Configuration;
using MiniExcelLibs;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Utilities.Helpers
{
    public static class DataImporters
    {
        public static async Task<IEnumerable<T>> ImportCsvAsync<T>(Stream stream)
        {
            using var reader = new StreamReader(stream);
            using var csv = new CsvReader(reader, new CsvConfiguration(CultureInfo.InvariantCulture));

            var records = new List<T>();
            await foreach (T item in csv.GetRecordsAsync<T>())
            {
                records.Add(item);
            }
            return records.AsEnumerable();
        }

        public static async Task<IEnumerable<T>> ImportExcelAsync<T>(Stream stream) where T : class, new() => await stream.QueryAsync<T>();

        public static async Task<IEnumerable<T>?> ImportJsonAsync<T>(Stream stream)
        {
            var options = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true,
                Converters =
                {
                    new JsonStringEnumConverter()
                }
            };
            
            return await JsonSerializer.DeserializeAsync<IEnumerable<T>>(stream, options);
        }
    }
}
