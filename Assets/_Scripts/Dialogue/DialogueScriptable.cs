using System;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Localization;

[Serializable]
public struct Dialogue
{
    public LocalizedString text;
    public UnityEvent onStart;
    public UnityEvent onFinish;
}

[CreateAssetMenu(fileName = "Dialogue", menuName = "Scriptable Objects/Dialogue", order = 1)]
public class DialogueScriptable : ScriptableObject
{
    public bool isRandom = false;
    public bool disableMovement = true;
    public Dialogue[] dialogues;
    public UnityEvent onEnd;
}