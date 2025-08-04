<a id="readme-top"></a>



<!-- PROJECT LOGO -->
<br />
<div align="center">
  <img alt="Logo" width="150" src="https://github.com/user-attachments/assets/f9a7b92e-b45e-494f-8761-705f23632c92" />
  <h3 align="center">C# GAME SERVER</h3>
  <p align="center">
    Socket 통신 오목 게임 서버 샘플 프로젝트<br>
    2025.08.04 ~ 2025.8.07 (4일 진행)
  </p>
</div>


<a id="roadmap"></a>

## Roadmap

- [x] Step 1 SuperSocketLite 학습 및 프로젝트 생성 (8/4)

  - [x] VS Code + .NET 개발 환경 세팅
  - [x] GitHub 저장소 생성 및 연동
  - [x] `.gitignore` 설정 및 기본 정리
  - [x] SuperSocketLite 구조 분석 (`AppServer`, `AppSession`, `RequestInfo`)
  - [x] `EchoServer` 샘플 실행 성공
  - [x] `EchoClient` 샘플 실행 성공

- [ ] Step 2 Echo 서버 및 클라이언트 개발 (8/4)

  - [ ] Echo 서버 프로젝트 구조 설계 (`EchoSession`, `EchoReceiveFilter`)
  - [ ] 텍스트 수신 → 그대로 응답하는 기본 구현
  - [ ] 클라이언트 콘솔 앱 제작
  - [ ] Echo 기능 동작 확인 (멀티 접속 포함)
  - [ ] 로그 및 에러 처리 추가
  - [ ] README 작성

- [ ] Step 3 오목 서버 개발 (8/5~8/6)

  - [ ] 오목 플레이어 세션 관리 (`GomokuSession`)
  - [ ] 좌표 송수신 프로토콜 설계 (예: `PUT x y`)
  - [ ] 게임 상태 저장 및 룰 검증 (`GomokuGameRoom`)
  - [ ] 콘솔 클라이언트에서 플레이 테스트 가능하도록 구현
  - [ ] 승패 판단 및 게임 종료 로직
  - [ ] README 작성

- [ ] Step 4 오목 클라이언트 개발 (8/7)

  - [ ] WFP 기반 오목 클라이언트 제작
  - [ ] 명령어 입력 및 보드 출력 구현
  - [ ] 클라이언트 간 통신 테스트 (2인 대전 시뮬레이션)
  - [ ] 리팩토링 및 코드 정리
  - [ ] README 작성

- [ ] 선택 과제 (시간 여유 있을 시)

  - [ ] 오목 API 서버 제작 (예: 매칭 대기열 관리용 REST API)
  - [ ] 오목 매칭 서버 별도 구성 (`MatchServer`)
