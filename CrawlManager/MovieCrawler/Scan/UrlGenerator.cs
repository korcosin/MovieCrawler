using System.Text;

namespace GitHub.KorCosin.MovieCrawler.Scan
{
    public class UrlGenerator
    {
        StringBuilder sbUrl;
        public UrlGenerator()
        {
            sbUrl = new StringBuilder();
        }

        public string movieListAPI(string url, string key, string pageindex, string viewcount, string startdt, string enddt)
        {
            sbUrl.Clear();

            sbUrl.Append(url).Append("?")
                 .Append("key=").Append(key)
                 .Append("&")
                 .Append(Constant.ParamList.PARAM_KOBIS_PAGEINDEX).Append("=").Append(pageindex)
                 .Append("&")
                 .Append(Constant.ParamList.PARAM_KOBIS_VIEWCOUNT).Append("=").Append(viewcount);

            if (startdt != "")
            {
                sbUrl.Append("&")
                     .Append(Constant.ParamList.PARAM_KOBIS_STARTDT).Append("=").Append(startdt);
            }

            if (enddt != "")
            {
                sbUrl.Append("&")
                     .Append(Constant.ParamList.PARAM_KOBIS_ENDDT).Append("=").Append(enddt);
            }

            return sbUrl.ToString();
        }

        public string movieInfoAPI(string url, string key, string movieCd)
        {
            sbUrl.Clear();

            sbUrl.Append(url).Append("?")
                 .Append("key=").Append(key)
                 .Append("&")
                 .Append(Constant.ParamList.PARAM_KOBIS_MOVIECODE).Append("=").Append(movieCd);

            return sbUrl.ToString();
        }

        public string peopleInfoAPI(string url, string key, string movieNm)
        {
            sbUrl.Clear();

            sbUrl.Append(url).Append("?")
                 .Append("key=").Append(key)
                 .Append("&")
                 .Append("itemPerPage=10000")
                 .Append("&")
                 .Append(Constant.ParamList.PARAM_KOBIS_FILMONAME).Append("=").Append(movieNm);

            return sbUrl.ToString();
        }

        public string tmdbInfoAPI(string url, string key, string movieNm, string movieNmEn, string openDt)
        {
            sbUrl.Clear();

            sbUrl.Append(url).Append("?")
                 .Append("api_key=").Append(key)
                 .Append("&")
                 .Append("language=ko-KR");

            if (movieNmEn != "")
            {
                sbUrl.Append("&")
                     .Append(Constant.ParamList.PARAM_TMDB_QUERY).Append("=").Append(movieNmEn.Replace("#", ""));
            }
            else
            {
                sbUrl.Append("&")
                     .Append(Constant.ParamList.PARAM_TMDB_QUERY).Append("=").Append(movieNm.Replace("#", ""));
            }

            if (openDt != "미개봉" && openDt.Length >= 4)
            {
                sbUrl.Append("&")
                     .Append(Constant.ParamList.PARAM_TMDB_YEAR).Append("=").Append(openDt.Substring(0, 4));
            }

            return sbUrl.ToString();
        }
    }
}
