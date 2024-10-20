using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using Photon.Pun.Demo.PunBasics;
using Unity.VisualScripting;
using System.IO.Pipes;
using TMPro;

namespace SourGrape.hongyeop
{
    public class PlayerStatusController : MonoBehaviourPun
    {
        #region public properties
        public GameObject NearObject;
        public int PlayerHealth = 1;
        public bool IsDead = false; // ���� ����
        public bool IsDance = false; // ���� ����
        public int SlowCount = 0;
        #endregion

        #region exposed private properties
        #endregion

        #region private properties
        private float _deathSpinSpeed; // �ʴ� ȸ�� ����
        private float _deathLaunchForce; // ���ư� ��
        private float _deathLaunchForceY; // Y�� ���ư��� ����
        private Vector3 arrowImpactDirection; // ȭ�� ���� ����
        private Quaternion DieAngle; // �׾��� ���� �÷��̾� ����
        private Rigidbody _myRigid; // Rigidbody ������Ʈ�� ������ ����
        private Animator _anim; // Animator ������Ʈ�� ������ ����
        private PlayerController _playerMoveController;
        private PlayerAttackController _playerAttackController;
        private GameObject _gameManager;
        private GameObject _playerList;
        #endregion

        void Start()
        {
            _deathSpinSpeed = 1500f; // �ʴ� ȸ�� ����
            _deathLaunchForce = 10f; // ���ư� ��
            _deathLaunchForceY = 1.2f; // Y�� ���ư��� ����                               
            // Rigidbody ������Ʈ ��������
            _myRigid = GetComponent<Rigidbody>();
            // Animator ������Ʈ ��������
            _anim = GetComponentInChildren<Animator>();
            _playerMoveController = GetComponent<PlayerController>();
            _playerAttackController = GetComponent<PlayerAttackController>();
            _gameManager = GameObject.Find("GameManager");
            _playerList = GameObject.Find("PlayerList");

            foreach (Player p in PhotonNetwork.PlayerList)
            {
                if(p.CustomProperties.TryGetValue("PlayerIdx", out object pIdx))
                {
                    int idx = (int)pIdx;
                    Transform panel = _playerList.transform.GetChild(0);
                    GameObject player = panel.transform.GetChild(idx).gameObject;
                    GameObject nickname = player.transform.GetChild(0).gameObject;
                    nickname.GetComponent<TextMeshProUGUI>().text = p.NickName;
                }
            }
        }

        void FixedUpdate()
        {
            // �÷��̾��� ü���� 0�� �� Death �Լ� ����
            if (photonView.IsMine && PlayerHealth <= 0 && !IsDead)
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
            // ���� �ִϸ��̼� ����
            _playerMoveController.enabled = false; // �÷��̾� �̵� ��Ȱ��ȭ
            _playerAttackController.enabled = false; // �÷��̾� ���� ��Ȱ��ȭ
            _anim.SetBool("isDance", true); // ���� �ִϸ��̼�
            GameObject danceEffect = PhotonNetwork.Instantiate("Effect/LevelupCylinderWhite", transform.position, transform.rotation); // ���� ����Ʈ
            IsDance = false;

        }

        private void Death()
        {
            DieAngle = transform.rotation;
            IsDead = true;
            if (photonView.IsMine)
            {
                // PlayerController ��ũ��Ʈ�� ��Ȱ��ȭ
                if (_playerMoveController != null)
                {
                    Destroy(_playerMoveController);
                }

                // PlayerAttackController ��ũ��Ʈ�� ��Ȱ��ȭ
                if (_playerAttackController != null)
                {
                    Destroy(_playerAttackController);
                }
            }

            // ���ۺ��� ���� �ڷ�ƾ ȣ��
            StartCoroutine(SpinRapidly(5f));

            // �ݴ� �������� ���� ���� ȭ�� ������ ���ư��� ��
            Vector3 launchDirection = arrowImpactDirection + Vector3.up * _deathLaunchForceY;
            _myRigid.AddForce(launchDirection.normalized * _deathLaunchForce, ForceMode.Impulse);
            Debug.Log("AddForce");

            // �ڷ�ƾ�� ȣ���Ͽ� 5�� �Ŀ� ������Ʈ�� ��Ȱ��ȭ
            StartCoroutine(DisableAfterDelay(5f));
        }

        [PunRPC]
        void SetTriggerIsDead()
        {
            _anim.SetTrigger("isDead");
        }

        private IEnumerator SpinRapidly(float duration)
        {
            float elapsedTime = 0f;
            DeathScreamingSound();
            try
            {
                while (elapsedTime < duration)
                {
                    if (elapsedTime > 0.3 && _myRigid.transform.position.y < 0.3f) // ���� ���� ��� ����
                    {
                        yield break;
                    }
                    // x�� �������� ȸ�� �ӵ��� ������Ŵ
                    transform.Rotate(Vector3.up * _deathSpinSpeed * Time.deltaTime);
                    elapsedTime += Time.deltaTime;
                    yield return null; // ���� �����ӱ��� ���
                }
            }
            finally // �ڷ�ƾ ���� �� �׻� �״� �ִϸ��̼� ����
            {
                // �÷��̾� ��� �ִϸ��̼�
                photonView.RPC("SetTriggerIsDead", RpcTarget.All);
                //_anim.SetTrigger("isDead");
                //bDebug.Log("died");
            }
        }

