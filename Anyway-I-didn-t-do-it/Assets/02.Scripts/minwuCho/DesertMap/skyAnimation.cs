using UnityEngine;

public class RotateObject : MonoBehaviour
{
    public float duration = 360.0f; 

    void Update()
    {
        float rotationSpeed = 360.0f / duration;

        transform.Rotate(0, 0, rotationSpeed * Time.deltaTime);
    }
}