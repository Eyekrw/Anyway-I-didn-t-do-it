using SourGrape.hongyeop;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SourGrape.kiyoung
{
    public class River : MonoBehaviour
    {
        private void OnTriggerEnter(Collider other)
        {
            // 충돌한 오브젝트가 Player일 경우
            if (other.CompareTag("Player"))
            {
                PlayerStatusController playerStatus = other.GetComponent<PlayerStatusController>();

                if (playerStatus != null && !playerStatus.IsDead)
                {
                    // 체력 감소
                    playerStatus.PlayerHealth -= 1;
                }
            }
        }
    }
}
