using TMPro;
using UnityEngine;
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

    public void Init(int level, bool __)
    {
        realImage1.sprite = data.levels[level].image;
        realImage2.sprite = data.levels[level].image;
    }

    public void UpdateTextSize(float value)
    {
        tmp_sliderSize.text = $"{value} px";
        TilemapBrushes.UpdateBrushSize((int)value);
    }

    public void Open_ToolsUp_Popup()
    {
        Vector2 position = new(RaycastTransform.ScreenPosition.x, -54);
        toolsUpPopupRect.anchoredPosition = position;

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
        circleClose.SetActive(false);
    }
}