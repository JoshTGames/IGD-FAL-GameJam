using UnityEngine;
using UnityEngine.Events;
using JoshGames.Character;

/*
--- This code has has been written by Joshua Thompson (https://joshgames.co.uk) ---
        --- Copyright ©️ 2023-2024 Joshua Thompson. All Rights Reserved. ---
*/

namespace JoshGames.Interaction{
    public abstract class InteractBase : MonoBehaviour, IInteractable{
        [SerializeField, Tooltip("If false, this object will not be able to be interacted with")] protected bool isActive = true;
        [SerializeField, Tooltip("The name of the object")] protected string objectName;
        [SerializeField, Tooltip("The name of the action")] protected string actionName;
        [SerializeField, Tooltip("Time it takes to interact with the object")] protected float triggerTime = 1.0f;   
        [SerializeField, Tooltip("Range for the character to be in order to interact with the object")] protected float interactRange = 0.5f;
        [SerializeField, Tooltip("Allows for extra functionality")] protected UnityEvent<CharacterBase> onInteract;


        private void OnDrawGizmosSelected() => Gizmos.DrawWireSphere(transform.position, interactRange);


        public virtual InteractErr Interact(CharacterBase character){
            InteractErr err = QueryInteractable(character);
            if(err != InteractErr.SUCCESS){ return err; }
            return InteractErr.SUCCESS;
        }

        public virtual InteractErr QueryInteractable(CharacterBase character){
            if(!isActive){ return InteractErr.ERR_NOT_ACTIVE; }
            else if((character.transform.position - transform.position).magnitude > interactRange){ return InteractErr.ERR_NOT_IN_RANGE; }
            else if(character.elapsedTriggerTime < triggerTime){ return InteractErr.ERR_WAITING_FOR_TRIGGER_TIME; }
            return InteractErr.SUCCESS;
        }

        public InteractData GetInteractData() => (isActive)? new InteractData(isActive, transform, objectName, actionName, triggerTime, interactRange) : null;
    }
}