using JoshGames.Interaction;
using JoshGames.Character;
using JoshGames.AI.Sensors;
using UnityEngine;

/*
--- This code has has been written by Joshua Thompson (https://joshgames.co.uk) ---
        --- Copyright ©️ 2023-2024 Joshua Thompson. All Rights Reserved. ---
*/

public class Interact_Tear : InteractBase, IInteractable{
    [SerializeField] float soundRange = 10f;
    [SerializeField] AudioSource audioSource;

    public override InteractErr Interact(CharacterBase character){
        InteractErr err = base.Interact(character);
        if(err != InteractErr.SUCCESS){ return err; }
        
        new Sound(transform.position, soundRange, audioSource.clip).Create();
        audioSource.Play();

        CameraController cam = CameraController.instance;
        cam.Shake();
        cam.Pop();

        onInteract?.Invoke(character);
        isActive = false;
        Destroy(gameObject);
        return err;
    }

    private void OnDrawGizmosSelected() {
        Gizmos.DrawWireSphere(transform.position, soundRange);
    }
}
