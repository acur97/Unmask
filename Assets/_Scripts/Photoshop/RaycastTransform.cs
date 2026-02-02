using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class RaycastTransform : MonoBehaviour
{
    public static Vector2 ScreenPosition;

    [SerializeField] private Camera cam;
    [SerializeField] private float dragThreshold = 5;

    [Header("Hand")]
    [SerializeField] private Transform handRoot;
    [SerializeField] private Transform handOffset;

    private Ray ray;
    PointerEventData data;

    GameObject hoverTarget;
    GameObject pressTarget;
    GameObject dragTarget;

    bool isDragging;
    Vector2 pressPosition;

    private void Update()
    {
        ProcessPointer();
    }

    private bool RaycastScreen()
    {
        ScreenPosition = Vector2.zero;

        ray = cam.ScreenPointToRay(Input.mousePosition);

        handOffset.localPosition = Input.GetMouseButton(0) ? new Vector3(0, 0, 0) : new Vector3(0, 0, -0.1f);

        if (!Physics.Raycast(ray, out RaycastHit hit))
            return false;

        handRoot.position = hit.point;

        ScreenPosition = new Vector2(
            hit.textureCoord.x * 1920,
            hit.textureCoord.y * 1080
        );

        return true;
    }

    private void ProcessPointer()
    {
        if (RaycastScreen())
        {
            UpdatePointer(ScreenPosition);

            HandleMove();
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
        List<RaycastResult> results = new();
        EventSystem.current.RaycastAll(data, results);
        return results;
    }

    #region Move / Hover
    private void HandleMove()
    {
        ExecuteEvents.Execute(hoverTarget, data, ExecuteEvents.pointerMoveHandler);

        var results = RaycastUI();
        GameObject newHover = results.Count > 0 ? results[0].gameObject : null;

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

        List<RaycastResult> results = RaycastUI();
        if (results.Count == 0)
            return;

        pressTarget = results[0].gameObject;

        data.pointerPressRaycast = results[0];
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