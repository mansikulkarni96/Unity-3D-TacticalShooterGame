using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Trap : MonoBehaviour
{

    private void OnTriggerEnter(Collider collision)
    {
        if (collision.tag == "Player" )
        {
            PlayerMovement PlayerTarget = collision.transform.gameObject.GetComponent<PlayerMovement>();
            if ( PlayerTarget != null)
            {
                PlayerTarget.ApplyDamageCollider(collision);
            }
        }
    }
}
