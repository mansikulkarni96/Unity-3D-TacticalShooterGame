using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Shooting : MonoBehaviour
{
    public GameObject bullet;
    public float speed = 100f;

    protected JoyButton joybutton;

    protected bool shoot;
    // Start is called before the first frame update
    void Start()
    {
        joybutton = FindObjectOfType<JoyButton>();
    }
    [SerializeField]
    Transform firePoint;

    // Update is called once per frame
    void Update()
    {

        if(!shoot && joybutton.Pressed)
        {
            print("Button Pressed");
            shoot = true;
        }

        if (shoot && !joybutton.Pressed)
        {
            shoot = false;
        }


        if (Input.GetButton("Fire2") || shoot)
        {
            print("Shot a bullet");
            GameObject instBullet = Instantiate(bullet, firePoint.position, firePoint.rotation) as GameObject;
            Rigidbody instBulletRigidbody = instBullet.GetComponent<Rigidbody>();
            instBulletRigidbody.AddForce(firePoint.forward * speed);
        }
    }
}