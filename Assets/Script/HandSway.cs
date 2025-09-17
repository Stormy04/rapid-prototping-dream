using UnityEngine;

public class HandSway : MonoBehaviour
{
    public float swayAmount = 0.05f;
    public float smoothAmount = 6f;

    private Vector3 initialPos;

    void Start()
    {
        initialPos = transform.localPosition;
    }

    void Update()
    {
        float moveX = -Input.GetAxis("Mouse X") * swayAmount;
        float moveY = -Input.GetAxis("Mouse Y") * swayAmount;

        Vector3 targetPos = new Vector3(moveX, moveY, 0f) + initialPos;
        transform.localPosition = Vector3.Lerp(transform.localPosition, targetPos, Time.deltaTime * smoothAmount);
    }
}
