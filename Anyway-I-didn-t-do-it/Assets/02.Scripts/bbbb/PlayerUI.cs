using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using TMPro;
using ExitGames.Client;


namespace SourGrape.bbbb
{
    public class PlayerUI : MonoBehaviourPun
    {
        public TMPro.TextMeshProUGUI pingText;
        public TMPro.TextMeshProUGUI timeText;
        private int leftTime = 120;
        private bool isUpdatingPing = false;  // 코루틴이 실행 중인지 확인하는 플래그

        // Start is called before the first frame update
        void Start()
        {
            // 시작 시 바로 pingUpdate 코루틴을 실행
            if (!PhotonNetwork.IsConnected)
            {
                PhotonNetwork.ConnectUsingSettings();
            }
            StartCoroutine(pingUpdate());
        }

        // ping 정보를 1초마다 갱신하는 코루틴
        private IEnumerator pingUpdate()
        {
            isUpdatingPing = true;
            while (PhotonNetwork.IsConnected) // 연결이 유지되는 동안 핑 정보를 갱신
            {
                pingText.text = $"Ping: {PhotonNetwork.GetPing()} ms";
                yield return new WaitForSeconds(1f);  // 1초마다 업데이트
            }
            isUpdatingPing = false; // 연결이 끊기면 코루틴 종료
        }

        private IEnumerator TimeUpdater()
        {
            while (true)
            {
                timeText.text = $"{leftTime/60}:{leftTime%60}";
                yield return new WaitForSeconds(1f);
                leftTime--;
            }
        }

        [PunRPC]
        void TimerStart()
        {
            StartCoroutine(TimeUpdater());
        }

    }
}
