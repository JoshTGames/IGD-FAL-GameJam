using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HandCollisionDetector : MonoBehaviour{
    [HideInInspector] public float damage;
    [HideInInspector] public bool isActive = false;
    [SerializeField] AudioSource source;
    [SerializeField] AudioClip onHit;
    private void OnTriggerEnter2D(Collider2D other){ 
        if(!isActive){ return; }
        isActive = false;
        other.GetComponentInParent<HealthBase>()?.Damage(damage);

        source.clip = onHit;
        source.Play();
    }
}
