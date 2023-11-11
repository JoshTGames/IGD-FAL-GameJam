using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using JoshGames.Character;

public class HandsToTarget : MonoBehaviour{
    Camera cam;
    [SerializeField] CharacterBase character;
    [SerializeField] Settings settings;
    [SerializeField] HandSettings[] hands;
    public HandSettings[] HANDS{
        get{ return hands; }
    }


    public float hasParentChangedDirection = 0; // This is set to the direction of the parent. If it changes, it'll teleport the hands to the desired position

    private void Start() {
        if(!character){ 
            Debug.LogError($"{transform.parent.parent.parent.name}: Needs to assign 'CharacterBase' class to 'HandsToTarget' script");
            return; 
        }
        cam = Camera.main;

        // Sets the default position of the hand so that the hand can always return back to default position
        foreach(HandSettings handData in hands){ 
            handData.offsetPos = handData.hand.transform.localPosition; 
            handData.offsetRot = handData.hand.transform.localRotation.eulerAngles.z; 
        }
    }

    private void FixedUpdate() {
        if(!character){ return; }
        foreach(HandSettings handData in hands){ 
            handData.hand.transform.localPosition = GetPosition(handData); 
            handData.hand.transform.localRotation = GetRotation(handData);
        }        
        hasParentChangedDirection = character.transform.localScale.x;
    }


    Vector2 GetPosition(HandSettings handData){
        Vector2 direction = (character.POSITION_OF_INTEREST - (Vector2)character.transform.position);
        direction.x *= hasParentChangedDirection;

        Vector2 targetPosition = (Vector2)Utilities.ClampMagnitude((handData.offsetPos * settings.offsetMultiplier) + direction.normalized, settings.distanceFromBody.min, settings.distanceFromBody.max);
        if(handData.overridePos != null){
            float overridePos = (float)handData.overridePos;
            targetPosition = (direction.normalized * overridePos);
        }
        return Vector2.SmoothDamp(handData.hand.transform.localPosition, targetPosition, ref handData.smoothVelocity, (character.transform.localScale.x != hasParentChangedDirection)? 0: settings.positionSmoothing);
    }

    Quaternion GetRotation(HandSettings handData){
        Vector2 direction = (character.POSITION_OF_INTEREST - (Vector2)character.transform.position);
        direction.x *= hasParentChangedDirection;

        float angle = (Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg) + handData.offsetRot;
        if(handData.overrideRot != null){
            float overrideRot = (float)handData.overrideRot;
            angle = (Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg) + handData.offsetRot + overrideRot;
        }
        angle = Mathf.SmoothDampAngle(handData.hand.transform.localRotation.eulerAngles.z, angle, ref handData.rotationSmoothVelocity, settings.rotationSmoothing);
        return Quaternion.AngleAxis(angle, Vector3.forward);
    }

    [Serializable] public class Settings{
        public Utilities.MinMax distanceFromBody = new Utilities.MinMax(0.1f, 0.5f);
        public float offsetMultiplier = 1f;
        [Range(0, 1)] public float positionSmoothing = 0.1f;
        [Range(0, 1)] public float rotationSmoothing = 0.1f;
    }

    [Serializable] public class HandSettings{
        public HandCollisionDetector hand;
        [HideInInspector] public Vector2 offsetPos;
        [HideInInspector] public float offsetRot;
        [HideInInspector] public Vector2 smoothVelocity;
        [HideInInspector] public float rotationSmoothVelocity;

        [HideInInspector] public float? overridePos;
        [HideInInspector] public float? overrideRot;
    }
}
