namespace GitHub.KorCosin.MovieCrawler.Scan.Constant
{
    public class ParamList
    {
        /// <summary>
        /// 현재 페이지
        /// </summary>
        public const string PARAM_KOBIS_PAGEINDEX = "curPage";
        /// <summary>
        /// 한 페이지당 출력 될 결과 행 수
        /// </summary>
        public const string PARAM_KOBIS_VIEWCOUNT = "itemPerPage";
        /// <summary>
        /// 상영시작년도
        /// </summary>
        public const string PARAM_KOBIS_STARTDT = "prdtStartYear";
        /// <summary>
        /// 상영종료년도
        /// </summary>
        public const string PARAM_KOBIS_ENDDT = "prdtEndYear";
        /// <summary>
        /// 조회 할 날짜(박스오피스용)
        /// </summary>
        public const string PARAM_KOBIS_TARGETDT = "targetDt";
        /// <summary>
        /// 영화코드
        /// </summary>
        public const string PARAM_KOBIS_MOVIECODE = "movieCd";
        /// <summary>
        /// 인물코드
        /// </summary>
        public const string PARAM_KOBIS_PEOPLECODE = "peopleCd";
        /// <summary>
        /// 필모그라피
        /// </summary>
        public const string PARAM_KOBIS_FILMONAME = "filmoNames";


        public const string PARAM_TMDB_QUERY = "query";
        public const string PARAM_TMDB_YEAR = "year";
    }
}
