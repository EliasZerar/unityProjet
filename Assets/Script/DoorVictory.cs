using UnityEngine;

public class DoorVictoryTrigger : MonoBehaviour
{
    private bool m_HasTriggered = false;

    private void OnTriggerEnter(Collider other)
    {
        if (m_HasTriggered) return;
        if (!other.CompareTag("Player")) return;
        m_HasTriggered = true;
        GameManager.Instance?.RegisterCompletedInteraction("door");
    }
}