using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;

[RequireComponent(typeof(NavMeshAgent))]
public class PlayerMovement : MonoBehaviour
{
    private CharacterController characterController;
    private Animator animator;
    protected Joystick joystick;
    protected FixedJoyButton FixedJoybutton;

    [SerializeField]
    private float moveSpeed = 300f;
    [SerializeField]
    private float rotationSpeed = 5f;

    NavMeshAgent player;

    private float damage = 25;
    private float maxHealth = 1000;
    private float currentHealth;
    public Slider HealthBar;
    public Text healthText;

    protected bool dead=false;
    void Start()
    {
        player = GetComponent<NavMeshAgent>();
        joystick = FindObjectOfType<Joystick>();
        FixedJoybutton = FindObjectOfType<FixedJoyButton>();
        currentHealth = maxHealth;
        HealthBar.maxValue = maxHealth;
        HealthBar.value = maxHealth;
        healthText.text = maxHealth.ToString();
    }

    private void Awake()
    {
        characterController = GetComponent<CharacterController>();
        animator = GetComponentInChildren<Animator>();
    }

    private void Update()
    {
        if(currentHealth <= 0)
        {
            dead = true;
        }
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

    public void ApplyDamageCollision(Collision collision)
    {
        if (currentHealth > 0)
        {
            TakeDamage();
        }
        else
        {
            Destroy(collision.gameObject);
        }      
    }

    public void ApplyDamageCollider(Collider collision)
    {
        if (currentHealth > 0)
        {
            TakeDamage();
        }
        else
        {
            Destroy(collision.gameObject);
        }
    }

    public void TakeDamage()
    {
        currentHealth = currentHealth - damage;
        HealthBar.value = currentHealth;
        healthText.text = currentHealth.ToString();
    }

    public void DestroyPlayer()
    {
        if (currentHealth > 0)
        {
            TakeDamage();
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public bool isDead()
    {
        if (dead == true)
            return true;
        else
            return false;
    }     
}
