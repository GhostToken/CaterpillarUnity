using FIMSpace.FSpine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using HedgehogTeam.EasyTouch;

public class Caterpillar : MonoBehaviour
{
    NavMeshAgent agent;

    // Start is called before the first frame update
    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
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
        Ray ray;
        RaycastHit hit;
        ray = Camera.main.ScreenPointToRay(gesture.position);
        if (!Physics.Raycast(ray, out hit, 1000))
        {
            return;
        }

        agent.destination = hit.point;
    }
}
