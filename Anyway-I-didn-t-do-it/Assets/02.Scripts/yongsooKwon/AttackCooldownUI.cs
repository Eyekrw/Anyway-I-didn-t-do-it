using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Photon.Pun;
using SourGrape.hongyeop;

namespace SourGrape.yongsoo
{

    public class AttackCooldownUI : MonoBehaviour
    {
        private PlayerAttackController _playerAttackController;  // 로컬 플레이어의 PlayerController
        public TextMeshProUGUI _cooldownText;  // 인스펙터에서 할당할 TextMeshProUGUI 필드
        private float cooldown = 1f;
        public Image _hideImage;

        // Start is called before the first frame update
        void Start()
        {
            if (PhotonNetwork.IsConnected)
            {
                Debug.Log("네트워크 연결. 어택");

                PlayerAttackController[] players = FindObjectsOfType<PlayerAttackController>();
                foreach (PlayerAttackController player in players)
                {
                    Debug.Log("찾았다.");
                    if (player.photonView.IsMine)  // 자신이 로컬 플레이어인지 확인
                    {
                        _playerAttackController = player;
                        Debug.Log("찾았다. 내 어택");
                        //cooldown = _playerAttackController.EquippedWeapon.AttackRate;   
                        break;
                    }
                }
            }
        }

        // Update is called once per frame
        void Update()
        {
            if (_playerAttackController != null)
            {
                if (cooldown > _playerAttackController.FireDelay)
                {
                    float remainingCooldown = cooldown - _playerAttackController.FireDelay;
                    float _ratioTime = remainingCooldown / cooldown;
                    _hideImage.fillAmount = _ratioTime;
                    //_cooldownText.text = remainingCooldown.ToString("F1");  // 소수점 1자리까지 표시
                }
                else
                {
                    _hideImage.fillAmount = 0;
                    if (_cooldownText != null)  // null 체크 추가
                    {
                        //_cooldownText.text = "0";  // 쿨다운이 없을 경우 0을 표시
                    }
                }
            }
            else
            {
                PlayerAttackController[] players = FindObjectsOfType<PlayerAttackController>();
                foreach (PlayerAttackController player in players)
                {
                    if (player.photonView.IsMine)  // 자신이 로컬 플레이어인지 확인
                    {
                        _playerAttackController = player;
                        cooldown = _playerAttackController.EquippedWeapon.AttackRate;
                        break;
                    }
                }
            }
        }
    }
}
