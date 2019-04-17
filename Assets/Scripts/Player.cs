// Player behavior was learned through watching several tutorials on YouTube
// https://www.youtube.com/watch?v=sLAyey2WDyc&list=PLB5_EOMkLx_Wa0sRby_krVpglLS7IYH3_

// For firing bullets: 
// https://www.youtube.com/watch?v=THnivyG0Mvo&t=230s
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class Player : Agent
{
    const int MAX_HEALTH = 10000;

    CharacterController characterController;
    Animator animator;
    Joystick joystick;
    FixedJoyButton joybutton;
    bool isFiring;

    public float moveSpeed = 250;
    public float rotationSpeed = 10f;
    public Transform firePoint;
    public GameObject bullet;
    public float fireSpeed = 1000f;
    public Slider healthBar;
    public Text healthText;

    AudioSource source;
    public AudioClip fireSound;

    void Start()
    {
        currentHealth = MAX_HEALTH;
        healthBar.maxValue = MAX_HEALTH;
        healthBar.value = MAX_HEALTH;
        healthText.text = MAX_HEALTH.ToString();
        joystick = FindObjectOfType<Joystick>();
        joybutton = FindObjectOfType<FixedJoyButton>();
    }

    private void Awake()
    {
        characterController = GetComponent<CharacterController>();
        animator = GetComponentInChildren<Animator>();
        source = GetComponent<AudioSource>();
    }

    void Update()
    {
        Move();
        Shoot();
        if (isFiring)
        {
            StartCoroutine("playAudio");
        }
    }

    void Move()
    {
        var movement = new Vector3(joystick.Horizontal, 0, joystick.Vertical);

        if (movement.magnitude > 0)
        {
            if (!Input.GetButton("Fire2"))
            {
                characterController.SimpleMove(movement * Time.deltaTime * moveSpeed);
            }
            Quaternion newDirection = Quaternion.LookRotation(movement);
            transform.rotation = Quaternion.Slerp(transform.rotation, newDirection, Time.deltaTime * rotationSpeed);
        }

        if (!Input.GetButton("Fire2"))
        {
            animator.SetFloat("Speed", movement.magnitude);
        }
    }

    void Shoot()
    {
        if (Input.GetButton("Fire2") || joybutton.Pressed)
        {
            isFiring = true;
            FireGun();
        }
        else
        {
            isFiring = false;
        }

        animator.SetBool("isFiring", isFiring);
    }

    private void FireGun()
    {
        GameObject instBullet = Instantiate(bullet, firePoint.position, firePoint.rotation) as GameObject;
        Rigidbody instBulletRigidbody = instBullet.GetComponent<Rigidbody>();
        instBulletRigidbody.AddForce(firePoint.forward * fireSpeed);
        Destroy(instBullet, 0.5f);
    }


    public override void CheckHealth()
    {
        healthBar.value = currentHealth;
        healthText.text = currentHealth.ToString();
        if (currentHealth <= 0)
        {
            isDead = true;
            Destroy(this.gameObject);
        }
    }


    IEnumerator playAudio()
    {
        source.PlayOneShot(fireSound);
        yield return new WaitForSeconds(.3f);

    }
}


