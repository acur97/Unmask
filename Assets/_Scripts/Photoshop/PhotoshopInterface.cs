using TMPro;
using UnityEngine;
using UnityEngine.Localization.Components;
using UnityEngine.Localization.SmartFormat.Extensions;
using UnityEngine.Localization.SmartFormat.PersistentVariables;
using UnityEngine.UI;

public class PhotoshopInterface : MonoBehaviour
{
    public static PhotoshopInterface instance;

    [SerializeField] private GameData data;

    [Space]
    [SerializeField] private Image realImage1;
    [SerializeField] private Image realImage2;

    [Space]
    public Slider sizeSlider;
    [SerializeField] private TextMeshProUGUI tmp_sliderSize;

    [Space]
    [SerializeField] private RectTransform toolsUpPopupRect;
    [SerializeField] private CanvasGroup toolsUpPopup;

    [Space]
    [SerializeField] private GameObject circleClose;

    [Space]
    [SerializeField] private LocalizeStringEvent fileTitleEvent;
    [SerializeField] private LocalizeStringEvent timerEvent;

    private bool started = false;
    private float timeLeft;
    private int minutes;
    private int secs;
    private int prevSecs = -1;

    private const string _px = "{0} px";

    private void Awake()
    {
        instance = this;

        GameManager.OnPrepareLevel += Init;
        GameManager.OnWinLevel += CircleCloseOn;
        GameManager.OnCloseLevel += CircleCloseOff;

        CircleCloseOff();

        toolsUpPopup.gameObject.SetActive(false);
        toolsUpPopup.alpha = 0;
    }

    private void OnDestroy()
    {
        GameManager.OnPrepareLevel -= Init;
        GameManager.OnWinLevel -= CircleCloseOn;
        GameManager.OnCloseLevel -= CircleCloseOff;
    }

    private void Update()
    {
        if (!GameManager.IsPlaying || !started || CharacterDialogue.instance.movementDisabled)
            return;

        if (timeLeft > 1)
        {
            timeLeft -= Time.deltaTime;
            FormatTime();
        }
        else
        {
            GameManager.instance.LoseLevel();

            timerEvent.SetEntry("Programs_TimeLeft_Error");
        }
    }

    private void FormatTime()
    {
        minutes = Mathf.FloorToInt(timeLeft / 60f);
        secs = Mathf.FloorToInt(timeLeft % 60f);

        if (secs != prevSecs)
        {
            prevSecs = secs;

            using (PersistentVariablesSource.UpdateScope())
            {
                (timerEvent.StringReference["minutes"] as IntVariable).Value = minutes;
                (timerEvent.StringReference["seconds"] as IntVariable).Value = secs;
            }
        }
        else
        {
            prevSecs = secs;
        }
    }

    public void Init(int level, bool __)
    {
        realImage1.sprite = data.levels[level].image;
        realImage2.sprite = data.levels[level].image;

        (fileTitleEvent.StringReference["fileTitle"] as StringVariable).Value = data.levels[level].fileName;

        timeLeft = data.levels[level].timer;
        timerEvent.SetEntry("Programs_TimeLeft");
        prevSecs = -1;

        started = true;
    }

    public void UpdateTextSize(float value)
    {
        tmp_sliderSize.SetText(_px, value);
        TilemapBrushes.UpdateBrushSize((int)value);
    }

    public void Open_ToolsUp_Popup()
    {
        toolsUpPopupRect.anchoredPosition = new(RaycastTransform.ScreenPosition.x, -54);

        for (int i = 0; i < toolsUpPopupRect.childCount; i++)
        {
            toolsUpPopupRect.GetChild(i).SetSiblingIndex(Random.Range(0, toolsUpPopupRect.childCount));
        }

        toolsUpPopup.alpha = 0;
        toolsUpPopup.gameObject.SetActive(true);
        LeanTween.alphaCanvas(toolsUpPopup, 1, 0.15f);
    }

    public void Close_ToolsUp_Popup()
    {
        LeanTween.alphaCanvas(toolsUpPopup, 0, 0.15f).setOnComplete(() =>
        {
            toolsUpPopup.gameObject.SetActive(false);
        });
    }

    public void SetCanDraw(bool on)
    {
        TilemapBrushes.CanDraw = on;
    }

    public void CircleCloseOn()
    {
        circleClose.SetActive(true);
    }

    public void CircleCloseOff()
    {
        started = false;

        circleClose.SetActive(false);
        timerEvent.SetEntry("Programs_TimeLeft_Zero");
    }
}