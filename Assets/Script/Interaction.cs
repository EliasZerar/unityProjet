using Unity.Cinemachine;
using UnityEngine;

[RequireComponent(typeof(Collider))]
public abstract class Interaction : MonoBehaviour
{
    [Header("Interaction Settings")]
    [SerializeField] private string m_PromptMessage = "Press E to interact";
    [SerializeField] private CinemachineCamera m_InteractionCamera;
    [SerializeField] private InteractionPrompt m_InteractionPrompt;

    private CameraController m_CameraController;
    private CapsuleMovement m_PlayerMovement;
    private bool m_IsPlayerNearby = false;
    protected bool m_IsInteracting = false;

    protected InteractionPrompt InteractionPromptRef => m_InteractionPrompt;

    private void Start()
    {
        m_CameraController = FindFirstObjectByType<CameraController>();
        m_PlayerMovement   = FindFirstObjectByType<CapsuleMovement>();
    }

    private void Update()
    {
        if (m_IsInteracting)
        {
            OnInteractionUpdate();
            return;
        }

        if (m_IsPlayerNearby && Input.GetKeyDown(KeyCode.E))
            StartInteraction();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player"))
            return;

        m_IsPlayerNearby = true;
        m_InteractionPrompt?.Show(m_PromptMessage);
    }

    private void OnTriggerExit(Collider other)
    {
        if (!other.CompareTag("Player"))
            return;

        m_IsPlayerNearby = false;
        m_InteractionPrompt?.Hide();
    }

    private void StartInteraction()
    {
        m_IsInteracting = true;
        m_InteractionPrompt?.Hide();
        m_PlayerMovement?.DisableMovement();
        m_CameraController?.SwitchToInteractionCamera(m_InteractionCamera);
        OnInteractionStarted();
    }

    protected void CompleteInteraction()
    {
        m_IsInteracting = false;
        m_PlayerMovement?.EnableMovement();
        m_CameraController?.SwitchBackToPlayerCamera();
        OnInteractionCompleted();
        DisableTrigger();
    }

    private void DisableTrigger()
    {
        foreach (Collider col in GetComponents<Collider>())
        {
            if (col.isTrigger)
                col.enabled = false;
        }

        m_IsPlayerNearby = false;
        m_InteractionPrompt?.Hide();
    }

    protected virtual void OnInteractionStarted() { }
    protected virtual void OnInteractionUpdate() { }
    protected virtual void OnInteractionCompleted() { }
}