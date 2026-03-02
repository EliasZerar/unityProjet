using UnityEngine;
using UnityEngine.UI;

public class ScrubInteraction : Interaction
{
    [Header("Scrubbing Settings")]
    [SerializeField] private float m_ScrubDistanceRequired = 300f;
    [SerializeField] private float m_KeyboardScrubAmount = 30f;
    [SerializeField] private KeyCode m_ScrubKey = KeyCode.F;

    [Header("Visual Feedback")]
    [SerializeField] private Renderer m_TargetRenderer;
    [SerializeField] private Material m_DirtyMaterial;
    [SerializeField] private Material m_CleanMaterial;

    [Header("Scrubbing UI")]
    [SerializeField] private string m_ScrubPromptMessage = "Frottez!";
    [SerializeField] private Image m_ProgressBarFill;
    [SerializeField] private GameObject m_ProgressBarRoot;

    [Header("Player")]
    [SerializeField] private GameObject m_PlayerVisual;

    private float m_ScrubDistanceAccumulated = 0f;
    private Vector2 m_LastMousePosition;
    private bool m_IsScrubbing = false;

    protected override void OnInteractionStarted()
    {
        m_ScrubDistanceAccumulated = 0f;
        m_IsScrubbing = false;
        UpdateProgressBar(0f);
        UpdateCleanVisual(0f);

        if (m_ProgressBarRoot != null)
            m_ProgressBarRoot.SetActive(true);

        if (m_PlayerVisual != null)
            m_PlayerVisual.SetActive(false);

        InteractionPromptRef?.Show(m_ScrubPromptMessage);
    }

    protected override void OnInteractionUpdate()
    {
        HandleMouseScrub();

        if (Input.GetKeyDown(m_ScrubKey))
            AddProgress(m_KeyboardScrubAmount);
    }

    private void HandleMouseScrub()
    {
        if (Input.GetMouseButtonDown(0))
            BeginScrub();

        if (Input.GetMouseButton(0) && m_IsScrubbing)
            AccumulateMouseScrub();

        if (Input.GetMouseButtonUp(0))
            m_IsScrubbing = false;
    }

    private void BeginScrub()
    {
        m_IsScrubbing = true;
        m_LastMousePosition = Input.mousePosition;
    }

    private void AccumulateMouseScrub()
    {
        Vector2 currentMousePosition = Input.mousePosition;
        float delta = Vector2.Distance(currentMousePosition, m_LastMousePosition);
        m_LastMousePosition = currentMousePosition;
        AddProgress(delta);
    }

    private void AddProgress(float amount)
    {
        m_ScrubDistanceAccumulated += amount;
        float progress = Mathf.Clamp01(m_ScrubDistanceAccumulated / m_ScrubDistanceRequired);
        UpdateProgressBar(progress);
        UpdateCleanVisual(progress);

        if (m_ScrubDistanceAccumulated >= m_ScrubDistanceRequired)
            CompleteInteraction();
    }

    private void UpdateCleanVisual(float progress)
    {
        if (m_TargetRenderer == null || m_DirtyMaterial == null || m_CleanMaterial == null)
            return;

        m_TargetRenderer.material.Lerp(m_DirtyMaterial, m_CleanMaterial, progress);
    }

    private void UpdateProgressBar(float progress)
    {
        if (m_ProgressBarFill != null)
            m_ProgressBarFill.fillAmount = progress;
    }

    protected override void OnInteractionCompleted()
    {
        if (m_ProgressBarRoot != null)
            m_ProgressBarRoot.SetActive(false);

        if (m_PlayerVisual != null)
            m_PlayerVisual.SetActive(true);

        if (m_TargetRenderer != null && m_CleanMaterial != null)
            m_TargetRenderer.material = new Material(m_CleanMaterial);

        GameManager.Instance?.RegisterCompletedInteraction();
    }

    public void ReceiveOscJoystick(float x, float y)
    {
        if (!m_IsInteracting) return;
        float magnitude = new Vector2(x, y).magnitude;
        AddProgress(magnitude * m_KeyboardScrubAmount);
    }

    public void ReceiveOscScrub(float value)
    {
        if (!m_IsInteracting) return;
        AddProgress(Mathf.Abs(value) * m_KeyboardScrubAmount);
    }
}