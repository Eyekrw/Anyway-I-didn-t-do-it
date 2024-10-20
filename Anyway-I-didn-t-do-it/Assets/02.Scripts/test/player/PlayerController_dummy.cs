using SourGrape.hongyeop;
using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Content;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;

namespace SourGrape.hongyeop
{
    public class PlayerController_dummy : MonoBehaviourPun
    {
        #region public properties
        public GameObject ProjectileParticle; // Effect attached to the gameobject as child
        public bool IsDashing = false;
        public float PlayerDefaultSpeed = 5.0f; // deflaut 이동 속도
        #endregion

        #region exposed private properties
        [SerializeField] // private property를 Inspector 창에서 수정할 수 있게 해줌
        private Rigidbody _myRigid;
        [SerializeField]
        private float _moveSpeed;
        [SerializeField]
        private float _mouseSensitivity;
        [SerializeField]
        private float _aniSmoothSpeed; // 애니메이션 부드러운 전환을 위한 변수
        [SerializeField]
        private float _dashCooldown; // 대시 쿨다운
        #endregion

        #region private properties
        private Coroutine _changeSpeedCoroutine; // 속도 변경 코루틴
        private Animator _anim;
        // 버튼 감지 변수
        private bool _walkDown; // Walk button
        private bool _rotateCameraViewDown; // RotateCameraView button
        private bool _dashDown; // 일단은 space
        private bool _teaBagging1Down; // TeaBagging1 button
        private bool _teaBagging2Down; // TeaBagging2 button
        private bool _teaBagging3Down; // TeaBagging3 button

        private float _aniMoveDirX; // 애니메이션 제어 변수
        private float _aniMoveDirZ; // 애니메이션 제어 변수

        private float _dashCurrentCooldown = 0f; // 대시 쿨다운 타이머
        private float _dashSpeed = 50.0f; // 대시 중 속도
        private float _dashCurrentTime; // 대쉬 타이머
        private float _dashMaxTime = 0.17f; // 대쉬 지속 시간
        private Vector3 _dashDirection; // 대쉬 방향
        #endregion

        public void ChangeSpeedForDuration(float newSpeed, float duration)
        {
            // 현재 실행 중인 코루틴이 있으면 중지
            if (_changeSpeedCoroutine != null)
            {
                StopCoroutine(_changeSpeedCoroutine);
            }
            // 새로운 코루틴 시작
            _changeSpeedCoroutine = StartCoroutine(ChangeSpeedCoroutine(newSpeed, duration));
        }

        void Start() // 해당 오브젝트가 처음 등장할 때 한 번만 호출되는 코드
        {
            _anim = GetComponentInChildren<Animator>();
            _anim.applyRootMotion = false;  // Root Motion을 비활성화하여 애니메이션이 회전에 영향을 주지 않도록 함
            _myRigid = GetComponent<Rigidbody>();
            _moveSpeed = PlayerDefaultSpeed;
            _mouseSensitivity = 4f;
            _aniSmoothSpeed = 6f;
            _dashCooldown = 3.0f;
        }
        void Update()
        {
            // if(photonView.IsMine)
            GetInput(); // 버튼 입력 감지
        }


        void FixedUpdate() // 물리 연산을 처리하는 고정된 시간 간격마다 호출 (update는 프레임마다 호출)
        {
            // if (photonView.IsMine)
            {
                Dash();
                Move();
                Rotate(); // 마우스 X축 회전
                AnimatorUpdate(); // Animator 상태 업데이트
                TeaBagging();
            }
        }

        private IEnumerator ChangeSpeedCoroutine(float newSpeed, float duration)
        {
            _moveSpeed = newSpeed;
            yield return new WaitForSeconds(duration);  // 주어진 시간 동안 대기
            _moveSpeed = PlayerDefaultSpeed;
            _changeSpeedCoroutine = null; // 코루틴이 종료되었음을 표시
        }

        private void GetInput()
        {
            _walkDown = Input.GetButton("Walk");
            _rotateCameraViewDown = Input.GetButton("RotateCameraView");
            _dashDown = Input.GetKeyDown(KeyCode.Space);
            _teaBagging1Down = Input.GetButton("TeaBagging1");
            _teaBagging2Down = Input.GetButton("TeaBagging2");
            _teaBagging3Down = Input.GetKeyDown(KeyCode.Alpha3);
        }

        private void Dash()
        {
            _dashCurrentCooldown -= Time.deltaTime;
            if (_dashCurrentCooldown > 0) // cooldown일 경우 대쉬 불가
            {
                Debug.Log("Dash on cooldown: " + _dashCurrentCooldown);
                return;
            }

            if (_dashDown) // 대쉬 버튼 누를 경우
            {
                _dashCurrentCooldown = _dashCooldown; // 쿨다운 초기화

                // 대쉬 방향 설정
                float horizontal = Input.GetAxis("Horizontal");
                float vertical = Input.GetAxis("Vertical");
                _dashDirection = (transform.right * horizontal + transform.forward * vertical).normalized;
                if (_dashDirection == Vector3.zero)// 아무 입력이 없을 경우에는 앞으로 대쉬
                {
                    _dashDirection = transform.forward;
                }
                StartCoroutine(PerformDash()); // 대쉬 코루틴
                _anim.SetTrigger("isDash"); // 대쉬 애니메이션 트리거
                Quaternion dashRotation = Quaternion.LookRotation(_dashDirection, Vector3.up);
                if (ProjectileParticle != null)
                {
                    GameObject projectileParticle = Instantiate(ProjectileParticle, transform.position, dashRotation);
                    projectileParticle.transform.rotation = Quaternion.LookRotation(_dashDirection, Vector3.up);
                    Destroy(projectileParticle, _dashMaxTime);
                }
            }
        }

