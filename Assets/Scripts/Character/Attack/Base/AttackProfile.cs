using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;


/*
--- This code has has been written by Joshua Thompson (https://joshgames.co.uk) ---
        --- Copyright ©️ 2023-2024 Joshua Thompson. All Rights Reserved. ---
*/

namespace JoshGames.Character.AI{
    [CreateAssetMenu(fileName = "New Attack Profile", menuName = "ScriptableObjects/Character/New Attack Profile")]
    public class AttackProfile : ScriptableObject{
        public float attackDamage = 1.0f;
        public float timeBetweenAttack = 1.0f;
        public Utilities.MinMax rangeForAttack = new Utilities.MinMax(0.25f, 0.5f);
        [Range(0, 1), Tooltip("1 is straight ahead. Greater the value, the more accurate the character will have to be")] public float attackAngle = 0.85f;
        public SpecialAttackType[] attackTypes;
    }    

    [Serializable] public class SpecialAttackType{
        public SpecialAttack attackType;
        [Tooltip("How long will the affect be at play (IN SECONDS)")] public float affectDuration;
        [Tooltip("This has no use for the 'STUN' attack")] public float affectAmount;
    }

    [Serializable] public enum SpecialAttack{
        NONE,
        STUN,
        BLEED
    }
}