using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using JoshGames.Character;
using JoshGames.Character.AI;
using JoshGames.AI.BehaviourTree;
using JoshGames.AI.Sensors;

/*
--- This code has has been written by Joshua Thompson (https://joshgames.co.uk) ---
        --- Copyright ©️ 2023-2024 Joshua Thompson. All Rights Reserved. ---
*/

public class WendeergoController : AIController, IHear{
    [SerializeField] Blackboard blackboard;

    /// <summary>
    /// For editor: So that we can see what the wendeergo sees
    /// </summary>
    /// <value>A sight class</value>
    public Sight TARGETS_TO_TRACK{
        get{ return blackboard.targetSettings; }
    }
    float? searchTime;
    
    protected override void Start() {
        base.Start();
    }

    protected override BTNode CreateTree(){
        BTNode faceTarget = new BTFaceTarget(this);
        BTNode strike = new BTAttack(this, blackboard.strike);
        BTNode charge = new BTAttack(this, blackboard.charge);
        BTNode attackTypes = new BTSelector(new List<BTNode>{charge, strike});
        BTNode attack = new BTSequence(new List<BTNode>{faceTarget, attackTypes});        


        BTNode chase = new BTChase(this);
        BTNode followSound = new BTMoveToSound(this, blackboard.timeTillDestorySound);
        BTNode wander = new BTWander(this, new Utilities.MinMax(-15, 15), new Utilities.MinMax(5, 20));

        return new BTSelector(new List<BTNode>{attack, chase, followSound, wander}); // Attack, Chase, Wander
    }

    protected override void FixedUpdate(){
        base.FixedUpdate();
        CharacterProfile.MoveSettings settings = profile.movementSettings;
        
        // Speed gets faster the less tears remaining
        speed = Utilities.Remap(WorldState.instance.GetGameProgress(1), 0, 1, settings.walkSpeed, settings.runSpeed);   

        RespondToSight();     
    }    


    void RespondToSight(){
        List<Transform> targets = blackboard.targetSettings.Search(transform.position, (POSITION_OF_INTEREST - (Vector2)transform.position).normalized);

        Transform selectedTarget = null;
        foreach(Transform selected in targets){
            // Find closest target
            if(selectedTarget && (selected.position - transform.position).magnitude >= (selectedTarget.position - transform.position).magnitude){ continue; }
            if(selected.parent.GetInstanceID() == transform.GetInstanceID()){ continue; } // Stops self selecting
            selectedTarget = selected.parent;
        }
        if(!selectedTarget && target && searchTime == null){ // If the character has lost the player, continue tracking them for x seconds
            searchTime = Time.time + UnityEngine.Random.Range(blackboard.searchTime.min, blackboard.searchTime.max);
            return;
        }
        else if(!selectedTarget && target && searchTime != null && Time.time >= (float)searchTime){
            searchTime = null;
            target = null;
            return;
        }
        else if(selectedTarget){ 
            searchTime = null;
            target = selectedTarget.GetComponent<CharacterBase>(); 
            CameraController.instance.Shake(blackboard.searchTime.min);
        }
    }

    public void RespondToSound(Sound sound) => heardSound = sound.position;
    

    [Serializable] public class Blackboard{
        [Header("Combat")]
        public Sight targetSettings;
        [Tooltip("For the strike attack")] public AttackBase strike;
        [Tooltip("For the charge attack")] public AttackBase charge;
        [Tooltip("How long the character will track the player after losing them")] public Utilities.MinMax searchTime = new Utilities.MinMax(5f, 10f);
        [Tooltip("This is a worst case scenario if it takes too long to move towards sound to just ignore it")] public float timeTillDestorySound = 20f;
    }    
}
