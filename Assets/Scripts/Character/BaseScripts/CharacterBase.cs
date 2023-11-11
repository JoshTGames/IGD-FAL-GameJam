using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using JoshGames.Interaction;

/*
--- This code has has been written by Joshua Thompson (https://joshgames.co.uk) ---
        --- Copyright ©️ 2023-2024 Joshua Thompson. All Rights Reserved. ---
*/

namespace JoshGames.Character{
    [RequireComponent(typeof(Rigidbody2D), typeof(Animator))]
    public abstract class CharacterBase : HealthBase{
        [SerializeField] protected CharacterProfile profile;
        
        #region Essential variables
        protected Rigidbody2D physicsAgent;
        protected Animator animator;
        protected Collider2D objCollider;
        [SerializeField] protected AudioSource footstepSFXManager;
        protected Velocity velocity;
        Vector2 smoothedPosition, smoothingVelocity;
        float timeTillBlink;
        #endregion
        
        #region  Interactable variables
        protected IInteractable selectedInteractable, lockedInteractable;
        protected InteractErr interactResult;
        [HideInInspector] public IInteractable SELECTED_INTERACTABLE{
            get{ return selectedInteractable; }
        }
        [HideInInspector] public float elapsedTriggerTime = 0;
        #endregion

        /// <summary>
        /// Used to dictate what the character is "looking at" and ultimately will change the direction the character is facing and other things
        /// </summary>
        protected Vector2 positionOfInterest = Vector2.zero; // This is accessed from higher scripts.

        /// <summary>
        /// Returns the 'positionOfInterest' variable
        /// </summary>
        /// <value>Used to dictate what the character is "looking at" and ultimately will change the direction the character is facing and other things</value>
        public Vector2 POSITION_OF_INTEREST{
            get{ return positionOfInterest; }
        }

        public float stunTimer; // If this exceeds Time.time then the character is unable to move

        protected override void Start() {
            base.Start();
            SetHealth(profile.health);
            physicsAgent = (physicsAgent)? physicsAgent : GetComponent<Rigidbody2D>();
            animator = (animator)? animator : GetComponent<Animator>();
            objCollider = (objCollider)? objCollider : GetComponent<Collider2D>();

            velocity = new Velocity(transform.position);
            timeTillBlink = Time.time + Random.Range(profile.blinkTime.min, profile.blinkTime.max);
        }


        protected virtual void Update() {
            ManageCharacterRotation();
            ManageAnimations();
            HandleInteraction();
        }

        protected virtual void FixedUpdate() {
            velocity.CalculateVelocity(transform.position);
            SearchForInteractables();
        }

        /// <summary>
        /// This function handles movement of the character
        /// </summary>
        /// <param name="direction">The direction the character desired to go</param>
        /// <param name="speed">The speed at which the character will travel</param>
        /// <param name="smoothing">The acceleration/deceleration of the character</param>
        protected virtual void Move(Vector2 direction, float? speed = null, float? smoothing = null){
            if(Time.time < stunTimer){ return; }
            float newSpeed = (speed == null)? profile.movementSettings.walkSpeed : (float)speed; 
            float newSmoothing = (smoothing == null)? profile.movementSettings.moveSmoothing : (float)smoothing; 

            smoothedPosition = Vector2.SmoothDamp(smoothedPosition, direction * newSpeed, ref smoothingVelocity, newSmoothing);

            physicsAgent.MovePosition((Vector2)transform.position + smoothedPosition);
        }
        
