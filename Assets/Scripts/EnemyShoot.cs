using System.Collections;
using System.Collections.Generic;
//using System.Drawing.Color;
using UnityEngine;

public class EnemyShoot : MonoBehaviour
{
    private Animator enemyAnimator;


    private PlayerMovement playerMovement;
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

    public Transform player;
    public Transform enemy;

    private Vector3 distance;
    private float range;



    void Start()
    {
       // joybutton = FindObjectOfType<FixedJoyButton>();
    }

    void Awake()
    {
        enemyAnimator = GetComponentInChildren<Animator>();
    }

    void Update()
    {
        enemyAnimator.SetBool("isFiring", false);
        if (GameObject.Find("Player") != null){
            distance = enemy.position - player.position;
            distance.y = 0;
            range = distance.magnitude;
            distance /= range;
            if (range < 4)
            {
                shoot = true;
            }
            else
            {
                shoot = false;
            }

            if (shoot)
            {
                FireGun();
            }

            enemyAnimator.SetBool("isFiring", shoot);
        }
    }

    private void FireGun()
    {
        RaycastHit hit;
        Ray ray = new Ray(firePoint.position, firePoint.forward);
        Debug.DrawRay(firePoint.position, firePoint.forward * 10, Color.red, 2f);
        enemy.LookAt(player);
        if (Physics.Raycast(ray, out hit, 100))
        {
            if (hit.collider.tag == "Player")
            {
                GameObject instBullet = Instantiate(bullet, firePoint.position, firePoint.rotation) as GameObject;
                Rigidbody instBulletRigidbody = instBullet.GetComponent<Rigidbody>();
                instBulletRigidbody.AddForce(firePoint.forward * speed);
                Destroy(instBullet, 0.5f);
            }
        }
    }
}
