using System;
using System.Linq;
using System.Text;
using System.Xml.Linq;
using System.Data.SqlClient;
using System.Data;
using System.Collections.Generic;

namespace GitHub.KorCosin.MovieCrawler.Scan
{
    public class Scanner
    {
        Args.KobisInfo kobisInfo;
        Args.TmdbInfo tmdbInfo;

        StringBuilder sbParams;

        int totalcnt = 0;
        int pagecnt = 0;

        int _scanCount = 0;

        public Scanner(Args.KobisInfo kobisInfo, Args.TmdbInfo tmdbInfo)
        {
            this.kobisInfo = kobisInfo;
            this.tmdbInfo = tmdbInfo;

            this.sbParams = new StringBuilder();
        }

        public void scan()
        {
            switch (kobisInfo.Target)
            {
                case Constant.ApiList.MOVIE_BOXOFFICE:
                    boxoffice();
                    break;
                case Constant.ApiList.MOVIE_LIST:
                    movie();
                    break;
                case Constant.ApiList.PEOPLE_LIST:
                    people();
                    break;
                default:
                    break;
            }
        }

        /// <summary>
        /// 박스오피스 API 스캔
        /// </summary>
        private void boxoffice()
        {
            string url = kobisInfo.Services[Constant.ApiList.MOVIE_BOXOFFICE];

            sbParams.Clear();
            sbParams.Append("?")
                    .Append("key=").Append(kobisInfo.Key)
                    .Append("&")
                    .Append(Constant.ParamList.PARAM_KOBIS_TARGETDT).Append("=").Append(kobisInfo.Params[Constant.ParamList.PARAM_KOBIS_TARGETDT]);

            XDocument xDoc = XDocument.Load(url + sbParams.ToString());

            var root = xDoc.Descendants("boxOfficeResult");
            var dailyBoxOffice =
                root.Descendants("dailyBoxOfficeList")
                    .Descendants("dailyBoxOffice")
                    .Select(b => b);

            foreach (var movie in dailyBoxOffice)
            {
                Console.WriteLine(movie.Element("movieNm").Value);
            }
        }

