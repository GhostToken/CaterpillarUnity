using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Caterpillar : MonoBehaviour
{
    #region Navigation 

    public float Speed = 3.0f;

    public LayerMask LayerMask_TapToMove;
    public LayerMask LayerMask_Joystick;
    public GameObject MoveMarkerPrefab;

    public GameObject JoystickPrefab;

    private CaterpillarAI AI;
    private CaterpillarInput Input;



    // Start is called before the first frame update
    void Start()
    {
        if(Options.UseTapToMove)
        {
            AI = gameObject.AddComponent<CaterpillarAI>();
            AI.speed = Speed;
            AI.LayerMask = LayerMask_TapToMove;
            AI.MoveMarkerPrefab = MoveMarkerPrefab;
        }
        else
        {
            Input = gameObject.AddComponent<CaterpillarInput>();
            Input.BaseSpeed = Speed;
            Input.GroundLayerMask = LayerMask_Joystick;

            GameObject JoystickPrefabInstance = GameObject.Instantiate(JoystickPrefab);
            JoystickPrefabInstance.GetComponentInChildren<ETCJoystick>().onMove.AddListener(Input.OnMove);
            JoystickPrefabInstance.GetComponentInChildren<ETCJoystick>().onMoveEnd.AddListener(Input.OnMoveEnd);
        }
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
