using System;
using System.IO;
using System.IO.Compression;
using System.Threading.Tasks;

namespace ct
{
    class Program
    {
        static async Task Main(string[] args)
        {
            DemoCandles();

            string path = Path.Combine(AppContext.BaseDirectory, "..", "..", "..", "bybit", "BTCUSD2020-03-12.csv.gz");
            using FileStream file = File.OpenRead(path);
            using GZipStream zip = new GZipStream(file, CompressionMode.Decompress);

            DataSource source = new ByBitPublicFileDataSource(zip);
            await foreach (Message message in source.GetMessagesAsync())
            {
                Console.WriteLine(message.ToString());
                if (message.Kind == MessageKind.EndOfData)
                {
                    break;
                }
            }
        }

        private static void DemoCandles()
        {
            DateTime start = new DateTime(2024, 1, 1, 0, 0, 0, DateTimeKind.Utc);
            Candles candles = new Candles(TimeFrame.Minutes1);
            for (int i = 0; i < 10; i++)
            {
                DateTime stamp = start.AddMinutes(i).AddSeconds(10);
                candles.AddTrade(stamp, i + 1.0, 1.0);
            }

            foreach (Candle candle in candles.Enumerate(TimeFrame.Minutes5, start, start.AddMinutes(10)))
            {
                Console.WriteLine($"{candle.Stamp:O} O:{candle.Open} H:{candle.High} L:{candle.Low} C:{candle.Close} V:{candle.Volume}");
            }
        }
    }
}
