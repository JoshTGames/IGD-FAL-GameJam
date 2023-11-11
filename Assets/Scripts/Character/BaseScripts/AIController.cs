using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using JoshGames.AI.BehaviourTree;
using JoshGames.AI.Sensors;

/*
--- This code has has been written by Joshua Thompson (https://joshgames.co.uk) ---
        --- Copyright ©️ 2023-2024 Joshua Thompson. All Rights Reserved. ---
*/
namespace JoshGames.Character.AI {
    public abstract class AIController : CharacterBase{
        [Header("AI Settings")]
        [SerializeField, Range(0.01f, 5.0f), Tooltip("This dictates how often the tree is checked (IN SECONDS)")] float treeUpdateFrequency = 0.5f;
        [HideInInspector] public CharacterBase target;
        [HideInInspector] public Vector2? heardSound = null;
        [HideInInspector] public float speed = 0;
        [HideInInspector] public Vector2 moveDirection, previousMoveDirection; // Used to change the direction the character will try to move to

        [SerializeField] LayerMask steeringObstacles;
        ContextSteering2D steering;
        BTNode rootNode;

        protected abstract BTNode CreateTree();

        protected override void Start(){
            base.Start();
            steering = new ContextSteering2D(steeringObstacles, 16, 2f, 1.25f, 5f);
            rootNode = CreateTree();
            InvokeRepeating("CheckTree", 0f, treeUpdateFrequency);
        }

        protected override void FixedUpdate(){
            base.FixedUpdate();
            Move(steering.Solve(objCollider.bounds.center, moveDirection), speed, profile.movementSettings.moveSmoothing);
            previousMoveDirection = (moveDirection != Vector2.zero)? moveDirection : previousMoveDirection;
        }

        bool checkTreeSuccess = true;
        /// <summary>
        /// Whenever this is called, the rootNode will be scanned for behaviours and reports its status
        /// </summary>
        void CheckTree(){
            if(!checkTreeSuccess){ return; }
            if(rootNode == null){
                Debug.LogWarning($"{this.name}: Doesn't have a rootNode!");
                checkTreeSuccess = false;
                return;
            }

            switch(rootNode.Evaluate()){
                case NodeState.Success:
                    OnRootSuccess();
                    break;
                case NodeState.Running:
                    OnRootRunning();
                    break;
                case NodeState.Fail:
                    OnRootFail();
                    break;
            }
        }

        /// <summary>
        /// When the root node reports 'success'
        /// </summary>
        protected virtual void OnRootSuccess(){}
        /// <summary>
        /// When the root node reports 'running'
        /// </summary>
        protected virtual void OnRootRunning(){}
        /// <summary>
        /// When the root node reports 'fail'
        /// </summary>
        protected virtual void OnRootFail(){}

        /// <summary>
        /// This updates the direction the character is facing
        /// </summary>
        /// <param name="position">The desired direction to face the character</param>
        public void UpdatePositionOfInterest(Vector2 position) => positionOfInterest = position;


        void OnDrawGizmos() {
            Gizmos.color = Color.yellow;
            if(objCollider){
                Gizmos.DrawRay(objCollider.bounds.center, (Vector3)moveDirection);
            }

            for(int i = 0; i < steering?.GET_DIRECTIONS.Length; i++){
                Gizmos.color = Color.green;
                Gizmos.DrawRay(objCollider.bounds.center, (Vector3)steering.GET_DIRECTIONS[i] * steering.GET_INTERESTS[i]);
                Gizmos.color = Color.red;
                Gizmos.DrawRay(objCollider.bounds.center, (Vector3)steering.GET_DIRECTIONS[i] * steering.GET_DANGER[i]);
            }
        }
    }
}