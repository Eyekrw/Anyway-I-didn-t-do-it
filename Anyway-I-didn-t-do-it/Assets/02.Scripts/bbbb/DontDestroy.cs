using UnityEngine;
using UnityEngine.Networking;
using System.Collections;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine.SceneManagement;

namespace SourGrape.bbbb{
    public class DontDestroy : MonoBehaviourPunCallbacks
    {
        private static DontDestroy instance;

        void Awake()
        {
            // 이미 존재하는 인스턴스가 있는지 확인
            if (instance == null)
            {
                // 첫 생성된 인스턴스이므로, 인스턴스로 할당하고 파괴되지 않도록 설정
                instance = this;
                DontDestroyOnLoad(gameObject);
            }
            else
            {
                // 이미 인스턴스가 있다면, 중복된 객체를 파괴
                Destroy(gameObject);
            }
        }

        private void OnApplicationQuit()
        {
            StartCoroutine(LogoutRequest());
        }

        private IEnumerator LogoutRequest()
        {
            // 로그아웃 요청 생성
            UnityWebRequest request = new UnityWebRequest(Parameters.URL + "api/user/logout", "POST");
            request.SetRequestHeader("Authorization", $"Bearer {PlayerPrefs.GetString("JWT")}");
            request.downloadHandler = new DownloadHandlerBuffer();

            // 요청 보내기
            yield return request.SendWebRequest();

            // 응답 확인
            if (request.result == UnityWebRequest.Result.Success)
            {
                Debug.Log("Logout successful.");
            }
            else
            {
                Debug.LogError($"Logout failed: {request.error}");
            }
        }

        public override void OnDisconnected(DisconnectCause cause)
        {
            base.OnDisconnected(cause);
            GameObject _obj = Instantiate(Resources.Load<GameObject>(Parameters.ErrorPanel));
            _obj.GetComponent<bbbb.ErrorPanel>().message = Parameters.MsgDisconnect;
            StartCoroutine(BackToMain());
        }

        private IEnumerator BackToMain()
        {
            yield return new WaitForSeconds(3f);
            SceneManager.LoadScene("MainScene_bbbb");
        }
    }
}
