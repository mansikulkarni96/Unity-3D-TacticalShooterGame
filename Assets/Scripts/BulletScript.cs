using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletScript : MonoBehaviour
{
    public int damage = 25;

    private void OnCollisionEnter(Collision collision)
    {
        Agent target = collision.transform.gameObject.GetComponent<Agent>();
        if (target != null)
        {
            target.ApplyDamage(damage);
            Destroy(this.gameObject);
        }

            //if (collision.gameObject.tag == "enemy" && hasHit == false)
            //{
            //    EnemyMovement enemyTarget = collision.transform.gameObject.GetComponent<EnemyMovement>();
            //    NewEnemyScript enemyTargetNew = collision.transform.gameObject.GetComponent<NewEnemyScript>();


            //    if (enemyTarget != null)
            //    {
            //        hasHit = true;
            //        enemyTarget.ApplyDamage(collision);

            //    }
            //    if (enemyTargetNew != null)
            //    {
            //        hasHit = true;
            //        enemyTargetNew.ApplyDamageCollision(collision);
            //    }
            //}
            //if(collision.gameObject.tag == "Player" || hasShoot == false)
            //{ 
            //    PlayerMovement playerTarget = collision.transform.gameObject.GetComponent<PlayerMovement>();
            //if ( playerTarget != null)
            //    {
            //        hasShoot = true;
            //        playerTarget.ApplyDamageCollision(collision);

            //    }
            //}
    }
}
