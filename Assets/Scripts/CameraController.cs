using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using JoshGames.Interaction;
using JoshGames.Character;

/*
--- This code has has been written by Joshua Thompson (https://joshgames.co.uk) ---
        --- Copyright ©️ 2023-2024 Joshua Thompson. All Rights Reserved. ---
*/

public class CameraController : MonoBehaviour{
    Camera cam;
    public static CameraController instance;
    public Transform target;
    [SerializeField] float positionSmoothing = 0.1f;
    [SerializeField] float peakDistance = 0;
    [SerializeField] float bobbingFrequency, bobbingAmplitude;
    [SerializeField] float clampDistance = 40f;
    [SerializeField] ShakeSettings shakeSettings;
    [SerializeField] PopSettings popSettings;
    Vector2 positionVelocity;

    float bobbingTimer;
    float shakeTimer;
    float shakeTime, popTime;
    float defaultZoom;

    float targetFrequency, targetAmplitude;

    Vector2 direction;

    void Start(){
        // Create singleton
        if(instance){
            Destroy(this.gameObject);
            return;
        }
        instance = this;
        cam = Camera.main;
        defaultZoom = cam.orthographicSize;
    }

    public void Shake(float? duration = null) => shakeTime = Time.time + ((duration == null)? shakeSettings.shakeTime : (float)duration);
    public void Pop() => popTime = Time.time + popSettings.popTime;

    void LateUpdate(){
        if(!target){ return; }

        Vector2 mousePos = cam.ScreenToWorldPoint(Input.mousePosition);
        direction = mousePos - (Vector2)target.position;
        Vector2 newPos = (Vector2)target.position + direction.normalized * Mathf.Clamp(direction.magnitude, 0, peakDistance) * 0.5f;

        


        bobbingTimer += bobbingFrequency * Time.deltaTime;
        shakeTimer += shakeSettings.shakeBobbingFrequency * Time.deltaTime;
        Vector2 cameraBobble = new Vector2(Mathf.Cos(bobbingTimer / 2) * bobbingAmplitude, Mathf.Sin(bobbingTimer) * bobbingAmplitude); // NEEDS TOUCHING UP
        Vector2 cameraShake = (Time.time < shakeTime)? new Vector2(Mathf.Cos(shakeTimer) * shakeSettings.shakeBobbingAmplitude, Mathf.Sin(bobbingTimer) * bobbingAmplitude / 2) : Vector2.zero;
        cam.orthographicSize = Mathf.SmoothDamp(cam.orthographicSize, (Time.time < popTime)? popSettings.newFOV : defaultZoom, ref popSettings.popVelocity, popSettings.popSmoothing);

        Vector3 modifiedPosition = new Vector3(0, 0, transform.position.z) + (Vector3)Vector2.SmoothDamp(transform.position, newPos + cameraBobble + cameraShake, ref positionVelocity, positionSmoothing);

        // Clamping
        float camSizeX = cam.orthographicSize * cam.aspect;
        float camSizeY = cam.orthographicSize;        
        Vector3 clampedPos = GetClampedPos(modifiedPosition, Vector3.zero, new Vector3(clampDistance, clampDistance), camSizeX, camSizeY);
        clampedPos.z = -10;

        transform.position = clampedPos;
    }

    /// <summary>
    /// This function will clamp a position inside the boundaries provided and return the new position
    /// </summary>
    /// <param name="targetPosition">The position to be clamped</param>
    /// <param name="boundsPosition">The center position of the boundaries</param>
    /// <param name="boundsSize">Half the size of the boundaries</param>
    /// <param name="offsetX">Offset appended onto the boundaries e.g. Camera size</param>
    /// <param name="offsetY">Offset appended onto the boundaries e.g. Camera size</param>
    /// <param name="offsetZ">Offset appended onto the boundaries e.g. Camera size</param>
    /// <returns>A position clamped inside the boundaries</returns>
    Vector3 GetClampedPos(Vector3 targetPosition, Vector3 boundsPosition, Vector3 boundsSize, float offsetX = 0, float offsetY = 0, float offsetZ = 0){
        Vector3 clampedPos = new Vector3(
            Mathf.Clamp(targetPosition.x, (boundsPosition.x - boundsSize.x) + offsetX, (boundsPosition.x + boundsSize.x) - offsetX),
            Mathf.Clamp(targetPosition.y, (boundsPosition.y - boundsSize.y) + offsetY, (boundsPosition.y + boundsSize.y) - offsetY),
            Mathf.Clamp(targetPosition.z, (boundsPosition.z - boundsSize.z) + offsetZ, (boundsPosition.z + boundsSize.z) - offsetZ)
        );
        return clampedPos;
    }


    [System.Serializable] public class ShakeSettings{
        public float shakeBobbingFrequency = 50;
        public float shakeBobbingAmplitude = 1;
        public float shakeTime = 3;
    }
    [System.Serializable] public class PopSettings{
        public float newFOV = 5;
        public float popTime = 3;
        public float popSmoothing = 0.05f;
        [HideInInspector] public float popVelocity;
    }
}
