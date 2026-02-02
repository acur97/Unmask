using Cysharp.Threading.Tasks;
using UnityEngine;

public class CharacterController : MonoBehaviour
{
    public static CharacterController instance;

    [SerializeField] private GameData data;

    [Space]
    [SerializeField] private Animator anim;
    [SerializeField] private SpriteRenderer sprite;
    [SerializeField] private Rigidbody2D rb;

    [Space]
    [SerializeField] private Vector2 startPos;
    [SerializeField] private float endXpos;

    private bool canMove = false;
    private Vector2 move;

    private const string _Horizontal = "Horizontal";
    private const string _Vertical = "Vertical";
    private readonly int _Front = Animator.StringToHash("Front");
    private readonly int _Back = Animator.StringToHash("Back");
    private readonly int _Side = Animator.StringToHash("Side");

    private readonly int _Idle = Animator.StringToHash("Idle");
    private readonly int _Talk = Animator.StringToHash("Talk");
    private readonly int _Happy = Animator.StringToHash("Happy");
    private readonly int _Scared = Animator.StringToHash("Scared");
    private readonly int _Dead = Animator.StringToHash("Dead");

    private void Awake()
    {
        instance = this;

        GameManager.OnStartLevel += StartLevel;
        GameManager.OnWinLevel += WinLevel;
        GameManager.OnLoseLevel += LoseLevel;
    }

    private void OnDestroy()
    {
        GameManager.OnStartLevel -= StartLevel;
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
        anim.SetTrigger(_Happy);

        LeanTween.moveLocalX(gameObject, endXpos, 1);
    }

    private void LoseLevel()
    {
        canMove = false;
        move = Vector2.zero;
        anim.SetTrigger(_Dead);
    }

    private void Update()
    {
        if (!canMove)
            return;

        move = new Vector2(Input.GetAxisRaw(_Horizontal), Input.GetAxisRaw(_Vertical));

        if (move.x < 0)
        {
            anim.SetBool(_Front, false);
            anim.SetBool(_Back, false);
            anim.SetBool(_Side, true);
            sprite.flipX = false;
        }
        else if (move.x > 0)
        {
            anim.SetBool(_Front, false);
            anim.SetBool(_Back, false);
            anim.SetBool(_Side, true);
            sprite.flipX = true;
        }
        else if (move.y > 0)
        {
            anim.SetBool(_Front, false);
            anim.SetBool(_Back, true);
            anim.SetBool(_Side, false);
        }
        else if (move.y < 0)
        {
            anim.SetBool(_Front, true);
            anim.SetBool(_Back, false);
            anim.SetBool(_Side, false);
        }
        else
        {
            anim.SetBool(_Front, false);
            anim.SetBool(_Back, false);
            anim.SetBool(_Side, false);
        }
    }

    private void FixedUpdate()
    {
        rb.MovePosition(rb.position + move * data.character_speed);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        GameManager.instance.WinLevel();
    }

    public async UniTaskVoid Set_Scared()
    {
        anim.SetTrigger(_Scared);

        await UniTask.WaitForSeconds(3);

        anim.SetTrigger(_Idle);
    }
}