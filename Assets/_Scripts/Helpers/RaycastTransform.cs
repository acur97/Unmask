using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class RaycastTransform : MonoBehaviour
{
    public static Vector2 ScreenPosition = Vector2.zero;
    public static Vector2 CenteredMousePos = Vector2.zero;

    [SerializeField] private Camera cam;
    [SerializeField] private float dragThreshold = 5;

    [Space]
    [SerializeField] private Vector2 screenOffset = new(-1255, -805);
    [SerializeField] private Vector2 screenOffsetSize = new(1.59f, 1.59f);

    [Header("Mouse")]
    [SerializeField] private Transform pcMouse;
    [SerializeField] private Vector2 screenOffset2;
    [SerializeField] private Vector2 screenOffsetSize2;
    [SerializeField] private Material pcPointer;
    [SerializeField] private Texture2D pointerNormal;
    [SerializeField] private Texture2D pointerSelect;
    [SerializeField] private Texture2D pointerMask;
    [SerializeField] private GameObject maskReference;

    [Header("Hand")]
    [SerializeField] private Transform handRoot;
    [SerializeField] private Transform handOffset;

    private Ray ray;
    private readonly RaycastHit[] hits = new RaycastHit[1];
    private PointerEventData data = new(EventSystem.current);

    private GameObject hoverTarget;
    private GameObject pressTarget;
    private GameObject dragTarget;

    private bool isDragging;
    private Vector2 pressPosition;

    private List<RaycastResult> RaycastUI_results;
    private List<RaycastResult> HandleMove_results;
    private List<RaycastResult> BeginPress_results;
    private GameObject newHover;

    private readonly Vector3 forward1 = new(0, 0, -0.1f);

    private void Awake()
    {
        pcPointer.SetTexture(Hash._BaseMap, pointerNormal);
        pcPointer.SetTexture(Hash._EmissionMap, pointerNormal);

        pcMouse.GetChild(0).transform.localPosition = Vector2.zero;
    }

    private void Update()
    {
        ProcessPointer();
    }

    private bool RaycastScreen()
    {
        ScreenPosition = Vector2.zero;

        ray = cam.ScreenPointToRay(Input.mousePosition);

        handOffset.localPosition = Input.GetMouseButton(0) ? Vector3.zero : forward1;

        if (Physics.RaycastNonAlloc(ray, hits) == 0)
            return false;

        handRoot.position = hits[0].point;

        ScreenPosition.Set(
            hits[0].textureCoord.x * 1920,
            hits[0].textureCoord.y * 1080);

        CenteredMousePos.x = (((ScreenPosition.x * screenOffsetSize.x) + screenOffset.x) / 959) * 178;
        CenteredMousePos.y = (((ScreenPosition.y * screenOffsetSize.y) + screenOffset.y) / 539) * 100;

        pcMouse.position = new Vector2(
            (ScreenPosition.x * screenOffsetSize2.x) + screenOffset2.x,
            (ScreenPosition.y * screenOffsetSize2.y) + screenOffset2.y);

        return true;
    }

    private void ProcessPointer()
    {
        if (RaycastScreen())
        {
            Cursor.visible = false;
            UpdatePointer(ScreenPosition);

            HandleMove();
            HandlePressState();
        }
        else
        {
            Cursor.visible = true;
            HandlePressState();
        }
    }

    private void UpdatePointer(Vector2 screenPos)
    {
        data ??= new PointerEventData(EventSystem.current)
        {
            pointerId = 0,
            button = PointerEventData.InputButton.Left
        };

        data.delta = screenPos - data.position;
        data.position = screenPos;
    }


    private List<RaycastResult> RaycastUI()
    {
        RaycastUI_results = new();
        EventSystem.current.RaycastAll(data, RaycastUI_results);
        return RaycastUI_results;
    }

    #region Move / Hover
    private void HandleMove()
    {
        ExecuteEvents.Execute(hoverTarget, data, ExecuteEvents.pointerMoveHandler);

        HandleMove_results = RaycastUI();
        newHover = HandleMove_results.Count > 0 ? HandleMove_results[0].gameObject : null;

        if (hoverTarget == null)
        {
            pcPointer.SetTexture(Hash._BaseMap, pointerNormal);
            pcPointer.SetTexture(Hash._EmissionMap, pointerNormal);

            pcMouse.GetChild(0).transform.localPosition = Vector2.zero;
        }
        else if (hoverTarget == maskReference)
        {
            pcPointer.SetTexture(Hash._BaseMap, pointerMask);
            pcPointer.SetTexture(Hash._EmissionMap, pointerMask);

            pcMouse.GetChild(0).transform.localPosition = Vector2.zero;
        }
        else
        {
            pcPointer.SetTexture(Hash._BaseMap, pointerSelect);
            pcPointer.SetTexture(Hash._EmissionMap, pointerSelect);

            pcMouse.GetChild(0).localPosition = new Vector2(-11.6f, 0);
        }

        if (newHover == hoverTarget)
            return;

        if (hoverTarget != null)
            ExecuteEvents.Execute(hoverTarget, data, ExecuteEvents.pointerExitHandler);

        hoverTarget = newHover;

        if (hoverTarget != null)
            ExecuteEvents.Execute(hoverTarget, data, ExecuteEvents.pointerEnterHandler);
    }
    #endregion

    private void HandlePressState()
    {
        if (Input.GetMouseButtonDown(0))
        {
            BeginPress();
        }
        else if (Input.GetMouseButtonUp(0))
        {
            HandleRelease();
        }
        else if (Input.GetMouseButton(0))
        {
            HandleDrag();
        }
    }

    #region Press / Down
    private void BeginPress()
    {
        pressPosition = data.position;

        EventSystem.current.SetSelectedGameObject(null);

        BeginPress_results = RaycastUI();
        if (BeginPress_results.Count == 0)
            return;

        pressTarget = BeginPress_results[0].gameObject;

        data.pointerPressRaycast = BeginPress_results[0];
        data.pointerPress = pressTarget;
        data.rawPointerPress = pressTarget;
        data.eligibleForClick = true;

        ExecuteEvents.Execute(pressTarget, data, ExecuteEvents.pointerDownHandler);
        ExecuteEvents.Execute(pressTarget, data, ExecuteEvents.initializePotentialDrag);
    }
    #endregion

    #region Drag
    private void HandleDrag()
    {
        if (pressTarget == null)
            return;

        if (!isDragging && Vector2.Distance(data.position, pressPosition) >= dragThreshold)
        {
            isDragging = true;
            dragTarget = pressTarget;

            data.dragging = true;

            ExecuteEvents.Execute(dragTarget, data, ExecuteEvents.beginDragHandler);
        }

        if (isDragging && dragTarget != null)
        {
            ExecuteEvents.Execute(dragTarget, data, ExecuteEvents.dragHandler);
        }
    }
    #endregion

    #region Release / Up / Click
    private void HandleRelease()
    {
        if (pressTarget != null)
        {
            ExecuteEvents.Execute(pressTarget, data, ExecuteEvents.pointerUpHandler);

            if (isDragging)
            {
                ExecuteEvents.Execute(dragTarget, data, ExecuteEvents.endDragHandler);
                //ExecuteEvents.Execute(null, data, ExecuteEvents.pointerClickHandler);
            }
            else if (data.eligibleForClick)
            {
                ExecuteEvents.Execute(pressTarget, data, ExecuteEvents.pointerClickHandler);
            }
        }

        data.dragging = false;
        data.eligibleForClick = false;
        data.pointerPress = null;
        data.rawPointerPress = null;

        pressTarget = null;
        dragTarget = null;
        isDragging = false;
    }
    #endregion
}