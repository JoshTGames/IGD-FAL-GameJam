using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using JoshGames.Character;
using JoshGames.Interaction;

/*
--- This code has has been written by Joshua Thompson (https://joshgames.co.uk) ---
        --- Copyright ©️ 2023-2024 Joshua Thompson. All Rights Reserved. ---
*/

public class InteractUI : MonoBehaviour{
    Camera cam;
    
    [SerializeField] Image fillProgress;
    [SerializeField] TextMeshProUGUI objectName, actionName;
    [SerializeField] float offsetSensitivity = 1; 
    [SerializeField] float motionSmoothing = 0.1f;
    Vector2 smoothVelocity;

    private void Start() => cam = Camera.main;

    private void Update() {
        CharacterBase player = WorldState.instance.player;

        InteractData data = player.SELECTED_INTERACTABLE?.GetInteractData();
        transform.GetChild(0).gameObject.SetActive(data != null);
        
        if(data == null){ return; }

        fillProgress.fillAmount = player.elapsedTriggerTime / data.triggerTime;
        objectName.text = data.objectName;
        actionName.text = data.actionName;

        InterpolatePosition(data);        
    }

    void InterpolatePosition(InteractData data){
        Vector2 directionToScreenCenter = cam.ViewportToScreenPoint(new Vector3(0.5f, 0.5f));
        Vector2 interactScreenPos = cam.WorldToScreenPoint(data.obj.position);
        Vector2 newPos = interactScreenPos + (directionToScreenCenter + interactScreenPos).normalized * offsetSensitivity;
        transform.position = Vector2.SmoothDamp(transform.position, newPos, ref smoothVelocity, motionSmoothing);
    }
}
