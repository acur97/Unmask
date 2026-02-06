using System;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    public static bool readyIntro = false;
    public static GameManager instance;

    public static Action<int> OnPrepareLevel;
    public static int CurrentLevel;
    public static Action OnStartLevel;
    public static bool IsPlaying;
    public static Action OnWinLevel;
    public static Action OnLoseLevel;
    public static Action OnCloseLevel;

    [SerializeField] private GameData data;
    [SerializeField] private Graphic blackFade;

    [Space]
    [SerializeField] private PcInterface pc;
    [SerializeField] private PhotoshopInterface photoshop;

    [Space]
    [SerializeField] private Animator tabletAnim;

    [Space]
    [SerializeField] private IconLevel iconPrefab;
    private IconLevel[] icons;
    [SerializeField] private Transform iconParent;
    private int childIcon;

    private void Awake()
    {
        instance = this;

        if (readyIntro)
        {
            readyIntro = false;

            blackFade.color = Color.black;
            blackFade.CrossFadeAlpha(0, 1, false);
        }
        else
        {
            blackFade.CrossFadeAlpha(0, 0, false);
        }

        icons = new IconLevel[data.levels.Length];
        for (int i = 0; i < icons.Length; i++)
        {
            icons[i] = Instantiate(iconPrefab, iconParent);

            if (i == 1)
            {
                icons[i].Set(i, true);
            }
            else
            {
                icons[i].Set(i, false);
            }
        }
    }

    public void ResetScene()
    {
        SceneManager.LoadScene(1);
    }

    public void WinLevel()
    {
        IsPlaying = false;
        OnWinLevel?.Invoke();

        childIcon = CurrentLevel + 1;
        if (childIcon < icons.Length)
        {
            icons[childIcon].ClearIcon();
        }
    }

    public void LoseLevel()
    {
        IsPlaying = false;
        OnLoseLevel?.Invoke();

        tabletAnim.SetTrigger(Hash._Close);
    }

    public void CloseLevel()
    {
        IsPlaying = false;

        OnCloseLevel?.Invoke();
    }

    public void StartLevel(int level)
    {
        CurrentLevel = level;

        photoshop.Init(data.levels[CurrentLevel].image);

        OnPrepareLevel?.Invoke(CurrentLevel);
        OnStartLevel?.Invoke();
        IsPlaying = true;
    }
}