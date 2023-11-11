using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using JoshGames.Character;
using JoshGames.AI.Sensors;

/*
--- This code has has been written by Joshua Thompson (https://joshgames.co.uk) ---
        --- Copyright ©️ 2023-2024 Joshua Thompson. All Rights Reserved. ---
*/

public class PlayerController : CharacterBase{
    Camera cam;
    bool tryingToRun = false;
    bool isRunning = false;
    [SerializeField] GameObject bloodFX;
    [SerializeField] float footstepNoiseRadius;

    [Header("Stamina settings")]
    [HideInInspector] public float stamina = 100;
    [SerializeField] float timeTillStaminaDrain = 3f, timeTillStaminaFull = 5f;
    [SerializeField] float timeTillStaminaGenAfterDrain = 1;
    [HideInInspector] public bool isStaminaDrained;
    float staminaDrainedTimer;

    Vector2 moveDirection; // Used to change the direction the player will try to move to
    protected override void Start() {
        base.Start();
        cam = Camera.main;
        CameraController.instance.target = transform;   
    }

    protected override void Update() {
        base.Update();
        positionOfInterest = (Vector2)cam.ScreenToWorldPoint(Input.mousePosition);        
    }
    
    protected override void FixedUpdate() {
        base.FixedUpdate();
        isRunning = (tryingToRun && moveDirection != Vector2.zero && !isStaminaDrained);
        switch(isRunning){
            case true:
                AffectStaminaOverTime(-100, timeTillStaminaDrain);
                break;
            case false:
                AffectStaminaOverTime(100, timeTillStaminaFull);
                break;
        }

        CharacterProfile.MoveSettings settings = profile.movementSettings;
        Move(moveDirection, isRunning? settings.runSpeed : settings.walkSpeed, settings.moveSmoothing);
    }
        
    public void OnMove(InputValue value) => moveDirection = value.Get<Vector2>();
    public void OnRun(InputValue value) => tryingToRun = (value.Get<float>() == 1)? true: false;
    public void OnInteract(InputValue value) => lockedInteractable = (value.Get<float>() == 1)? selectedInteractable : null;

    /// <summary>
    /// This is a base function which will slowely affect the stamina by generating/draining per second
    /// </summary>
    /// <param name="amount">The amount to be affected the stamina per second</param>
    void AffectStaminaOverTime(float amount, float duration = 1){
        if(Time.time < staminaDrainedTimer){ return; }

        float amountPerDeltaTime = amount * Time.fixedDeltaTime / duration;
        stamina += amountPerDeltaTime;
        stamina = Mathf.Clamp(stamina, 0, 100);

        if(stamina <= 0){ 
            staminaDrainedTimer = Time.time + timeTillStaminaGenAfterDrain; 
            isStaminaDrained = true;
        }
        else if(stamina >= 100){ isStaminaDrained = false; }
    }

    protected override void OnDamage(){
        cam.GetComponent<CameraController>().Pop();
        Instantiate(bloodFX, transform.position, Quaternion.identity, transform).GetComponent<ParticleSystem>().Play();
    }

    public override void PlayFootstepNoise(){
        base.PlayFootstepNoise();
        new Sound(transform.position, footstepNoiseRadius).Create();
    }

    private void OnDrawGizmosSelected() {
        Gizmos.DrawWireSphere(transform.position, footstepNoiseRadius);
    }
}
