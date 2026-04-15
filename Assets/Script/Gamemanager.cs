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
    [SerializeField] private StoveInteraction m_StoveInteraction;

    [Header("Task List")]
    [SerializeField] private GameObject m_TaskListRoot;
    [SerializeField] private TextMeshProUGUI m_TaskTv;
    [SerializeField] private TextMeshProUGUI m_TaskBoard;

    [Header("Audio")]
    [SerializeField] private AudioSource m_AudioSource;
    [SerializeField] private AudioClip m_VictoryClip;
    [SerializeField] private AudioClip m_GameOverClip;

    [Header("Story Intro")]
    [TextArea(3, 8)]
    [SerializeField] private string m_IntroText =
    "Tu te réveilles dans un appartement abandonné.\n" +
    "Quelqu'un a dissimulé un code secret dans cette pièce.\n\n" +
    "Nettoie le tableau pour révéler les premiers chiffres.\n" +
    "Résous le simon sur la télévision pour découvrir les suivants.\n\n" +
    "Entre le code sur la porte et fuis avant que le temps ne s'écoule.\n\n" +
    "Tu as 2 minutes.\n\n" +
    "Appuie sur ESPACE pour commencer.";

    private CapsuleMovement m_PlayerMovement;
    private bool m_CanRestart = false;

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

    private void Update()
    {
        if (m_StoryPanel != null && m_StoryPanel.activeSelf)
        {
            if (Input.GetKeyDown(KeyCode.Space) || Input.GetKeyDown(KeyCode.F))
                StartGame();
        }

        if (m_CanRestart &&
            (m_GameOverPanel != null && m_GameOverPanel.activeSelf ||
             m_VictoryPanel != null && m_VictoryPanel.activeSelf))
        {
            if (Input.anyKeyDown)
                RestartGame();
        }
    }

    public void StartGame()
    {
        if (m_StoryPanel == null || !m_StoryPanel.activeSelf) return;

        m_StoryPanel.SetActive(false);

        if (m_TimerPanel != null)
            m_TimerPanel.SetActive(true);

        if (m_TaskListRoot != null)
            m_TaskListRoot.SetActive(true);

        m_PlayerMovement?.EnableMovement();
        m_TaskTimer?.StartTimer();
    }

    public void OnTimerExpired()
    {
        m_TaskTimer?.StopTimer();
        m_PlayerMovement?.DisableMovement();
        m_StoveInteraction?.HideStoveUI();

        if (m_TaskListRoot != null)
            m_TaskListRoot.SetActive(false);

        ShowGameOver();
    }

    public void RegisterCompletedInteraction(string taskType)
    {
        if (taskType == "stove" && m_TaskTv != null)
            m_TaskTv.gameObject.SetActive(false);
        else if (taskType == "board" && m_TaskBoard != null)
            m_TaskBoard.gameObject.SetActive(false);
        else if (taskType == "door")
        {
            m_TaskTimer?.StopTimer();
            m_PlayerMovement?.DisableMovement();

            if (m_TaskListRoot != null)
                m_TaskListRoot.SetActive(false);

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

        if (m_TaskListRoot != null)
            m_TaskListRoot.SetActive(false);

        if (m_IntroTextDisplay != null)
            m_IntroTextDisplay.text = m_IntroText;
    }

    private void ShowGameOver()
    {
        if (m_GameOverPanel != null)
            m_GameOverPanel.SetActive(true);

        if (m_AudioSource != null && m_GameOverClip != null)
            m_AudioSource.PlayOneShot(m_GameOverClip);

        StartCoroutine(EnableRestartAfterDelay());
    }

    private void ShowVictory()
    {
        if (m_VictoryPanel != null)
            m_VictoryPanel.SetActive(true);

        if (m_AudioSource != null && m_VictoryClip != null)
            m_AudioSource.PlayOneShot(m_VictoryClip);

        StartCoroutine(EnableRestartAfterDelay());
    }

    private System.Collections.IEnumerator EnableRestartAfterDelay()
    {
        yield return new WaitForSeconds(1f);
        m_CanRestart = true;
    }
}