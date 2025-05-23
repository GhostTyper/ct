using System;
using System.IO;
using System.IO.Compression;
using System.Threading.Tasks;

namespace ct.tests
{
    class Program
    {
        static async Task Main()
        {
            await RunTests();
        }

        private static async Task RunTests()
        {
            bool order2020 = await TestOrderAsync("BTCUSD2020-03-12.csv.gz");
            bool order2022 = await TestOrderAsync("BTCUSD2022-06-28.csv.gz");
            bool order2025 = await TestOrderAsync("BTCUSD2025-04-29.csv.gz");

            long volume2020 = await SumVolumeAsync("BTCUSD2020-03-12.csv.gz");
            long volume2022 = await SumVolumeAsync("BTCUSD2022-06-28.csv.gz");
            long volume2025 = await SumVolumeAsync("BTCUSD2025-04-29.csv.gz");

            if (volume2020 != 2940209655 ||
                volume2022 != 927092462 ||
                volume2025 != 572018150)
            {
                throw new Exception("Volume test failed.");
            }

            if (!order2020 || !order2022 || !order2025)
            {
                throw new Exception("Order test failed.");
            }

            Console.WriteLine("All tests passed.");
        }

        private static async Task<bool> TestOrderAsync(string fileName)
        {
            string path = Path.Combine(AppContext.BaseDirectory, "..", "..", "..", "..", "ct", "bybit", fileName);
            using FileStream file = File.OpenRead(path);
            using GZipStream zip = new GZipStream(file, CompressionMode.Decompress);

            DataSource source = new ByBitPublicFileDataSource(zip);
            DateTime? previousStamp = null;
            await foreach (Message message in source.GetMessagesAsync())
            {
                if (message.Kind == MessageKind.Trade)
                {
                    if (previousStamp.HasValue && message.Stamp < previousStamp.Value)
                    {
                        return false;
                    }
                    previousStamp = message.Stamp;
                }
            }
            return true;
        }

        private static async Task<long> SumVolumeAsync(string fileName)
        {
            string path = Path.Combine(AppContext.BaseDirectory, "..", "..", "..", "..", "ct", "bybit", fileName);
            using FileStream file = File.OpenRead(path);
            using GZipStream zip = new GZipStream(file, CompressionMode.Decompress);

            DataSource source = new ByBitPublicFileDataSource(zip);
            long volume = 0;
            await foreach (Message message in source.GetMessagesAsync())
            {
                if (message.Kind == MessageKind.Trade)
                {
                    TradeMessage trade = (TradeMessage)message;
                    volume += (long)trade.Volume;
                }
            }

            return volume;
        }
    }
}
