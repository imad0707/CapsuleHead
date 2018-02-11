using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Wezens : MonoBehaviour, IDamageable {

    public float startHealth;
    public float health { get; protected set; }
    protected bool dead;

    public event System.Action OnDeath;

    protected virtual void Start()
    {
        health = startHealth;
    }

    public virtual void Raak(float damage, Vector3 raakPunt, Vector3 raakRichting)          //(float damage, RaycastHit hit)
    {
        //health -= damage;
        //if (health <= 0)        // (health <=0 && !dead)
        //{
        //    Die();
        //}
        Damage(damage); 
        //Doe iets met de raak var
    }

    public virtual void Damage (float damage)
    {
        health -= damage;

        if (health <= 0)        
        {
            Die();
        }
    }

    protected void Die()
    {
        dead = true;
        if (OnDeath != null)
        {
            OnDeath();
        }
        GameObject.Destroy(gameObject);
    }
	
}
