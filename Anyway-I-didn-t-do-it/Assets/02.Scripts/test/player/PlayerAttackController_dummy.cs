using SourGrape.hongyeop;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

namespace SourGrape.hongyeop
{
    public class PlayerAttackController_dummy : MonoBehaviourPun
    {
        #region public variables
        public bool IsFireReady;
        public Weapon EquippedWeapon;
        public float FireDelay;
        #endregion

        #region private variables
        private Animator _anim;
        private bool _fireDown; // Fire button
        private bool _isAttacking;
        private PlayerController_dummy _playerMoveController;
        #endregion

        void Start()
        {
            _anim = GetComponentInChildren<Animator>();
            EquippedWeapon = GameObject.Find("Tennis Racket").GetComponent<Weapon>(); 
            _playerMoveController = GetComponent<PlayerController_dummy>();

        }

        void Update()
        {
            //if (photonView.IsMine) 
                GetInput(); // 버튼 입력 감지
        }

        void FixedUpdate()
        {
            //if (photonView.IsMine)
            {
                Attack();
            }
        }

        private void GetInput()
        {
            _fireDown = Input.GetButton("Fire1");
        }

        private void Attack()
        {
            if (EquippedWeapon == null || _isAttacking || _playerMoveController.IsDashing) // 대쉬하는 경우 공격 불가
            {
                return;
            }
            FireDelay += Time.deltaTime;
            IsFireReady = EquippedWeapon.AttackRate < FireDelay; // 공격 딜레이 처리
            if (_fireDown && IsFireReady)
            {
                StopCoroutine("PerformAttack");
                StartCoroutine("PerformAttack");
            }
        }

        private IEnumerator PerformAttack()
        {
            _isAttacking = true; // 공격 시작
            //EquippedWeapon.Use(); // 무기 판정 처리
            _anim.SetTrigger("DoSwing"); // 애니메이션 trigger
            _playerMoveController.enabled = false; // 플레이어 이동 비활성화
            yield return new WaitForSeconds(0.5f); // 루틴 시간 (이동 정지 시간)
            FireDelay = 0; // 공격 딜레이 시작 (초기화)
            _playerMoveController.enabled = true; // 플레이어 이동 활성화
            _isAttacking = false;
        }

        public void AttackStart()
        {
            _anim.SetBool("isMeleeEnabled", true);
        }

        public void AttackEnd()
        {
            _anim.SetBool("isMeleeEnabled", false);
        }
    }
}

