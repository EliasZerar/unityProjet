using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    [Header("References")]
    [SerializeField] private TaskTimer m_TaskTimer;
    [SerializeField] private GameObject m_GameOverPanel;
    [SerializeField] private GameObject m_VictoryPanel;
    [SerializeField] private GameObject m_StoryPanel;
    [SerializeField] private GameObject m_TimerPanel;
    [SerializeField] private TextMeshProUGUI m_IntroTextDisplay;

    [Header("Story Intro")]
    [TextArea(3, 8)]
    [SerializeField] private string m_IntroText =
        "Tu te réveilles seul dans un appartement abandonné.\n" +
        "Une odeur étrange flotte dans l'air. Tu dois nettoyer les lieux\n" +
        "et trouver les indices avant l'arrivée de l'inspecteur.\n\n" +
        "Tu as 2 minutes.";

    private int m_CompletedInteractions = 0;
    private const int INTERACTIONS_REQUIRED = 2;
    private CapsuleMovement m_PlayerMovement;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }

    private void Start()
    {
        m_PlayerMovement = FindFirstObjectByType<CapsuleMovement>();
        ShowStoryIntro();
    }

    public void StartGame()
    {
        if (m_StoryPanel != null)
            m_StoryPanel.SetActive(false);

        if (m_TimerPanel != null)
            m_TimerPanel.SetActive(true);

        m_PlayerMovement?.EnableMovement();
        m_TaskTimer?.StartTimer();
    }

    public void OnTimerExpired()
    {
        m_TaskTimer?.StopTimer();
        m_PlayerMovement?.DisableMovement();
        ShowGameOver();
    }

    public void RegisterCompletedInteraction()
    {
        m_CompletedInteractions++;

        if (m_CompletedInteractions >= INTERACTIONS_REQUIRED)
        {
            m_TaskTimer?.StopTimer();
            m_PlayerMovement?.DisableMovement();
            ShowVictory();
        }
    }

    public void RestartGame()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    private void ShowStoryIntro()
    {
        m_PlayerMovement?.DisableMovement();

        if (m_StoryPanel != null)
            m_StoryPanel.SetActive(true);

        if (m_TimerPanel != null)
            m_TimerPanel.SetActive(false);

        if (m_GameOverPanel != null)
            m_GameOverPanel.SetActive(false);

        if (m_VictoryPanel != null)
            m_VictoryPanel.SetActive(false);

        if (m_IntroTextDisplay != null)
            m_IntroTextDisplay.text = m_IntroText;
    }

    private void ShowGameOver()
    {
        if (m_GameOverPanel != null)
            m_GameOverPanel.SetActive(true);
    }

    private void ShowVictory()
    {
        if (m_VictoryPanel != null)
            m_VictoryPanel.SetActive(true);
    }
}