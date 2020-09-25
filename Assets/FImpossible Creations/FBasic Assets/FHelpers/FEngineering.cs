using UnityEngine;

/// <summary>
/// FM: Class which contains many helpful methods which operates on Vectors and Quaternions or some other floating point maths
/// </summary>
public static class FEngineering
{


    #region Rotations and directions


    public static bool VIsZero(Vector3 vec)
    {
        if (vec.sqrMagnitude == 0f) return true; return false;
        //if (vec.x != 0f) return false; if (vec.y != 0f) return false; if (vec.z != 0f) return false; return true;
    }

    public static bool VIsSame(Vector3 vec1, Vector3 vec2)
    {
        if (vec1.x != vec2.x) return false; if (vec1.y != vec2.y) return false; if (vec1.z != vec2.z) return false; return true;
    }


    public static Vector3 TransformVector(Quaternion parentRot, Vector3 parentLossyScale, Vector3 childLocalPos)
    {
        return parentRot * Vector3.Scale(childLocalPos, parentLossyScale);
    }

    public static Vector3 InverseTransformVector(Quaternion tRotation, Vector3 tLossyScale, Vector3 worldPos)
    {
        worldPos = Quaternion.Inverse(tRotation) * worldPos;
        return new Vector3(worldPos.x / tLossyScale.x, worldPos.y / tLossyScale.y, worldPos.z / tLossyScale.z);
    }

    #endregion


    #region Just Rotations related

    /// <summary>
    /// Locating world rotation in local space of parent transform
    /// </summary>
    public static Quaternion QToLocal(Quaternion parentRotation, Quaternion worldRotation)
    {
        return Quaternion.Inverse(parentRotation) * worldRotation;
    }

    /// <summary>
    /// Locating local rotation of child local space to world
    /// </summary>
    public static Quaternion QToWorld(Quaternion parentRotation, Quaternion localRotation)
    {
        return parentRotation * localRotation;
    }

    /// <summary>
    /// Offsetting rotation of child transform with defined axis orientation
    /// </summary>
    public static Quaternion QRotateChild(Quaternion offset, Quaternion parentRot, Quaternion childLocalRot)
    {
        return (offset * parentRot) * childLocalRot;
    }

    public static Quaternion ClampRotation(Vector3 current, Vector3 bounds)
    {
        WrapVector(current);

        if (current.x < -bounds.x) current.x = -bounds.x; else if (current.x > bounds.x) current.x = bounds.x;
        if (current.y < -bounds.y) current.y = -bounds.y; else if (current.y > bounds.y) current.y = bounds.y;
        if (current.z < -bounds.z) current.z = -bounds.z; else if (current.z > bounds.z) current.z = bounds.z;

        return Quaternion.Euler(current);
    }


    public static bool QIsZero(Quaternion rot)
    {
        if (rot.x != 0f) return false; if (rot.y != 0f) return false; if (rot.z != 0f) return false; return true;
    }

    public static bool QIsSame(Quaternion rot1, Quaternion rot2)
    {
        if (rot1.x != rot2.x) return false; if (rot1.y != rot2.y) return false; if (rot1.z != rot2.z) return false; if (rot1.w != rot2.w) return false; return true;
    }


    /// <summary> Wrapping angle (clamping in +- 360) </summary>
    public static float WrapAngle(float angle)
    {
        angle %= 360;
        if (angle > 180) return angle - 360;
        return angle;
    }

    public static Vector3 WrapVector(Vector3 angles)
    { return new Vector3(WrapAngle(angles.x), WrapAngle(angles.y), WrapAngle(angles.z)); }

    /// <summary> Unwrapping angle </summary>
    public static float UnwrapAngle(float angle)
    {
        if (angle >= 0) return angle;
        angle = -angle % 360;
        return 360 - angle;
    }

    public static Vector3 UnwrapVector(Vector3 angles)
    { return new Vector3(UnwrapAngle(angles.x), UnwrapAngle(angles.y), UnwrapAngle(angles.z)); }


    #endregion


    #region Animation Related


    public static Quaternion SmoothDampRotation(Quaternion current, Quaternion target, ref Quaternion velocityRef, float duration, float delta)
    {
        return SmoothDampRotation(current, target, ref velocityRef, duration, Mathf.Infinity, delta);
    }

    public static Quaternion SmoothDampRotation(Quaternion current, Quaternion target, ref Quaternion velocityRef, float duration, float maxSpeed, float delta)
    {
        float dot = Quaternion.Dot(current, target);
        float sign = dot > 0f ? 1f : -1f;
        target.x *= sign;
        target.y *= sign;
        target.z *= sign;
        target.w *= sign;

        Vector4 smoothVal = new Vector4(
            Mathf.SmoothDamp(current.x, target.x, ref velocityRef.x, duration, maxSpeed, delta),
            Mathf.SmoothDamp(current.y, target.y, ref velocityRef.y, duration, maxSpeed, delta),
            Mathf.SmoothDamp(current.z, target.z, ref velocityRef.z, duration, maxSpeed, delta),
            Mathf.SmoothDamp(current.w, target.w, ref velocityRef.w, duration, maxSpeed, delta)).normalized;

        Vector4 correction = Vector4.Project(new Vector4(velocityRef.x, velocityRef.y, velocityRef.z, velocityRef.w), smoothVal);
        velocityRef.x -= correction.x;
        velocityRef.y -= correction.y;
        velocityRef.z -= correction.z;
        velocityRef.w -= correction.w;

        return new Quaternion(smoothVal.x, smoothVal.y, smoothVal.z, smoothVal.w);
    }


    #endregion



    #region Helper Maths

    /// <summary>
    /// Inverse Lerp without clamping
    /// </summary>
    public static float InverseLerp(float from, float to, float value)
    {
        if (to != from) // Prevent from dividing by zero
            return Mathf.Clamp((value - from) / (to - from), -1f, 1f);

        return 0;
    }


    public static float HyperCurve(float value)
    {
        return -(1f / (3.2f * value - 4)) - 0.25f;
    }


    #endregion



    #region Matrixes


    /// <summary>
    /// Extracting position from Matrix
    /// </summary>
    public static Vector3 PosFromMatrix(Matrix4x4 m)
    {
        return m.GetColumn(3);
    }

    /// <summary>
    /// Extracting rotation from Matrix
    /// </summary>
    public static Quaternion RotFromMatrix(Matrix4x4 m)
    {
        return Quaternion.LookRotation(m.GetColumn(2), m.GetColumn(1));
    }

    /// <summary>
    /// Extracting scale from Matrix
    /// </summary>
    public static Vector3 ScaleFromMatrix(Matrix4x4 m)
    {
        return new Vector3
        (
            m.GetColumn(0).magnitude,
            m.GetColumn(1).magnitude,
            m.GetColumn(2).magnitude
        );
    }


    #endregion


}
