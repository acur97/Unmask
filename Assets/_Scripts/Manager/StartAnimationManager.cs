using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class StartAnimationManager : MonoBehaviour
{
    [SerializeField] private Animator anim;
    [SerializeField] private Graphic nextText;

    [Space]
    [SerializeField] private AudioSource source;
    [SerializeField] private AudioClip[] clips;

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

        SetNext();
        GameManager.readyIntro = true;
    }

    private void SetNext()
    {
        anim.SetTrigger(Hash._Next);

        source.Stop();
        source.clip = clips[count];
        source.Play();
    }

    public void Next()
    {
        canNext = false;

        nextText.CrossFadeAlpha(0, 0.15f, false);
        SetNext();
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