using SourGrape.hongyeop;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

namespace SourGrape.hongyeop
{
    public class PlayerAttackController : MonoBehaviourPun
    {
        #region public variables
        public bool IsFireReady;
        public Weapon EquippedWeapon;
        public float FireDelay;
        //public GameObject attackSound;
        #endregion

        #region private variables
        private Animator _anim;
        private bool _fireDown; // Fire button
        private bool _isAttacking;
        private PlayerController _playerMoveController;
        private Vector3 _hitboxSize = new Vector3(1.2f, 1f, 1.2f);
        private Vector3 _hitboxPosition;
        private LayerMask _playerLayer;
        private Collider frontAttackBox;
        private Collider leftAttackBox;
        #endregion

        void Start()
        {
            _anim = GetComponentInChildren<Animator>();
            EquippedWeapon = GameObject.Find("Tennis Racket").GetComponent<Weapon>(); 
            _playerMoveController = GetComponent<PlayerController>();
            _playerLayer = LayerMask.GetMask("Player");
            Transform subAttackBox = transform.Find("SubAttackBox");
            if (subAttackBox != null)
            {
                frontAttackBox = subAttackBox.Find("front").GetComponent<Collider>();
                leftAttackBox = subAttackBox.Find("left").GetComponent<Collider>();

                // 처음엔 Collider를 비활성화해둠
                frontAttackBox.enabled = false;
                leftAttackBox.enabled = false;
            }
        }

        void Update()
        {
            if (photonView.IsMine) GetInput(); // 버튼 입력 감지
        }

        void FixedUpdate()
        {
            if (photonView.IsMine)
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
            FireDelay += Time.deltaTime;
            IsFireReady = EquippedWeapon.AttackRate < FireDelay; // 공격 딜레이 처리
            if (EquippedWeapon == null || _isAttacking || _playerMoveController.IsDashing) // 대쉬하는 경우 공격 불가
            {
                return;
            }
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
            //_playerMoveController.enabled = false; // 플레이어 이동 비활성화
            _playerMoveController.SetFlag(false);
            //Attack_fill();
            yield return new WaitForSeconds(0.5f); // 루틴 시간 (이동 정지 시간)
            FireDelay = 0; // 공격 딜레이 시작 (초기화)
            //_playerMoveController.enabled = true; // 플레이어 이동 활성화
            _playerMoveController.SetFlag(true);
            _isAttacking = false;
        }

        public void AttackStart()
        {
            _anim.SetBool("isMeleeEnabled", true);
        }

        public void AttackSoundStart()
        {
            GameObject soundObj = PhotonNetwork.Instantiate("yong/Sound_attack", transform.position, Quaternion.identity);

            AudioSource audioSource = soundObj.GetComponent<AudioSource>();
            if (audioSource != null)
            {
                audioSource.Play();

                //PhotonNetwork.Destroy(soundObj, audioSource.clip.length);
                StartCoroutine(DestroyAfterSound(audioSource.clip.length, soundObj));
            }

        }

        private IEnumerator DestroyAfterSound(float delay, GameObject obj)
        {
            yield return new WaitForSeconds(delay); // 사운드 재생 시간이 지나면
            PhotonNetwork.Destroy(obj); // 오브젝트 삭제
        }
        public void EnableFrontAttackCollider()
        {
            if (frontAttackBox != null)
            {

                if(frontAttackBox != null)
        {
                    frontAttackBox.enabled = true;
                    Debug.Log("Front Collider 활성화됨.");
                    frontAttackBox.transform.position += new Vector3(0,0.1f,0);
                }
            }
        }

        public void EndFrontAttackCollider()
        {
            if (frontAttackBox != null)
            {
                frontAttackBox.enabled = false;
                frontAttackBox.transform.position -= new Vector3(0, 0.1f, 0);

            }
        }

        public void EnableLeftAttackCollider()
        {
            if (leftAttackBox != null)
            {
                leftAttackBox.enabled = true;
                Debug.Log("Left Collider 활성화됨.");
                leftAttackBox.transform.position += new Vector3(0, 0.1f, 0);
            }
        }

        public void EndLeftAttackCollider()
        {
            if (leftAttackBox != null)
            {
                leftAttackBox.enabled = false;
                leftAttackBox.transform.position -= new Vector3(0, 0.1f, 0);
            }
        }


        private void Attack_fill()
        {
            Collider[] hitEnermies = Physics.OverlapBox(transform.position + transform.forward * 1.5f + Vector3.up * 0.7f, _hitboxSize, transform.rotation, _playerLayer);

            foreach (Collider c in hitEnermies)
            {
                if (c.CompareTag("Player") && c.gameObject != gameObject)
                {
                    c.SendMessage("OnTriggerEnter", EquippedWeapon.GetComponent<Collider>(), SendMessageOptions.DontRequireReceiver);
                }

            }
        }

        private void Attack_fill_left()
        {
            Collider[] hitEnermies = Physics.OverlapBox(transform.position + Vector3.left * 1f + Vector3.forward * 0.1f, _hitboxSize, transform.rotation, _playerLayer);

            foreach (Collider c in hitEnermies)
            {
                if (c.CompareTag("Player") && c.gameObject != gameObject)
                {
                    c.SendMessage("OnTriggerEnter", EquippedWeapon.GetComponent<Collider>(), SendMessageOptions.DontRequireReceiver);
                }

            }
        }

        public void AttackEnd()
        {
            _anim.SetBool("isMeleeEnabled", false);
        }


    }
}

