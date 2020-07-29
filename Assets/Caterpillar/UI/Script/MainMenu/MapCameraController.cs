using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using HedgehogTeam.EasyTouch;

public class MapCameraController : MonoBehaviour
{
    public MapControls MapControl;

    private Camera MapCamera;
    private Vector3 DragStartPoint;
    private Vector3 DragMapStartPoint;
    private bool DragInitialized = false;
    private float DragStartTime = 0.0f;

    void Start()
    {
        MapCamera = GetComponent<Camera>();
    }

    private void OnEnable()
    {
        EasyTouch.On_SimpleTap  += OnTap;
        EasyTouch.On_DragStart  += OnDragStart;
        EasyTouch.On_Drag       += OnDrag;
        EasyTouch.On_DragEnd    += OnDragEnd;
    }

    private void OnDisable()
    {
        EasyTouch.On_SimpleTap  -= OnTap;
        EasyTouch.On_DragStart  -= OnDragStart;
        EasyTouch.On_Drag       -= OnDrag;
        EasyTouch.On_DragEnd    -= OnDragEnd;
    }

    private void OnDragStart(Gesture gesture)
    {
        Ray ray;
        RaycastHit hit;

        ray = MapCamera.ScreenPointToRay(gesture.position);
        if (Physics.Raycast(ray, out hit, 1000))
        {
            DragStartPoint = hit.point;
            DragMapStartPoint = MapControl.transform.position;
        }
    }

    private void OnDrag(Gesture gesture)
    {
        Ray ray;
        RaycastHit hit;

        ray = MapCamera.ScreenPointToRay(gesture.position);
        if (!Physics.Raycast(ray, out hit, 1000))
        {
            return;
        }

        Vector3 offset = hit.point - DragStartPoint;

        MapControl.DragTo(DragMapStartPoint + offset);
    }

    private void OnDragEnd(Gesture gesture)
    {
        MapControl.EndDrag();
    }

    public void OnTap(Gesture gesture)
    {
        Ray ray;
        RaycastHit hit;
        ray = MapCamera.ScreenPointToRay(gesture.position);
        if (!Physics.Raycast(ray, out hit, 1000))
        {
            return;
        }

        MapButton Button = hit.collider.GetComponent<MapButton>();
        if (Button != null)
        {
            Menu.Instance.OpenLevelPopup(Button.LevelID);
        }
    }
}
