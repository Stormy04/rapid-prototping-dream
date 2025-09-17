using UnityEngine;

public class Flashlight : MonoBehaviour
{
    public Light flashlight;

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.F)) // Press F to toggle
        {
            flashlight.enabled = !flashlight.enabled;
        }
    }
    void Start()
    {
        // Replace "Main Camera" with your camera's name if different
        Transform cameraTransform = Camera.main.transform;
        transform.SetParent(cameraTransform);
        transform.localPosition = new Vector3(0, 0, 0.5f); // Adjust as needed
        transform.localRotation = Quaternion.identity;
    }
}
