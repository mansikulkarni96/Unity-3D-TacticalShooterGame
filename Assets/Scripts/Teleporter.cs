using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Teleporter : MonoBehaviour
{
    public GameObject pos;
    public Player player;
    private void OnTriggerStay(Collider col)
    {
        Player target = col.transform.gameObject.GetComponent<Player>();
        Quaternion rot = target.transform.rotation;
        if (col.tag == "Player")
        {
            player.transform.position = pos.transform.position;
        }
    }
}
