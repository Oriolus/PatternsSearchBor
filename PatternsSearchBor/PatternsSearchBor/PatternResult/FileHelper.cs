using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using PatternsSearchBor.Model;

namespace PatternsSearchBor.PatternResult
{
    // @Deprecated
    public class FileHelper
    {

        private string Splitter { get; set; } = "\t\t";

        public FileHelper(string splitter)
        {
            Splitter = splitter;
        }

        public FileHelper()
        {
        }

        public void SaveToFile(string filename, Tree tree)
        {
            using (var fileOut = File.OpenWrite(filename))
            {
                using (var fileWriter = new StreamWriter(fileOut, Encoding.UTF8))
                {
                    tree.LogBytes(str => fileWriter.WriteLine(str));
                }
            }
        }

        public List<Result> LoadFromFile(string filename)
        {
            List<Result> result = new List<Result>();
            using (var fileStream = File.OpenRead(filename))
            {
                using (var streamReader = new StreamReader(fileStream, Encoding.UTF8))
                {
                    string line = null;
                    while ((line = streamReader.ReadLine()) != null)
                    {
                        Result res = null;
                        if (tryParseLine(line, out res))
                        {
                            result.Add(res);
                        }
                        else
                        {
                            Console.WriteLine($"Error with: {line}");
                        }
                    }
                }
            }
            return result;
        }

        public void StreamLoadFromFile(string filename, Action<Result> onLineParsed, Action<string> onParseError)
        {
            List<Result> result = new List<Result>();
            using (var fileStream = File.OpenRead(filename))
            {
                using (var streamReader = new StreamReader(fileStream, Encoding.UTF8))
                {
                    string line = null;
                    while ((line = streamReader.ReadLine()) != null)
                    {
                        Result res = null;
                        if (tryParseLine(line, out res))
                        {
                            onLineParsed.Invoke(res);
                        }
                        else
                        {
                            onParseError(line);
                        }
                    }
                }
            }
        }

        private bool tryParseLine(string line, out Result result)
        {
            string[] parts = line.Split(new string[] { Splitter }, StringSplitOptions.None);

            if (parts.Length == 2 && int.TryParse(parts[0], out int count))
            {
                result = new Result() { HintCount = count, Sentence = parts[1].Trim() };
            }
            else
            {
                result = null;
                return false;
            }

            return true;
        }

    }
}
