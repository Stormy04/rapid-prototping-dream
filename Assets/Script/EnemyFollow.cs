using UnityEngine;

public class EnemyFollow : MonoBehaviour
{
    public Transform player;
    public float speed = 3f;
    public GameObject jumpScareImage; // Assign UI Image GameObject here
    public AudioSource jumpScareAudio; // Assign AudioSource here

    private bool hasScared = false;

    void Update()
    {
        if (player != null && !hasScared)
        {
            // Move towards player
            Vector3 direction = (player.position - transform.position).normalized;
            transform.position += direction * speed * Time.deltaTime;

            // Look at player
            transform.LookAt(player);
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") && !hasScared)
        {
            hasScared = true;

            // Show jumpscare
            jumpScareImage.SetActive(true);

            // Play scream sound
            if (jumpScareAudio != null)
                jumpScareAudio.Play();

            // Freeze game
            Time.timeScale = 0f;
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }
    }
}
