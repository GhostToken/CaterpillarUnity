using System.Collections;
using System.Collections.Generic;
using UnityEngine.Events;
using UnityEngine;

public class MapControls : MonoBehaviour
{
    public Vector3 MinBounds;
    public Vector3 MaxBounds;

    public Vector3 MinDragBounds;
    public Vector3 MaxDragBounds;
    public float TimeToGetBackInBounds;

    private Coroutine MagneticEffect;

    public void DragTo(Vector3 NewPosition)
    {
        transform.position = NewPosition;

        Vector3 NewLocalPosition = transform.localPosition;
        NewLocalPosition.x = Mathf.Clamp(NewLocalPosition.x, MinDragBounds.x, MaxDragBounds.x);
        NewLocalPosition.y = Mathf.Clamp(NewLocalPosition.y, MinDragBounds.y, MaxDragBounds.y);
        NewLocalPosition.z = Mathf.Clamp(NewLocalPosition.z, MinDragBounds.z, MaxDragBounds.z);
        transform.localPosition = NewLocalPosition;
    }

    public void BeginDrag()
    {
        if(MagneticEffect != null)
        {
            StopCoroutine(MagneticEffect);
        }
    }

    public void EndDrag()
    {
        MagneticEffect = StartCoroutine(UpdateMagneticPosition());
    }

    public IEnumerator UpdateMagneticPosition()
    {
        float StartTime = Time.time;

        Vector3 StartPositon = transform.localPosition;
        Vector3 TargetPositon = StartPositon;
        TargetPositon.x = Mathf.Clamp(TargetPositon.x, MinBounds.x, MaxBounds.x);
        TargetPositon.y = Mathf.Clamp(TargetPositon.y, MinBounds.y, MaxBounds.y);
        TargetPositon.z = Mathf.Clamp(TargetPositon.z, MinBounds.z, MaxBounds.z);

        while (Time.time < StartTime + TimeToGetBackInBounds)
        {
            float Progress = (Time.time - StartTime) / TimeToGetBackInBounds;
            transform.localPosition = Vector3.Lerp(StartPositon, TargetPositon, Progress);
            yield return null;
        }

        MagneticEffect = null;
    }
}
