using UnityEngine;
using TMPro;

[RequireComponent(typeof(Collider))]
public class DoorInteraction : Interaction
{
    static readonly string k_CorrectCode = "2847";

    [Header("Door Settings")]
    [SerializeField] Animator m_DoorAnimator;
    [SerializeField] AudioSource m_DoorAudio;

    [Header("Code UI")]
    [SerializeField] GameObject m_DoorCodePanel;
    [SerializeField] TextMeshProUGUI m_CodeDisplay;
    [SerializeField] TextMeshProUGUI m_InstructionText;

    [Header("HUD")]
    [SerializeField] GameObject m_TaskListRoot;
    [SerializeField] GameObject m_TimerPanel;

    string m_EnteredCode = "";
    bool m_DoorUnlocked;

    protected override void OnInteractionStarted()
    {
        if (m_DoorUnlocked) return;

        m_TaskListRoot?.SetActive(false);
        m_TimerPanel?.SetActive(false);

        m_EnteredCode = "";
        UpdateCodeDisplay();
        SetInstruction("Entre le code à 4 chiffres");
        m_DoorCodePanel?.SetActive(true);
    }

    protected override void OnInteractionUpdate()
    {
        if (m_DoorUnlocked) return;

        if (Input.GetKeyDown(KeyCode.Return) || Input.GetKeyDown(KeyCode.KeypadEnter))
        {
            CancelInteraction();
            return;
        }

        for (int i = 0; i <= 9; i++)
        {
            if ((Input.GetKeyDown(KeyCode.Alpha0 + i) || Input.GetKeyDown(KeyCode.Keypad0 + i)) && m_EnteredCode.Length < 4)
            {
                m_EnteredCode += i.ToString();
                UpdateCodeDisplay();
            }
        }

        if (Input.GetKeyDown(KeyCode.Backspace) && m_EnteredCode.Length > 0)
        {
            m_EnteredCode = m_EnteredCode[..^1];
            UpdateCodeDisplay();
        }

        if (m_EnteredCode.Length == 4)
            ValidateCode();
    }

    public void TriggerExitFromOsc()
    {
        if (!m_IsInteracting || m_DoorUnlocked) return;
        CancelInteraction();
    }

    void CancelInteraction()
    {
        m_DoorCodePanel?.SetActive(false);
        SetInstruction("");
        m_TaskListRoot?.SetActive(true);
        m_TimerPanel?.SetActive(true);
        CompleteInteraction();
    }

    void ValidateCode()
    {
        if (m_EnteredCode != k_CorrectCode)
        {
            SetInstruction("Code incorrect, réessaie");
            m_EnteredCode = "";
            UpdateCodeDisplay();
            return;
        }

        m_DoorUnlocked = true;
        m_DoorCodePanel?.SetActive(false);
        SetInstruction("");
        m_DoorAnimator?.SetTrigger("Open");
        m_DoorAudio?.Play();

        foreach (var col in GetComponentsInChildren<Collider>(true))
            col.enabled = false;

        GameManager.Instance?.RegisterCompletedInteraction("door");
    }

    void UpdateCodeDisplay()
    {
        if (m_CodeDisplay == null) return;

        var display = "";
        for (int i = 0; i < 4; i++)
            display += i < m_EnteredCode.Length ? m_EnteredCode[i] + " " : "_ ";

        m_CodeDisplay.text = display.TrimEnd();
    }

    void SetInstruction(string text)
    {
        if (m_InstructionText != null)
            m_InstructionText.text = text;
    }

    protected override void OnInteractionCompleted() { }
}