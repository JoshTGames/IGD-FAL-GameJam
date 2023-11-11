using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.SceneManagement;
using TMPro;
using JoshGames.Character;

/*
--- This code has has been written by Joshua Thompson (https://joshgames.co.uk) ---
        --- Copyright ©️ 2023-2024 Joshua Thompson. All Rights Reserved. ---
*/

public class GameController : MonoBehaviour{
    [Header("UI Settings")]
    [SerializeField] GameObject statePanel;
    [SerializeField] TextMeshProUGUI uiTitle;
    [SerializeField] GameState winState, loseState;

    [Header("Game Settings")]
    [SerializeField] Transform tearFolder;
    [SerializeField] Characters characters;
    [SerializeField, Tooltip("The min/max radius the Wendeergo will search within based on the remaining tears in the level")] Utilities.MinMax playerSearchRadius;

    [Header("Audio Settings")]
    [SerializeField] AudioSource mainSource;
    [SerializeField] AudioSource chaseSource;
    [SerializeField, Tooltip("Changes in volume based on how close the wendeergo is to player")] Utilities.MinMax mainSourceVolume = new Utilities.MinMax(0.25f, 1f);
    [SerializeField, Tooltip("Changes in volume based on how close the wendeergo is to player")] Utilities.MinMax distanceFromPlayer = new Utilities.MinMax(3f, 10f);
    [SerializeField, Tooltip("In seconds to change the soundTracks")] float transitionSmoothing = 1f;
    float mainSourceTransitionVelocity, chaseSourceTransitionVelocity;

    private void Start() {
        // Generate map - Tears

        // Spawn player
        CharacterBase _player = Instantiate(characters.player.gameObject, Vector2.zero, Quaternion.identity).GetComponent<CharacterBase>();

        // Spawn wendeergo
        CharacterBase _wendeergo = Instantiate(characters.enemy.gameObject, new Vector2(-1,-1) * 25f, Quaternion.identity).GetComponent<CharacterBase>();
        
        new WorldState(_player, _wendeergo, playerSearchRadius, tearFolder);
    }

    private void Update() {
        ManageGameStateResult();
        ManageAudio();
    }

    /// <summary>
    /// Stops the time and plays a selected win/lose state screen based on if the game has "finished"
    /// </summary>
    void ManageGameStateResult(){
        bool hasWon = WorldState.instance.numberOfTears <= 0;
        bool hasDied = WorldState.instance.player == null;

        Time.timeScale = (hasWon || hasDied)? 0 : 1;
        statePanel.SetActive(hasWon || hasDied);
        uiTitle.text = (hasWon)? winState.text : loseState.text;
        uiTitle.color = (hasWon)? winState.stateColour : loseState.stateColour;
    }

    void ManageAudio(){
        if(!WorldState.instance.player){ return; }

        Vector3 playerPos = WorldState.instance.player.transform.position;
        Vector3 wendeergoPos = WorldState.instance.wendeergo.transform.position;
        // Get distance between the 2 characters and reverse the 0-1 remapped output as we will need this to be louder the closer to the player the enemy is
        float distance = 1 - Utilities.Remap((wendeergoPos - playerPos).magnitude, distanceFromPlayer.min, distanceFromPlayer.max, 0, 1);
        float mainSourceTargetVolume = (WorldState.instance.isBeingChased)? 0 : Utilities.Remap(distance, 0, 1, mainSourceVolume.min, mainSourceVolume.max);

        mainSource.volume = Mathf.SmoothDamp(mainSource.volume, mainSourceTargetVolume, ref mainSourceTransitionVelocity, transitionSmoothing);
        chaseSource.volume = Mathf.SmoothDamp(chaseSource.volume, (WorldState.instance.isBeingChased)? 1 : 0, ref chaseSourceTransitionVelocity, transitionSmoothing);
    }


    public void Replay() => SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    public void Quit() => Application.Quit();

    [Serializable] public class Characters{
        public CharacterBase player;
        public CharacterBase companion;
        public CharacterBase enemy;
    }

    [Serializable] public class GameState{
        public Color32 stateColour = Color.green;
        public string text = "YOU SURVIVED!";
    }
}
