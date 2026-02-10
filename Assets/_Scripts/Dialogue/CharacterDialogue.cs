using Cysharp.Threading.Tasks;
using System.Threading;
using TMPro;
using UnityEngine;

public class CharacterDialogue : MonoBehaviour
{
    public static CharacterDialogue instance;

    [SerializeField] private GameData data;

    [Space]
    [SerializeField] private GameObject root;
    [SerializeField] private RectTransform panelRoot;
    [SerializeField] private CanvasGroup canvasGroup;
    [SerializeField] private TextMeshProUGUI text;
    [SerializeField] private GameObject btnExit;
    [SerializeField] private GameObject btnNext;

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
    }

    private void Update()
    {
        if (!inDialogue)
            return;

        if (!currentDialogue.isRandom && Input.anyKeyDown)
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
        }
        else
        {
            // poner a la derecha
            nextPivot.x = 1;
            nextAnchored.x = -50;
        }

        if (PlayerController.instance.transform.position.y > 1f)
        {
            // poner abajo
            nextPivot.y = 1;
            nextAnchored.y = -50;
        }
        else
        {
            // poner arriba
            nextPivot.y = 0;
            nextAnchored.y = 50;
        }

        panelRoot.pivot = Vector2.Lerp(panelRoot.pivot, nextPivot, Time.deltaTime * 2.5f);
        panelRoot.anchoredPosition = Vector2.Lerp(panelRoot.anchoredPosition, nextAnchored, Time.deltaTime * 2.5f);

        root.transform.position = Vector2.Lerp(root.transform.position, PlayerController.instance.transform.position, Time.deltaTime * 10);
    }

    public void Conversation_Scared()
    {
        InitConversation(data.dialogue_scared);
    }

    public void InitConversation(DialogueScriptable dialogueScriptable)
    {
        token?.Cancel();
        token = new CancellationTokenSource();
        LeanTween.cancel(tweenId);

        //panelRoot.pivot = Vector2.zero;
        //panelRoot.anchoredPosition = Vector2.zero;

        inDialogue = true;

        currentDialogue = dialogueScriptable;

        movementDisabled = currentDialogue.disableMovement;

        btnExit.SetActive(false);
        btnNext.SetActive(false);
        text.text = string.Empty;

        root.SetActive(true);

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

        movementDisabled = false;
        inDialogue = false;

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

        AnimateText().Forget();
    }

    private async UniTaskVoid AnimateText()
    {
        titleIndex = 0;

        while (!skipText && text.text.Length < title.Length)
        {
            text.text = titleParts[titleIndex];
            titleIndex++;

            await UniTask.Delay(50, cancellationToken: token.Token);
        }

        skipText = false;
        isAnimating = false;
        isFinished = true;

        text.text = title;

        PlayerController.instance.SetTalking(false);

        currentDialogue.dialogues[dialogueIndex].onFinish?.Invoke();

        if (!currentDialogue.isRandom)
        {
            await UniTask.WaitForSeconds(1, cancellationToken: token.Token);

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
            await UniTask.WaitForSeconds(2, cancellationToken: token.Token);

            FinishConversation();
        }
    }

    // Cuando el texto persiga al personaje, revisar si esta en la mitad derecha o izquierda
    // dependiendo de eso, el texto se girara para que este de un lado o del otro, para prevenir
    // que atraviese la pantalla
    // hacer lo mismo con arriba o abajo
    // mover PosX y PosY de acuerdo a cual esquina deberia estar
    // moverlo con vector2.lerp para darle el suavizado
}