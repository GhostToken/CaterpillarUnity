using FIMSpace.FSpine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using HedgehogTeam.EasyTouch;
using Pathfinding;
using UnityEngine.Timeline;

public class Caterpillar : MonoBehaviour
{
    #region Navigation 

    public LayerMask LayerMask;
    public IAstarAI AI;
    public GameObject MoveMarker;

    private GameObject MoveMarkerInstance;

    private GameObject Marker
    {
        get
        {
            if(MoveMarkerInstance == null)
            {
                MoveMarkerInstance = GameObject.Instantiate(MoveMarker);
            }
            return MoveMarkerInstance;
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        AI = GetComponent<IAstarAI>();
    }

    private void OnEnable()
    {
        EasyTouch.On_SimpleTap += OnTap;
    }

    private void OnDisable()
    {
        EasyTouch.On_SimpleTap -= OnTap;
    }

    public void OnTap(Gesture gesture)
    {
        if( Partie.Paused == true)
        {
            return;
        }

        Ray ray;
        RaycastHit hit;
        ray = Camera.main.ScreenPointToRay(gesture.position);
        if (!Physics.Raycast(ray, out hit, 1000, LayerMask))
        {
            return;
        }

        Marker.transform.position = hit.point;

        AI.destination =  hit.point;
    }

    #endregion

    #region Gameplay

    private void OnTriggerEnter(Collider other) 
    {
        Food food = other.GetComponent<Food>();
        if( food == null)
        {
            food = other.GetComponentInParent<Food>();
        }
        if( food == null)
        {
            food = other.transform.parent.GetComponentInParent<Food>();
        }
        if( food != null)
        {
            food.Mange();
        }
    }

    #endregion
}
