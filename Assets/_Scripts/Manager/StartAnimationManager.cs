using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class StartAnimationManager : MonoBehaviour
{
    [SerializeField] private Animator anim;
    [SerializeField] private Graphic nextText;

    private bool canNext = false;
    private int count;

    private void Awake()
    {
        nextText.CrossFadeAlpha(0, 0, false);
    }

    private async UniTaskVoid Start()
    {
        await UniTask.NextFrame();
        await UniTask.WaitForSeconds(1);

        anim.SetTrigger(Hash._Next);
        GameManager.readyIntro = true;
    }

    public void Next()
    {
        canNext = false;

        nextText.CrossFadeAlpha(0, 0.15f, false);
        anim.SetTrigger(Hash._Next);
    }

    public void OnFinish()
    {
        canNext = true;

        nextText.CrossFadeAlpha(1, 1.5f, false);
    }

    public void OnEnd()
    {
        SceneManager.LoadScene(1);
    }

    private void Update()
    {
        if (canNext && Input.anyKeyDown)
        {
            count++;

            if (count != 5)
            {
                Next();
            }
            else
            {
                canNext = false;
                anim.SetTrigger(Hash._Next);
            }
        }
    }
}