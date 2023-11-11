using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using JoshGames.AI.BehaviourTree;
using JoshGames.Character.AI;

/*
--- This code has has been written by Joshua Thompson (https://joshgames.co.uk) ---
        --- Copyright ©️ 2023-2024 Joshua Thompson. All Rights Reserved. ---
*/

public class BTChase : BTNode{
    AIController ai;
    public BTChase(AIController ai) => this.ai = ai; 

    public override NodeState Evaluate(){
        WorldState.instance.isBeingChased = ai.target;
        if(!ai.target){ return NodeState.Fail; }
        ai.moveDirection = (ai.target.transform.position - ai.transform.position).normalized;
        ai.UpdatePositionOfInterest(ai.transform.position + (Vector3)ai.previousMoveDirection);
        return NodeState.Success;
    }
}
