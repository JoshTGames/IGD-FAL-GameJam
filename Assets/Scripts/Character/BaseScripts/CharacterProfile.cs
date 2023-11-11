using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using JoshGames.AI.Sensors;

/*
--- This code has has been written by Joshua Thompson (https://joshgames.co.uk) ---
        --- Copyright ©️ 2023-2024 Joshua Thompson. All Rights Reserved. ---
*/

namespace JoshGames.Character{
    [CreateAssetMenu(fileName = "New Character Profile", menuName = "ScriptableObjects/Character/New Profile")]
    public class CharacterProfile : ScriptableObject{
        [Header("Character Stats")]
        [Tooltip("Max health the character has")] public float health = 100.0f;
        [Tooltip("Stamina the character has")] public float stamina = 100.0f;
        public MoveSettings movementSettings;

        [Header("Interact Settings")]
        [Tooltip("Used to find interactable objects")] public Sight interactSettings;
        [Header("SFX settings")]
        public AudioClip[] footsteps;
        
        [Header("Animation Settings")]
        public Utilities.MinMax blinkTime = new Utilities.MinMax(5, 10);
        
        [Serializable] public class MoveSettings{
            [Tooltip("Walk speed of the character")] public float walkSpeed = 10.0f;
            [Tooltip("Run speed of the character")] public float runSpeed = 15.0f;
            [Tooltip("Acceleration/Deceleration: Smaller the value, quicker it is to reach desired speed")] public float moveSmoothing = 0.1f;
            [Tooltip("This dictates the velocity required to trigger the movement animation")] public float animationVelocityDelta = 0.04f;
        }
    }
}