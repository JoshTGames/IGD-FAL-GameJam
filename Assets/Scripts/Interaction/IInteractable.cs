using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using JoshGames.Character;

/*
--- This code has has been written by Joshua Thompson (https://joshgames.co.uk) ---
        --- Copyright ©️ 2023-2024 Joshua Thompson. All Rights Reserved. ---
*/

namespace JoshGames.Interaction{
    public interface IInteractable{
        /// <summary>
        /// This is used to check to see if the interactable can even be interacted with, it'll be called immediately in the 'Interact' function.
        /// </summary>
        /// <param name="character">This is the character that is trying to interact with the object.</param>
        /// <returns>An error value (which could be success)</returns>
        InteractErr QueryInteractable(CharacterBase character);
        InteractErr Interact(CharacterBase character);


        InteractData GetInteractData();
    }

    public class InteractData{
        public bool isActive;
        public Transform obj;
        public string objectName, actionName;
        public float triggerTime;
        public float interactRange;
        public InteractData(bool isActive, Transform obj, string objectName, string actionName, float triggerTime, float interactRange){
            this.isActive = isActive;
            this.obj = obj;
            this.objectName = objectName;
            this.actionName = actionName;
            this.triggerTime = triggerTime;
            this.interactRange = interactRange;
        }
    }

    /// <summary>
    /// Allows us to return various errors or success to anything which is trying to interact with an object
    /// </summary>
    public enum InteractErr{
        NONE,
        SUCCESS,
        ERR_NOT_ACTIVE,
        ERR_NOT_IN_RANGE,
        ERR_WAITING_FOR_TRIGGER_TIME
    }
}