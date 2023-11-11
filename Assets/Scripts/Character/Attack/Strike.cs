using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using JoshGames.Character.AI;
public class Strike : AttackBase{
    int selectedHand = 0, previousHand = 0;
    float elapsedTime = 0; // This is used to bring the hand back after x amount of time
    [SerializeField] AudioSource source;
    [SerializeField] AudioClip onActivate;

    private void Update() {
        HandsToTarget.HandSettings[] hands = handsToTarget.HANDS;  
        foreach(HandsToTarget.HandSettings settings in hands){
            if(!settings.hand.GetInstanceID().Equals(hands[selectedHand].hand.GetInstanceID()) || !isActive){
                settings.hand.isActive = false;
            }
            settings.overridePos = (settings.hand.isActive)? profile.rangeForAttack.max : null;
        }

        elapsedTime += Time.deltaTime;
        if(elapsedTime >= (profile.timeBetweenAttack)){ isActive = false; }
    }

    public override Err Attack(AIController ai){
        Err err = base.Attack(ai); // Check basic conditions for attack
        // No point running past this point if it is unable to attack
        if(err != Err.SUCCESS){ return err; }

        HandsToTarget.HandSettings[] hands = handsToTarget.HANDS;        
        // Iterate next hand
        previousHand = selectedHand;
        selectedHand = (selectedHand + 1) % hands.Length;
        hands[selectedHand].hand.damage = profile.attackDamage;
        hands[selectedHand].hand.isActive = true;
        elapsedTime = 0;

        source.clip = onActivate;
        source.Play();
        return err;
    }
}
