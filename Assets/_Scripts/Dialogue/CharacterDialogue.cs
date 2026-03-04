using Cysharp.Threading.Tasks;
using System.Threading;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CharacterDialogue : MonoBehaviour
{
    public static CharacterDialogue instance;

    [SerializeField] private GameData data;

    [Space]
    [SerializeField] private GameObject root;
    [SerializeField] private RectTransform panelRoot;
    [SerializeField] private HorizontalLayoutGroup horizontalLayoutGroup;
    [SerializeField] private GameObject blockRaycast;

    [Space]
    [SerializeField] private CanvasGroup canvasGroup;
    [SerializeField] private TextMeshProUGUI text;
    [SerializeField] private GameObject btnExit;
    [SerializeField] private GameObject btnNext;

    [Space]
    [SerializeField] private AudioSource source;
    [SerializeField] private AudioClip clip;

    private DialogueScriptable currentDialogue;
    [HideInInspector] public bool movementDisabled = false;

    private string title;
    [HideInInspector] private string titlePart;
    [HideInInspector] private int titleLength;
    private string[] titleParts;
    private int titleIndex;

    [Space]
    private bool inDialogue = false;
    private bool isAnimating = false;
    private bool isFinished = true;
    private bool isFirstText = false;
    private bool skipText = false;
    private bool waitForNext = false;

    private int dialogueIndex = 0;

    [Space]
    private Vector2 nextPivot = new(0.5f, 0.5f);
    private Vector2 nextAnchored = Vector2.zero;

    private CancellationTokenSource token;
    private int tweenId = 0;

    private void Awake()
    {
        instance = this;

        DisablePanel();
    }

    private void DisablePanel()
    {
        root.SetActive(false);
        canvasGroup.alpha = 0;
        btnExit.SetActive(false);
        btnNext.SetActive(false);

        movementDisabled = false;
        inDialogue = false;
    }

    private void Update()
    {
        if (!inDialogue)
            return;

        if (!currentDialogue.isRandom && Input.GetMouseButtonDown(0))
        {
            if (isAnimating)
            {
                skipText = true;
            }
            else if (waitForNext || isFinished)
            {
                NextConversation();
            }
        }

        if (PlayerController.instance.transform.position.x < 52.2f)
        {
            // poner a la izquierda
            nextPivot.x = 0;
            nextAnchored.x = 50;
            horizontalLayoutGroup.childAlignment = TextAnchor.UpperLeft;
        }
        else
        {
            // poner a la derecha
            nextPivot.x = 1;
            nextAnchored.x = -50;
            horizontalLayoutGroup.childAlignment = TextAnchor.UpperRight;
        }

        if (PlayerController.instance.transform.position.y > 1f)
        {
            // poner abajo
            nextPivot.y = 1;
            nextAnchored.y = Mathf.Lerp(nextAnchored.y, -50, Time.deltaTime * 10);
        }
        else
        {
            // poner arriba
            nextPivot.y = 0;
            nextAnchored.y = Mathf.Lerp(nextAnchored.y, 50, Time.deltaTime * 10);
        }

        panelRoot.pivot = Vector2.Lerp(panelRoot.pivot, nextPivot, Time.deltaTime * 75);
        panelRoot.anchoredPosition = nextAnchored;

        root.transform.position = Vector2.Lerp(root.transform.position, PlayerController.instance.transform.position, Time.deltaTime * 10);
    }

    public void Conversation_Scared()
    {
        InitConversation(data.dialogue_scared);
    }

    public void Conversation_Tutorial()
    {
        InitConversation(data.dialogue_tutorial);
    }

    public void Conversation_EndTutorial()
    {
        InitConversation(data.dialogue_endTutorial);
    }

    public void Conversation_CorruptedLevel()
    {
        InitConversation(data.dialogue_corruptedLevel);
    }

    public void Conversation_EndGame()
    {
        InitConversation(data.dialogue_endGame);
    }

    public void InitConversation(DialogueScriptable dialogueScriptable)
    {
        token?.Cancel();
        token = new CancellationTokenSource();
        LeanTween.cancel(tweenId);

        //panelRoot.pivot = Vector2.zero;
        //panelRoot.anchoredPosition = Vector2.zero;

        inDialogue = true;
        isFirstText = true;

        currentDialogue = dialogueScriptable;

        movementDisabled = currentDialogue.disableMovement;

        btnExit.SetActive(false);
        btnNext.SetActive(false);
        text.text = string.Empty;

        root.SetActive(true);

        if (isFirstText)
            source.PlayOneShot(clip);

        blockRaycast.SetActive(!currentDialogue.isRandom);

        tweenId = LeanTween.alphaCanvas(canvasGroup, 1, 0.25f).setOnComplete(InitPanel).id;
    }

    private void InitPanel()
    {
        if (currentDialogue.isRandom)
        {
            dialogueIndex = currentDialogue.dialogues.Length - 1;
            SetText(currentDialogue.dialogues[Random.Range(0, currentDialogue.dialogues.Length)].dialogue);
        }
        else
        {
            dialogueIndex = -1;
            waitForNext = true;

            NextConversation();
        }
    }

    private void NextConversation()
    {
        btnExit.SetActive(false);
        btnNext.SetActive(false);

        dialogueIndex++;

        if (dialogueIndex > currentDialogue.dialogues.Length - 1)
        {
            FinishConversation();

            return;
        }

        currentDialogue.dialogues[dialogueIndex].onStart?.Invoke();

        SetText(currentDialogue.dialogues[dialogueIndex].dialogue);
    }

    private void FinishConversation()
    {
        currentDialogue.onEnd?.Invoke();

        tweenId = LeanTween.alphaCanvas(canvasGroup, 0, 0.2f).setOnComplete(DisablePanel).id;
    }

    private void SetText(string newText)
    {
        isAnimating = true;
        isFinished = false;

        title = newText;
        titlePart = string.Empty;
        text.text = titlePart;
        titleLength = newText.Length;
        titleParts = new string[titleLength];

        for (int i = 0; i < titleLength; i++)
        {
            titlePart += newText[i];
            titleParts[i] = titlePart;
        }

        titlePart = string.Empty;
        titleLength = 0;

        PlayerController.instance.SetTalking(true);

        if (isFirstText)
            isFirstText = false;
        else
            source.PlayOneShot(clip);

        AnimateText().Forget();
    }

    private async UniTaskVoid AnimateText()
    {
        titleIndex = 0;

        await UniTask.NextFrame();

        while (!skipText && text.text.Length < title.Length)
        {
            text.text = titleParts[titleIndex];
            titleIndex++;

            await UniTask.Delay(25, cancellationToken: token.Token);
        }

        skipText = false;
        isAnimating = false;
        isFinished = true;

        text.text = title;

        PlayerController.instance.SetTalking(false);

        currentDialogue.dialogues[dialogueIndex].onFinish?.Invoke();

        if (!currentDialogue.isRandom)
        {
            if (dialogueIndex == currentDialogue.dialogues.Length - 1)
            {
                btnExit.SetActive(true);
            }
            else
            {
                btnNext.SetActive(true);
            }
        }
        else
        {
            await UniTask.WaitForSeconds(1.5f, cancellationToken: token.Token);

            FinishConversation();
        }
    }
}