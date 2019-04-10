using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Trap : MonoBehaviour
{
    public GameObject explosion;
    public int damage = 300;

    private void OnTriggerEnter(Collider col)
    {
        if (col.tag == "Player")
        {
            PlayerMovement target = col.transform.gameObject.GetComponent<PlayerMovement>();
            if (target != null)
            {
                target.ApplyDamage(col, damage);
                Explode();
            }
        }
    }

    private void Explode()
    {
        GameObject inst = Instantiate(explosion, this.transform.position, this.transform.rotation) as GameObject;
        Destroy(this.gameObject);
    }
}
