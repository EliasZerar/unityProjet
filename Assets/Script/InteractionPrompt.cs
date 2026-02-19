using UnityEngine;
using TMPro;

/// Displays and hides the interaction prompt UI (e.g. "Press E to interact").
/// Attach this to a World Space Canvas placed above the interactable object,
/// or to a Screen Space Canvas managed by each interactable.
public class InteractionPrompt : MonoBehaviour
{
    [SerializeField] private GameObject m_PromptRoot;
    [SerializeField] private TextMeshProUGUI m_PromptText;

    private void Awake()
    {
        Hide();
    }

    public void Show(string message)
    {
        m_PromptText.text = message;
        m_PromptRoot.SetActive(true);
    }

    public void Hide()
    {
        m_PromptRoot.SetActive(false);
    }
}