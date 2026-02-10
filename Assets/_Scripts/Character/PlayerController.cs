using Cysharp.Threading.Tasks;
using System.Threading;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public static PlayerController instance;

    [SerializeField] private GameData data;

    [Space]
    [SerializeField] private Animator anim;
    [SerializeField] private SpriteRenderer sprite;
    [SerializeField] private Rigidbody2D rb;

    [Space]
    [SerializeField] private Vector2 startPos;
    [SerializeField] private float endXpos;

    private bool canMove = true;
    private Vector2 move;

    private CancellationTokenSource token;

    private void Awake()
    {
        instance = this;

        GameManager.OnStartLevel += StartLevel;
        GameManager.OnWinLevel += WinLevel;
        GameManager.OnLoseLevel += LoseLevel;
        GameManager.OnCloseLevel += CloseLevel;
    }

    private void OnDestroy()
    {
        GameManager.OnStartLevel -= StartLevel;
        GameManager.OnWinLevel -= WinLevel;
        GameManager.OnLoseLevel -= LoseLevel;
        GameManager.OnCloseLevel -= CloseLevel;
    }

    private void StartLevel()
    {
        transform.position = startPos;

        canMove = true;
    }

    private void WinLevel()
    {
        canMove = false;
        move = Vector2.zero;
        anim.SetTrigger(Hash._Happy);

        LeanTween.moveLocalX(gameObject, endXpos, 1);
    }

    private void LoseLevel()
    {
        canMove = false;
        move = Vector2.zero;
        anim.SetTrigger(Hash._Dead);
    }

    private void CloseLevel()
    {
        canMove = true;
        move = Vector2.zero;
        anim.SetTrigger(Hash._Idle);
    }

    private void Update()
    {
        if (!canMove)
            return;

        move = new Vector2(Input.GetAxisRaw(Hash._Horizontal), Input.GetAxisRaw(Hash._Vertical));

        if (move.x < 0)
        {
            anim.SetBool(Hash._Front, false);
            anim.SetBool(Hash._Back, false);
            anim.SetBool(Hash._Side, true);
            sprite.flipX = false;
        }
        else if (move.x > 0)
        {
            anim.SetBool(Hash._Front, false);
            anim.SetBool(Hash._Back, false);
            anim.SetBool(Hash._Side, true);
            sprite.flipX = true;
        }
        else if (move.y > 0)
        {
            anim.SetBool(Hash._Front, false);
            anim.SetBool(Hash._Back, true);
            anim.SetBool(Hash._Side, false);
        }
        else if (move.y < 0)
        {
            anim.SetBool(Hash._Front, true);
            anim.SetBool(Hash._Back, false);
            anim.SetBool(Hash._Side, false);
        }
        else
        {
            anim.SetBool(Hash._Front, false);
            anim.SetBool(Hash._Back, false);
            anim.SetBool(Hash._Side, false);
        }
    }

    private void FixedUpdate()
    {
        rb.MovePosition(rb.position + move * data.character_speed);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (GameManager.IsPlaying)
            GameManager.instance.WinLevel();
    }

    public async UniTaskVoid Set_Scared()
    {
        token?.Cancel();
        token = new CancellationTokenSource();

        anim.SetTrigger(Hash._Scared);

        await UniTask.WaitForSeconds(2.5f, cancellationToken: token.Token);

        if (GameManager.IsPlaying)
            anim.SetTrigger(Hash._Idle);
    }

    public void SetTalking(bool on)
    {
        anim.SetBool(Hash._IsTalking, on);
    }
}