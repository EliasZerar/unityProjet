using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class StoveInteraction : Interaction
{
    [Header("Stove Settings")]
    [SerializeField] private int m_SequenceLength = 4;
    [SerializeField] private float m_ShowDuration = 0.6f;
    [SerializeField] private float m_BetweenDelay = 0.3f;

    [Header("Plate Visuals")]
    [SerializeField] private Image m_Plate1;
    [SerializeField] private Image m_Plate2;
    [SerializeField] private Image m_Plate3;
    [SerializeField] private Image m_Plate4;

    [Header("Colors")]
    [SerializeField] private Color m_OffColor = new Color(0.2f, 0.2f, 0.2f);
    [SerializeField] private Color m_OnColor = new Color(1f, 0.4f, 0f);
    [SerializeField] private Color m_SuccessColor = new Color(0.2f, 0.9f, 0.2f);
    [SerializeField] private Color m_ErrorColor = new Color(0.9f, 0.1f, 0.1f);

    [Header("UI")]
    [SerializeField] private TextMeshProUGUI m_InstructionText;
    [SerializeField] private GameObject m_StoveUIRoot;
    [SerializeField] private GameObject m_PlayerVisual;

    private List<int> m_Sequence = new List<int>();
    private int m_PlayerIndex = 0;
    private bool m_PlayerCanInput = false;
    private Image[] m_Plates;

    protected override void OnInteractionStarted()
    {
        m_Plates = new Image[] { m_Plate1, m_Plate2, m_Plate3, m_Plate4 };

        if (m_StoveUIRoot != null)
            m_StoveUIRoot.SetActive(true);

        if (m_PlayerVisual != null)
            m_PlayerVisual.SetActive(false);

        ResetPlates();
        GenerateSequence();
        StartCoroutine(PlaySequence());
    }

    protected override void OnInteractionUpdate()
    {
        if (!m_PlayerCanInput) return;

        if (Input.GetKeyDown(KeyCode.Alpha1)) HandleInput(0);
        else if (Input.GetKeyDown(KeyCode.Alpha2)) HandleInput(1);
        else if (Input.GetKeyDown(KeyCode.Alpha3)) HandleInput(2);
        else if (Input.GetKeyDown(KeyCode.Alpha4)) HandleInput(3);
    }

    private void GenerateSequence()
    {
        m_Sequence.Clear();
        for (int i = 0; i < m_SequenceLength; i++)
            m_Sequence.Add(Random.Range(0, 4));
    }

    private IEnumerator PlaySequence()
    {
        m_PlayerCanInput = false;
        SetInstruction("Observe la séquence...");

        yield return new WaitForSeconds(0.5f);

        foreach (int index in m_Sequence)
        {
            yield return StartCoroutine(FlashPlate(index, m_OnColor));
            yield return new WaitForSeconds(m_BetweenDelay);
        }

        m_PlayerIndex = 0;
        m_PlayerCanInput = true;
        SetInstruction("Reproduis la séquence ! (1 2 3 4)");
    }

    private IEnumerator FlashPlate(int index, Color color)
    {
        SetPlateColor(index, color);
        yield return new WaitForSeconds(m_ShowDuration);
        SetPlateColor(index, m_OffColor);
    }

    public void HandleInput(int index)
    {
        if (!m_PlayerCanInput) return;

        StartCoroutine(FlashPlate(index, m_SuccessColor));

        if (m_Sequence[m_PlayerIndex] == index)
        {
            m_PlayerIndex++;

            if (m_PlayerIndex >= m_Sequence.Count)
            {
                m_PlayerCanInput = false;
                StartCoroutine(SuccessSequence());
            }
        }
        else
        {
            m_PlayerCanInput = false;
            StartCoroutine(FailSequence());
        }
    }

    private IEnumerator SuccessSequence()
    {
        SetInstruction("Bien joué !");
        for (int i = 0; i < 4; i++)
            SetPlateColor(i, m_SuccessColor);

        yield return new WaitForSeconds(1f);

        if (m_StoveUIRoot != null)
            m_StoveUIRoot.SetActive(false);

        if (m_PlayerVisual != null)
            m_PlayerVisual.SetActive(true);

        CompleteInteraction();
    }

    private IEnumerator FailSequence()
    {
        SetInstruction("Raté ! Recommence...");
        for (int i = 0; i < 4; i++)
            SetPlateColor(i, m_ErrorColor);

        yield return new WaitForSeconds(1f);

        ResetPlates();
        GenerateSequence();
        StartCoroutine(PlaySequence());
    }

    private void ResetPlates()
    {
        if (m_Plates == null) return;
        for (int i = 0; i < 4; i++)
            SetPlateColor(i, m_OffColor);
    }

    private void SetPlateColor(int index, Color color)
    {
        if (m_Plates != null && index < m_Plates.Length && m_Plates[index] != null)
            m_Plates[index].color = color;
    }

    private void SetInstruction(string text)
    {
        if (m_InstructionText != null)
            m_InstructionText.text = text;
    }

    protected override void OnInteractionCompleted()
    {
        GameManager.Instance?.RegisterCompletedInteraction();
    }

    public void ReceiveOscButton1() { HandleInput(0); }
    public void ReceiveOscButton2() { HandleInput(1); }
    public void ReceiveOscButton3() { HandleInput(2); }
    public void ReceiveOscButton4() { HandleInput(3); }
}