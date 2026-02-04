using System;
using UnityEngine;

public static class Hash
{
    public static readonly int _Open = Animator.StringToHash("Open");
    public static readonly int _Close = Animator.StringToHash("Close");
}

[Serializable]
public struct Level
{
    public string name;
    public Sprite image;
    public GameObject[] patterns;
    public float timer;
}

[CreateAssetMenu(fileName = "Game Data", menuName = "Scriptable Objects/Game Data", order = 0)]
public class GameData : ScriptableObject
{
    [Header("Limiters")]
    public float limiter_timeToCpu;
    [TextArea] public string limiter_textCpu;
    public float limiter_speedToHdd;
    [TextArea] public string limiter_textHdd;
    public float limiter_distanceToRam;
    [TextArea] public string limiter_textRam;
    public float limiter_time;

    [Header("Bugs")]
    public float bugs_closeDistance;
    public float bugs_contactDistance;

    [Header("Character")]
    public float character_speed;

    [Space]
    public Level[] levels;
}