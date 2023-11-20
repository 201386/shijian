using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class shanghai : MonoBehaviour,enemyshanghai
{
    protected float health;
    public float startingHealth;
    protected bool dead = false;
    public event Action onDath;

    public void TaskHit(float damage, RaycastHit hit)
    {
        TashDamage(damage);
    }
    protected virtual void Start()
    {
        health = startingHealth;
    }
    public void TashDamage(float damage)
    {
        health -= damage;
        if (health <= 0 && !dead)
        {
            Die();
        }
    }
    void Update()
    {
        
    }
    protected void Die()
    {
        dead = true;
        onDath();
        Destroy(gameObject);
    }
   
}
