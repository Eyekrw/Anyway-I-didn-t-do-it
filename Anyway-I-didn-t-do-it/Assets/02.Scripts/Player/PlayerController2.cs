using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Content;
using UnityEngine;

namespace SourGrape.hongyeop
{
    public class PlayerController2 : MonoBehaviour
    {
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

        [SerializeField]
        private float _moveAcc;

        [SerializeField]
        private float _maxSpeed;

        #endregion

        #region private properties
        private Animator _anim;
        // 버튼 감지 변수
        private bool _walkDown; // Walk button
        private bool _rotateCameraViewDown; // RotateCameraView button
        private bool _dashDown;

        private float _aniMoveDirX; // 애니메이션 제어 변수
        private float _aniMoveDirZ; // 애니메이션 제어 변수

        private float _dashCurrentCooldown = 0f; // 대시 쿨다운 타이머
        private float _dashSpeed = 50.0f; // 대시 속도
        private float _dashMaxTime = 0.17f; // 대쉬 지속 시간
        private float _dashCurrentTime;
        private bool _dashCan = true; // 대시 가능 여부
        private bool _dashInvisible = false; // 대시 무적 여부
        private bool _dashCheck = false; // 현재 대시 상황 여부
        private Vector3 _dashDirection;

        #endregion

        void Start() // 해당 오브젝트가 처음 등장할 때 한 번만 호출되는 코드
        {
            _anim = GetComponentInChildren<Animator>();
            _anim.applyRootMotion = false;  // Root Motion을 비활성화하여 애니메이션이 회전에 영향을 주지 않도록 함
            _myRigid = GetComponent<Rigidbody>();
            _moveSpeed = 5f;
            _mouseSensitivity = 4f;
            _aniSmoothSpeed = 6f;
            _dashCooldown = 3.0f;
            _moveAcc = 10f;
            _maxSpeed = 10f;
        }
        void Update()
        {
            GetInput(); // 버튼 입력 감지
            DashInput();
        }

        void FixedUpdate() // 물리 연산을 처리하는 고정된 시간 간격마다 호출 (update는 프레임마다 호출)
        {
            if (_dashCheck)
            {
                Dash();
            }
            else
            {
                Move(); // WASD 이동
            }
            Rotate(); // 마우스 X축 회전
            AnimatorUpdate(); // Animator 상태 업데이트
        }
        private void GetInput()
        {
            _walkDown = Input.GetButton("Walk");
            _rotateCameraViewDown = Input.GetButton("RotateCameraView");
            _dashDown = Input.GetKeyDown(KeyCode.Space);
            if (_dashDown)
            {
                Debug.Log("누름");
                Debug.Log(_dashCurrentCooldown);
            }
        }

        private void DashInput()
        {
            if (Input.GetKeyDown(KeyCode.Space) && _dashCan && !_dashCheck && _dashCurrentCooldown <= 0)
            {
                DashStart();
                Debug.Log("대시");
            }
            if (_dashCurrentCooldown > 0)
            {
                _dashCurrentCooldown -= Time.deltaTime;
            }
        }


        private void Dash()
        {
            if (_dashCurrentTime > 0)
            {
                _myRigid.MovePosition(transform.position + transform.forward * _dashSpeed * Time.deltaTime);
                _dashCurrentTime -= Time.deltaTime;
            }
            else
            {
                _dashCheck = false;
                _dashInvisible = false;
            }
        }


        private void DashStart()
        {
            _dashCheck = true;
            _dashCurrentTime = _dashMaxTime;
            _dashInvisible = true;

            float horizontal = Input.GetAxis("Horizontal");
            float vertical = Input.GetAxis("Vertical");
            _dashDirection = (transform.right * horizontal + transform.forward * vertical).normalized;

            if (_dashDirection == Vector3.zero)
            {
                // 아무 입력이 없을 경우에는 앞으로 대쉬
                _dashDirection = transform.forward;
            }
            _anim.SetTrigger("isDash"); // 대쉬 애니메이션 트리거
            _dashCurrentCooldown = _dashCooldown; // 쿨다운 시작
        }

        private void Move()
        {
            float moveX = Input.GetAxis("Horizontal");
            float moveZ = Input.GetAxis("Vertical");

            Vector3 moveDir = new Vector3(moveX, 0f, moveZ).normalized;

            if (moveDir.magnitude < _maxSpeed)
            {
                _myRigid.AddForce(moveDir * _moveAcc * Time.deltaTime, ForceMode.VelocityChange);
            }
            else
            {
                _myRigid.velocity = _myRigid.velocity.normalized * _maxSpeed;
            }
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
    }
}