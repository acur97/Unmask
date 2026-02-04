using Cysharp.Threading.Tasks;
using TMPro;
using UnityEngine;

public class NotificationPopup : MonoBehaviour
{
    [SerializeField] private Animator anim;
    [SerializeField] private TextMeshProUGUI title;

    public void Init(string text)
    {
        title.text = text;

        TimeToClose().Forget();
    }

    private async UniTaskVoid TimeToClose()
    {
        await UniTask.WaitForSeconds(3);

        anim.SetTrigger(Hash._Close);
    }

    public void Destroy()
    {
        Destroy(gameObject);
    }
}