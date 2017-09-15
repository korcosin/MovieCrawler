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

            if (new System.IO.FileInfo(_rootdir + "\\history\\recent.history").Exists)
            {
                Console.Write("[info] read history data...");
                string line;
                using (System.IO.StreamReader reader = new System.IO.StreamReader(_rootdir + "\\history\\recent.history"))
                {
                    while ((line = reader.ReadLine()) != null)
                    {
                        crawledData.Add(line);
                    }
                }
                Console.WriteLine("[OK][{0}]", DateTime.Now.ToString("yyyymmddhhmmss"));
            }

            return crawledData;
        }

        public void backupFile()
        {
            if (new System.IO.FileInfo(_rootdir + "\\history\\recent.history").Exists)
            {
                System.IO.DirectoryInfo dirInfo = new System.IO.DirectoryInfo(_rootdir + "\\history\\backup");

                if (!dirInfo.Exists) dirInfo.Create();

                Console.Write("[info] backup...");
                System.IO.File.Move(_rootdir + "\\history\\recent.history", _rootdir + "\\history\\backup\\" + DateTime.Now.ToString("yyyymmddhhmmss"));
                Console.WriteLine("[OK][{0}]", DateTime.Now.ToString("yyyymmddhhmmss"));
            }
        }

        public void createFile()
        {
            System.IO.DirectoryInfo dirInfo = new System.IO.DirectoryInfo(_rootdir + "\\history");

            if (!dirInfo.Exists) dirInfo.Create();

            _writer = new System.IO.StreamWriter(_rootdir + "\\history\\recent.history");
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
