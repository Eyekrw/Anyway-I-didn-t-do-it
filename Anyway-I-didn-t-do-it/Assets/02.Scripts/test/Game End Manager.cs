//using SourGrape.hongyeop;
//using System.Collections;
//using System.Collections.Generic;
//using UnityEngine;

//public class GameEndManager : MonoBehaviour
//{
//    // 태그가 "Player"인 오브젝트들을 저장할 퍼블릭 변수
//    public List<GameObject> Winners; // 승자들
//    public List<GameObject> Ghosts; // 유령들

//    private Camera _camera;

//    void Start()
//    {
//        _camera = Camera.main; // 메인 카메라 가져오기
//    }

//    void Update()
//    {
//        // 키보드 1번 누르면 시작 - 필요한 호출 지점으로 변경하면 됨
//        if (Input.GetKeyDown(KeyCode.Alpha1))
//        {
//            FindWinners(); // 승자 찾기
//            WinnersDance(); // 승자 댄스
//            FindGhosts(); // 유령 찾기
//            GhostsBuster(); // 유령 없애기
//            _camera.GetComponent<CameraController_dummy>().enabled = false; // 카메라 컨트롤러 끄기
//            _camera.GetComponent<GameEndCameraManager>().enabled = true; // 카메라 컨트롤러 끄기
//        }
//    }

//    private void FindWinners()
//    {
//        // 태그가 "Player"인 모든 오브젝트를 찾아서 배열에 저장
//        GameObject[] allPlayers = GameObject.FindGameObjectsWithTag("Player");
//        Winners = new List<GameObject>();

//        foreach (GameObject player in allPlayers)
//        {
//            PlayerStatusController_dummy status = player.GetComponent<PlayerStatusController_dummy>();
//            if (status != null && !status.IsDead)
//            {
//                Winners.Add(player);
//            }
//        }
//    }

//    private void WinnersDance()
//    {
//        // Winners 리스트에 있는 모든 오브젝트의 IsDance 프로퍼티를 true로 설정
//        foreach (GameObject player in Winners)
//        {
//            PlayerStatusController_dummy status = player.GetComponent<PlayerStatusController_dummy>();
//            if (status != null)
//            {
//                status.IsDance = true;
//            }
//        }
//    }

//    private void FindGhosts()
//    {
//        // 태그가 "PlayerGhost"인 모든 오브젝트를 찾아서 배열에 저장
//        GameObject[] allGhosts = GameObject.FindGameObjectsWithTag("PlayerGhost");
//        Ghosts = new List<GameObject>();

//        foreach (GameObject ghost in allGhosts)
//        {
//            PlayerGhostController status = ghost.GetComponent<PlayerGhostController>();
//            if (status != null)
//            {
//                Ghosts.Add(ghost);
//            }
//        }
//    }

//    private void GhostsBuster()
//    {
//        // Ghosts 리스트에 있는 모든 오브젝트의 IsDead 프로퍼티를 true로 설정
//        foreach (GameObject ghost in Ghosts)
//        {
//            PlayerGhostController status = ghost.GetComponent<PlayerGhostController>();
//            if (status != null)
//            {
//                status.IsDead = true;
//            }
//        }
//    }
//}