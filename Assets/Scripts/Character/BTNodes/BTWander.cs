using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using JoshGames.AI.BehaviourTree;
using JoshGames.Character.AI;

/*
--- This code has has been written by Joshua Thompson (https://joshgames.co.uk) ---
        --- Copyright ©️ 2023-2024 Joshua Thompson. All Rights Reserved. ---
*/

public class BTWander : BTNode{
    AIController ai;
    Utilities.MinMax travelDistance;
    Utilities.MinMax waitTime;
    float deltaDistance;

    Vector3 targetPosition;
    float timeTillNewPosition;

    public BTWander(AIController ai, Utilities.MinMax travelDistance, Utilities.MinMax waitTime, float deltaDistance = 0.5f){
        this.ai = ai;
        this.travelDistance = travelDistance;
        this.waitTime = waitTime;
        this.deltaDistance = deltaDistance;
    }

    public override NodeState Evaluate(){
        if(!WorldState.instance.player){ return NodeState.Fail; } // Safety measure 

        if(Time.time >= timeTillNewPosition){
            targetPosition = ai.transform.position + new Vector3(Random.Range(travelDistance.min, travelDistance.max), Random.Range(travelDistance.min, travelDistance.max));
            timeTillNewPosition = Time.time + Random.Range(waitTime.min, waitTime.max);
        }

        Vector3 direction = WorldState.instance.player.transform.position - targetPosition;
        targetPosition = WorldState.instance.player.transform.position - Vector3.ClampMagnitude(direction, WorldState.instance.PLAYER_SEARCH_RADIUS);

        if((targetPosition - ai.transform.position).magnitude < deltaDistance){ ai.moveDirection = Vector2.zero; }
        else{
            ai.moveDirection = (targetPosition - ai.transform.position).normalized;
        }

        ai.UpdatePositionOfInterest(ai.transform.position + (Vector3)ai.previousMoveDirection);
        return NodeState.Success;
    }
}
