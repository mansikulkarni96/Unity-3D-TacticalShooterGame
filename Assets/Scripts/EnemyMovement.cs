using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEngine.AI;


[RequireComponent(typeof(NavMeshAgent))]
public class EnemyMovement : MonoBehaviour
{
    public Transform target;
    private CharacterController enemyController;
    private Animator animator1;
    private int health = 1000;
    private int damage = 25;
    
    Vector3 destination;
    NavMeshAgent enemy;

    [SerializeField]
    private float moveSpeed1 = 300f;
    [SerializeField]
    private float rotationSpeed1 = 5f;
    private PlayerMovement playerMovement;

    void Start()
    {     
        enemy = GetComponent<NavMeshAgent>();
        destination = enemy.destination;
    }

    private void Awake()
    {
        enemyController = GetComponent<CharacterController>();
        animator1 = GetComponentInChildren<Animator>();
    }

    private void Update()
    {
        if (target!=null && GameObject.Find("Player")!=null) {
            if (Vector3.Distance(destination, target.position) > 1.0f)
            {
                destination = target.position;
                enemy.destination = destination;
                animator1.SetFloat("Speed", destination.magnitude);
            }
           
        }
    }

    public void ApplyDamage(Collision collision)
    {
        if (health > 0)
        {
            health = health - damage;
        }
        else
        {
            Die(collision);
        }
        Debug.Log("health = " + health);
    }

    public void Die(Collision collision)
    {
        Destroy(collision.gameObject);
    }
}
