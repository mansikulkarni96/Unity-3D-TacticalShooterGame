using System.Collections;
using System.Collections.Generic;
//using System.Drawing.Color;
using UnityEngine;

public class Weapon : MonoBehaviour
{
    private Animator animator;
    //[SerializeField]
    //[Range(0.5f, 1.5f)]
    ////private float fireRate = 1;


    [SerializeField]
    float speed = 100f;

    //[SerializeField]
    //[Range(1f, 10f)]
    //private float damage = 1;

    [SerializeField]
    private Transform firePoint;

    [SerializeField]
    private GameObject bullet;

    protected FixedJoyButton joybutton;
    protected bool shoot;

   

    void Start()
    {
        joybutton = FindObjectOfType<FixedJoyButton>();
    }

    void Awake()
    {
        animator = GetComponentInChildren<Animator>();
    }

    void Update()
    {
        if (!shoot && joybutton.Pressed)
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
            FireGun();
        }

        animator.SetBool("isFiring", joybutton.Pressed || Input.GetButton("Fire2"));
    }

    private void FireGun()
    {
        //Debug.DrawRay(firePoint.position, firePoint.forward * 100, Color.red, 2f);

        GameObject instBullet = Instantiate(bullet, firePoint.position, firePoint.rotation) as GameObject;
        Rigidbody instBulletRigidbody = instBullet.GetComponent<Rigidbody>();
        instBulletRigidbody.AddForce(firePoint.forward * speed);
        Destroy(instBullet, 0.5f);
    }

}