        [PunRPC]
        public void Sink()
        {
            // Collider�� GetComponent�� ����
            Collider col = GetComponent<Collider>();
            if (col != null)
            {
                col.enabled = false;
            }
            // 2�� ���� ������ ����ɰ� ��
            StartCoroutine(SinkOverTime(2f));
        }

        private IEnumerator SinkOverTime(float sinkDuration)
        {
            float elapsedTime = 0f;
            Vector3 initialPosition = transform.position;
            initialPosition.y = 0; // y ���� 0���� ����
            Quaternion initialRotation = Quaternion.identity;
            Vector3 targetPosition = initialPosition;
            targetPosition.y -= 1f; // ����ɴ� ��ǥ����

            while (elapsedTime < sinkDuration)
            {
                if (_myRigid != null)
                {
                    _myRigid.MovePosition(Vector3.Lerp(initialPosition, targetPosition, elapsedTime / sinkDuration));
                    Debug.Log("Move Move Move!");
                }
                else
                {
                    transform.position = Vector3.Lerp(initialPosition, targetPosition, elapsedTime / sinkDuration);
                    Debug.Log("Are you human?");
                }
                elapsedTime += Time.deltaTime;
                yield return null; // ���� �����ӱ��� ���
            }
        }

        [PunRPC]
        public void DisableGameObject()
        {
            gameObject.SetActive(false);
        }



        private IEnumerator DisableAfterDelay(float delay)
        {
            // ���� �ð� ���� ���
            yield return new WaitForSeconds(delay - 2);

            // ī�޶� ����
            Camera mainCamera = Camera.main;
            if (mainCamera != null)
            {
                CameraController cameraController = mainCamera.GetComponent<CameraController>();
                cameraController.enabled = false;
            }

            //// Collider ��Ȱ��ȭ
            //Collider collider = GetComponent<Collider>();
            //if (collider != null)
            //{
            //    collider.enabled = false;
            //}

            // RPC ȣ��
            photonView.RPC("Sink", RpcTarget.All);

            // ghostPlayer ����
            Vector3 initialPosition = transform.position;
            initialPosition.y = 0; // y ���� 0���� ����
            GameObject ghostPlayer = PhotonNetwork.Instantiate("Ghost/Ghost", initialPosition, DieAngle);

            // ���� �÷��̾��� ghostPlayer�� PlayerGhostController Ȱ��ȭ
            if (photonView.IsMine)
            {
                // ���� ���� Ȱ��ȭ
                PlayerGhostController ghostController = ghostPlayer.GetComponent<PlayerGhostController>();
                if (ghostController != null)
                {
                    ghostController.enabled = true;
                }
                // ���� ī�޶� Ȱ��ȭ
                CameraGhostController cameraGhostController = mainCamera.GetComponent<CameraGhostController>();
                if (cameraGhostController != null)
                {
                    cameraGhostController.enabled = true;
                }
            }

            // ������Ʈ ��ü�� ��Ȱ��ȭ (�������� ��ȣ�ۿ��� ��Ȱ��ȭ)
            //gameObject.SetActive(false);

            // PhotonNetwork.Destroy(gameObject);

            // ������Ʈ ��ü�� ��Ȱ��ȭ ����ȭ
            photonView.RPC("DisableGameObject", RpcTarget.All);

        }

        [PunRPC]
        public void ImDie(int playerIdx, int idx, string nickname, int deathCnt)
        {
            //Debug.Log(nickname+"이 죽었어" + deathCnt);
            _gameManager.GetComponent<SourGrape.kiyoung.GameManager>().DeathOrder[deathCnt] = idx;
            // ���߿� ���� �׾����� �Ǵ��ؼ� 1�� ������ ���� �ϴ� üũ�� ����(���� �ʿ�x)
            _gameManager.GetComponent<SourGrape.kiyoung.GameManager>().DeathCount = deathCnt + 1;
            //_gameManager.GetComponent<SourGrape.kiyoung.GameManager>().DeathOrder[idx] = deathCnt;
            //_gameManager.GetComponent<SourGrape.kiyoung.GameManager>().IsDead[idx] = true;
            _gameManager.GetComponent<SourGrape.kiyoung.GameManager>().IsDead[idx] = true;

            Transform panel = _playerList.transform.GetChild(0);
            GameObject player = panel.transform.GetChild(playerIdx).gameObject;
            GameObject dieSig = player.transform.GetChild(1).gameObject;
            dieSig.SetActive(true);
        }

