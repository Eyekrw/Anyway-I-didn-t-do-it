using SourGrape.hongyeop;
using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Content;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using SourGrape.kiyoung;

namespace SourGrape.hongyeop
{
    public class PlayerController : MonoBehaviourPun
    {
        #region public properties
        public GameObject ProjectileParticle; // Effect attached to the gameobject as child
        public bool IsDashing = false;
        public float PlayerDefaultSpeed = 5.0f; // deflaut �̵� �ӵ�
        #endregion

        #region exposed private properties
        [SerializeField] // private property�� Inspector â���� ������ �� �ְ� ����
        private Rigidbody _myRigid;
        [SerializeField]
        private float _moveSpeed;
        [SerializeField]
        private float _mouseSensitivity;
        [SerializeField]
        private float _aniSmoothSpeed; // �ִϸ��̼� �ε巯�� ��ȯ�� ���� ����
        [SerializeField]
        private float _dashCooldown; // ��� ��ٿ�
        #endregion

        #region private properties
        private Coroutine _changeSpeedCoroutine; // �ӵ� ���� �ڷ�ƾ
        private Animator _anim;
        // ��ư ���� ����
        private bool _walkDown; // Walk button
        private bool _rotateCameraViewDown; // RotateCameraView button
        private bool _dashDown; // �ϴ��� space
        private bool _teaBagging1Down; // TeaBagging1 button
        private bool _teaBagging2Down; // TeaBagging2 button
        private bool _teaBagging3Down; // TeaBagging3 button

        private bool _moveflag;
        private bool isOnIceMap = false; // ���� �� ���θ� Ȯ���� ����
        private bool isOnPalette = false;   // 현재 뗏목 위에 있는가
        private bool isOnReversedMap = false;
        private Palette _currentPalette;  // 현재 팔레트 참조를 저장
        private Vector3 _platformVelocity;    // 팔레트 속도

        private float _aniMoveDirX; // �ִϸ��̼� ���� ����
        private float _aniMoveDirZ; // �ִϸ��̼� ���� ����

        private float _dashCurrentCooldown = 0f; // ��� ��ٿ� Ÿ�̸�
        private float _dashSpeed = 50.0f; // ��� �� �ӵ�
        private float _dashCurrentTime; // �뽬 Ÿ�̸�
        private float _dashMaxTime = 0.17f; // �뽬 ���� �ð�
        private Vector3 _dashDirection; // �뽬 ����
        #endregion

        void Start() // �ش� ������Ʈ�� ó�� ������ �� �� ���� ȣ��Ǵ� �ڵ�
        {
            _anim = GetComponentInChildren<Animator>();
            _anim.applyRootMotion = false;  // Root Motion�� ��Ȱ��ȭ�Ͽ� �ִϸ��̼��� ȸ���� ������ ���� �ʵ��� ��
            _myRigid = GetComponent<Rigidbody>();
            _moveSpeed = PlayerDefaultSpeed;
            _mouseSensitivity = 4f;
            _aniSmoothSpeed = 6f;
            _dashCooldown = 3.0f;
            _moveflag = true;
            //PlayerSetting();
        }
        void Update()
        {
            if (photonView.IsMine)
            {
                if (_moveflag)
                {
                GetInput(); // ��ư �Է� ����
                }
            }
        }


        void FixedUpdate() // ���� ������ ó���ϴ� ������ �ð� ���ݸ��� ȣ�� (update�� �����Ӹ��� ȣ��)
        {
            if (photonView.IsMine)
            {
                Dash();
                if (_moveflag)
                {
                    if (isOnIceMap)
                    {
                        MoveOnIce();
                    }
                    else if (isOnPalette)
                    {
                        MoveOnPalette();  // 팔레트 위에 있을 때 이동 로직
                    }
                    else
                    {
                        Move();
                    }
                }
                else if (isOnPalette)
                {
                    MoveOnPalette();
                }
                Rotate(); // ���콺 X�� ȸ��
                AnimatorUpdate(); // Animator ���� ������Ʈ
                TeaBagging();
            }
        }

        public void PlayerSetting()
        {
            //GameObject mapBorder = GameObject.Find("MapBorder");
            //if (mapBorder != null)
            //{
            //    mapBorder.GetComponent<MapBorderMaterialSwitcher>().SetPlayer(this.transform);
            //}
            //if (mainCamera != null)
            //{
            //    mainCamera.gameObject.SetActive(true); // ī�޶� ���� Ȱ��ȭ <-- ���ص� �� �׳� �� ���·� �����ص� ��
            //}
        }

        public void ChangeSpeedForDuration(float newSpeed, float duration)
        {
            // ���� ���� ���� �ڷ�ƾ�� ������ ����
            if (_changeSpeedCoroutine != null)
            {
                StopCoroutine(_changeSpeedCoroutine);
            }
            // ���ο� �ڷ�ƾ ����
            _changeSpeedCoroutine = StartCoroutine(ChangeSpeedCoroutine(newSpeed, duration));
        }