        /// <summary>
        /// 영화 API 스캔
        /// </summary>
        private void movie()
        {
            History history = new History(kobisInfo.History);
            UrlGenerator urlGenerator = new UrlGenerator();

            string kobiskey = kobisInfo.Key;
            string tmdbkey = tmdbInfo.Key;
            string pageindex = kobisInfo.Params[Constant.ParamList.PARAM_KOBIS_PAGEINDEX];
            string viewcount = kobisInfo.Params[Constant.ParamList.PARAM_KOBIS_VIEWCOUNT];
            string startdt = kobisInfo.Params[Constant.ParamList.PARAM_KOBIS_STARTDT];
            string enddt = kobisInfo.Params[Constant.ParamList.PARAM_KOBIS_ENDDT];

            List<string> crawledData = history.getCrawledDataList();
            history.backupFile();

            Console.Write("[info] kobis api reading...");
            XDocument xDoc = XDocument.Load(
                                urlGenerator.movieListAPI(
                                    kobisInfo.Services[Constant.ApiList.MOVIE_LIST],
                                    kobiskey, pageindex, viewcount, startdt, enddt
                                )
                            );
            Console.WriteLine("[OK][{0}]", DateTime.Now.ToString("yyyymmddhhmmss"));

            var root = xDoc.Descendants("movieListResult");
            var movieList = root.Descendants("movieList").Descendants("movie").Select(m => m);

            XElement movieXml = new XElement("root");

            List<string> kobisList = new List<string>();
            history.createFile();
            foreach (var movie in movieList)
            {
                string movieCd = movie.Element("movieCd").Value;

                history.writeInFile(movieCd);
                kobisList.Add(movieCd);
            }
            history.closeFile();

            Console.Write("[info] filtering exist data...");
            List<string> newItem = kobisList.AsParallel()
                                            .Where(code => !crawledData.Contains(code))
                                            .ToList();
            Console.WriteLine("[OK][{0}]", DateTime.Now.ToString("yyyymmddhhmmss"));

            Console.Write("[info] target data [{0}] counts", newItem.Count);
            foreach (var item in newItem)
            {
                #region MOVIE_DETAIL
                xDoc = XDocument.Load(
                            urlGenerator.movieInfoAPI(
                                kobisInfo.Services[Constant.ApiList.MOVIE_DETAIL],
                                kobiskey, item
                            )
                       );

                var movieInfo = xDoc.Descendants("movieInfoResult").Descendants("movieInfo").Select(info => info).First();

                string movieCd = Utils.XmlUtil.getElementValue(movieInfo.Element("movieCd"), "");
                string movieNm = Utils.XmlUtil.getElementValue(movieInfo.Element("movieNm"), "");
                string movieNmEn = Utils.XmlUtil.getElementValue(movieInfo.Element("movieNmEn"), "");
                string movieNmOg = Utils.XmlUtil.getElementValue(movieInfo.Element("movieNmOg"), "");
                string showTm = Utils.XmlUtil.getElementValue(movieInfo.Element("showTm"), "0");
                string openDt = Utils.XmlUtil.getElementValue(movieInfo.Element("openDt"), "미개봉");
                string genres = string.Join(",", movieInfo.Element("genres")
                                                        .Elements("genre")
                                                        .Select(genre =>
                                                            Utils.XmlUtil.getElementValue(genre.Element("genreNm"), "")
                                                        )
                                           );
                string audits = Utils.StringUtil.replaceNullOrEmpty(
                                    movieInfo.Element("audits")
                                            .Elements("audit")
                                            .Select(audit =>
                                                Utils.XmlUtil.getElementValue(audit.Element("watchGradeNm"), "")
                                    ).FirstOrDefault()
                                , "");
                string isAdult = (audits == "" || audits != "청소년관람불가") ? "N" : "Y";
                string nations = Utils.StringUtil.replaceNullOrEmpty(
                                    movieInfo.Element("nations")
                                            .Elements("nation")
                                            .Select(nation =>
                                                Utils.XmlUtil.getElementValue(nation.Element("nationNm"), "")
                                            ).FirstOrDefault()
                                , "");
                nations = (openDt != "미개봉") ? "한국" : nations;
                var directors = movieInfo.Element("directors")
                                        .Elements("director")
                                        .Select(director =>
                                            Utils.XmlUtil.getElementValue(director.Element("peopleNm"), "")
                                        );
                var actors = movieInfo.Element("actors")
                                    .Elements("actor")
                                    .Select(actor =>
                                        Utils.XmlUtil.getElementValue(actor.Element("peopleNm"), "")
                                    );
                #endregion

                #region PEOPLE_DETAIL
                xDoc = XDocument.Load(
                            urlGenerator.peopleInfoAPI(
                                kobisInfo.Services[Constant.ApiList.PEOPLE_LIST],
                                kobiskey,
                                movieNm
                            )
                       );

                var peopleList = xDoc.Descendants("peopleListResult").Descendants("peopleList").Descendants("people").Select(info => info);

                string directorsCd = string.Join(",", peopleList.Where(p =>
                                                                    p.Element("repRoleNm").Value == "감독" &&
                                                                    directors.Contains(p.Element("peopleNm").Value)
                                                                )
                                                                .Select(p =>
                                                                    Utils.XmlUtil.getElementValue(p.Element("peopleCd"), "")
                                                                )
                                           );
                string actorsCd = string.Join(",", peopleList.Where(p =>
                                                                p.Element("repRoleNm").Value == "배우" &&
                                                                actors.Contains(p.Element("peopleNm").Value)
                                                            )
                                                            .Select(p =>
                                                                Utils.XmlUtil.getElementValue(p.Element("peopleCd"), "")
                                                            )
                                            );
                #endregion

                #region TMDB_SEARCH
                System.Net.WebClient client = new System.Net.WebClient();
                client.Encoding = System.Text.Encoding.UTF8;

                var json = client.DownloadString(
                                urlGenerator.tmdbInfoAPI(
                                    tmdbInfo.Services[Constant.ApiList.MOVIE_LIST],
                                    tmdbkey, movieNm, movieNmEn, openDt
                                )
                           );

                Newtonsoft.Json.Linq.JObject jObject = Newtonsoft.Json.Linq.JObject.Parse(json);

                int tmdbKey = 0;
                string overview = string.Empty;
                string poster = string.Empty;
                string thumbnail = string.Empty;

                int findCount = Convert.ToInt32(jObject["total_results"]);
                Newtonsoft.Json.Linq.JArray jResult = ((Newtonsoft.Json.Linq.JArray)jObject["results"]);

                dynamic results;
                if (findCount > 1)
                {
                    results = jResult.Where(result =>
                                    result["release_date"].ToString().Replace("-", "") == openDt.Replace("미개봉", "")
                                )
                                .Select(result => result
                                ).FirstOrDefault();
                }
                else if (findCount == 1)
                {
                    results = jResult.Select(result => result).FirstOrDefault();
                }
                else
                {
                    results = null;
                }

                if (results != null)
                {
                    tmdbKey = (results["id"] != null) ? Convert.ToInt32(results["id"].ToString()) : 0;
                    overview = (results["overview"] != null) ? results["overview"].ToString() : "";
                    poster = (results["poster_path"] != null) ? results["poster_path"].ToString() : "";
                    thumbnail = (results["backdrop_path"] != null) ? results["backdrop_path"].ToString() : "";

                    poster = (poster != "") ? tmdbInfo.PosterRootUrl + poster : "";
                    thumbnail = (thumbnail != "") ? tmdbInfo.ThumbnailRootUrl + thumbnail : "";

                    string path = string.Format("{0}\\{1}\\", tmdbInfo.DownloadPath, movieCd);

                    if (!System.IO.Directory.Exists(path))
                    {
                        System.IO.Directory.CreateDirectory(path);
                    }

                    using (System.Net.WebClient wc = new System.Net.WebClient())
                    {
                        if (poster.Trim() != "")
                        {
                            try
                            {
                                string filenm = poster.Substring(poster.LastIndexOf("/") + 1);
                                client.DownloadFile(new Uri(poster), path + filenm);
                                poster = poster.Replace(tmdbInfo.PosterRootUrl, string.Format("{0}/{1}", tmdbInfo.DownloadUrl, movieCd));
                            }
                            catch (System.Net.WebException)
                            {
                            }
                        }

                        if (thumbnail.Trim() != "")
                        {
                            try
                            {
                                string filenm = thumbnail.Substring(thumbnail.LastIndexOf("/"));
                                client.DownloadFile(new Uri(thumbnail), path + filenm);
                                thumbnail = thumbnail.Replace(tmdbInfo.ThumbnailRootUrl, string.Format("{0}/{1}", tmdbInfo.DownloadUrl, movieCd));
                            }
                            catch (System.Net.WebException)
                            {
                            }
                        }
                    }
                }
                #endregion

                movieXml.Add(
                        new XElement("movieList",
                            new XElement("movieCd", movieCd),
                            new XElement("movieNm", movieNm),
                            new XElement("movieNmEn", movieNmEn),
                            new XElement("movieNmOg", movieNmOg),
                            new XElement("showTm", showTm),
                            new XElement("openDt", openDt),
                            new XElement("genres", genres),
                            new XElement("audits", audits),
                            new XElement("isAdult", isAdult),
                            new XElement("nations", nations),
                            new XElement("directors", directorsCd),
                            new XElement("actors", actorsCd),
                            new XElement("tmdbKey", tmdbKey),
                            new XElement("overview", overview),
                            new XElement("poster", poster),
                            new XElement("thumbnail", thumbnail)
                        )
                );
                _scanCount++;
                printRealtimeCrawlCount();
            }

            Console.WriteLine();
            Console.WriteLine("CRAWL COUNT : {0}", _scanCount);
        }

