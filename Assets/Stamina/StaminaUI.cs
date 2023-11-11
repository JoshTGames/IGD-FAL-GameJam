using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using JoshGames.Character;
public class StaminaUI : MonoBehaviour{
    [Header("General Settings")]
    [SerializeField] Image staminaUIBar;
    [SerializeField] Image delayedUIBar;
    [SerializeField] Image backgroundUIBar;
    [SerializeField] float timeTillFadeOnceFull = 1, fadeDuration = 1;

    [Header("Delayed UI bar settings")]
    [SerializeField] float delayedUIBarSmoothing;
    float delayedSmoothingVelocity;

    [Header("Colour settings")]
    [SerializeField, Tooltip("This is the colour the bar will go to when the character is unable to use stamina")] Color drainedColour;
    Color normalColour, delayedNormalColour, backgroundNormalColour; // assigned on start



    float fadeTime;

    private void Start() {
        normalColour = staminaUIBar.color;
        delayedNormalColour = delayedUIBar.color;
        backgroundNormalColour = backgroundUIBar.color;

        staminaUIBar.color = staminaUIBar.color * new Color(1, 1, 1, 0);
        delayedUIBar.color = delayedUIBar.color * new Color(1, 1, 1, 0);
        backgroundUIBar.color = backgroundUIBar.color * new Color(1, 1, 1, 0);
    }

    // Update is called once per frame
    void Update(){
        PlayerController player = (PlayerController)WorldState.instance.player;
        float stamina = player.stamina;
        float percentFull = stamina / 100;
        staminaUIBar.fillAmount = percentFull;

        delayedUIBar.fillAmount = Mathf.SmoothDamp(delayedUIBar.fillAmount, staminaUIBar.fillAmount, ref delayedSmoothingVelocity, delayedUIBarSmoothing);

        if(percentFull >= 1 && Time.time >= fadeTime){
            staminaUIBar.color = Color.Lerp(staminaUIBar.color, staminaUIBar.color * new Color(1, 1, 1, 0), Time.deltaTime / fadeDuration);
            delayedUIBar.color = Color.Lerp(delayedUIBar.color, delayedUIBar.color * new Color(1, 1, 1, 0), Time.deltaTime / fadeDuration);
            backgroundUIBar.color = Color.Lerp(backgroundUIBar.color, backgroundUIBar.color * new Color(1, 1, 1, 0), Time.deltaTime / fadeDuration);
            return;
        }
        else if(percentFull < 1){ fadeTime = Time.time + timeTillFadeOnceFull; }
        staminaUIBar.color = (player.isStaminaDrained)? drainedColour : normalColour;
        delayedUIBar.color = delayedNormalColour;
        backgroundUIBar.color = backgroundNormalColour;
    }
}
