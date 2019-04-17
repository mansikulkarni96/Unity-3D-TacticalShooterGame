using UnityEngine;
using UnityEngine.AI;
using System.Collections;



public class Wander : MonoBehaviour
{

    public Transform[] points;
    private int destPoint = 0;
    private NavMeshAgent agent;
    private Animator animator;

    public Transform agent1;
    private Vector3 prevPosition1;

    //Previous Position of the transform
    private Vector3 prevPosition;

    private Vector3 relativePosition;
    private Vector3 relativeVelocity;

    private float distance;
    private float shortestTime;

    public float collisionRadius = 0.4f;

    public Transform player;
    private Vector3 playerPosition;
    Vector3 vel;
    Vector3 vel1;
    Vector3 playerSpeed;


    private float moveSpeed = 3.5f;
    private float rotationSpeed = 1.0f;
    public int safeDistance = 10;

    public int minDistance = 5;




    void Awake()
    {
        animator = GetComponentInChildren<Animator>();
    }

    void Start()
    {
        shortestTime = Mathf.Infinity;

        agent = GetComponent<NavMeshAgent>();


        agent.autoBraking = false;

        GotoNextPoint();
    }


    void GotoNextPoint()
    {

        if (points.Length == 0)
            return;

        // Set the agent to go to the currently selected destination.
        agent.destination = points[destPoint].position;
        animator.SetFloat("Speed", agent.destination.magnitude);


        // Choose the next point in the array as the destination,
        // cycling to the start if necessary.

        destPoint = (destPoint + 1) % points.Length;
    }


    void Update()
    {

        // Choose the next destination point when the agent gets
        // close to the current one.
        vel = (transform.position - prevPosition) / Time.deltaTime;
        prevPosition = transform.position;

        vel1 = (agent1.position - prevPosition1) / Time.deltaTime;
        prevPosition1 = agent1.position;

        playerSpeed = (player.position - playerPosition) / Time.deltaTime;
        playerPosition = player.position;

        wallAvoidance();
        CollisionAvoidance();
        //evade();
        if (!agent.pathPending && agent.remainingDistance < 3f)
        {
            print(destPoint);
            GotoNextPoint();
        }
    }



    void CollisionAvoidance()
    {


        relativePosition = transform.position - agent1.position;
        relativeVelocity = vel - vel1;

        //float ca = (relativePosition.magnitude * relativeVelocity.magnitude) / (relativeVelocity.magnitude * relativeVelocity.magnitude);
        //print(ca);
        //distance = relativePosition.magnitude;


        float relativeSpeed = relativeVelocity.magnitude;
        float timeToCollision = Vector3.Dot(relativePosition, relativeVelocity);
        timeToCollision /= relativeSpeed * relativeSpeed * -1;
        print(timeToCollision);
        //float minSeparation = distance - relativeSpeed * timeToCollision;
        //if (minSeparation > 2 * collisionRadius)
        //    return;
        //if (minSeparation <= 0.0f || distance < 2 *collisionRadius)
        //{
        //    relativePosition = agent1.position;
        //}
        //else

        if (timeToCollision < 2 && timeToCollision > 0)
        {
            //agent.autoRepath = true;
            float step = 3.5f * Time.deltaTime; // calculate distance to move
            Vector3 position = transform.position + timeToCollision * vel * 1;
            agent.SetDestination(position);




        }

        else
        {
            return;
        }

    }
    public void wallAvoidance()
    {

        RaycastHit hit;

        if (Physics.Raycast(transform.position, transform.forward, out hit, 4))// if raycast hits something within ten infront
        {
            if (hit.collider.gameObject.tag == "wall")//any object
            {
                print("hit detected");
              //  Debug.DrawRay(transform.position, hit.point, Color.red, 2f);//show a debug line

                transform.Rotate(new Vector3(0, 180, 0), Time.deltaTime * 10f);//rotate 180 degrees
                                                                               // Vector3 newPos = RandomNavSphere(transform.position, wanderRadius, -1);

                // agent.SetDestination(newPos);
            }
        }
        if (Physics.Raycast(transform.position, -transform.right, out hit, 5))//if raycast hits something within 5 on the left
        {
            if (hit.collider.gameObject.tag == "wall")
            {
                print("hit detected1");
               // Debug.DrawRay(transform.position, hit.point, Color.red, 2f);
                transform.Rotate(new Vector3(0, 45, 0), Time.deltaTime * 10f);//rotate right
                //Vector3 newPos = RandomNavSphere(transform.position, wanderRadius, -1);

                //agent.SetDestination(newPos);
                //  transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(velocity), 10 * Time.deltaTime);
            }
        }
        if (Physics.Raycast(transform.position, transform.right, out hit, 5))//if raycast hits something within 5 on left
        {
            //Debug.DrawRay(transform.position, hit.point, Color.red, 2f);
            if (hit.collider.gameObject.tag == "wall")
            {
                print("hit detected2");
                transform.Rotate(new Vector3(0, -45, 0), Time.deltaTime * 10f);
                //Vector3 newPos = RandomNavSphere(transform.position, wanderRadius, -1);

                // agent.SetDestination(newPos);
            }
        }

        if (Physics.Raycast(transform.position, Quaternion.AngleAxis(30f, transform.up) * transform.forward, out hit, 4))//if raycast hits something within 5 on left
        {
            //Debug.DrawRay(transform.position, hit.point, Color.red, 2f);
            if (hit.collider.gameObject.tag == "wall")
            {
                print("hit detected angle");
                transform.Rotate(new Vector3(0, 180, 0), Time.deltaTime * 10f);
                // Vector3 newPos = RandomNavSphere(transform.position, wanderRadius, -1);

                // agent.SetDestination(newPos);
            }
        }

        if (Physics.Raycast(transform.position, Quaternion.AngleAxis(-30f, transform.up) * transform.forward, out hit, 4))//if raycast hits something within 5 on left
        {
           // Debug.DrawRay(transform.position, hit.point, Color.red, 2f);
            if (hit.collider.gameObject.tag == "wall")
            {
                print("hit detected angle");
                transform.Rotate(new Vector3(0, 180, 0), Time.deltaTime * 10f);
                //Vector3 newPos = RandomNavSphere(transform.position, wanderRadius, -1);

                //agent.SetDestination(newPos);
            }
        }
    }

    // Evade Steering Behaviour
    void evade()
    {

        //int iterationAhead = 30;

        //Vector3 targetFuturePosition = player.position + (playerSpeed * iterationAhead);
        //Vector3 direction = transform.position - targetFuturePosition;
        //direction.y = 0;
        //transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(direction), rotationSpeed * Time.deltaTime);
        //if (direction.magnitude < safeDistance)
        //{
        //    Vector3 moveVector = direction.normalized * moveSpeed * Time.deltaTime;
        //    transform.position += moveVector;

        //}

        int iterationAhead = 30;
        
        Vector3 targetFuturePosition = player.transform.position + (playerSpeed * iterationAhead);
        Vector3 direction = transform.position - targetFuturePosition; ;
        direction.y = 0;
        transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(direction), rotationSpeed * Time.deltaTime);
        if (direction.magnitude > minDistance)
        {
            Vector3 moveVector = direction.normalized * moveSpeed * Time.deltaTime;
            transform.position += moveVector;


        }
}
    }