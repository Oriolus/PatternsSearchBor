using PatternsSearchBor.PatternResult;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using PatternsSearchBor.Algorithm;
using PatternsSearchBor.Model;

namespace PatternsSearchBor
{
    class Program
    {

        private static string FileName = @"output.txt";
        private static string ResultFile = @"result_01.txt";

        private static void CreatePatterns(int repeatCount = 10000)
        {
            var result = new StringsGenerator().Generate(repeatCount);

            using (var file = File.Open(FileName, FileMode.OpenOrCreate))
            {
                using (StreamWriter writer = new StreamWriter(file, Encoding.UTF8))
                {
                    foreach (string _s in result)
                    {
                        writer.WriteLine(_s);
                    }
                }
            }
        }

        private static Action<string> Print = s => Console.WriteLine(s);

        private static void findPatterns()
        {
            //File.Delete(FileName);
            //CreatePatterns(15000);

            NodeDeduplicator deduplicator = new InverseNodeDeduplicator();

            Tree tree = Tree.GetRootedTree(); // new Tree();
            tree.Print = Print;

            int count = 0;
            using (var fileStream = File.OpenRead(FileName))
            {
                using (StreamReader reader = new StreamReader(fileStream, Encoding.UTF8))
                {
                    string newLine = string.Empty;
                    while ((newLine = reader.ReadLine()) != null)
                    {
                        if (!string.IsNullOrWhiteSpace(newLine))
                        {
                            newLine = newLine + " " + Node.LineEnd;
                            string[] words = newLine.Split(' ');

                            Node current = tree.Root;
                            foreach (string word in words)
                            {
                                current = current.AddOrGet(word);
                                current.Print = Print;
                            }
                            count++;

                        }
                    }
                }
            }

            Console.WriteLine();

            Stopwatch cwDeduplicate = new Stopwatch();

            cwDeduplicate.Start();

            deduplicator.Deduplicate(tree.Root);

            cwDeduplicate.Stop();

            tree.BeautyLog();

            Console.WriteLine($"{count}. Duration: {cwDeduplicate.ElapsedMilliseconds / 1000.0}");

            //FileHelper fileHelper = new FileHelper();
            //fileHelper.SaveToFile(ResultFile, tree);
        }


        private static void refindPatternds()
        {
            bool logAll = false;
            string loadFilename = ResultFile;
            string splitter = "\t\t";

            NodeDeduplicator deduplicator = new InverseNodeDeduplicator();

            Tree tree = Tree.GetRootedTree();
            tree.Print = Print;
            tree.Root.Print = Print;

            tree.LogEnabled = logAll;

            FileHelper fh = new FileHelper(splitter);

            var resultLines = fh.LoadFromFile(loadFilename);

            int count = 0;
            foreach (var res in resultLines)
            {
                var sent = res.Sentence.Replace("^ ", "").Trim();
                Node current = tree.Root;

                string[] words = sent.Split(' ');
                foreach (string word in words)
                {
                    current = current.AddOrGet(word);
                    current.IsTemplateValue = Node.IsTemplateNode(current.Value);
                }
                count++;

                if (count % 10 == 0)
                {
                    deduplicator.Deduplicate(tree.Root);
                }

                current.HintCount = res.HintCount;
            }

            if (logAll) tree.BeautyLog();

            deduplicator.Deduplicate(tree.Root);

            tree.BeautyLog();

            string toFile = "new_file_02.txt";

            if (File.Exists(toFile)) File.Delete(toFile);
            fh.SaveToFile(toFile, tree);
        }

        static void Main(string[] args)
        {
            findPatterns();

            Console.WriteLine("Done!");
            Console.ReadLine();
        }
    }
}
