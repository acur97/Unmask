using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.Localization;

public class Notifications : MonoBehaviour
{
    public static Notifications instance;

    [SerializeField] private NotificationPopup prefab;
    [SerializeField] private Transform parent;

    [Space]
    [SerializeField] private AudioSource source;
    [SerializeField] private AudioClip clip;

    private void Awake()
    {
        instance = this;
    }

    public async UniTaskVoid ShowPopup(LocalizedString message)
    {
        (await Extensions.AsyncInstantiate(prefab, parent)).Init(await message.GetLocalizedStringAsync());
        source.PlayOneShot(clip);
    }
}