        private IEnumerator PerformDash()
        {
            _dashCurrentTime = _dashMaxTime; // 대쉬 지속 시간 초기화
            IsDashing = true; // 대쉬 확인 변수

            // 대쉬 지속 시간 동안
            while (_dashCurrentTime > 0)
            {
                // 대쉬 중 속도 유지
                _myRigid.velocity = _dashDirection * _dashSpeed;

                // 대쉬 지속 시간 감소
                _dashCurrentTime -= Time.deltaTime;

                yield return null; // 다음 프레임까지 대기
            }

            // 대쉬가 끝나면 속도를 0으로 설정 (또는 원래 이동 속도로 복귀)
            _myRigid.velocity = Vector3.zero;
            IsDashing = false; // 대쉬 종료
        }

        private void Move()
        {
            float moveDirX = Input.GetAxisRaw("Horizontal");
            float moveDirZ = Input.GetAxisRaw("Vertical");

            Vector3 moveHorizontal = transform.right * moveDirX;
            Vector3 moveVertical = transform.forward * moveDirZ;

            Vector3 velocity = (moveHorizontal + moveVertical).normalized * _moveSpeed * (_walkDown ? 0.3f : 1f);

            _myRigid.MovePosition(transform.position += velocity * Time.deltaTime); // rigidbody를 처리할 경우 물리값 같이 처리
        }

        private void Rotate()
        {
            if (_rotateCameraViewDown) // "RotateCameraView" 버튼을 누를 경우 Rotate가 무시되어 화면 회전 X (카메라만 회전하게 됨)
            {
                return;
            }
            float yRoation = Input.GetAxisRaw("Mouse X");
            Vector3 rotation = new Vector3(0f, yRoation, 0f) * _mouseSensitivity;
            _myRigid.MoveRotation(_myRigid.rotation * Quaternion.Euler(rotation));
        }


        private void AnimatorUpdate()
        {
            // 이동 속도를 계산 (Walk 버튼을 누르면 0.3배로 감소)
            float targetMoveDirX = Input.GetAxisRaw("Horizontal") * (_walkDown ? 0.3f : 1f);
            float targetMoveDirZ = Input.GetAxisRaw("Vertical") * (_walkDown ? 0.3f : 1f);

            // Target 값에 근접할 때 타겟값에 설정 (threshold)
            if (Mathf.Abs(_aniMoveDirX - targetMoveDirX) < 0.1f)
            {
                _aniMoveDirX = targetMoveDirX;
            }
            if (Mathf.Abs(_aniMoveDirZ - targetMoveDirZ) < 0.1f)
            {
                _aniMoveDirZ = targetMoveDirZ;
            }

            // Lerp를 이용하여 부드러운 이동 처리
            if (_aniMoveDirX < targetMoveDirX)
            {
                _aniMoveDirX += Time.deltaTime * _aniSmoothSpeed;
            }
            else if (_aniMoveDirX > targetMoveDirX)
            {
                _aniMoveDirX -= Time.deltaTime * _aniSmoothSpeed;
            }

            if (_aniMoveDirZ < targetMoveDirZ)
            {
                _aniMoveDirZ += Time.deltaTime * _aniSmoothSpeed;
            }
            else if (_aniMoveDirZ > targetMoveDirZ)
            {
                _aniMoveDirZ -= Time.deltaTime * _aniSmoothSpeed;
            }

            // 애니메이터 상태 업데이트
            _anim.SetFloat("DirX", _aniMoveDirX); // 좌우 이동에 따른 애니메이션 제어
            _anim.SetFloat("DirZ", _aniMoveDirZ); // 앞뒤 이동에 따른 애니메이션 제어
        }

        private void TeaBagging()
        {
            // teabagging1
            if (_teaBagging1Down)
            {
                _anim.SetBool("isTeaBagging1", true);
            }
            else
            {
                _anim.SetBool("isTeaBagging1", false);
            }

            // teabagging2
            if (_teaBagging2Down)
            {
                _anim.SetBool("isTeaBagging2", true);
            }
            else
            {
                _anim.SetBool("isTeaBagging2", false);
            }

            // teabagging3
            if (_teaBagging3Down)
            {
                Debug.Log("TeaBagging3 버튼이 눌렸습니다!");
                _anim.SetTrigger("DoTeaBagging3");
                _anim.SetBool("isTeaBagging", true);

            }
            else
            {
                _anim.SetBool("isTeaBagging", false);
            }
        }
    }
}
