using Cysharp.Threading.Tasks;
using UnityEngine;

public class Notifications : MonoBehaviour
{
    public static Notifications instance;

    [SerializeField] private NotificationPopup prefab;
    [SerializeField] private Transform parent;

    private void Awake()
    {
        instance = this;
    }

    public async UniTaskVoid ShowPopup(string message)
    {
        (await Extensions.AsyncInstantiate(prefab, parent)).Init(message);
    }
}