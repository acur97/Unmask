using System;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static bool readyIntro = false;
    public static GameManager instance;

    public static Action<int> OnPrepareLevel;
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
    private IconLevel icon;
    [SerializeField] private Transform iconParent;

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

        for (int i = 0; i < data.levels.Length; i++)
        {
            icon = Instantiate(iconPrefab, iconParent);

            if (i == 1)
            {
                icon.Set(i, true);
            }
            else
            {
                icon.Set(i, false);
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
        photoshop.Init(data.levels[level].image);

        OnPrepareLevel?.Invoke(level);
        OnStartLevel?.Invoke();
        IsPlaying = true;
    }
}