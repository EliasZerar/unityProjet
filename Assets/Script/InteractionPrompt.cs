using UnityEngine;
using TMPro;


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