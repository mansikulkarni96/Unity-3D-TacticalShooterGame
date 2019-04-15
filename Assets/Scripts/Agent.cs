using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Agent : MonoBehaviour
{
    protected int currentHealth;
    public bool isDead;

    public void ApplyDamage(int damage)
    {
       currentHealth -= damage;
       CheckHealth();
    }

    public abstract void CheckHealth();
}
