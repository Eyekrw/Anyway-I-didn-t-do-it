using UnityEngine;
using Photon.Pun;

namespace SourGrape.minwu
{
    public class Wheel : MonoBehaviourPun
    {
        // Static fields
        public static float s_RotationSpeed = 6f;

        // Private fields
        private float _currentRotation;
        private float _timer;
        private int _state; // 0: rotating forward, 1: stopped, 2: rotating backward

        private void Start()
        {
            _currentRotation = 0f;
            _timer = 0f;
            _state = 0; // Start with rotating forward
        }

        private void FixedUpdate()
        {
            if (PhotonNetwork.IsMasterClient)
            {
                // 마스터 클라이언트에서만 회전 로직 수행
                switch (_state)
                {
                    case 0: // Rotating forward
                        _currentRotation += s_RotationSpeed * Time.fixedDeltaTime;
                        if (_timer >= 20f)
                        {
                            _state = 1;
                            _timer = 0f;
                        }
                        break;

                    case 1: // Stopped
                        if (_timer >= 5f)
                        {
                            _state = 2;
                            _timer = 0f;
                        }
                        break;

                    case 2: // Rotating backward
                        _currentRotation += -s_RotationSpeed * Time.fixedDeltaTime;
                        if (_timer >= 20f)
                        {
                            _state = 0;
                            _timer = 0f;
                        }
                        break;
                }

                _timer += Time.fixedDeltaTime;

                // 회전값을 transform에 적용하여 PhotonTransformView가 자동으로 동기화하도록 함
                transform.rotation = Quaternion.Euler(0, _currentRotation, 0);
            }
        }
    }
}