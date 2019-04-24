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
    }
}
