using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using HedgehogTeam.EasyTouch;

public class CameraFollowing : MonoBehaviour
{
    public Transform Target;
    public Vector3 Offset;
    public float MaxCameraSpeed;
    public float MaxCameraRotationSpeed;

    private Vector3 LastTargetPosition = Vector3.zero;
    private Vector3 TargetMove = Vector3.zero;

    private float TwistToComplete = 0.0f;

    // Start is called before the first frame update
    void Start()
    {
        Vector3 TargetPosition = Target.position + Target.TransformVector(Offset);

        LastTargetPosition = Target.position;

        transform.position = TargetPosition;
        transform.LookAt(Target.position);
    }

    // Update is called once per frame
    void Update()
    {
        Gesture current = EasyTouch.current;

        // Pinch
        //if (current.type == EasyTouch.EvtType.On_Pinch)
        //{
        //    Camera.main.fieldOfView += current.deltaPinch * 10 * Time.deltaTime;
        //}

        // Twist
        if (current.type == EasyTouch.EvtType.On_Twist)
        {
            TwistToComplete += current.twistAngle;
        }


        UpdateRotation();
        UpdatePosition();
    }

    void UpdateRotation()
    {
        float Rotation = Mathf.Clamp(TwistToComplete, -MaxCameraRotationSpeed * Time.deltaTime, MaxCameraRotationSpeed * Time.deltaTime);
        TwistToComplete -= Rotation;

        transform.RotateAround(Target.position, Vector3.up, Rotation);
    }

    void UpdatePosition()
    {
        TargetMove += (Target.position - LastTargetPosition);
        TargetMove.y = 0.0f;
        LastTargetPosition = Target.position;

        Vector3 NewPosition = Vector3.MoveTowards(transform.position, transform.position + TargetMove, MaxCameraSpeed * Time.deltaTime);

        TargetMove -= (NewPosition - transform.position);

        transform.position = NewPosition;
    }
}