        public void SetFlag(bool temp)
        {
            _moveflag = temp;
        }
        private IEnumerator ChangeSpeedCoroutine(float newSpeed, float duration)
        {
            _moveSpeed = newSpeed;
            yield return new WaitForSeconds(duration);  // �־��� �ð� ���� ���
            _moveSpeed = PlayerDefaultSpeed;
            _changeSpeedCoroutine = null; // �ڷ�ƾ�� ����Ǿ����� ǥ��
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

        private IEnumerator DestroyAfterDelay(GameObject obj, float delay)
        {
            yield return new WaitForSeconds(delay);
            PhotonNetwork.Destroy(obj);
        }

        private void Dash()
        {
            _dashCurrentCooldown -= Time.deltaTime;
            if (_dashCurrentCooldown > 0) // cooldown�� ��� �뽬 �Ұ�
            {
                //Debug.Log("Dash on cooldown: " + _dashCurrentCooldown);
                return;
            }

            if (_dashDown) // �뽬 ��ư ���� ���
            {
                _dashCurrentCooldown = _dashCooldown; // ��ٿ� �ʱ�ȭ

                // �뽬 ���� ����
                float horizontal = Input.GetAxis("Horizontal");
                float vertical = Input.GetAxis("Vertical");
                if (isOnReversedMap)
                {
                    horizontal = -horizontal;
                    vertical = -vertical;
                }
                _dashDirection = (transform.right * horizontal + transform.forward * vertical).normalized;
                if (_dashDirection == Vector3.zero)// �ƹ� �Է��� ���� ��쿡�� ������ �뽬
                {
                    _dashDirection = transform.forward;
                }
                StartCoroutine(PerformDash()); // �뽬 �ڷ�ƾ
                _anim.SetTrigger("isDash"); // �뽬 �ִϸ��̼� Ʈ����
                Quaternion dashRotation = Quaternion.LookRotation(_dashDirection, Vector3.up);
                // 대쉬 이펙트 photon 생성
                GameObject projectileParticle = PhotonNetwork.Instantiate("Effect/MagicPillarBlastBlue", transform.position, dashRotation);
                projectileParticle.transform.rotation = Quaternion.LookRotation(_dashDirection, Vector3.up); 
                StartCoroutine(DestroyAfterDelay(projectileParticle, 2f));
            }
        }

        private IEnumerator PerformDash()
        {
            _dashCurrentTime = _dashMaxTime; // �뽬 ���� �ð� �ʱ�ȭ
            IsDashing = true; // �뽬 Ȯ�� ����

            // �뽬 ���� �ð� ����
            while (_dashCurrentTime > 0)
            {
                // �뽬 �� �ӵ� ����
                _myRigid.velocity = _dashDirection * _dashSpeed;

                // �뽬 ���� �ð� ����
                _dashCurrentTime -= Time.deltaTime;

                yield return null; // ���� �����ӱ��� ���
            }

            // �뽬�� ������ �ӵ��� 0���� ���� (�Ǵ� ���� �̵� �ӵ��� ����)
            _myRigid.velocity = Vector3.zero;
            IsDashing = false; // �뽬 ����
        }

        private void Move()
        {
            float moveDirX = Input.GetAxisRaw("Horizontal");
            float moveDirZ = Input.GetAxisRaw("Vertical");

            if (isOnReversedMap)
            { 
                moveDirX = -moveDirX;
                moveDirZ = -moveDirZ;
            }

            Vector3 moveHorizontal = transform.right * moveDirX;
            Vector3 moveVertical = transform.forward * moveDirZ;

            Vector3 velocity = (moveHorizontal + moveVertical).normalized * _moveSpeed * (_walkDown ? 0.3f : 1f);

            _myRigid.MovePosition(transform.position += velocity * Time.deltaTime); // rigidbody�� ó���� ��� ������ ���� ó��
        }

        private void Rotate()
        {
            if (_rotateCameraViewDown) // "RotateCameraView" ��ư�� ���� ��� Rotate�� ���õǾ� ȭ�� ȸ�� X (ī�޶� ȸ���ϰ� ��)
            {
                return;
            }
            float yRoation = Input.GetAxisRaw("Mouse X");
            Vector3 rotation = new Vector3(0f, yRoation, 0f) * _mouseSensitivity;
            _myRigid.MoveRotation(_myRigid.rotation * Quaternion.Euler(rotation));
        }


        private void AnimatorUpdate()
        {
            // �̵� �ӵ��� ��� (Walk ��ư�� ������ 0.3��� ����)
            float targetMoveDirX = Input.GetAxisRaw("Horizontal") * (_walkDown ? 0.3f : 1f);
            float targetMoveDirZ = Input.GetAxisRaw("Vertical") * (_walkDown ? 0.3f : 1f);

            // Target ���� ������ �� Ÿ�ٰ��� ���� (threshold)
            if (Mathf.Abs(_aniMoveDirX - targetMoveDirX) < 0.1f)
            {
                _aniMoveDirX = targetMoveDirX;
            }
            if (Mathf.Abs(_aniMoveDirZ - targetMoveDirZ) < 0.1f)
            {
                _aniMoveDirZ = targetMoveDirZ;
            }

            // Lerp�� �̿��Ͽ� �ε巯�� �̵� ó��
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

            // �ִϸ����� ���� ������Ʈ
            _anim.SetFloat("DirX", _aniMoveDirX); // �¿� �̵��� ���� �ִϸ��̼� ����
            _anim.SetFloat("DirZ", _aniMoveDirZ); // �յ� �̵��� ���� �ִϸ��̼� ����
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
                _anim.SetTrigger("DoTeaBagging3");
                _anim.SetBool("isTeaBagging", true);

            }
            else
            {
                _anim.SetBool("isTeaBagging", false);
            }
        }

