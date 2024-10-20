using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;
using System.Collections;
using TMPro;
using UnityEngine.SceneManagement;

namespace SourGrape.kiyoung
{
    public class SignUpManager : MonoBehaviour
    {
        public TMP_InputField usernameInput;
        public TMP_InputField passwordInput;
        public TMP_InputField nicknameInput;
        public TMP_InputField emailInput;
        public Button signUpButton;
        public Button signInButton;
        public TMP_Text errorText;

        private HttpRequestManager _httpRequestManager;

        private void Start()
        {
            _httpRequestManager = new HttpRequestManager();
            signUpButton.onClick.AddListener(BtnSignUp);
            signInButton.onClick.AddListener(BtnSignIn);
            errorText.text = "";
        }

        private void BtnSignUp()
        {
            string username = usernameInput.text;
            string password = passwordInput.text;
            string nickname = nicknameInput.text;
            string email = emailInput.text;

            if (string.IsNullOrWhiteSpace(username) || string.IsNullOrWhiteSpace(password) ||
                string.IsNullOrWhiteSpace(nickname) || string.IsNullOrWhiteSpace(email))
            {
                errorText.text = "No Null Input";
                return;
            }

            string url = "http://j11a708.p.ssafy.io/api/user/join";

            string jsonData = $"{{\"loginId\":\"{username}\", \"originalPwd\":\"{password}\", \"nickname\":\"{nickname}\", \"email\":\"{email}\"}}";

            StartCoroutine(_httpRequestManager.PostRequest(url, jsonData, HandleSignUpResponse));
        }

        private void HandleSignUpResponse(UnityWebRequest request)
        {
            if (request.result == UnityWebRequest.Result.Success)
            {
                Debug.Log("회원가입 성공: " + request.downloadHandler.text);
                SceneManager.LoadScene("MainScene_bbbb");
            }
            else
            {
                errorText.text = "Signup Failed: " + request.downloadHandler.text;
                Debug.LogError("Signup Failed: " + request.error);
            }
        }

        private void BtnSignIn()
        {
            SceneManager.LoadScene("MainScene_bbbb");
        }
    }
}