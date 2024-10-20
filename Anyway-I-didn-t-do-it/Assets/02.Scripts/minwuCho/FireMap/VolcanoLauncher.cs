using SourGrape.hongyeop;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using SourGrape.kiyoung;

namespace SourGrape.minwu
{
    public class VolcanoLauncher : MonoBehaviourPun
    {
        [SerializeField]
        private GameObject _lavaPrefab; // 라바 프리팹
        private float _spawnInterval = 2f; // 생성 간격
        private float _minLaunchForce = 5f; // 최소 발사 힘
        private float _maxLaunchForce = 15f; // 최대 발사 힘
        private float _maxDistance = 32f;
        [SerializeField]
        private AudioClip _lavaSound; // 라바 소리 오디오 클립
        private AudioSource _audioSource; // 오디오 소스 컴포넌트
        // TODO : 화살 오류 수정 후 해당 부분을 반영해서 RPC로 화산에서 발사하도록 / 오브젝트 풀링을 적용해서 코드 수정
        private void Start()
        {
            if (PhotonNetwork.IsMasterClient)
            {
                InvokeRepeating(nameof(SpawnLava), 0f, _spawnInterval);
            }
            _audioSource = GetComponent<AudioSource>();
        }

        private void SpawnLava()
        {
            GameObject lava = PhotonNetwork.Instantiate("Projectiles/Lava", transform.position, Quaternion.identity);

            float randomDirection = Random.Range(0f, 360f);
            float randomAngle = Random.Range(0f, 45f);

            Vector3 direction = Quaternion.Euler(randomAngle, randomDirection, 0) * Vector3.forward;

            float launchForce = Random.Range(_minLaunchForce, _maxLaunchForce);

            Rigidbody rb = lava.GetComponent<Rigidbody>();
            if (rb != null)
            {
                rb.AddForce(direction * launchForce, ForceMode.Impulse);
            }

            LavaDistanceChecker distanceChecker = lava.AddComponent<LavaDistanceChecker>();
            distanceChecker.Initialize(transform.position, _maxDistance);
            if (_audioSource != null && _lavaSound != null)
            {
                _audioSource.PlayOneShot(_lavaSound);
            }
        }
    }

    public class LavaDistanceChecker : MonoBehaviour
    {
        private Vector3 _centerPosition;
        private float _maxDistance;
        private GameManager _gameManager;  

        public void Initialize(Vector3 centerPosition, float maxDistance)
        {
            _centerPosition = centerPosition;
            _maxDistance = maxDistance;
            GameObject gameManagerObject = GameObject.Find("GameManager");

            if (gameManagerObject != null)
            {
                _gameManager = gameManagerObject.GetComponent<GameManager>();
            }
        }

        private void Update()
        {
            Vector3 currentPosition = transform.position;

            float distanceXZ = Vector3.Distance(new Vector3(currentPosition.x, 0, currentPosition.z),
                                                new Vector3(_centerPosition.x, 0, _centerPosition.z));

            if (PhotonNetwork.IsMasterClient && _gameManager._isRoundOver)
            {
                PhotonNetwork.Destroy(gameObject);
            }


            if (distanceXZ > _maxDistance)
            {
                PhotonNetwork.Destroy(gameObject);
            }
        }
    }
}