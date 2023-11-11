using JoshGames.AI.BehaviourTree;
using JoshGames.Character.AI;
using UnityEngine;

/*
--- This code has has been written by Joshua Thompson (https://joshgames.co.uk) ---
        --- Copyright ©️ 2023-2024 Joshua Thompson. All Rights Reserved. ---
*/

public class BTAttack : BTNode{
    AIController ai;
    AttackBase attackClass;
    public BTAttack(AIController ai, AttackBase attackClass){
        this.ai = ai;
        this.attackClass = attackClass;
    }

    public override NodeState Evaluate(){
        if(!ai || !ai.target || !attackClass){ return NodeState.Fail; } // Safety check

        switch(attackClass.Attack(ai)){
            case AttackBase.Err.SUCCESS:
                ai.moveDirection = Vector2.zero;
                return NodeState.Success;
            case AttackBase.Err.ERR_IS_ACTIVE:
                ai.moveDirection = Vector2.zero;
                return NodeState.Running;
            default:
                return NodeState.Fail;
        }
    }
}
