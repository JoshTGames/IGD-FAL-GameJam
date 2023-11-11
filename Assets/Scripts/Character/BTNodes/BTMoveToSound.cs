using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using JoshGames.AI.BehaviourTree;
using JoshGames.Character.AI;
using JoshGames.AI.Sensors;

/*
--- This code has has been written by Joshua Thompson (https://joshgames.co.uk) ---
        --- Copyright ©️ 2023-2024 Joshua Thompson. All Rights Reserved. ---
*/

public class BTMoveToSound : BTNode{
    AIController ai;
    float deltaPosition;
    float timeTillStopChasingSound;

    float? timeTillDestorySound; // This will be the actual timer

    public BTMoveToSound(AIController ai, float timeTillStopChasingSound, float deltaPosition = 0.5f){
        this.ai = ai;
        this.timeTillStopChasingSound = timeTillStopChasingSound;
        this.deltaPosition = deltaPosition;
    } 

    public override NodeState Evaluate(){
        if(ai.heardSound == null){ return NodeState.Fail; }
        Vector3 soundPos = (Vector2)ai.heardSound;

        if((soundPos - ai.transform.position).magnitude <= deltaPosition){
            ai.heardSound = null;
            timeTillDestorySound = null;

            return NodeState.Success;
        }

        if(timeTillDestorySound != null && Time.time >= (float)timeTillDestorySound){ 
            ai.heardSound = null; 
            timeTillDestorySound = null;
            return NodeState.Fail;
        }
        else if(timeTillDestorySound == null){ timeTillDestorySound = Time.time + timeTillStopChasingSound; }


        ai.moveDirection = (soundPos - ai.transform.position).normalized;
        ai.UpdatePositionOfInterest(ai.transform.position + (Vector3)ai.previousMoveDirection);
        return NodeState.Success;
    }
}
