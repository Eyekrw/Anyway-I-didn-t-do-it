using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using SourGrape.yewon;

namespace SourGrape.hongyeop
{
    public class PlayerStatusController_dummy : MonoBehaviourPun
    {
        #region public properties
        public GameObject NearObject;
        public int PlayerHealth = 1;
        public bool IsDead = false; // 죽음 여부
        public bool IsDance = false; // 댄스 여부
        public int SlowCount = 0;
        #endregion

        #region exposed private properties
        #endregion

        #region private properties
        private float _deathSpinSpeed; // 초당 회전 각도
        private float _deathLaunchForce; // 날아갈 힘
        private float _deathLaunchForceY; // Y축 날아가는 비율
        private Vector3 arrowImpactDirection;
        private Rigidbody _myRigid; // Rigidbody 컴포넌트를 참조할 변수
        private Animator _anim; // Animator 컴포넌트를 참조할 변수
        private PlayerController_dummy _playerMoveController;
        private PlayerAttackController_dummy _playerAttackController;
        private GameObject _playerList;
        #endregion

        void Start()
        {
            _deathSpinSpeed = 1500f; // 초당 회전 각도
            _deathLaunchForce = 15f; // 날아갈 힘
            _deathLaunchForceY = 1.2f; // Y축 날아가는 비율                               
            // Rigidbody 컴포넌트 가져오기
            _myRigid = GetComponent<Rigidbody>();
            // Animator 컴포넌트 가져오기
            _anim = GetComponent<Animator>();
            _playerMoveController = GetComponent<PlayerController_dummy>();
            _playerAttackController = GetComponent<PlayerAttackController_dummy>();
            _playerList = GameObject.Find("PlayerList");
        }

        void FixedUpdate()
        {
            // 플레이어의 체력이 0일 때 Death 함수 실행
            if (PlayerHealth <= 0 && !IsDead)
            {
                Death();
            }
            if (IsDance)
            {
                Dance();
            }
        }

        private void Dance()
        {
            // 댄스 애니메이션 실행
            StartCoroutine(Dancing(5f)); // 5초간 댄스
            IsDance = false;
        }

        private IEnumerator Dancing(float duration)
        {
            _anim.SetBool("isDance", true); // 댄스 애니메이션
            _playerMoveController.enabled = false; // 플레이어 이동 비활성화
            _playerAttackController.enabled = false; // 플레이어 공격 비활성화
            yield return new WaitForSeconds(duration);
            _playerMoveController.enabled = true; // 플레이어 이동 활성화
            _playerAttackController.enabled = true; // 플레이어 공격 활성화
            _anim.SetBool("isDance", false); // 댄스 애니메이션
        }

        private void Death()
        {
            IsDead = true;

            // PlayerController 스크립트를 비활성화
            if (_playerMoveController != null)
            {
                Destroy(_playerMoveController);
            }

            // PlayerAttackController 스크립트를 비활성화
            if (_playerAttackController != null)
            {
                Destroy(_playerAttackController);
            }

            // 빙글빙글 도는 코루틴 호출
            StartCoroutine(SpinRapidly(5f));

            // 반대 방향으로 힘을 가해 화면 밖으로 날아가게 함
            Vector3 launchDirection = arrowImpactDirection + Vector3.up * _deathLaunchForceY;
            _myRigid.AddForce(launchDirection.normalized * _deathLaunchForce, ForceMode.Impulse);

            // 코루틴을 호출하여 3초 후에 오브젝트를 비활성화
            StartCoroutine(DisableAfterDelay(5f));
        }

        private IEnumerator SpinRapidly(float duration)
        {
            float elapsedTime = 0f;
            try
            {
                while (elapsedTime < duration)
                {
                    if (elapsedTime > 0.3 && _myRigid.transform.position.y < 0.3f) // 땅에 닿을 경우 종료
                    {
                        yield break;
                    }
                    // x축 기준으로 회전 속도를 증가시킴
                    transform.Rotate(Vector3.up * _deathSpinSpeed * Time.deltaTime);
                    elapsedTime += Time.deltaTime;
                    yield return null; // 다음 프레임까지 대기
                }
            }
            finally // 코루틴 종료 시 항상 죽는 애니메이션 실행
            {
                // 플레이어 사망 애니메이션
                _anim.SetTrigger("isDead");
                Debug.Log("died");
            }
        }

