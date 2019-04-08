using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletScript : MonoBehaviour
{
    public float damage = 25f;
    public bool hasHit = false;
    public bool hasShoot = false;
    public float health = 100;

    // Use this for initialization
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.tag == "enemy" && hasHit == false)
        {
            EnemyMovement enemyTarget = collision.transform.gameObject.GetComponent<EnemyMovement>();

            if (enemyTarget != null)
            {
                hasHit = true;
                enemyTarget.ApplyDamage(collision);

            }
        }
        if(collision.gameObject.tag == "Player" || hasShoot == false)
        { 
            PlayerMovement playerTarget = collision.transform.gameObject.GetComponent<PlayerMovement>();
        if ( playerTarget != null)
            {
                hasShoot = true;
                playerTarget.ApplyDamageCollision(collision);

            }
        }
    }
}
