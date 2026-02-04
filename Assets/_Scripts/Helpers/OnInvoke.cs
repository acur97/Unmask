using UnityEngine;
using UnityEngine.Events;

public class OnInvoke : MonoBehaviour
{
    [SerializeField] private UnityEvent invoke;

    public void InvokeEvent()
    {
        invoke?.Invoke();
    }
}