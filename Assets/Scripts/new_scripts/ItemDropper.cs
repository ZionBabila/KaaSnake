using UnityEngine;

public class ItemDropper : MonoBehaviour
{
    [Header("Drop Settings")]
    public GameObject itemToDrop; // Drag the Trophy prefab here
    public Transform spawnPoint;  // Optional: A point near the parrot's beak/claws

    public void DropItem()
    {
        if (itemToDrop != null)
        {
            // Create the item at the spawn point (or at parrot's position)
            Transform finalSpawnPoint = (spawnPoint != null) ? spawnPoint : transform;
            
            GameObject droppedItem = Instantiate(itemToDrop, finalSpawnPoint.position, Quaternion.identity);
            
            Debug.Log(gameObject.name + " dropped " + itemToDrop.name);
        }
    }
}