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

    Vector3 destination;
    NavMeshAgent enemy;

    [SerializeField]
    private float moveSpeed1 = 300f;
    [SerializeField]
    private float rotationSpeed1 = 5f;



    void Start()
    {
        enemy = GetComponent<NavMeshAgent>();
        destination = enemy.destination;
    }

    private void Awake()
    {
        //joystick = FindObjectOfType<Joystick>();
        enemyController = GetComponent<CharacterController>();
        animator1 = GetComponentInChildren<Animator>();
    }

    private void Update()
    {

        // var horizontal = Input.GetAxis("Horizontal");
        // var vertical = Input.GetAxis("Vertical");
        /*
        var movement = new Vector3(joystick.Horizontal, 0, joystick.Vertical);

        if (movement.magnitude > 0)
        {
            if (!Input.GetButton("Fire2"))
            {
                characterController.SimpleMove(movement * Time.deltaTime * moveSpeed1);

            }
            Quaternion newDirection = Quaternion.LookRotation(movement);
            transform.rotation = Quaternion.Slerp(transform.rotation, newDirection, Time.deltaTime * rotationSpeed1);
        }

        if (!Input.GetButton("Fire2"))
        {
            animator.SetFloat("Speed", movement.magnitude);
        }
    }

    if (Vector3.Distance(destination, target.position) > 1.0f)
        {
            destination = target.position;
            agent.destination = destination;
        }

*/

        if (Vector3.Distance(destination, target.position) > 1.0f)
        {
            destination = target.position;
            enemy.destination = destination;
        }

    }

}
