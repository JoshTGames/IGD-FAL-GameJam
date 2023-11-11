using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public abstract class HealthBase : MonoBehaviour{
    float maxHealth = 100, currentHealth = 100;

    protected float MAX_HEALTH{
        get{ return maxHealth; }
        set{
            float healthPercent = GetHealthPercentage();

            maxHealth = value;
            HEALTH = MAX_HEALTH * healthPercent; // Sets the health proportionate to the maxHealth
        }
    }

    protected float HEALTH{
        get{ return currentHealth; }
        set{ 
            if(value <= 0){ 
                events.onDead?.Invoke(this); 
                OnDead();
            }
            else if(value > currentHealth){ 
                events.onHeal?.Invoke(this); 
                OnHeal();
            }
            else if(value < currentHealth){ 
                events.onDamage?.Invoke(this); 
                OnDamage();
            }
            currentHealth = Mathf.Clamp(value, 0, MAX_HEALTH);             
        }
    }
    [SerializeField] protected HealthEvents events;
    [System.Serializable] public class HealthEvents{
        public UnityEvent<HealthBase> onHeal;
        public UnityEvent<HealthBase> onDamage;
        public UnityEvent<HealthBase> onDead;
    }

    protected void SetHealth(float _amount) => currentHealth = _amount;

    protected virtual void Start() => currentHealth = MAX_HEALTH;

    /// <summary>
    /// Improves the health of this entity
    /// </summary>
    /// <param name="_amount">The amount of health that will effect this entity</param>
    public void Heal(float _amount) => HEALTH += _amount;
    /// <summary>
    /// Deducts health from this entity
    /// </summary>
    /// <param name="_amount">The amount of health that will effect this entity</param>
    public void Damage(float _amount) => HEALTH -= _amount;

    /// <summary>
    /// Divides health by maxHealth to get a percentage of remaining health
    /// </summary>
    /// <returns>A value between 0-1 showing the percentage of health remaining</returns>
    public float GetHealthPercentage() => HEALTH / MAX_HEALTH;


    /// <summary>
    /// Allows for functionality when the entity is healed
    /// </summary>
    protected virtual void OnHeal(){}

    /// <summary>
    /// Allows for functionality when the entity is damaged
    /// </summary>
    protected virtual void OnDamage(){}
    
    /// <summary>
    /// Allows for functionality when the entity is dead
    /// </summary>
    protected virtual void OnDead() => Destroy(gameObject);
}
