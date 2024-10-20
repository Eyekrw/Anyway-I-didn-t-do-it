using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using SourGrape.kiyoung;
using Photon.Realtime;
using TMPro;

namespace SourGrape.bbbb
{
    public class ArrowManager : MonoBehaviourPunCallbacks
    {
        // �� �Ŵ����� �� �߾ӿ� �����ؾ� ���� ����.
        public bool GameStart = false;

        private string arrowPath = "bbbb/Projectiles/Arrow";
        private kiyoung.TurretSpawner _turretSpawner;
        private List<GameObject> TurretList;
        //private float st;
        private bool trigger = true;
        private int randomArrow = Parameters.randomArrow;
        private int targetArrow = Parameters.targetArrow; // per Players
        private int spawnLocCount = Parameters.shootLocation;

        private void Start()
        {
            //if (!PhotonNetwork.IsConnected)
            //    PhotonNetwork.ConnectUsingSettings();
            //else
            //{
            //    arrowSpawner();
            //}
            //st = Time.time;
            if(PhotonNetwork.IsMasterClient)
                arrowSpawner();
        }


        private List<Vector3> GetLocation(Transform mapCenter, float spawnDist, int N)
        {// �� �߽ɿ��� ������ 50.0f�� ��ġ�� N����ŭ ������ �������� ��ġ
            List<Vector3> _spawnPoint = new List<Vector3>();
            for (int i=0; i < N; i++)
            {
                float angle = i * Mathf.PI * 2 / N;
                Vector3 Pos = new Vector3(
                    mapCenter.position.x + Mathf.Cos(angle) * spawnDist,
                    mapCenter.position.y,
                    mapCenter.position.z + Mathf.Sin(angle) * spawnDist
                    );
                _spawnPoint.Add( Pos );
            }
            return _spawnPoint;
        }

        private void arrowSpawner()
        {
            GameStart = true;
            List<Vector3> _locations = GetLocation(transform, 50.0f, 36);
            object[] sendData = new object[4] {
                (Vector3) Vector3.zero, 
                (float)0, 
                (int) 0, 
                _locations.ToArray() 
            };//���� ��ġ, ���� �ð�, ���, ���� ���� ��ġ��
            int _cnt = 0;
            for (int i=0; i<Parameters.GameTime/10; i++)
            {
                sendData[1] = (float)(1 + i * 10);
                //���� ȭ�� ����
                Debug.Log($"{i} shoot time : {sendData[1]}");
                _cnt += 9;
                for (int ra = 0; ra < randomArrow; ra++)
                {// 9��
                    sendData[0] = _locations[(i + ra * (int)(spawnLocCount/randomArrow)) % spawnLocCount];
                    sendData[2] = 4;
                    GameObject _object_ = PhotonNetwork.InstantiateRoomObject(arrowPath, (Vector3)sendData[0] - new Vector3(0, 5, 0), Quaternion.identity, 0, sendData);
                    //_object_.GetComponent<ArrowAlgorithm>()._startPosition = ;
                    _object_.transform.localScale = new Vector3(5,5,5); 
                }
                _cnt += 4;
                for (int ta = 0; ta < targetArrow; ta++)
                {//2����
                    for (int pc = 0; pc < PhotonNetwork.CurrentRoom.PlayerCount; pc++)
                    {//���� ����ŭ
                        sendData[2] = pc;
                        PhotonNetwork.InstantiateRoomObject(arrowPath, new Vector3(0, -5, 0), Quaternion.identity, 0, sendData);
                    }
                }

            }

        }

        //public override void OnConnectedToMaster()
        //{
        //    base.OnConnectedToMaster();
        //    PhotonNetwork.JoinLobby();
        //}
        //public override void OnJoinedLobby()
        //{
        //    base.OnJoinedLobby();
        //    RoomOptions roomOptions = new RoomOptions()
        //    {
        //        IsVisible = false,
        //        IsOpen = true,
        //    };
        //    PhotonNetwork.JoinOrCreateRoom("ABCDEF", roomOptions, TypedLobby.Default);
        //}

        //public override void OnJoinedRoom()
        //{
        //    base.OnJoinedRoom();
        //    if(PhotonNetwork.IsMasterClient){
        //        Debug.Log("in room, act arrow");
        //        //arrowSpawner(); 
        //    }
        //}
    }
}
