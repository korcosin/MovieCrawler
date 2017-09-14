namespace GitHub.KorCosin.MovieCrawler.Utils
{
    public class StringUtil
    {
        public static string replaceNullOrEmpty(string str, string defaultValue)
        {
            return (str != null && str != "") ? str : defaultValue;
        }
    }
}