        private IEnumerator DisableAfterDelay(float delay)
        {
            // 지연 시간 동안 대기
            yield return new WaitForSeconds(delay - 2);

            // 카메라 고정
            Camera mainCamera = Camera.main;
            if (mainCamera != null)
            {
                CameraController cameraController = mainCamera.GetComponent<CameraController>();
                cameraController.enabled = false;
            }

            // Collider 비활성화
            Collider collider = GetComponent<Collider>();
            if (collider != null)
            {
                collider.enabled = false;
            }

            // 2초 동안 서서히 가라앉게 함
            float sinkDuration = 2f;
            float elapsedTime = 0f;
            Vector3 initialPosition = transform.position;
            Vector3 targetPosition = initialPosition;
            targetPosition.y -= 1f; // 가라앉는 목표지점

            while (elapsedTime < sinkDuration)
            {
                transform.position = Vector3.Lerp(initialPosition, targetPosition, elapsedTime / sinkDuration);
                elapsedTime += Time.deltaTime;
                yield return null; // 다음 프레임까지 대기
            }

            // 오브젝트 자체를 비활성화 (렌더링과 상호작용을 비활성화)
            gameObject.SetActive(false);
        }

        [PunRPC]
        public void ImDie(int idx)
        {
            // 나중에 누가 죽었는지 판단해서 1등 가리기 위해 일단 체크도 해줌(당장 필요x)
            //_playerList.GetComponent<PlayerList>().DeathCount--;  // 죽었음을 모두가 알게 됨
            GameObject panel = _playerList.transform.GetChild(0).gameObject;
            GameObject player = panel.transform.GetChild(idx).gameObject;
            GameObject dieSig = player.transform.GetChild(1).gameObject;
            dieSig.SetActive(true);
        }

        // 다른 콜라이더가 트리거에 진입할 때 호출되는 메서드
        private void OnTriggerEnter(Collider other)
        {
            // AreaSlow 트리거에 진입할 경우
            if (other.CompareTag("AreaSlow"))
            {
                SlowCount++;
            }

            // 대쉬일 경우 무적 처리 (아래에 있는 대상에 한해 무적)
            if (_playerMoveController.IsDashing) 
            {
                return;
            }
            // 화살 맞을 경우
            if (other.CompareTag("Arrows"))
            {
                // NearObject 변수에 트리거에 들어온 "Arrows" 오브젝트 저장
                NearObject = other.gameObject;

                // 화살 맞을 경우 플레이어 체력 감소
                PlayerHealth -= 1;
                Debug.Log("Player HP: " + PlayerHealth);

                //if (photonView.IsMine && PhotonNetwork.LocalPlayer.CustomProperties.TryGetValue("PlayerIdx", out object PlayerIdx))
                //{
                //    int idx = (int)PlayerIdx;
                //    photonView.RPC("ImDie", RpcTarget.All, idx);
                //}
                //else
                //{
                //    Debug.Log("안죽었는데? 아니면 내가 아냐");
                //}

                // 화살의 방향을 arrowImpactDirection에 저장
                arrowImpactDirection = (transform.position - other.transform.position).normalized;
            }
            // 다른 플레이어한테 맞을 경우
            Animator otherAnim = other.transform.root.GetComponent<Animator>();
            if (otherAnim != null && other != transform.root)
            {
                bool isMeleeEnabled = otherAnim.GetBool("isMeleeEnabled");
                if (other.CompareTag("Weapon") && isMeleeEnabled)
                {
                    if (!IsDead)
                    {
                        StartCoroutine(HitIn());
                    }
                }
            }
        }

        private void OnTriggerExit(Collider other)
        {
            // AreaSlow 트리거에서 나갈 경우
            if (other.CompareTag("AreaSlow"))
            {
                SlowCount--;
            }
        }

        private void OnTriggerStay(Collider other)
        {
            // AreaSlow 스피드 업데이트
            if (other.CompareTag("AreaSlow"))
            {
                UpdateSpeed();
            }
        }

        private void UpdateSpeed()
        {
            if (SlowCount > 0)
            {
                float newSpeed = _playerMoveController.PlayerDefaultSpeed - 2 * SlowCount > 0 ? _playerMoveController.PlayerDefaultSpeed - 2 * SlowCount : 0.3f;
                _playerMoveController.ChangeSpeedForDuration(newSpeed, 0.5f);
            }
        }

        private IEnumerator HitIn()
        {
            _anim.SetTrigger("isHit"); // 피격 애니메이션
            _playerMoveController.enabled = false; // 플레이어 이동 비활성화
            _playerAttackController.enabled = false; // 플레이어 공격 비활성화
            yield return new WaitForSeconds(1f);
            _playerMoveController.enabled = true; // 플레이어 이동 활성화
            _playerAttackController.enabled = true; // 플레이어 공격 활성화
        }
    }
}