        private void MoveOnIce()
        {
            float moveDirX = Input.GetAxisRaw("Horizontal");
            float moveDirZ = Input.GetAxisRaw("Vertical");

            // 얼음맵의 경우 addforce를 통해 미끄러지는 이동
            Vector3 moveHorizontal = Camera.main.transform.right * moveDirX;
            Vector3 moveVertical = Camera.main.transform.forward * moveDirZ;

            Vector3 moveDir = (moveHorizontal + moveVertical).normalized;

            if (_myRigid.velocity.magnitude < _moveSpeed)
            {
                _myRigid.AddForce(moveDir * _moveSpeed, ForceMode.Force);
            }
            else
            {
                _myRigid.velocity = Vector3.Lerp(_myRigid.velocity, moveDir * _moveSpeed, 0.2f);
            }
        }

        // 팔레트 위에 있을 때 이동 처리
        private void MoveOnPalette()
        {
            Vector3 inputVelocity = Vector3.zero;

            // 플레이어 입력에 따른 이동 속도 계산
            if (_moveflag)  // _moveflag가 true일 때만 입력에 따른 이동을 계산
            {
                float moveDirX = Input.GetAxisRaw("Horizontal");
                float moveDirZ = Input.GetAxisRaw("Vertical");

                Vector3 moveHorizontal = transform.right * moveDirX;
                Vector3 moveVertical = transform.forward * moveDirZ;

                // 기본 이동 속도
                inputVelocity = (moveHorizontal + moveVertical).normalized * _moveSpeed;
            }

            // 현재 팔레트의 이동 속도를 실시간으로 참조
            _platformVelocity = _currentPalette != null ? _currentPalette._currentVelocity : Vector3.zero;

            // 팔레트 속도와 입력 속도 합산 (플래그가 false일 경우에는 inputVelocity = Vector3.zero이므로 팔레트 속도만 반영됨)
            Vector3 combinedVelocity = inputVelocity + _platformVelocity;

            // Rigidbody의 MovePosition을 이용해 물리적으로 이동
            _myRigid.MovePosition(transform.position + combinedVelocity * Time.fixedDeltaTime);

            // 물리 동기화 (팔레트와 캐릭터의 위치 차이 최소화)
            if (_moveflag)
            {
            }
            Physics.SyncTransforms();
        }

        // OnCollisionEnter, Exit으로 얼음맵 / 뗏목 / 거울맵 위인지 확인
        private void OnCollisionEnter(Collision collision)
        {
            if (collision.gameObject.name == "IcePlane")
            {
                isOnIceMap = true;
            }
            else if (collision.gameObject.CompareTag("Palette"))
            {
                isOnPalette = true;
                _currentPalette = collision.gameObject.GetComponent<Palette>(); // 팔레트 참조 설정
            }
            else if (collision.gameObject.name == "ReversedPlane")
            {
                isOnReversedMap = true;
            }
        }

        private void OnCollisionExit(Collision collision)
        {
            if (collision.gameObject.name == "IcePlane")
            {
                isOnIceMap = false;
            }
            else if (collision.gameObject.CompareTag("Palette"))
            {
                isOnPalette = false;
                _currentPalette = null; // 팔레트 참조 해제
            }
            else if (collision.gameObject.name == "ReversedPlane")
            {
                isOnReversedMap = false;
            }
        }

        public float GetCurrentDashCooldown()
        {
            if (_dashCurrentCooldown > 0.06f)
            {
            return _dashCurrentCooldown;  // �ܺο��� ��Ÿ�� ���� �������� ���� �Լ�
            } else { return 0; }
        }

        public float GetDashCooldown()
        {
            return _dashCooldown;
        }
    }
}
