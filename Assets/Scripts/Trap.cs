using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEditor;

public class Trap : MonoBehaviour
{
    public GameObject explosion;
    public int damage = 300;

    private void OnTriggerEnter(Collider col)
    {
        Agent target = col.transform.gameObject.GetComponent<Agent>();
        if (target != null)
        {
            target.ApplyDamage(damage);
            Explode();
            NavMesh.SetAreaCost(NavMesh.GetAreaFromName(this.tag), 1);
        }  
    }

    private void Explode()
    {
        GameObject inst = Instantiate(explosion, this.transform.position, this.transform.rotation) as GameObject;
        Destroy(this.gameObject);
    }
}
