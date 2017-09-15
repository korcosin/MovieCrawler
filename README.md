# MovieCrawler

##### 개요
* 용도
  * 영화데이터 수집기
* 스펙
  * C# (.NET FRAMEWORK 4.0 이상)
  * Nuget 패키지
* 수집대상
  * [영화진흥위원회](http://www.kobis.or.kr/kobisopenapi/homepg/main/main.do) ☜링크클릭
    * 영화 메타 데이터
      * 제목
      * 스태프 / 배우
      * 장르
      * 개봉일
      * ETC...
  * [The Movie DataBase(이하, TMDB)](https://developers.themoviedb.org/3/search/search-movies) ☜링크클릭
    * 영화 부가 데이터
      * 스토리
      * 포스터
      * 썸네일
* 사용중인서비스
  * [나초(나만의영화초이스)](http://www.nachomovie.com) ☜링크클릭
<hr/>

##### 사전준비사항
* API KEY 발급
  * 영화진흥위원회 API 사용을 위한 KEY
    * 로그인
    * 사이트 내 메뉴 키발급/관리 이동
    * API KEY 추가
  * TMDB API 사용을 위한 KEY
    * 로그인
    * 회원 정보 Settings 메뉴 클릭
    * API 메뉴 클릭
    * Request an API Key 클릭 후 필요 정보 작성 및 KEY 발급
<hr/>

##### 수집 Config 파일 설명
* 샘플 config 파일은 config 폴더의 config.xml이 존재합니다.
* &lt;kobis&gt;
  * &lt;key&gt;
    * 영화진흥위원회 API 키
  * &lt;rootdir&gt;
    * 수집이 진행 되는 루트 폴더
    * 해당 폴더에 history 폴더(동적수집을 위한), image 폴더(이미지수집을 위한), crawled 폴더(수집결과파일을 위한)가 생성됩니다.
  * &lt;service&gt;
    * &lt;item&gt;
      * 각각의 item 엘리먼트에 영화진흥위원회 API URL 정보를 작성합니다.
      * id 어트리뷰트 값 설명
        * boxoffice : 오늘의 박스오피스 랭킹
        * movie_list : 영화 목록 
        * movie_detail : 영화 상세정보
        * people_list : 인물 목록
        * people_detail : 인물 상세정보
      * active 어트리뷰트 값 설명
        * N : 수집대상 아님
        * Y : 수집대상
        * 예를 들어 movie_list 에 active 설정이 되어 있으면 영화 목록 수집을 진행함_
    * &lt;request&gt;
      * &lt;item&gt;
        * 영화진흥위원회 API에 사용 될 파라미터(쿼리스트링)
        * id 어트리뷰트 값 설명
          * itemPerPage : 한 페이지 당 검색 되는 아이템 갯수
          * curPage : 검색 할 페이지
          * prdtStartYear : 영화 제작 시작년도
          * prdtEndYear : 영화 제작 종료년도
* &lt;tmdb&gt;
  * &lt;key&gt;
    * TMDB API 키
  * &lt;service&gt;
    * &lt;item&gt;
      * TMDB API URL 정보를 작성합니다.
    * &lt;image&gt;
      * &lt;rooturl&gt;
        * TMDB 이미지 검색을 위한 이미지서버 HOME URL
        * 넓이 높이 사이즈에 대한 정보는 아래 링크를 통해 변경 가능합니다
          * [https://www.themoviedb.org/talk/53c11d4ec3a3684cf4006400](https://www.themoviedb.org/talk/53c11d4ec3a3684cf4006400)
      * &lt;download&gt;
        * &lt;directory&gt;
          * 이미지 다운로드 받을 경로
<hr/>

   
##### 수집 실행
* CrawlManager.exe -path &lt;설정파일(config.xml) 경로&gt;
  * 컨피그파일 경로 미 작성 시, 기본 루트페이지의 컨피그파일을 찾아서 실행합니다.
