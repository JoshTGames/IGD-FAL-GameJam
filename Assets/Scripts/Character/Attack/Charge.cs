using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using JoshGames.Character;

using JoshGames.Character.AI;
public class Charge : AttackBase{
    float elapsedTime = 0; // This is used to bring the hand back after x amount of time
    [SerializeField] AudioSource source;
    [SerializeField] AudioClip onActivate;

    Vector3 currentPosition;
    Vector3 targetChargePosition;
    [SerializeField] float chargeDuration = 0.5f;
    [SerializeField] float idleDuration = 1f;
    [SerializeField] float attackRadius;
    [SerializeField] float distanceMultiplier = 1;


    private void Update() {
        elapsedTime += Time.deltaTime;
        if(isActive){
            transform.position = Vector3.Lerp(currentPosition, targetChargePosition, Mathf.Clamp01(elapsedTime / chargeDuration));
            Collider2D[] cols = Physics2D.OverlapCircleAll(transform.position, attackRadius);
            foreach(Collider2D col in cols){
                CharacterBase character = col.GetComponentInParent<CharacterBase>();
                if(character && character.transform.GetInstanceID() == transform.GetInstanceID()){ continue; } // Stop it hitting itself
                if(character){ character.stunTimer = Time.time + profile.attackTypes[0].affectDuration; }
                isActive = false;
                break;
            }
        }
        if(elapsedTime >= (idleDuration)){ isActive = false; }
    }

    public override Err Attack(AIController ai){
        Err err = base.Attack(ai); // Check basic conditions for attack
        // No point running past this point if it is unable to attack
        if(err != Err.SUCCESS){ return err; }

        elapsedTime = 0;
        if(source){
            source.clip = onActivate;
            source.Play();
        }
        currentPosition = ai.transform.position;
        targetChargePosition = ai.target.transform.position + ((ai.target.transform.position - currentPosition).normalized * distanceMultiplier);
        return err;
    }

    private void OnDrawGizmosSelected() {
        Gizmos.DrawWireSphere(transform.position, attackRadius);
    }
}
