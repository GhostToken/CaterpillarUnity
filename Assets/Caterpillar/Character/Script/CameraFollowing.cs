﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using HedgehogTeam.EasyTouch;
using System;

public class CameraFollowing : MonoBehaviour
{
    public enum ECameraMode
    {
        Orbital,
        ThirdPerson
    }

    public ECameraMode Mode = ECameraMode.ThirdPerson;
    public Vector3 Offset;
    public float MaxCameraSpeed;
    public float MaxCameraRotationSpeed;
    public float MaxCameraZoomSpeed = 1.0f;
    public float MinOrthographicSize = 1;
    public float MaxOrthographicSize = 20;

    private Vector3 LastTargetPosition = Vector3.zero;
    private Vector3 TargetMove = Vector3.zero;

    private Transform Target;
    private float ZoomToComplete = 0.0f;
    private float TwistToComplete = 0.0f;

    // Start is called before the first frame update
    void Start()
    {
        Caterpillar Cat = null;
        if( Cat = FindObjectOfType<Caterpillar>() )
        {
            Target = Cat.transform;
        }

        Vector3 TargetPosition = Target.position + Target.TransformVector(Offset);

        LastTargetPosition = Target.position;

        transform.position = TargetPosition;
        transform.LookAt(Target.position);

        Mode = (Options.UseOrbitalCamera ? ECameraMode.Orbital : ECameraMode.ThirdPerson);
    }

    // Update is called once per frame
    void Update()
    {
        UpdateZoom();

        switch(Mode)
        {
            case ECameraMode.Orbital:
                {
                    UpdateRotation();
                    UpdatePosition();
                    break;
                }
            default:
            case ECameraMode.ThirdPerson:
                {
                    UpdateFollowing();
                    break;
                }
        }
    }

    void UpdateZoom()
    {
        Gesture current = EasyTouch.current;

        // Pinch
        if (current.type == EasyTouch.EvtType.On_Pinch)
        {
            ZoomToComplete += 10.0f * current.deltaPinch / Screen.width;
        }

        float Zoom = Mathf.Clamp(ZoomToComplete, -MaxCameraZoomSpeed * Time.deltaTime, MaxCameraZoomSpeed * Time.deltaTime);
        ZoomToComplete -= Zoom;

        Camera.main.orthographicSize = Mathf.Clamp(Camera.main.orthographicSize - Zoom, MinOrthographicSize, MaxOrthographicSize);
    }

    void UpdateFollowing()
    {
        Vector3 TargetForward = Target.forward;
        TargetForward.y = 0.0f;
        TargetForward = TargetForward.normalized;
        Vector3 TargetPosition = Target.position + Target.right * Offset.x + TargetForward * Offset.z + Vector3.up * Offset.y; 

        transform.position = Vector3.MoveTowards(transform.position, TargetPosition, MaxCameraSpeed * Time.deltaTime);
        transform.LookAt(Target.position);
    }

    void UpdateRotation()
    {
        Gesture current = EasyTouch.current;

        // Twist
        if (current.type == EasyTouch.EvtType.On_Twist)
        {
            TwistToComplete += current.twistAngle;
        }

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
