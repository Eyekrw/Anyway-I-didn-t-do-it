using Photon.Pun;
using UnityEngine;

namespace SourGrape.kiyoung
{
    public class Palette : MonoBehaviourPun
    {
        public float speed = 5f;
        public float distanceThreshold = 20f;

        // 속도를 외부에서 읽을 수 있도록 프로퍼티로 노출
        public Vector3 _currentVelocity { get; private set; }

        private Vector3 _startPosition;
        private Vector3 _targetPosition;
        private Vector3 _previousPosition;  // 이전 프레임 위치 저장

        private bool movingLeft = true;

        private void Start()
        {
            _startPosition = transform.position;
            _targetPosition = new Vector3(_startPosition.x - distanceThreshold, _startPosition.y, _startPosition.z);
            _previousPosition = _startPosition;  // 초기 위치 설정
        }

        private void FixedUpdate()
        {
            if (PhotonNetwork.IsMasterClient)
            {
                MovePalette();  // 마스터 클라이언트에서만 이동 로직 수행
                _previousPosition = transform.position;
            }
            else
            {
                // 클라이언트가 마스터의 속도를 따라가도록 설정
                _currentVelocity = (transform.position - _previousPosition) / Time.fixedDeltaTime;
                _previousPosition = transform.position;
            }
        }

        private void MovePalette()
        {
            if (transform.position == _targetPosition)
            {
                movingLeft = !movingLeft;
                _targetPosition = movingLeft
                    ? new Vector3(_startPosition.x - distanceThreshold, _startPosition.y, _startPosition.z)
                    : new Vector3(_startPosition.x + distanceThreshold, _startPosition.y, _startPosition.z);
            }

            // 현재 속도를 계산하고 이동
            Vector3 previousPosition = transform.position;
            transform.position = Vector3.MoveTowards(transform.position, _targetPosition, speed * Time.fixedDeltaTime);
            _currentVelocity = (transform.position - previousPosition) / Time.fixedDeltaTime;
        }
    }
}