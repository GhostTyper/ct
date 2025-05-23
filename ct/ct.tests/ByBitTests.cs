using System;
using System.IO;
using System.IO.Compression;
using System.Threading.Tasks;
using Xunit;

namespace ct.tests
{
    public class ByBitTests
    {
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
            long volume = 0L;
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

        [Fact]
        public async Task Order2020IsChronological()
        {
            bool order = await TestOrderAsync("BTCUSD2020-03-12.csv.gz");
            Assert.True(order);
        }

        [Fact]
        public async Task Order2022IsChronological()
        {
            bool order = await TestOrderAsync("BTCUSD2022-06-28.csv.gz");
            Assert.True(order);
        }

        [Fact]
        public async Task Order2025IsChronological()
        {
            bool order = await TestOrderAsync("BTCUSD2025-04-29.csv.gz");
            Assert.True(order);
        }

        [Fact]
        public async Task Volume2020Matches()
        {
            long volume = await SumVolumeAsync("BTCUSD2020-03-12.csv.gz");
            Assert.Equal(2940209655L, volume);
        }

        [Fact]
        public async Task Volume2022Matches()
        {
            long volume = await SumVolumeAsync("BTCUSD2022-06-28.csv.gz");
            Assert.Equal(927092462L, volume);
        }

        [Fact]
        public async Task Volume2025Matches()
        {
            long volume = await SumVolumeAsync("BTCUSD2025-04-29.csv.gz");
            Assert.Equal(572018150L, volume);
        }
    }
}
