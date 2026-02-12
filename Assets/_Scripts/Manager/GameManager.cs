using Cysharp.Threading.Tasks;
using System;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    public static bool readyIntro = false;
    public static GameManager instance;

    public static Action<int, bool> OnPrepareLevel;
    public static int CurrentLevel;
    public static bool IsPlaying;
    public static Action OnWinLevel;
    public static Action OnLoseLevel;
    public static Action OnCloseLevel;

    [SerializeField] private GameData data;
    [SerializeField] private Graphic blackFade;

    [Space]
    [SerializeField] private Animator tabletAnim;

    [Space]
    [SerializeField] private IconLevel iconPrefab;
    private IconLevel[] icons;
    [SerializeField] private Transform iconParent;
    private int childIcon;

    private int clearedLevelIndex = -1;

    [ContextMenu("Delete playerPrefs")]
    public void DeletePlayerPrefs()
    {
        PlayerPrefs.DeleteKey(Hash._LevelIndex);
    }

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

        SetIcons().Forget();
    }

    private async UniTaskVoid SetIcons()
    {
        clearedLevelIndex = PlayerPrefs.GetInt(Hash._LevelIndex, 0);

        icons = await Extensions.AsyncInstantiate(iconPrefab, data.levels.Length, iconParent);

        for (int i = 0; i < icons.Length; i++)
        {
            if (i <= clearedLevelIndex)
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
        PlayerPrefs.SetInt(Hash._LevelIndex, CurrentLevel + 1);

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

        OnPrepareLevel?.Invoke(CurrentLevel, icons[CurrentLevel].isAvalible && CurrentLevel < clearedLevelIndex);

        IsPlaying = true;
    }
}