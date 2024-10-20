using UnityEngine;

public class SmoothFollow2 : MonoBehaviour
{
    public Transform target;
    public Vector3 View_Pos_Add;

    public float distance = 3.0f;
    public float height = 3.0f;
    public float damping = 5.0f;
    public bool smoothRotation = true;
    public float rotationDamping = 10.0f;

    public float mouseSensitivity = 2f;

    private float mouseX, mouseY;

    void Update()
    {
        // 마우스 입력 처리
        mouseX += Input.GetAxis("Mouse X") * mouseSensitivity;
        mouseY -= Input.GetAxis("Mouse Y") * mouseSensitivity;
        mouseY = Mathf.Clamp(mouseY, -90f, 90f); // 수직 회전 제한

        Vector3 TargetPos = target.position + View_Pos_Add;

        if (smoothRotation)
        {
            Quaternion wantedRotation = Quaternion.Euler(mouseY, mouseX, 0);
            transform.rotation = Quaternion.Slerp(transform.rotation, wantedRotation, Time.deltaTime * rotationDamping);
        }
        else
        {
            transform.rotation = Quaternion.Euler(mouseY, mouseX, 0);
        }

        // 타겟 주위를 공전하도록 위치 조정
        Vector3 offset = new Vector3(0, height, -distance);
        Vector3 position = target.position + (Quaternion.Euler(mouseY, mouseX, 0) * offset) + View_Pos_Add;
        transform.position = Vector3.Lerp(transform.position, position, Time.deltaTime * damping);
    }
}