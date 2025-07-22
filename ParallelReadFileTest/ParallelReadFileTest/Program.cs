using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ParallelReadFileTest
{
    class Program
    {
        static void Main(string[] args)
        {
            var dir = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "trade1_PRE");
            var files = Directory.GetFiles(dir, "*.vxm");
            Console.WriteLine($"文件数量：{files.Length}");
            Stopwatch sw = new Stopwatch();
            sw.Start();
            var fileContents = ParallelReadFile(files, Environment.ProcessorCount);
            sw.Stop();
            Console.WriteLine($"并发读取耗时ms：{sw.ElapsedMilliseconds}");
            sw.Reset();
            sw.Start();
            ReadFiles(files);
            sw.Stop();
            Console.WriteLine($"顺序读取耗时ms：{sw.ElapsedMilliseconds}");
            Console.ReadLine();
        }

        public static IDictionary<string, string> ParallelReadFile(string[] files, int maxThread = 4)
        {
            var fileCount = files.Length;
            var section = (int)Math.Ceiling((fileCount * 1.0) / maxThread);
            var readResult = new Dictionary<string, string>();
            ParallelLoopResult result = Parallel.For(0, maxThread, threadIndex =>
            {
                var beginIndex = section * threadIndex;
                var endIndex = beginIndex + section;
                if (endIndex > fileCount)
                {
                    endIndex = fileCount;
                }
                for (int i = beginIndex; i < endIndex; i++)
                {
                    var filePath = files[i];
                    var content = File.ReadAllText(filePath);
                    readResult.Add(filePath, content);
                }
            });
            return readResult;
        }

        public static IDictionary<string, string> ReadFiles(string[] files)
        {
            var readResult = new Dictionary<string, string>();
            for (int i = 0; i < files.Length; i++)
            {
                var filePath = files[i];
                var content = File.ReadAllText(filePath);
                readResult.Add(filePath, content);
            }
            return readResult;
        }
    }
}
