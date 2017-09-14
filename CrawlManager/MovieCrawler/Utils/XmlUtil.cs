using System.Xml.Linq;

namespace GitHub.KorCosin.MovieCrawler.Utils
{
    public class XmlUtil
    {
        public static string getElementValue(XElement ele, string defaultValue)
        {
            return (ele != null || ele.Value != "") ? ele.Value : defaultValue;
        }
    }
}
