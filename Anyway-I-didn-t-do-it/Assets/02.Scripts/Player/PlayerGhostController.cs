using Photon.Pun;
using SourGrape.hongyeop;
using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Content;
using UnityEngine;

namespace SourGrape.hongyeop
{
    public class PlayerGhostController : MonoBehaviour
    {
        #region public properties
        #endregion

        #region exposed private properties
        [SerializeField]
        private float _moveSpeed;
        [SerializeField]
        private float _mouseSensitivity;
        #endregion

        #region private properties
        private Animator _anim;
        // 버튼 감지 변수
        private bool _walkDown; // Walk button
        private bool _rotateCameraViewDown; // RotateCameraView button
        #endregion

        void Start() // 해당 오브젝트가 처음 등장할 때 한 번만 호출되는 코드
        {
            _anim = GetComponent<Animator>();
            _anim.applyRootMotion = false;  // Root Motion을 비활성화하여 애니메이션이 회전에 영향을 주지 않도록 함
            _moveSpeed = 3f;
            _mouseSensitivity = 4f;
        }
        void Update()
        {
            GetInput(); // 버튼 입력 감지
        }

        void FixedUpdate()
        {
            Move();
            Rotate(); // 마우스 X축 회전
        }

        private void GetInput()
        {
            _walkDown = Input.GetButton("Walk");
            _rotateCameraViewDown = Input.GetButton("RotateCameraView");
        }


        private void Move()
        {
            float moveDirX = Input.GetAxisRaw("Horizontal");
            float moveDirZ = Input.GetAxisRaw("Vertical");

            Vector3 moveHorizontal = transform.right * moveDirX;
            Vector3 moveVertical = transform.forward * moveDirZ;

            Vector3 velocity = (moveHorizontal + moveVertical).normalized * _moveSpeed * (_walkDown ? 0.3f : 1f);

            transform.position += velocity * Time.deltaTime; // Transform을 사용하여 위치를 직접 변경
        }

        private void Rotate()
        {
            if (_rotateCameraViewDown) // "RotateCameraView" 버튼을 누를 경우 Rotate가 무시되어 화면 회전 X (카메라만 회전하게 됨)
            {
                return;
            }
            float yRotation = Input.GetAxisRaw("Mouse X");
            Vector3 rotation = new Vector3(0f, yRotation, 0f) * _mouseSensitivity;
            transform.Rotate(rotation); // Transform을 사용하여 회전을 직접 변경
        }

        private IEnumerator Dying()
        {
            _anim.SetTrigger("Die");
            // 지연 시간 동안 대기
            yield return new WaitForSeconds(2);

            // 오브젝트 자체를 비활성화 (렌더링과 상호작용을 비활성화)
            GameObject summonParticle = PhotonNetwork.Instantiate("Effect/GrenadeExplosionBlue", transform.position, transform.rotation);
            StartCoroutine(DestroyAfterDelay(summonParticle, 2f));
            PhotonNetwork.Destroy(gameObject);
            // gameObject.SetActive(false);
        }

        private IEnumerator DestroyAfterDelay(GameObject obj, float delay)
        {
            yield return new WaitForSeconds(delay);
            PhotonNetwork.Destroy(obj);
        }
    }
}