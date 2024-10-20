using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    public Transform target; // 따라갈 대상, 보통 캐릭터의 Transform
    public Vector3 offset = new Vector3(0, 5, -10); // 카메라와 캐릭터 간의 기본 거리
    public float smoothSpeed = 0.125f; // 카메라가 이동할 때 부드러움 정도

    void LateUpdate()
    {
        // 목표 위치 계산: 대상 위치 + 오프셋
        Vector3 desiredPosition = target.position + offset;

        // 부드럽게 카메라 위치 보간
        Vector3 smoothedPosition = Vector3.Lerp(transform.position, desiredPosition, smoothSpeed);

        // 카메라 위치를 업데이트
        transform.position = smoothedPosition;
    }
}