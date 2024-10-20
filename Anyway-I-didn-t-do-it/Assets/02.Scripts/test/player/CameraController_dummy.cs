using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

namespace SourGrape.hongyeop
{
    public class CameraController_dummy : MonoBehaviour
    {
        #region private exposed properties
        [SerializeField]
        private float _verticalRotationLimit; // 수직 회전 제한 각도
        [SerializeField]
        private float _distance; // 카메라와 플레이어 간 거리
        [SerializeField]
        private float _mouseSensitivityX; // 카메라 회전 속도 x
        [SerializeField]
        private float _mouseSensitivityY; // 카메라 회전 속도 y


        [SerializeField]
        private float _cameraAdjustY; // 카메라 Y축 조정
        #endregion

        #region private properties
        private Transform _target; // 플레이어의 Transform
        private float _horizontalRotation; // 카메라 회전 각도 x
        private float _verticalRotation; // 수직 회전 각도 y
        private PlayerStatusController_dummy _playerStatusController; // 플레이어 상태 컨트롤러
        #endregion
        void Start()
        {
            _verticalRotationLimit = 80.0f;
            _distance = 5.0f;
            _mouseSensitivityX = 2.0f;
            _mouseSensitivityY = 2.0f;
            _target = GameObject.FindGameObjectWithTag("Player").transform;
            _cameraAdjustY = 1.0f;
            _playerStatusController = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerStatusController_dummy>();
        }

        void Update()
        {
            // UpdateCameraPosition();
        }

        //private void UpdateCameraPosition()
        //{
        //    bool qDown = Input.GetButton("RotateCameraView");
        //    // 마우스 Y 움직임에 따라 수직 회전 각도 조정
        //    _verticalRotation -= Input.GetAxisRaw("Mouse Y") * _mouseSensitivityY;
        //    _verticalRotation = Mathf.Clamp(_verticalRotation, 20, _verticalRotationLimit);

        //    if (qDown || _playerStatusController.IsDead)
        //    {
        //        // 마우스 X 움직임에 따라 수평 회전 각도 조정
        //        float horizontalInput = Input.GetAxisRaw("Mouse X");
        //        _horizontalRotation += horizontalInput * _mouseSensitivityX;
        //    }
        //    else
        //    {
        //        // 캐릭터 방향에 따라 수평 회전 각도 조정
        //        _horizontalRotation = _target.eulerAngles.y;
        //    }
        //    // 회전 계산
        //    Quaternion targetRotation = Quaternion.Euler(_verticalRotation, _horizontalRotation, 0);

        //    // 카메라 위치 계산
        //    Vector3 offset = new Vector3(0, 0, -_distance); // Z 방향으로 뒤로 이동
        //    Vector3 position = _target.position + targetRotation * offset;

        //    // 카메라 위치와 회전 설정
        //    transform.position = position;
        //    transform.rotation = Quaternion.LookRotation(_target.position - transform.position);
        //    transform.LookAt(_target); // 카메라가 항상 플레이어를 바라보도록 설정
        //    transform.position += new Vector3(0, _cameraAdjustY, 0); // 카메라 세부조정
        //}
    }
}
