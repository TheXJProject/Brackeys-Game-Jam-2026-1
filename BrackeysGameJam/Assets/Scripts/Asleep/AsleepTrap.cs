using System;
using UnityEngine;

public class AsleepTrap : MonoBehaviour
{
    [SerializeField] private BoxCollider trapCollider;

    public static event Action<GameObject> onEnemyTrapped;

    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Enemy")) return;

        print("IT'S A TRAP");
        onEnemyTrapped?.Invoke(other.gameObject);

        trapCollider.enabled = false;
        other.transform.position = new Vector3(transform.position.x, other.transform.position.y, transform.position.z);
    }
}
