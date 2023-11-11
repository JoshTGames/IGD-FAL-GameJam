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

public class BTSelectTarget : BTNode{
    AIController ai;
    Sight sight;
    public BTSelectTarget(AIController ai, Sight sight){
        this.ai = ai;
        this.sight = sight;
    }

    public override NodeState Evaluate(){
        if(!ai){ return NodeState.Fail; } // Safety check

        List<Transform> targets = sight.Search(ai.transform.position, (ai.POSITION_OF_INTEREST - (Vector2)ai.transform.position).normalized);
        if(targets.Count <= 0){ return NodeState.Fail;}

        Transform selectedTarget = null;
        foreach(Transform selected in targets){
            // Find closest target
            if(selectedTarget && (selected.position - ai.transform.position).magnitude >= (selectedTarget.position - ai.transform.position).magnitude){ continue; }
            if(selected.parent.GetInstanceID() == ai.transform.GetInstanceID()){ continue; } // Stops self selecting
            selectedTarget = selected.parent;
        }
        ai.target = selectedTarget?.GetComponent<CharacterBase>();
        return (ai.target)? NodeState.Success : NodeState.Fail;
    }
}
