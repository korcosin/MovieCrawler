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

##### 수집 Config 파일 설명
TO-DO