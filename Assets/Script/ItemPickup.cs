using UnityEngine;

public class ItemPickup : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            GameManager.instance.PlayerFoundItem();
            Destroy(gameObject); // Remove the item from the scene
        }
    }
}
