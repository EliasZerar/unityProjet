using Unity.Cinemachine;
using UnityEngine;
using TMPro;

[RequireComponent(typeof(Collider))]
public abstract class Interaction : MonoBehaviour
{
    [Header("Interaction Settings")]
    [SerializeField] private string m_PromptMessage = "Appuyer sur la main pour interagir";
    [SerializeField] private CinemachineCamera m_InteractionCamera;
    [SerializeField] private InteractionPrompt m_InteractionPrompt;

    [Header("World Prompt 3D")]
    [SerializeField] private TextMeshPro m_WorldPrompt3D;

    private CameraController m_CameraController;
    private CapsuleMovement m_PlayerMovement;
    private bool m_IsPlayerNearby = false;
    protected bool m_IsInteracting = false;

    public bool IsPlayerNearby => m_IsPlayerNearby;
    public bool IsInteracting => m_IsInteracting;

    protected InteractionPrompt InteractionPromptRef => m_InteractionPrompt;

    private void Start()
    {
        m_CameraController = FindFirstObjectByType<CameraController>();
        m_PlayerMovement   = FindFirstObjectByType<CapsuleMovement>();

        if (m_WorldPrompt3D != null)
            m_WorldPrompt3D.gameObject.SetActive(false);
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

    public void TriggerFromOsc()
    {
        if (!m_IsPlayerNearby || m_IsInteracting) return;
        StartInteraction();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player")) return;

        m_IsPlayerNearby = true;

        if (m_WorldPrompt3D != null)
        {
            m_WorldPrompt3D.text = m_PromptMessage;
            m_WorldPrompt3D.gameObject.SetActive(true);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (!other.CompareTag("Player")) return;

        m_IsPlayerNearby = false;

        if (m_WorldPrompt3D != null)
            m_WorldPrompt3D.gameObject.SetActive(false);
    }

    private void StartInteraction()
    {
        m_IsInteracting = true;

        if (m_WorldPrompt3D != null)
            m_WorldPrompt3D.gameObject.SetActive(false);

        m_InteractionPrompt?.Hide();

        if (m_InteractionCamera != null)
        {
            m_PlayerMovement?.DisableMovement();
            m_CameraController?.SwitchToInteractionCamera(m_InteractionCamera);
        }

        OnInteractionStarted();
    }

    protected void CompleteInteraction()
    {
        m_IsInteracting = false;

        if (m_InteractionCamera != null)
        {
            m_PlayerMovement?.EnableMovement();
            m_CameraController?.SwitchBackToPlayerCamera();
        }

        OnInteractionCompleted();
        DisableAllColliders();
    }

    private void DisableAllColliders()
    {
        foreach (Collider col in GetComponentsInChildren<Collider>(true))
            col.enabled = false;

        m_IsPlayerNearby = false;

        if (m_WorldPrompt3D != null)
            m_WorldPrompt3D.gameObject.SetActive(false);
    }

    protected virtual void OnInteractionStarted() { }
    protected virtual void OnInteractionUpdate() { }
    protected virtual void OnInteractionCompleted() { }
}