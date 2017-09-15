using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;

namespace GitHub.KorCosin.MovieCrawler.Args
{
    public class Parser
    {
        private string path = string.Empty;
        private KobisInfo kobisInfo;
        private TmdbInfo tmdbInfo;

        public Parser(string[] args)
        {
            this.path = string.Format("{0}\\{1}",
                                System.IO.Path.GetDirectoryName(
                                        System.IO.Path.GetDirectoryName(
                                            System.IO.Directory.GetCurrentDirectory()
                                        )
                                ), "config\\config.xml");

            this.kobisInfo = new KobisInfo();
            this.tmdbInfo = new TmdbInfo();

            argsParsing(args);
            init();
        }

        private void argsParsing(string[] args)
        {
            for (int idx = 0; idx < args.Length; idx++)
            {
                if (args[idx].StartsWith("-path"))
                {
                    this.path = args[++idx];
                }
            }
        }

        private void init()
        {
            XDocument xDoc = XDocument.Load(this.path);
            
            var rootKobis = xDoc.Descendants("kobis");
            var service = rootKobis.Descendants("service").Descendants("item").Select(info => info);
            var request = rootKobis.Descendants("request").Descendants("item").Select(info => info);

            // API Key
            this.kobisInfo.Key = rootKobis.Descendants("key").SingleOrDefault().Value;

            // Service URL
            foreach (var item in service)
            {
                string id = item.Attribute("id").Value;
                string active = item.Attribute("active").Value;
                string url = item.Element("url").Value;

                if (active == "Y") this.kobisInfo.Target = id;

                this.kobisInfo.Services.Add(id, url);
            }

            this.kobisInfo.RootDir = rootKobis.Descendants("rootdir").SingleOrDefault().Value;

            // Request Params
            this.kobisInfo.Params.Add("targetDt", DateTime.Now.AddDays(-1).ToString("yyyyMMdd"));
            //this.kobisInfo.Params.Add("curPage", "1");
            foreach (var item in request)
            {
                string id = item.Attribute("id").Value;
                string val = item.Value;

                this.kobisInfo.Params.Add(id, val);
            }

            var rootTmdb = xDoc.Descendants("tmdb");
            service = rootTmdb.Descendants("service").Descendants("item").Select(info => info);

            // API Key
            this.tmdbInfo.Key = rootTmdb.Descendants("key").SingleOrDefault().Value;

            // Service URL
            foreach (var item in service)
            {
                string id = item.Attribute("id").Value;
                string url = item.Element("url").Value;

                this.tmdbInfo.Services.Add(id, url);
            }

            this.tmdbInfo.DownloadPath = rootTmdb.Descendants("download").Descendants("directory").SingleOrDefault().Value;
            this.tmdbInfo.PosterRootUrl = rootTmdb.Descendants("image").Descendants("rooturl").Descendants("poster").SingleOrDefault().Value;
            this.tmdbInfo.ThumbnailRootUrl = rootTmdb.Descendants("image").Descendants("rooturl").Descendants("thumbnail").SingleOrDefault().Value;
        }
        
        public KobisInfo getKobisInfo()
        {
            return kobisInfo;
        }

        public TmdbInfo getTmdbInfo()
        {
            return tmdbInfo;
        }
    }

    public class KobisInfo
    {
        public string Key { get; set; }
        public string Target { get; set; }
        public string RootDir { get; set; }
        public Dictionary<string, string> Services { get; set; }
        public Dictionary<string, string> Params { get; set; }

        public KobisInfo()
        {
            Services = new Dictionary<string, string>();
            Params = new Dictionary<string, string>();
        }
    }

    public class TmdbInfo
    {
        public string Key { get; set; }
        public Dictionary<string, string> Services { get; set; }
        public string DownloadPath { get; set; }
        public string DownloadUrl { get; set; }
        public string PosterRootUrl { get; set; }
        public string ThumbnailRootUrl { get; set; }

        public TmdbInfo()
        {
            Services = new Dictionary<string, string>();
        }
    }
}