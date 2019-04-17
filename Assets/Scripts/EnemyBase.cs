using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyBase : Agent
{
    const int MAX_HEALTH = 3000;
    public GameObject explosion1;
    public GameObject explosion2;
    public GameObject explosion3;
    public GameObject explosion4;
    void Start()
    {
        currentHealth = MAX_HEALTH;
    }

    public override void CheckHealth()
    {
        if (currentHealth <= 0)
        {
            Explode();
        }
    }

    private void Explode()
    {
        GameObject inst = Instantiate(explosion1, this.transform.position, this.transform.rotation) as GameObject;
        GameObject inst1 = Instantiate(explosion2, this.transform.position, this.transform.rotation) as GameObject;
        GameObject inst2 = Instantiate(explosion3, this.transform.position, this.transform.rotation) as GameObject;
        GameObject inst3 = Instantiate(explosion4, this.transform.position, this.transform.rotation) as GameObject;
        Destroy(this.gameObject.transform.parent.gameObject);
    }

}
