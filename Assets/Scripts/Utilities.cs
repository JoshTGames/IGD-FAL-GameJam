using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;

public static class Utilities{
    /// <summary>
    /// Remaps an input value from the fromMin to fromMax range and converts it and maps it to a new set of values between toMin and toMax
    /// </summary>
    /// <param name="inputValue">The value to be modified</param>
    /// <param name="fromMin">The minimum possible raw value</param>
    /// <param name="fromMax">The maximum possible raw value</param>
    /// <param name="toMin">The minimum new value</param>
    /// <param name="toMax">The maximum new value</param>
    /// <returns>The new 'inputValue'</returns>
    public static float Remap(float inputValue, float fromMin, float fromMax, float toMin, float toMax){
        float i = (((inputValue - fromMin) / (fromMax - fromMin)) * (toMax - toMin) + toMin);
        i = Mathf.Clamp(i, toMin, toMax);
        return i;
    }    

    /// <summary>
    /// Same as Vector3.SmoothDamp(), but works for Quaternions
    /// </summary>
    /// <param name="current">The current rotation</param>
    /// <param name="target">The rotation to reach</param>
    /// <param name="currentVelocity">A reference to a velocity variable</param>
    /// <param name="smoothTime">Changes how responsive </param>
    /// <returns>A interpolated value</returns>
    public static Quaternion SmoothDampQuaternion(Quaternion current, Quaternion target, ref Vector3 currentVelocity, float smoothTime){
        Vector3 c = current.eulerAngles;
        Vector3 t = target.eulerAngles;
        return Quaternion.Euler(
            Mathf.SmoothDampAngle(c.x, t.x, ref currentVelocity.x, smoothTime),
            Mathf.SmoothDampAngle(c.y, t.y, ref currentVelocity.y, smoothTime),
            Mathf.SmoothDampAngle(c.z, t.z, ref currentVelocity.z, smoothTime)
        );
    }

    /// <summary>
    /// This function clamps the magnitude of a vector between a min and max value
    /// </summary>
    /// <param name="v">The vector value to clamp</param>
    /// <param name="min">The min length of the vector</param>
    /// <param name="max">The max length of the vector</param>
    /// <returns>The clamped vector positioned inbetween the min and max value</returns>
    public static Vector3 ClampMagnitude(Vector3 v, float min, float max){
        float sm = v.sqrMagnitude;        
        if(sm > max * max){ return v.normalized * max; }
        else if(sm < min * min){ return v.normalized * min; }
        
        return v;
    }
    
    /// <summary>
    /// Clamps an angle between 0,360
    /// </summary>
    /// <param name="angle">Current angle</param>
    /// <param name="min">Min angle</param>
    /// <param name="max">Max angle</param>
    /// <returns>The clamped angle</returns>
    public static float clampAngle(float angle, float min, float max){
        if (angle > 180f) { angle -= 360; }
        angle = Mathf.Clamp(angle, min, max);

        if (angle < 0f) { angle += 360; }
        return angle;
    }
    
    
    [Serializable] public class MinMax{
        public float min, max;

        public MinMax(float min, float max){
            this.min = min;
            this.max = max;
        }
    }
}
