using System;
using System.Collections;
using UnityEngine;

public class AsleepTrap : MonoBehaviour
{
    [SerializeField] private BoxCollider trapCollider;
    [SerializeField] private SpriteRenderer spriteRenderer;

    public static event Action<GameObject> onEnemyTrapped;

    IEnumerator ChangeTrapColour()
    {
        const int numSteps = 100;
        Color minusColour = new Color(0, 1, 1, 0);
        for (int steps = 0; steps < numSteps; steps++)
        {
            yield return new WaitForSeconds(1 / (float)numSteps);
            spriteRenderer.color = Color.white - (minusColour / numSteps) * steps;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Enemy")) return;

        print("IT'S A TRAP");
        onEnemyTrapped?.Invoke(other.gameObject);

        trapCollider.enabled = false;
        other.transform.position = new Vector3(transform.position.x, other.transform.position.y, transform.position.z);
        StartCoroutine(ChangeTrapColour());
    }
}
