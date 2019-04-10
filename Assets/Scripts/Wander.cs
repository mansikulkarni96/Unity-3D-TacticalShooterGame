using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEngine.AI;

public class Wander : MonoBehaviour
{

    public float wanderRadius = 10;
    public float wanderTimer = 2;
    CharacterController charCtrl;

    private NavMeshAgent agent;
    private float timer;
    private Animator animator;


    void Start()
    {
        charCtrl = GetComponent<CharacterController>();
    }


    // Use this for initialization
    void OnEnable()
    {
        agent = GetComponent<NavMeshAgent>();
        timer = wanderTimer;
    }

    private void Awake()
    {
        animator = GetComponentInChildren<Animator>();
    }

    // Update is called once per frame
    void Update()
    {

        timer += Time.deltaTime;

        if (timer >= wanderTimer)
        {

            Vector3 newPos = RandomNavSphere(transform.position, wanderRadius, -1);

            agent.SetDestination(newPos);
            //animator.SetFloat("Speed", agent.speed);
            timer = 0;
        }


    }

    public Vector3 RandomNavSphere(Vector3 origin, float dist, int layermask)
    {

        AvoidObstacle();
        Vector3 randDirection = Random.insideUnitSphere * dist;

        randDirection += origin;

        NavMeshHit navHit;

        NavMesh.SamplePosition(randDirection, out navHit, dist, layermask);

        return navHit.position + new Vector3(1, 0, 0);
    }


    void AvoidObstacle()
    {

        RaycastHit hit;


        if (Physics.Raycast(transform.position, transform.forward, out hit, 10))// if raycast hits something within ten infront
        {
            if (hit.collider.gameObject)//any object
            {
                print("hit detected");
                //Debug.DrawRay(transform.position, hit.point, Color.red, 2f);//show a debug line

                transform.Rotate(new Vector3(0, 180, 0), Time.deltaTime * 10f);//rotate 180 degrees
            }
        }
        else if (Physics.Raycast(transform.position, -transform.right, out hit, 5))//if raycast hits something within 5 on the left
        {
            if (hit.collider.gameObject)
            {
                print("hit detected1");
                //Debug.DrawRay(transform.position, hit.point, Color.red, 2f);
                transform.Rotate(new Vector3(0, 45, 0), Time.deltaTime * 10f);//rotate right
                //  transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(velocity), 10 * Time.deltaTime);
            }
        }
        else if (Physics.Raycast(transform.position, transform.right, out hit, 5))//if raycast hits something within 5 on left
        {
            //Debug.DrawRay(transform.position, hit.point, Color.red, 2f);
            if (hit.collider.gameObject)
            {
                print("hit detected2");
                transform.Rotate(new Vector3(0, -45, 0), Time.deltaTime * 10f);
            }
        }
    }




}






