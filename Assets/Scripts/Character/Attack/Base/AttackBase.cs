using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using JoshGames.Character.AI;

/*
--- This code has has been written by Joshua Thompson (https://joshgames.co.uk) ---
        --- Copyright ©️ 2023-2024 Joshua Thompson. All Rights Reserved. ---
*/

namespace JoshGames.Character.AI{
    public abstract class AttackBase : MonoBehaviour{
        [SerializeField] protected AttackProfile profile;
        [SerializeField] protected HandsToTarget handsToTarget;

        
        protected float timeTillCanUse;
        protected bool isActive;

        /// <summary>
        /// This function should not be used to actually make objects do actions but instead switch logic in the script so that functions like the FixedUpdate function can do it
        /// </summary>
        /// <param name="ai">The character which is calling this</param>
        /// <returns>An error code</returns>
        public virtual Err Attack(AIController ai){
            if(isActive){ return Err.ERR_IS_ACTIVE; }
            if(Time.time < timeTillCanUse){ return Err.ERR_UNDER_COOLDOWN; }

            if((ai.target.transform.position - ai.transform.position).magnitude < profile.rangeForAttack.min){ return Err.ERR_TOO_CLOSE; }
            else if((ai.target.transform.position - ai.transform.position).magnitude > profile.rangeForAttack.max){ return Err.ERR_TOO_FAR; }
            
            if(handsToTarget && Vector2.Dot((ai.POSITION_OF_INTEREST - (Vector2)ai.transform.position).normalized, handsToTarget.HANDS[0].hand.transform.right * handsToTarget.hasParentChangedDirection) < profile.attackAngle){ return Err.ERR_NOT_FACING_TARGET; }
            timeTillCanUse = Time.time + profile.timeBetweenAttack;
            isActive = true;
            return Err.SUCCESS;
        }

        /// <summary>
        /// Compares 2 Vectors, the origin vector and then the direction of another
        /// </summary>
        /// <param name="other">The vector to be compared with</param>
        /// <returns>A value between -1,1</returns>
        protected float GetAccuracyToPositionOfInterest(CharacterBase character, Vector2 other) => Vector2.Dot((character.POSITION_OF_INTEREST - (Vector2)character.transform.position).normalized, (character.POSITION_OF_INTEREST - other).normalized);


        public enum Err{
            SUCCESS,
            ERR_IS_ACTIVE,
            ERR_UNDER_COOLDOWN,
            ERR_TOO_CLOSE,
            ERR_TOO_FAR,
            ERR_NOT_FACING_TARGET
        }
    }
}