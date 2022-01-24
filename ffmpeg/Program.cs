using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Xabe.FFmpeg;

namespace ffmpeg
{
    class Program
    {
        static async Task Main(string[] args)
        {
            var p = new Program();
            await p.RunAsync();
            Console.WriteLine("Hello World!");
        }

        public async Task RunAsync()
        {
            var outputPath = @"C:\files\02.mp4";
            var input = @"C:\files\02.mkv";

            IMediaInfo mediaInfo = await FFmpeg.GetMediaInfo(input);

            IStream videoStream = mediaInfo.VideoStreams.First();
            IStream audioStream = mediaInfo.AudioStreams.First();

            var conversion = FFmpeg.Conversions
                .New()
                .AddStream(audioStream, videoStream)
                .AddParameter("-movflags faststart")
                .AddParameter($"-ss {TimeSpan.FromSeconds(0)} -t {TimeSpan.FromSeconds(30)}")
                .SetOutput(outputPath)
                .UseMultiThread(true);

            Console.WriteLine(conversion.Build());

            conversion.OnProgress += (sender, args) =>
            {
                var percent = (int)(Math.Round(args.Duration.TotalSeconds / args.TotalLength.TotalSeconds, 2) * 100);
                Console.WriteLine($"[{args.Duration} / {args.TotalLength}] {percent}%");
            };

            await conversion.Start();
        }
    }
}
