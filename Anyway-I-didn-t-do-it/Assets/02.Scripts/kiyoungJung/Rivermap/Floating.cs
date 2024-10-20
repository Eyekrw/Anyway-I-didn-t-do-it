using UnityEngine;

public class FloatObject : MonoBehaviour
{
    public float floatStrength = 0.5f;  // Y축으로 떠다니는 강도
    public float floatSpeed = 2f;       // 떠다니는 속도
    private float _startY;

    void Start()
    {
        _startY = transform.position.y;  // 초기 Y 위치 저장
    }

    void Update()
    {
        // 사인파를 사용해 부드럽게 상하로 움직임
        transform.position = new Vector3(transform.position.x, _startY + Mathf.Sin(Time.time * floatSpeed) * floatStrength, transform.position.z);
    }
}