        /// <summary>
        /// 인물 API 스캔
        /// </summary>
        private void people()
        {
            string url = kobisInfo.Services[Constant.ApiList.PEOPLE_LIST];

            sbParams.Clear();
            sbParams.Append("?")
                    .Append("key=").Append(kobisInfo.Key)
                    .Append("&")
                    .Append(Constant.ParamList.PARAM_KOBIS_PAGEINDEX).Append("=").Append(kobisInfo.Params[Constant.ParamList.PARAM_KOBIS_PAGEINDEX])
                    .Append("&")
                    .Append(Constant.ParamList.PARAM_KOBIS_VIEWCOUNT).Append("=").Append(kobisInfo.Params[Constant.ParamList.PARAM_KOBIS_VIEWCOUNT]);

            XDocument xDoc = XDocument.Load(url + sbParams.ToString());
            var root = xDoc.Descendants("peopleListResult");

            totalcnt = Convert.ToInt32(root.Descendants("totCnt").Select(m => m.Value).Single());
            pagecnt = totalcnt / Convert.ToInt32(kobisInfo.Params[Constant.ParamList.PARAM_KOBIS_VIEWCOUNT]) + 1;

            //kobisInfo.Params[Constant.ParamList.PARAM_PAGEINDEX]
            var peopleList =
                            root.Descendants("peopleList").Descendants("people")
                                //.Where(m => m.Element("repRoleNm").Value == "감독" || m.Element("repRoleNm").Value == "배우")
                                .Select(m => m);

            XElement peopleXml = new XElement("root");

            foreach (var people in peopleList)
            {
                string peopleCd = Utils.XmlUtil.getElementValue(people.Element("peopleCd"), "");
                string peopleNm = Utils.XmlUtil.getElementValue(people.Element("peopleNm"), "");
                string peopleNmEn = Utils.XmlUtil.getElementValue(people.Element("peopleNmEn"), "");
                string repRoleNm = Utils.XmlUtil.getElementValue(people.Element("repRoleNm"), "");

                peopleXml.Add(
                    new XElement("peopleList",
                        new XElement("peopleCd", peopleCd),
                        new XElement("peopleNm", peopleNm),
                        new XElement("peopleNmEn", peopleNmEn),
                        new XElement("repRoleNm", repRoleNm)
                    ));
            }
        }

        private void printRealtimeCrawlCount()
        {
            if (_scanCount % 100 == 0)
            {
                Console.Write("[{0}]", _scanCount);
            }
            else if (_scanCount % 10 == 0)
            {
                Console.Write(".");
            }
        }
    }
}