        // �ٸ� �ݶ��̴��� Ʈ���ſ� ������ �� ȣ��Ǵ� �޼���
        private void OnTriggerEnter(Collider other)
        {
            // AreaSlow Ʈ���ſ� ������ ���
            if (other.CompareTag("AreaSlow"))
            {
                SlowCount++;
            }

            // �뽬�� ��� ���� ó�� (�Ʒ��� �ִ� ��� ���� ����)
            if (_playerMoveController && _playerMoveController.IsDashing) 
            {
                return;
            }
            // ȭ�� ���� ���
            if (other.CompareTag("Arrows") || other.CompareTag("FatalObject"))
            {
                // NearObject ������ Ʈ���ſ� ���� "Arrows" ������Ʈ ����
                NearObject = other.gameObject;
                ArrowEffects arroweffect= NearObject.GetComponentInChildren<ArrowEffects>();
                if (arroweffect != null)
                {
                    arroweffect.Boom();
                }

                // ȭ�� ���� ��� �÷��̾� ü�� ����
                PlayerHealth -= 1;
                //Debug.Log("Player HP: " + PlayerHealth);

                //if (photonView.IsMine)
                //{
                //    int actNum = PhotonNetwork.LocalPlayer.ActorNumber;
                //    string nickname = PhotonNetwork.LocalPlayer.NickName;
                //    photonView.RPC("ImDie", RpcTarget.All, actNum, nickname);
                //}
                if (photonView.IsMine && PhotonNetwork.LocalPlayer.CustomProperties.TryGetValue("PlayerIdx", out object PlayerIdx))
                {
                    int idx = (int)PlayerIdx;
                    int actNum = PhotonNetwork.LocalPlayer.ActorNumber;
                    string nickname = PhotonNetwork.LocalPlayer.NickName;
                    int deathCnt = _gameManager.GetComponent<SourGrape.kiyoung.GameManager>().DeathCount;
                    if (!_gameManager.GetComponent<SourGrape.kiyoung.GameManager>().IsDead.ContainsKey(actNum))
                    {
                        photonView.RPC("ImDie", RpcTarget.All, idx, actNum, nickname, deathCnt);
                    }
                }

                // ȭ���� ������ arrowImpactDirection�� ����
                arrowImpactDirection = (transform.position - other.transform.position).normalized;
            }
            // �ٸ� �÷��̾����� ���� ���
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
            // AreaSlow Ʈ���ſ��� ���� ���
            if (other.CompareTag("AreaSlow"))
            {
                SlowCount--;
            }
        }

        private void OnTriggerStay(Collider other)
        {
            // AreaSlow ���ǵ� ������Ʈ
            if (other.CompareTag("AreaSlow"))
            {
                UpdateSpeed();
            }
        }

        private void UpdateSpeed()
        {
            if (SlowCount > 0)
            {
                float calcSpeed = _playerMoveController.PlayerDefaultSpeed - 3 * SlowCount;
                float newSpeed = calcSpeed > 0 ? calcSpeed : 0.3f;
                if (_playerMoveController != null)
                {
                    _playerMoveController.ChangeSpeedForDuration(newSpeed, 0.5f);
                }
            }
        }
        private IEnumerator DestroyAfterSound(float delay, GameObject obj)
        {
            yield return new WaitForSeconds(delay); // 사운드 재생 시간이 지나면
            PhotonNetwork.Destroy(obj); // 오브젝트 삭제
        }

        public void HitYellSound()
        {
            GameObject soundObj = PhotonNetwork.Instantiate("yong/Sound_hit_yell", transform.position, Quaternion.identity);

            AudioSource audioSource = soundObj.GetComponent<AudioSource>();
            if (audioSource != null)
            {
                audioSource.Play();

                StartCoroutine(DestroyAfterSound(audioSource.clip.length, soundObj));
            }
        }

        public void HitSound()
        {
            GameObject soundObj = PhotonNetwork.Instantiate("yong/Sound_hit", transform.position, Quaternion.identity);

            AudioSource audioSource = soundObj.GetComponent<AudioSource>();
            if (audioSource != null)
            {
                audioSource.Play();

                StartCoroutine(DestroyAfterSound(audioSource.clip.length, soundObj));
            }
        }

        public void DeathScreamingSound()
        {
            GameObject soundObj = PhotonNetwork.Instantiate("yong/Sound_death_scream", transform.position, Quaternion.identity);

            AudioSource audioSource = soundObj.GetComponent<AudioSource>();
            if (audioSource != null)
            {
                audioSource.Play();

                StartCoroutine(DestroyAfterSound(audioSource.clip.length, soundObj));
            }
        }

        private IEnumerator HitIn()
        {
            _anim.SetTrigger("isHit"); // �ǰ� �ִϸ��̼�
            //_playerMoveController.enabled = false; // �÷��̾� �̵� ��Ȱ��ȭ
            _playerMoveController.SetFlag(false);
            _playerAttackController.enabled = false; // �÷��̾� ���� ��Ȱ��ȭ
            yield return new WaitForSeconds(3f);
            //_playerMoveController.enabled = true; // �÷��̾� �̵� Ȱ��ȭ
            _playerMoveController.SetFlag(true);
            _playerAttackController.enabled = true; // �÷��̾� ���� Ȱ��ȭ
        }
    }
}
