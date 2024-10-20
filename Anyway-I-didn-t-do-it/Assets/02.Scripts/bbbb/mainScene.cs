using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;
using UnityEngine.Networking;
using UnityEngine.Video;
using SourGrape.kiyoung;

namespace SourGrape.bbbb
{
    public class mainScene : MonoBehaviour
    {
        #region login properties
        public TMP_InputField InputID;
        public TMP_InputField InputPW;
        public TMP_Text errorText;
        #endregion

        #region signup properties
        public TMP_InputField SignUpID;
        public TMP_InputField SignUpPW;
        public TMP_InputField SignUpNickname;
        public TMP_Text SignUpErrorText;
        #endregion

        public GameObject loginPanel;   // 로그인 패널
        public GameObject signUpPanel;  // 회원가입 패널
        public VideoPlayer player;


        private GameObject PlayerData;
        private HttpRequestManager _httpRequestManager;

        void Start()
        {
            _httpRequestManager = new HttpRequestManager();
            errorText.text = "";
            Screen.SetResolution(1920, 1080, true);
        }

        void Update()
        {
            // Tab 키가 눌렸을 때 입력 필드를 이동
            if (Input.GetKeyDown(KeyCode.Tab))
            {
                HandleTabKeyNavigation();
            }

            // Enter 키 입력을 통해 로그인 및 회원가입 요청 제어
            if (Input.GetKeyDown(KeyCode.Return))
            {
                HandleEnterKeyAction();
            }
        }

        public void BtnSignIn()
        {
            if (!string.IsNullOrEmpty(InputID.text) && !string.IsNullOrEmpty(InputPW.text))
            {
                errorText.text = "";
                StartCoroutine(LoginRequest(InputID.text, InputPW.text));
            }
            else
            {
                errorText.text = "Login Failed. Please check input.";
                Debug.Log("Login Failed. Please check input.");
            }
        }

        private void HandleTabKeyNavigation()
        {
            // 현재 선택된 UI 요소를 가져옴
            var selected = UnityEngine.EventSystems.EventSystem.current.currentSelectedGameObject;

            // 로그인 패널이 활성화되어 있을 때
            if (loginPanel.activeSelf)
            {
                if (selected == InputID.gameObject)
                {
                    InputPW.Select();  // ID에서 비밀번호로 이동
                }
                else if (selected == InputPW.gameObject)
                {
                    InputID.Select();  // 비밀번호에서 다시 ID로 이동 (순환)
                }
            }
            // 회원가입 패널이 활성화되어 있을 때
            else if (signUpPanel.activeSelf)
            {
                if (selected == SignUpID.gameObject)
                {
                    SignUpPW.Select();  // 회원가입 ID에서 비밀번호로 이동
                }
                else if (selected == SignUpPW.gameObject)
                {
                    SignUpNickname.Select();  // 비밀번호에서 닉네임으로 이동
                }
                else if (selected == SignUpNickname.gameObject)
                {
                    SignUpID.Select();  // 닉네임에서 다시 회원가입 ID로 이동 (순환)
                }
            }
        }


        private void HandleEnterKeyAction()
        {
            var selected = UnityEngine.EventSystems.EventSystem.current.currentSelectedGameObject;

            // 로그인 패널의 비밀번호 필드에서 Enter 입력 시 로그인 요청
            if (loginPanel.activeSelf && selected == InputPW.gameObject)
            {
                BtnSignIn();  // 로그인 요청 함수 호출
            }
            // 회원가입 패널의 닉네임 필드에서 Enter 입력 시 회원가입 요청
            else if (signUpPanel.activeSelf && selected == SignUpNickname.gameObject)
            {
                BtnSignUp();  // 회원가입 요청 함수 호출
            }
        }

        private IEnumerator LoginRequest(string id, string password)
        {
            // �α��� API URL
            string url = Parameters.URL + "api/user/login";

            // �α��ο� �ʿ��� JSON ������ ����
            string jsonData = $"{{\"loginId\":\"{id}\", \"originalPwd\":\"{password}\"}}";

            // HttpRequestManager�� ����Ͽ� POST ��û�� ����
            yield return StartCoroutine(_httpRequestManager.PostRequest(url, jsonData, HandleLoginResponse));
        }

        // �α��� ���� ó��
        private void HandleLoginResponse(UnityWebRequest request)
        {
            if (request.result == UnityWebRequest.Result.Success)
            {
                Debug.Log("�α��� ����: " + request.downloadHandler.text);

                // JSON ������ LoginResponse ��ü�� ��ȯ
                var response = JsonUtility.FromJson<LoginResponse>(request.downloadHandler.text);

                // �α��� ���� �� ID�� JWT, �г����� PlayerPrefs�� ����
                PlayerPrefs.SetString("ID", InputID.text);
                PlayerPrefs.SetString("JWT", response.token);
                PlayerPrefs.SetString("NickName", response.nickname);
                Debug.Log("JWT" + PlayerPrefs.GetString("JWT"));

                // �κ� ������ �̵�
                SceneManager.LoadScene("LobbyScene_bbbb");
            }
            else
            {
                errorText.text = "Login Failed. Please check input."; // �α��� ���� �� ���� �޽��� ǥ��
                Debug.LogError("Login Failed. Please check input.");
            }
        }

        // 회원가입 버튼 클릭 시 호출될 함수
        public void BtnSignUp()
        {
            string username = SignUpID.text;
            string password = SignUpPW.text;
            string nickname = SignUpNickname.text;

            if (string.IsNullOrWhiteSpace(username) || string.IsNullOrWhiteSpace(password) ||
                string.IsNullOrWhiteSpace(nickname))
            {
                Debug.Log("No Null Input");
                SignUpErrorText.text = "Signup Failed. Please check input.";
                return;
            }

            string url = Parameters.URL + "api/user/join";

            string jsonData = $"{{\"loginId\":\"{username}\", \"originalPwd\":\"{password}\", \"nickname\":\"{nickname}\", \"email\":\"TFT\"}}";

            StartCoroutine(_httpRequestManager.PostRequest(url, jsonData, HandleSignUpResponse));
        }

        private void HandleSignUpResponse(UnityWebRequest request)
        {
            if (request.result == UnityWebRequest.Result.Success)
            {
                Debug.Log("회원가입 성공: " + request.downloadHandler.text);
                ShowLoginPanel(); // 회원가입 성공 시 로그인 패널로 전환
            }
            else
            {
                Debug.LogError("Signup Failed: " + request.error);
                SignUpErrorText.text = "Signup Failed. Please check input.";
            }
        }

        // 로그인 패널을 보여주는 함수
        public void ShowLoginPanel()
        {
            loginPanel.SetActive(true);
            signUpPanel.SetActive(false);
            errorText.text = "";
        }

        // 회원가입 패널을 보여주는 함수
        public void ShowSignUpPanel()
        {
            loginPanel.SetActive(false);
            signUpPanel.SetActive(true);
            errorText.text = "";
        }
    }

    public class LoginResponse
    {
        public string token;
        public string nickname;
    }
}
