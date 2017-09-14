using System;
using System.Collections.Generic;

namespace GitHub.KorCosin.MovieCrawler.Scan
{
    public class History
    {
        string _rootdir;
        System.IO.StreamWriter _writer;
        public History(string rootdir)
        {
            this._rootdir = rootdir;
        }

        public List<string> getCrawledDataList()
        {
            List<string> crawledData = new List<string>();

            Console.Write("[info] read history data...");
            string line;
            using (System.IO.StreamReader reader = new System.IO.StreamReader(_rootdir + "\\recent.history"))
            {
                while ((line = reader.ReadLine()) != null)
                {
                    crawledData.Add(line);
                }
            }
            Console.WriteLine("[OK][{0}]", DateTime.Now.ToString("yyyymmddhhmmss"));

            return crawledData;
        }

        public void backupFile()
        {
            Console.Write("[info] backup...");
            System.IO.File.Move(_rootdir + "\\recent.history", _rootdir + "\\backup\\" + DateTime.Now.ToString("yyyymmddhhmmss"));
            Console.WriteLine("[OK][{0}]", DateTime.Now.ToString("yyyymmddhhmmss"));
        }

        public void createFile()
        {
            _writer = new System.IO.StreamWriter(_rootdir + "\\recent.history");
        }

        public void writeInFile(string line)
        {
            _writer.WriteLine(line);
            _writer.Flush();
        }

        public void closeFile()
        {
            _writer.Close();
        }
    }
}
