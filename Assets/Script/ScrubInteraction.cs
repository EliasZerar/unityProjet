using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class ScrubInteraction : Interaction
{
    [Header("Scrubbing Settings")]
    [SerializeField] private float m_ScrubDistanceRequired = 300f;
    [SerializeField] private float m_KeyboardScrubAmount = 30f;
    [SerializeField] private KeyCode m_ScrubKey = KeyCode.F;

    [Header("Painting Visual")]
    [SerializeField] private Renderer m_PaintingRenderer;
    [SerializeField] private Texture2D m_CleanTexture;
    [SerializeField] private float m_StartBrightness = 0.15f;
    [SerializeField] private float m_RevealAnimDuration = 0.8f;

    [Header("Spider Web Quad")]
    [SerializeField] private Renderer m_SpiderWebRenderer;

    [Header("Code Reveal")]
    [SerializeField] private GameObject m_CodeReveal;

    [Header("Scrubbing UI")]
    [SerializeField] private string m_ScrubPromptMessage = "Frottez la toile !";
    [SerializeField] private Image m_ProgressBarFill;
    [SerializeField] private GameObject m_ProgressBarRoot;

    [Header("Player")]
    [SerializeField] private GameObject m_PlayerVisual;

    private float m_ScrubDistanceAccumulated;
    private Vector2 m_LastMousePosition;
    private bool m_IsScrubbing;
    private Material m_PaintingMat;
    private Material m_WebMat;

    protected override void OnInteractionStarted()
    {
        m_ScrubDistanceAccumulated = 0f;
        m_IsScrubbing = false;

        if (m_PaintingRenderer != null)
        {
            m_PaintingMat = new Material(m_PaintingRenderer.material);
            if (m_CleanTexture != null)
                m_PaintingMat.mainTexture = m_CleanTexture;
            m_PaintingMat.color = new Color(m_StartBrightness, m_StartBrightness, m_StartBrightness, 1f);
            m_PaintingRenderer.material = m_PaintingMat;
        }

        if (m_SpiderWebRenderer != null)
        {
            m_WebMat = new Material(m_SpiderWebRenderer.material);
            m_SpiderWebRenderer.material = m_WebMat;
            SetWebAlpha(1f);
            m_SpiderWebRenderer.gameObject.SetActive(true);
        }

        UpdateProgressBar(0f);
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

    protected override void OnInteractionCompleted()
    {
        StartCoroutine(RevealPainting());

        if (m_ProgressBarRoot != null)
            m_ProgressBarRoot.SetActive(false);

        if (m_SpiderWebRenderer != null)
            m_SpiderWebRenderer.gameObject.SetActive(false);

        if (m_PlayerVisual != null)
            m_PlayerVisual.SetActive(true);

        if (m_CodeReveal != null)
            m_CodeReveal.SetActive(true);

        GameManager.Instance?.RegisterCompletedInteraction("board");
    }

    private void HandleMouseScrub()
    {
        if (Input.GetMouseButtonDown(0)) BeginScrub();
        if (Input.GetMouseButton(0) && m_IsScrubbing) AccumulateMouseScrub();
        if (Input.GetMouseButtonUp(0)) m_IsScrubbing = false;
    }

    private void BeginScrub()
    {
        m_IsScrubbing = true;
        m_LastMousePosition = Input.mousePosition;
    }

    private void AccumulateMouseScrub()
    {
        Vector2 current = Input.mousePosition;
        float delta = Vector2.Distance(current, m_LastMousePosition);
        m_LastMousePosition = current;
        AddProgress(delta);
    }

    private void AddProgress(float amount)
    {
        m_ScrubDistanceAccumulated += amount;
        float progress = Mathf.Clamp01(m_ScrubDistanceAccumulated / m_ScrubDistanceRequired);

        UpdateProgressBar(progress);
        UpdateWebAlpha(progress);
        UpdatePaintingBrightness(progress);

        if (m_ScrubDistanceAccumulated >= m_ScrubDistanceRequired)
            CompleteInteraction();
    }

    private void UpdateWebAlpha(float progress)
    {
        SetWebAlpha(1f - progress);
    }

    private void SetWebAlpha(float alpha)
    {
        if (m_WebMat == null) return;
        Color c = m_WebMat.color;
        c.a = alpha;
        m_WebMat.color = c;
    }

    private void UpdatePaintingBrightness(float progress)
    {
        if (m_PaintingMat == null) return;
        float brightness = Mathf.Lerp(m_StartBrightness, 1f, progress);
        m_PaintingMat.color = new Color(brightness, brightness, brightness, 1f);
    }

    private void UpdateProgressBar(float progress)
    {
        if (m_ProgressBarFill != null)
            m_ProgressBarFill.fillAmount = progress;
    }

    private IEnumerator RevealPainting()
    {
        if (m_PaintingMat == null) yield break;

        float elapsed = 0f;
        while (elapsed < m_RevealAnimDuration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / m_RevealAnimDuration;
            float brightness = Mathf.Lerp(1.5f, 1f, t);
            m_PaintingMat.color = new Color(brightness, brightness, brightness, 1f);
            yield return null;
        }

        m_PaintingMat.color = Color.white;
    }

    public void ReceiveOscJoystick(float x, float y)
    {
        if (!m_IsInteracting) return;
        AddProgress(new Vector2(x, y).magnitude * m_KeyboardScrubAmount);
    }

    public void ReceiveOscScrub(float value)
    {
        if (!m_IsInteracting) return;
        AddProgress(Mathf.Abs(value) * m_KeyboardScrubAmount);
    }
}