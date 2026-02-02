using UnityEngine;

public class Notifications : MonoBehaviour
{
    public static Notifications instance;

    [SerializeField] private NotificationPopup prefab;
    [SerializeField] private Transform parent;
    private NotificationPopup popup;

    private void Awake()
    {
        instance = this;
    }

    public void ShowPopup(string message)
    {
        popup = Instantiate(prefab, parent);
        popup.Init(message);
    }
}