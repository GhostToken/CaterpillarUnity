using FIMSpace.FSpine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using HedgehogTeam.EasyTouch;
using Pathfinding;

public class Caterpillar : MonoBehaviour
{
    #region Navigation 

    public LayerMask LayerMask;
    public IAstarAI AI;

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

    //Vector3 RayCastPoint;
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

        //RayCastPoint = hit.point;
        AI.destination =  hit.point;
    }

    // public void OnDrawGizmos() 
    // {
    //     if( Application.isPlaying && Application.isEditor)
    //     {
    //         Gizmos.color = Color.blue;
    //         Gizmos.DrawWireSphere(RayCastPoint, 0.25f);
    //         Gizmos.color = Color.green;
    //         if( AI != null)
    //         {
    //             Gizmos.DrawWireSphere(AI.destination, 0.25f);
    //         }
    //     }
    // }

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
