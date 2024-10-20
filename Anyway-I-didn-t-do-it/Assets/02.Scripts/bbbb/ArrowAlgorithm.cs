using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using SourGrape.kiyoung;
using System.Net;
using UnityEngine.Pool;
using System.Linq;
using SourGrape.bbbb;
using bbbb;

namespace SourGrape.bbbb
{
    public class ArrowAlgorithm : MonoBehaviourPun
    {
        private GameObject GM;
        private GameManager gameManager; // 실제
        //private SampleGM gameManager; //테스트용

        private Rigidbody _rb;
        // 생성시 전달되는 값
        private Vector3 _startPosition; // 시작 위치
        private float _shootTime; // 발사 시간
        private int _Target; // 대상 = 4: 랜덤 발사체, 0~3 Players에서 가져오기
        private List<Vector3> _spawnList; //발사 위치들

        private Vector3 _arrowScalse = new Vector3(20, 20, 4);


        private bool Activate = false;

        private float speed = Parameters.ArrowSpeed;
        private float _height = Parameters.ArrowHeight;

        private GameObject[] _playerList;
        
        private void Awake()
        {
            _rb = GetComponent<Rigidbody>();
            PhotonView pv = GetComponent<PhotonView>();
            GM = GameObject.Find("GameManager");
            gameManager = GM.GetComponent<GameManager>();
            //gameManager = GM.GetComponent<SampleGM>();


            _startPosition = (Vector3)pv.InstantiationData[0];
            _shootTime = (float)pv.InstantiationData[1];
            _Target =  (int)pv.InstantiationData[2];
            _spawnList = ((Vector3[])pv.InstantiationData[3]).ToList();

            _playerList = getPlayerObject();
        }

        private void Update()
        {
            if (!PhotonNetwork.IsMasterClient) return;
            if (gameManager._isRoundOver && PhotonNetwork.IsMasterClient)
            {
                PhotonNetwork.Destroy(this.gameObject);
            }
            if (!Activate)
            {
                //Debug.Log($"game Time: {gameManager.ElapsedTime}");
                if (gameManager.ElapsedTime >= _shootTime)
                {
                    Activate = true;
                    DisableArrow();
                }
            }
            float _dist = Vector3.Distance(transform.position, Vector3.zero);
            if(_dist > 60.0f)
            {
                DisableArrow();
            }
        }


        //private void OnTriggerEnter(Collider other)
        //{
        //    Debug.Log("collide!");
        //    if (PhotonNetwork.IsMasterClient)
        //    {
        //        if(other.CompareTag("Player") )
        //        {
        //            StartCoroutine(DisableArrowWithDelay());
        //        }
        //    }
        //}
        //private IEnumerator DisableArrowWithDelay()
        //{
        //    yield return new WaitForSeconds(0.5f);
        //    DisableArrow();
        //}

        //private void OnTriggerExit(Collider other)
        //{
        //    Debug.Log("collide out!");
        //    if (PhotonNetwork.IsMasterClient)
        //    {
        //        if (other.CompareTag("Player") || other.CompareTag("ArrowBorder"))
        //        {
        //            DisableArrow();
        //        }
        //    }
        //}

        private void Shoot(Vector3 _start, Vector3 _target)
        {
            _start = new Vector3(_start.x, _height, _start.z);
            _target = new Vector3(_target.x, _height, _target.z);

            // Debug.Log($"shoot pos : {_start}  height: {_height}");
            transform.position = _start;
            transform.LookAt(_target);
            //transform.localScale = _arrowScalse;

            Vector3 direction = _target - _start;
            _rb.AddForce(direction.normalized * speed, ForceMode.Impulse);
        }

        public void DisableArrow()
        {//SetActive는 로컬 적용 함수. Position을 바꿔서 충돌 판정 삭제
            _rb.velocity = Vector3.zero;
            _rb.angularVelocity = Vector3.zero;
            //transform.localScale = Vector3.zero;
            transform.position = _startPosition;
            Vector3 sp;
            Vector3 tp;
            if (_Target >= PhotonNetwork.PlayerList.Length)
            {// 지정 대상 번호가 플레이어 수 이상. -> 현 위치에서 랜덤 위치 발사
                sp = _startPosition;
                Vector3 temp = (-transform.position).normalized;

                float randomAngleY = Random.Range(-45f, 45f);

                // Y축 기준으로 회전된 방향 벡터 계산
                Quaternion rotation = Quaternion.Euler(0, randomAngleY, 0);
                Vector3 randomDirection = rotation * temp;

                // 무작위 거리를 maxDistance 내에서 계산
                //float randomDistance = Random.Range(0, 50.0f);

                tp = transform.position + randomDirection;
            }
            else
            {// 랜덤 위치에서 대상으로 쏘기
                int randIdx = Random.Range(0, _spawnList.Count);
                sp = _spawnList[randIdx];

                if (_playerList.Length != PhotonNetwork.PlayerList.Length)
                    _playerList = getPlayerObject();

                tp = _playerList[_Target].transform.position;
            }
            Shoot(sp, tp);
        }

        private GameObject[] getPlayerObject()
        {
            GameObject[] ret;
            GameObject[] ret1 = GameObject.FindGameObjectsWithTag("Player");
            GameObject[] ret2 = GameObject.FindGameObjectsWithTag("PlayerGhost");
            if (ret1.Length == 0)
            {
                ret = ret2;
            }
            else if (ret2.Length == 0)
            {
                ret = ret1;
            }
            else
            {
                ret = ret1;
                ret.Concat(ret2).ToArray();
            }

            return ret;
        }
    }
}
