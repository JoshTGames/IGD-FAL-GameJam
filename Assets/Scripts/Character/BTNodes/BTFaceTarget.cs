using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using JoshGames.AI.BehaviourTree;
using JoshGames.Character;
using JoshGames.Character.AI;
using JoshGames.AI.Sensors;

/*
--- This code has has been written by Joshua Thompson (https://joshgames.co.uk) ---
        --- Copyright ©️ 2023-2024 Joshua Thompson. All Rights Reserved. ---
*/

public class BTFaceTarget : BTNode{
    AIController ai;
    public BTFaceTarget(AIController ai){ this.ai = ai; }

    public override NodeState Evaluate(){
        if(!ai || !ai.target){ return NodeState.Fail; } // Safety check
        ai.UpdatePositionOfInterest(ai.target.transform.position);
        return NodeState.Success;
    }
}
