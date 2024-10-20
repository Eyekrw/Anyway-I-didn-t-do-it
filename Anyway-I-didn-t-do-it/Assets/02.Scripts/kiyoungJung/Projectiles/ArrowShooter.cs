using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SourGrape.kiyoung
{
    public class ArrowShooter : MonoBehaviourPunCallbacks
    {
        public ObjectPoolingManager poolingManager;  // ȭ�� ������Ʈ Ǯ�� �Ŵ���

        #region private properties
        [SerializeField]
        private float _shootInterval = 2f;
        [SerializeField]
        private int _arrowMultiplier = 1;
        [SerializeField]
        private float _arrowHeight = 1.5f;
        [SerializeField]
        private float _arrowSpeed = 5f;

        private TurretSpawner _spawner;
        #endregion

        public override void OnEnable()
        {
            //GameManager.OnIncreaseArrowCount += IncreaseArrowCount;
        }

        public override void OnDisable()
        {
            //GameManager.OnIncreaseArrowCount -= IncreaseArrowCount;
        }

        // �� �ε� �� ĳ���� ������ �Ϸ�Ǹ� ���ӸŴ������� ȭ�� �߻縦 ���� ������ �Լ�
        public void StartShooting()
        {
            _spawner = new TurretSpawner(transform, 30f, 36);
            StartCoroutine(ArrowsShootingCycle()); // ĳ���� ���� �� ����
        }

        public void StopShooting()
        {
            _arrowMultiplier = 1;
            StopAllCoroutines();
        }

        private void IncreaseArrowCount()
        {
            _arrowMultiplier++;
        }

        private IEnumerator ArrowsShootingCycle()
        {
            while (true)
            {
                Debug.Log(_arrowMultiplier);
                // �÷��̾� ���� �߻�
                GameObject localPlayerObj = PhotonNetwork.LocalPlayer.TagObject as GameObject;
                if (localPlayerObj != null)
                {
                    Debug.Log("Player shot");
                    Transform playerTransform = localPlayerObj.transform;
                    List<Transform> targetedSpawnPoints = _spawner.GetRandomSpawnPoints(2 * _arrowMultiplier);

                    foreach (Transform spawnPoint in targetedSpawnPoints)
                    {
                        Vector3 targetPosition = new Vector3(playerTransform.position.x, playerTransform.position.y + _arrowHeight, playerTransform.position.z);
                        photonView.RPC("ShootArrowRPC", RpcTarget.MasterClient, spawnPoint.position, targetPosition);
                    }
                }

                if (PhotonNetwork.IsMasterClient)
                {
                    // ������ Ŭ���̾�Ʈ������ ���� �߻�
                    Debug.Log("Random shot");
                    List<Transform> spawnPoints = _spawner.GetRandomSpawnPoints(4 * _arrowMultiplier);
                    foreach (Transform spawnPoint in spawnPoints)
                    {
                        Vector3 targetPosition = GetRandomTargetPosition(spawnPoint);
                        photonView.RPC("ShootArrowRPC", RpcTarget.MasterClient, spawnPoint.position, targetPosition);
                    }
                }

                yield return new WaitForSeconds(_shootInterval);
            }
        }

        [PunRPC]
        private void ShootArrowRPC(Vector3 spawnPosition, Vector3 targetPosition)
        {
            GameObject arrow = poolingManager.GetPooledObject();
            if (arrow != null)
            {
                spawnPosition.y = _arrowHeight;
                targetPosition.y = _arrowHeight;

                arrow.transform.position = spawnPosition;
                arrow.transform.rotation = Quaternion.LookRotation(targetPosition - spawnPosition);
                arrow.SetActive(true);

                Arrow arrowScript = arrow.GetComponent<Arrow>();
                if (arrowScript != null)
                {
                    Vector3 direction = (targetPosition - spawnPosition).normalized;
                    arrowScript.Shoot(spawnPosition, direction, _arrowSpeed);
                }
            }
        }

        private Vector3 GetRandomTargetPosition(Transform spawnPoint)
        {
            Vector3 directionToCenter = (transform.position - spawnPoint.position).normalized;
            float randomAngleY = Random.Range(-45f, 45f);
            Quaternion randomRotation = Quaternion.Euler(0, randomAngleY, 0);
            Vector3 randomDirection = randomRotation * directionToCenter;
            return transform.position + randomDirection * _arrowSpeed;
        }
    }
}