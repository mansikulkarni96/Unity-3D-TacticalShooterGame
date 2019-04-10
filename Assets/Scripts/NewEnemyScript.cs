using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;


[RequireComponent(typeof(NavMeshAgent))]
public class NewEnemyScript : MonoBehaviour
{
    public Transform target;
    private CharacterController enemyController;
    private Animator animator1;
    private float damage = 25;
    private float maxHealth = 800;
    public float currentHealth;

    Vector3 destination;
    Vector3 startPosition;
    NavMeshAgent enemy;
    [SerializeField]
    private float rotationSpeed1 = 5f;
    private PlayerMovement PlayerMovement;
    public GameObject playerObject;

    void Start()
    {
        currentHealth = maxHealth;
        StartCoroutine(addHealth());
        enemy = GetComponent<NavMeshAgent>();
        destination = enemy.destination;
        startPosition = enemy.transform.position;
    }

    private void Awake()
    {
        enemyController = GetComponent<CharacterController>();
        animator1 = GetComponentInChildren<Animator>();
    }


    private void Update()
    {
        if (target != null && GameObject.Find("Player") != null)
        {
            PlayerMovement = playerObject.GetComponent<PlayerMovement>();
            if (currentHealth >= 200)
            {
                destination = target.position;
                enemy.destination = destination;
                Quaternion newDirection = Quaternion.LookRotation(destination);
                transform.rotation = Quaternion.Slerp(transform.rotation, newDirection, Time.deltaTime * rotationSpeed1);
                animator1.SetFloat("Speed", destination.magnitude);
            }
            else
            {
                destination = startPosition;
                enemy.destination = destination;
                Quaternion newDirection = Quaternion.LookRotation(destination);
                transform.rotation = Quaternion.Slerp(transform.rotation, newDirection, Time.deltaTime * rotationSpeed1);
                animator1.SetFloat("Speed", destination.magnitude);
            }
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

    public void TakeDamage()
    {
        currentHealth = currentHealth - damage;

    }

    public void Die(Collision collision)
    {
        Destroy(collision.gameObject);
    }

    IEnumerator addHealth()
    {
        while (true)
        {
            if (currentHealth < 50)
            {
                currentHealth += 1;
                yield return new WaitForSeconds(1);
                print("here");
                print("current health" + currentHealth);
            }
            else
            {
                yield return null;
            }
        }
    }
}
//else if (PlayerMovement.detected)
//{
//    print("start" + startPosition);
//    destination = startPosition;
//    enemy.destination = destination;
//    Quaternion newDirection = Quaternion.LookRotation(destination);
//    transform.rotation = Quaternion.Slerp(transform.rotation, newDirection, Time.deltaTime * rotationSpeed1);
//    animator1.SetFloat("Speed", destination.magnitude);
//    if (Vector3.Distance(enemy.transform.position, target.position) > 5.0f)
//    {
//        PlayerMovement.detected = false;
//        print("detected false or not" + PlayerMovement.detected);
//    }
//}
//}
