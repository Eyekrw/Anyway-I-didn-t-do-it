using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SourGrape.kiyoung;

namespace SourGrape.hongyeop
{
    public class GameEndCameraManager : MonoBehaviour
    {
        private List<GameObject> _winners; // 승자들
        private Camera _camera;

        void Start()
        {
            GameManager gameManager = FindObjectOfType<GameManager>(); // 전체 오브젝트에서 GameEndManager 찾기
            if (gameManager != null)
            {
                _winners = gameManager.Winners; // game end manager에서 승자들 가져오기
            }
            else
            {
                Debug.LogError("gameManager 찾을 수 없습니다.");
                return;
            }
            _camera = Camera.main; // 메인 카메라 가져오기
            FocusWinners(); // 승자들에게 초점 맞추기
        }

        private void FocusWinners()
        {
            StartCoroutine(FocusOnWinnersCoroutine());
        }

        private IEnumerator FocusOnWinnersCoroutine()
        {
            float totalDuration = 5f; // 전체 카메라 촬영 시간
            float durationPerWinner = totalDuration / _winners.Count; // 각 승자당 촬영 시간

            foreach (GameObject winner in _winners)
            {
                float elapsedTime = 0f;
                Vector3 initialPosition = _camera.transform.position;
                Quaternion initialRotation = _camera.transform.rotation;

                while (elapsedTime < durationPerWinner)
                {
                    float t = elapsedTime / durationPerWinner;
                    float angle = Mathf.Lerp(-30f, 10, t); // -60도에서 0도까지 회전

                    // 카메라 위치와 회전 설정
                    Vector3 offset = winner.transform.forward * 5 + Vector3.up * 3; // 플레이어 앞쪽으로 이동
                    _camera.transform.position = winner.transform.position + offset;
                    _camera.transform.LookAt(winner.transform.position);
                    _camera.transform.RotateAround(winner.transform.position, Vector3.up, angle);

                    elapsedTime += Time.deltaTime;
                    yield return null;
                }

                // 다음 승자로 넘어가기 전에 잠시 대기
                yield return new WaitForSeconds(0.5f);
            }

            // 댄스타임 종료 시: GameManager IsModalUp = true; 로 변경
            FindObjectOfType<GameManager>().IsModalUp = true;
            Debug.Log("GameManager.IsModalUp = true;");
        }
    }
}