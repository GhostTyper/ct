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
    }
}