        /// <summary>
        /// This function handles animations on the character and some of the maths required to play the animations correctly when character is rotated
        /// </summary>
        protected virtual void ManageAnimations(){
            animator.SetBool("IsMoving", (profile.movementSettings.walkSpeed - velocity.VALUE.magnitude) < profile.movementSettings.animationVelocityDelta); // Sets bool to true if the character is moving            
            animator.SetFloat("Speed", Utilities.Remap(velocity.VALUE.magnitude, profile.movementSettings.walkSpeed, profile.movementSettings.runSpeed, 1f, 2f));
            int roundedDirection =  Mathf.RoundToInt(velocity.VALUE.normalized.x);

            bool isMovingOnlyVertical = roundedDirection == 0 && velocity.VALUE.normalized.y != 0; // If character is only moving on Y axis
            animator.SetFloat("Direction", (isMovingOnlyVertical)? 1 : transform.localScale.x * -roundedDirection);

            if(Time.time >= timeTillBlink && profile.blinkTime.max >0){
                timeTillBlink = Time.time + Random.Range(profile.blinkTime.min, profile.blinkTime.max);
                animator.SetTrigger("Blink");
            }
        }

        /// <summary>
        /// If the target position is on either side of the character, it'll make the character rotate to look at that position
        /// </summary>
        void ManageCharacterRotation(){
            Vector3 direction = (positionOfInterest - (Vector2)transform.position).normalized;
            int xDirection = Mathf.RoundToInt(direction.x);
            if(xDirection == 0){ return; }
            transform.localScale = new Vector2(xDirection, transform.localScale.y);
        }

        /// <summary>
        /// This handles what should happen for some of the interact errors
        /// </summary>
        protected virtual void HandleInteraction(){
            if(Time.time < stunTimer){ return; }
            if(lockedInteractable != null && selectedInteractable == lockedInteractable){ interactResult = lockedInteractable.QueryInteractable(this); }
            else{ 
                interactResult = InteractErr.NONE; 
                lockedInteractable = null;
            }

            switch(interactResult){
                case InteractErr.SUCCESS:
                    lockedInteractable.Interact(this);
                    break;
                case InteractErr.ERR_WAITING_FOR_TRIGGER_TIME:
                    elapsedTriggerTime += Time.deltaTime;
                    break;
                default:
                    elapsedTriggerTime = 0;
                    break;
            }
        }

        /// <summary>
        /// Scans for proximity of the character and the "position of interest" to get an angle to see if there is an interactable in view
        /// </summary>
        protected virtual void SearchForInteractables(){
            if(Time.time < stunTimer){ return; }
            List<Transform> interactables = profile.interactSettings.Search(transform.position, (positionOfInterest - (Vector2)transform.position).normalized);
            Transform selectedTransform = null;            
            foreach(Transform t in interactables){
                if(selectedTransform && (t.position - transform.position).magnitude > (selectedTransform.position - transform.position).magnitude){ continue; }
                selectedTransform = t;
            }
            selectedInteractable = selectedTransform?.GetComponent<InteractBase>();
        }

        /// <summary>
        /// Plays a random footstep noise when called
        /// </summary>
        public virtual void PlayFootstepNoise(){
            footstepSFXManager.clip = profile.footsteps[Random.Range(0, profile.footsteps.Length -1)];
            footstepSFXManager.Play();
        }

        /// <summary>
        /// This class allows
        /// </summary>
        public class Velocity{
            public Vector3 previousPosition;
            Vector3 velocity;

            public Vector3 VALUE{
                get{ return velocity; }
            }

            /// <summary>
            /// Calculates the position by subtracting the passed position from the previous position
            /// </summary>
            /// <param name="position">The concurrent position</param>
            /// <returns></returns>
            public Vector3 CalculateVelocity(Vector3 position){
                velocity = previousPosition - position;
                previousPosition = position;
                return velocity;
            }

            /// <summary>
            /// This is a constructor for the Velocity class
            /// </summary>
            /// <param name="initialPosition">The position to start the velocity calculation</param>
            public Velocity(Vector3 initialPosition) => this.previousPosition = initialPosition;
        }
    
        private void OnDrawGizmos() {            
            if(profile.interactSettings.DEBUG_SETTINGS.doDebug){
                Gizmos.color = profile.interactSettings.DEBUG_SETTINGS.debugColour;
                Gizmos.DrawWireSphere(transform.position, profile.interactSettings.VIEW_DISTANCE);
            }
        }
    }
}