using UnityEngine;
using UnityEngine.Events;
using TMPro;

/// Countdown timer for a task. Displays remaining time and fires events on expiry.
/// Attach to a UI Canvas and wire up OnTimerExpired to your game over logic.
public class TaskTimer : MonoBehaviour
{
    [Header("Timer Settings")]
    [SerializeField] private float m_Duration = 60f;
    [SerializeField] private TextMeshProUGUI m_TimerText;

    [Header("Events")]
    public UnityEvent OnTimerExpired;

    private float m_TimeRemaining;
    private bool m_IsRunning = false;

    private void Awake()
    {
        m_TimeRemaining = m_Duration;
        UpdateDisplay();
    }

    private void Update()
    {
        if (!m_IsRunning)
            return;

        m_TimeRemaining -= Time.deltaTime;

        if (m_TimeRemaining <= 0f)
        {
            m_TimeRemaining = 0f;
            m_IsRunning = false;
            UpdateDisplay();
            OnTimerExpired?.Invoke();
            return;
        }

        UpdateDisplay();
    }

    public void StartTimer() => m_IsRunning = true;
    public void StopTimer()  => m_IsRunning = false;

    public void ResetTimer()
    {
        m_TimeRemaining = m_Duration;
        m_IsRunning = false;
        UpdateDisplay();
    }

    private void UpdateDisplay()
    {
        if (m_TimerText == null)
            return;

        int minutes = Mathf.FloorToInt(m_TimeRemaining / 60f);
        int seconds = Mathf.FloorToInt(m_TimeRemaining % 60f);
        m_TimerText.text = $"{minutes:00}:{seconds:00}";

        // Visual warning when time is low
        m_TimerText.color = m_TimeRemaining <= 10f ? Color.red : Color.white;
    }
}