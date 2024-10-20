using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Photon.Pun;
using SourGrape.hongyeop;


namespace SourGrape.Yongsoo
{
    public class DashCooldownUI : MonoBehaviour
    {
        private PlayerController _playerMoveController;  // 로컬 플레이어의 PlayerController
        public TextMeshProUGUI _cooldownText;  // 남은 쿨타임을 표시할 TextMeshPro
        private float cooldown = 3f;
        public Image _hideImage;
        private float _ratioTime;


        // Start is called before the first frame update
        void Start()
        {
            // 네트워크 연결된 경우 로컬 플레이어만 찾아서 UI에 연결
            if (PhotonNetwork.IsConnected)
            {
                Debug.Log("네트워크 연결. 대시");
                PlayerController[] players = FindObjectsOfType<PlayerController>();
                foreach (PlayerController player in players)
                {
                    Debug.Log("찾았다. 대시");
                    if (player.photonView.IsMine)  // 자신이 로컬 플레이어인지 확인
                    {
                        _playerMoveController = player;
                        Debug.Log("찾았다. 내 대시");
                        //cooldown = player.GetDashCooldown();
                        break;
                    }
                }
            }
        }

        // Update is called once per frame
        void Update()
        {
            if (_playerMoveController != null)
            {
                //남은 대시 쿨타임 표시
                float _currentCooldown = _playerMoveController.GetCurrentDashCooldown();
                _ratioTime = _currentCooldown / cooldown;
                _hideImage.fillAmount = _ratioTime;
                //Debug.Log(_ratioTime);
                //_cooldownText.text = $"Cooldown: {_currentCooldown:F1}s";
            }
            else
            {
                PlayerController[] players = FindObjectsOfType<PlayerController>();
                foreach (PlayerController player in players)
                {
                    //Debug.Log("찾았다. 대시");
                    if (player.photonView.IsMine)  // 자신이 로컬 플레이어인지 확인
                    {
                        _playerMoveController = player;
                        //Debug.Log("찾았다. 내 대시");
                        //cooldown = player.GetDashCooldown();
                        break;
                    }
                }
            }
        }
    }
}
