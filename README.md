# Anyway-I-didn-t-do-it

## 캐쥬얼 서바이벌 게임 Anyway I didn't do it

### ▶️ UCC
(이미지를 누르면 영상 링크로 이동합니다.)

[![UCC 영상](http://img.youtube.com/vi/HOwiTeRc6-E/0.jpg)](https://www.youtube.com/watch?v=HOwiTeRc6-E)

### 🎮 시연 영상
(이미지를 누르면 영상 링크로 이동합니다.)

[![시연 영상](http://img.youtube.com/vi/mzCqQ7Afbu0/0.jpg)](https://www.youtube.com/watch?v=mzCqQ7Afbu0)

### 📑 기획

<div style="display: flex; justify-content: space-between;">
  <img src="images/image.png" alt="alt text" style="width: 49%;">
  <img src="images/image-1.png" alt="alt text" style="width: 49%;">
</div>
<br>

- 초기 개발 기간이 짧은 것을 고려하여 캐주얼한 게임을 목표로 개발
- 사람들이 가장 쉽게 재미를 느낄 수 있게 대전을 기반으로 제작

### 🔥 게임 스토리

> 발 디딜 틈 없이 꽉차버린 지옥, 지옥의 관리자는 지옥을 널널하게 만들기 위해 고민을 하는데.
> 지옥에서 펼쳐지는 엔터테인먼트!
> 천국으로 가기 위해 7가지 지옥에서 펼쳐지는 배틀로얄.
> 최후의 승리자는 누구?

### 📂 시스템 아키텍처

![architecture](images/architecture.png)

### UVCS를 통한 브랜치 관리

- 큰 유니티 에셋 용량 - > 큰 용량 관리 편리 2만개 이상의 브랜치 관리 가능
  -> 기록 관리 유리한 측면 있음

![alt text](images/uvcs.gif)

## 게임 특징

### 다양한 애니메이션

![애니메이션](<images/다양한 애니메이션.gif>)

- 애니메이션 블렌드 트리를 사용하여 앞뒤좌우 부드러운 애니메이션 구현

![티배깅](images/티배깅.gif)

- 애니메이션 이모트를 사용한 티배깅 가능

### 유령

![유령](images/유령.gif)

- 죽을 경우 유령이 되어 부활
- 유령의 영역에 닿으면 감속
- 위치 선정 및 적절한 대쉬 사용 -> 전략적 다양성 증가!

### 랭킹

![게임 끝 춤추기](<images/게임 끝 춤추기.gif>)

![최종 순위](<images/최종 순위.gif>)

![랭킹 보드](<images/랭킹 보드.png>)

- 라운드는 총 3라운드, 라운드마다 등수에 따른 점수 획득
- 마지막 라운드 종료 후 최종 우승자의 이긴 횟수를 업데이트 & 랭킹 보드에서 실시간 확인 가능

### 다양한 맵

#### 살인 지옥 (불 맵)

<div style="display: flex; align-items: center;">
  <img src="images/firemap.png" alt="살인 지옥" style="width: 30%;"/>
  <div style="width: 50%; padding-left: 10px;">
    <strong>화산에서 날라오는 마그마를 피하세요.
  </div>
</div>

#### 천륜 지옥 (사막 맵)

<div style="display: flex; align-items: center;">
  <img src="images/desertmap.png" alt="살인 지옥" style="width: 30%;"/>
  <div style="width: 50%; padding-left: 10px;">
    <strong>모래바람이 시야를 가립니다
  </div>
</div>

#### 불의 지옥 (얼음 맵)

<div style="display: flex; align-items: center;">
  <img src="images/icemap.png" alt="살인 지옥" style="width: 30%;"/>
  <div style="width: 50%; padding-left: 10px;">
    <strong>미끄러운 바닥을
	조심하세요
  </div>
</div>

#### 배신 지옥 (거울 맵)

<div style="display: flex; align-items: center;">
  <img src="images/mirrormap.png" alt="살인 지옥" style="width: 30%;"/>
  <div style="width: 50%; padding-left: 10px;">
    <strong>방향키는 나를 배신했다.
  </div>
</div>

#### 폭력 지옥 (바람 맵)

<div style="display: flex; align-items: center;">
  <img src="images/windmap.png" alt="살인 지옥" style="width: 30%;"/>
  <div style="width: 50%; padding-left: 10px;">
    <strong>돌아가는 바퀴에서 살아남으세요.
  </div>
</div>

#### 거짓 지옥 (칼 맵)

<div style="display: flex; align-items: center;">
  <img src="images/swordmap.png" alt="살인 지옥" style="width: 30%;"/>
  <div style="width: 50%; padding-left: 10px;">
    <strong>떨어지면 죽습니다.
  </div>
</div>

#### 나태 지옥 (강 맵)

<div style="display: flex; align-items: center;">
  <img src="images/rivermap.png" alt="살인 지옥" style="width: 30%;"/>
  <div style="width: 50%; padding-left: 10px;">
    <strong>뗏목 위에서 살아남으세요
  </div>
</div>
<br>
<br>

## 💻 Stacks

### Client

<img src="https://img.shields.io/badge/unity-%23000000.svg?style=for-the-badge&logo=unity&logoColor=white">
<img src="https://img.shields.io/badge/photon-004480?style=for-the-badge&logo=photon&logoColor=white">

### Server

<img src="https://img.shields.io/badge/java-007396?style=for-the-badge&logo=java&logoColor=white">
<img src="https://img.shields.io/badge/springboot-6DB33F?style=for-the-badge&logo=springboot&logoColor=white">
<img src="https://img.shields.io/badge/mysql-4479A1?style=for-the-badge&logo=mysql&logoColor=white">
<img src="https://img.shields.io/badge/MongoDB-47A248?style=for-the-badge&logo=mongodb&logoColor=white">

### Infra

<img src="https://img.shields.io/badge/Docker-2496ED?style=for-the-badge&logo=Docker&logoColor=white">
<img src="https://img.shields.io/badge/Jenkins-D24939?style=for-the-badge&logo=Jenkins&logoColor=white">
<img src="https://img.shields.io/badge/AmazonEC2-FF9900?style=for-the-badge&logo=AmazonEC2&logoColor=white">

### ETC

<img src="https://img.shields.io/badge/git-F05032?style=for-the-badge&logo=git&logoColor=white">
<img src="https://img.shields.io/badge/GitLab-FC6D26?style=for-the-badge&logo=GitLab&logoColor=white">
<img src="https://img.shields.io/badge/UVCS-ff4300?style=for-the-badge&logo=UVCS&logoColor=white">

## 팀원 소개

|                                             이예원                                             |                                             변재혁                                              |                                             권용수                                              |                                             진홍엽                                              |                                             정기영                                              |                                             조민우                                              |
| :--------------------------------------------------------------------------------------------: | :---------------------------------------------------------------------------------------------: | :---------------------------------------------------------------------------------------------: | :---------------------------------------------------------------------------------------------: | :---------------------------------------------------------------------------------------------: | :---------------------------------------------------------------------------------------------: |
|                                           서버 / UI                                            |                                            서버 / UI                                            |                                          캐릭터 / 스킬                                          |                                          캐릭터 / 스킬                                          |                                          맵제작 / 화살                                          |                                          맵제작 / 화살                                          |
|                              [Eyekrw](https://github.com/Eyekrw)                               |                                              []()                                               |                               [kwonwd](https://github.com/kwonwd)                               |                             [HYndrome](https://github.com/HYndrome)                             |                       [FlashingFuture](https://github.com/FlashingFuture)                       |                             [mauercho](https://github.com/mauercho)                             |
| <img src = "https://avatars.githubusercontent.com/u/62163559?v=4" width ="100" height = "130"> | <img src = "https://avatars.githubusercontent.com/u/156048545?v=4" width ="100" height = "130"> | <img src = "https://avatars.githubusercontent.com/u/156151476?v=4" width ="100" height = "130"> | <img src = "https://avatars.githubusercontent.com/u/118808892?v=4" width ="100" height = "130"> | <img src = "https://avatars.githubusercontent.com/u/148306893?v=4" width ="100" height = "130"> | <img src = "https://avatars.githubusercontent.com/u/156387215?v=4" width ="100" height = "130"> |
