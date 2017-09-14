namespace GitHub.KorCosin.MovieCrawler
{
    /// <summary>
    /// PROGRAM START
    /// </summary>
    class Program
    {
        public static void Main(string[] args)
        {
            Args.Parser parser = new Args.Parser(args);

            Scan.Scanner runner = new Scan.Scanner(parser.getKobisInfo(), parser.getTmdbInfo());

            runner.scan();
        }
    }
}