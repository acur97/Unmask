using System;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;

    public static Action<int> OnPrepareLevel;
    public static Action OnStartLevel;
    public static bool IsPlaying;
    public static Action OnWinLevel;
    public static Action OnLoseLevel;

    [SerializeField] private GameData data;

    [Header("References")]
    [SerializeField] private Image realImage1;
    [SerializeField] private Image realImage2;

    private void Awake()
    {
        instance = this;
    }

    private void Start()
    {
        StartLevel(1);
    }

    public void WinLevel()
    {
        IsPlaying = false;
        OnWinLevel?.Invoke();
    }

    public void LoseLevel()
    {
        IsPlaying = false;

        Debug.LogError("PANTALLAZO AZUL !!!");

        OnLoseLevel?.Invoke();
    }

    private void StartLevel(int level)
    {
        realImage1.sprite = data.levels[level].image;
        realImage2.sprite = data.levels[level].image;

        OnPrepareLevel?.Invoke(level);
        OnStartLevel?.Invoke();
        IsPlaying = true;
    }
}