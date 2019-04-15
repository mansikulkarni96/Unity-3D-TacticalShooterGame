using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class ShooterEnemy : MonoBehaviour
{
    public GameObject player;
    public GameObject agent;
    public Transform[] points;
    public Transform firePoint;
    public Transform rayPoint;
    public Transform tracePoint;
    public GameObject bullet;
    public int fireSpeed;
    NavMeshAgent navAgent;
    Animator animator;
    Blackboard bb = new Blackboard();
    Task chp;
    bool inChase;
    SphereCollider sphere;
    bool playerSpotted;
    bool playerInLineOfSight;


    public NavMeshAgent NavAgent
    {
        get { return navAgent; }
        set { navAgent = value; }
    }

    public Animator Anim
    {
        get { return animator; }
        set { animator = value; }
    }

    public bool InChase
    {
        get { return inChase; }
        set { inChase = value; }
    }

    public bool PlayerSpotted
    {
        get { return playerSpotted; }
        set { playerSpotted = value; }
    }

    public bool PlayerInLineOfSight
    {
        get { return playerInLineOfSight; }
        set { playerInLineOfSight = value; }
    }

    void Start()
    {
        agent = this.gameObject;
        navAgent = GetComponent<NavMeshAgent>();
        bb.put("Player", player);
        bb.put("Agent", this.gameObject);
        chp = new Chase(bb);
        animator = GetComponentInChildren<Animator>();
        sphere = agent.GetComponent<SphereCollider>();
    }

    void Awake()
    {
        animator = GetComponentInChildren<Animator>();
    }

    void Update()
    {
        chp.Execute();
    }

    void OnTriggerStay(Collider col)
    {
        if (col.gameObject == player)
        {
            Vector3 dir = player.transform.position - agent.transform.position;
            float a = Vector3.Angle(dir, agent.transform.forward);
            Debug.DrawRay(tracePoint.position, agent.transform.forward * 8, Color.green, 2f);
            Debug.DrawRay(tracePoint.position, dir * 8, Color.green, 2f);
            // Angle between agent forward and player is < 55
            if (a < 90)
            {
                RaycastHit hit;
                Ray ray = new Ray(agent.transform.position + agent.transform.up, dir.normalized);
                // Player is not behind a wall
                if (Physics.Raycast(ray, out hit, sphere.radius))
                {
                    playerInLineOfSight = false;
                    if (hit.collider.gameObject == player)
                    {
                        playerSpotted = true;

                        if (a < 0.5f)
                        {
                            playerInLineOfSight = true;
                        }
                     }
                }
            }
        }
    }

    private void OnTriggerExit(Collider col)
    {
        if (col.gameObject == player)
        {
            playerSpotted = false;
            playerInLineOfSight = false;
        }
    }
}

class ChaseHelpPatrol : Selector
{
    public ChaseHelpPatrol(Blackboard bb) : base(bb)
    {
        children.Add(new RangeChaseShoot(this.blackboard));
        children.Add(new Help(this.blackboard));
        children.Add(new Patrol(this.blackboard));
    }

    public override bool Execute()
    {
        Selector s = new Selector(this.blackboard, this.children);
        return s.Execute();
    }
}

class RangeChaseShoot : Sequence
{
    public RangeChaseShoot(Blackboard bb) : base(bb)
    {
        children.Add(new IsWithinRange(this.blackboard));
        children.Add(new Chase(this.blackboard));
        children.Add(new AimAndShoot(this.blackboard));
    }

    public override bool Execute()
    {
        Sequence s = new Sequence(this.blackboard, this.children);
        return s.Execute();
    }
}

class AimAndShoot : Selector
{
    public AimAndShoot(Blackboard bb) : base(bb)
    {
        children.Add(new LineAndShoot(this.blackboard));
        children.Add(new Aim(this.blackboard));
    }

    public override bool Execute()
    {
        Selector s = new Selector(this.blackboard, this.children);
        return s.Execute();
    }
}

class LineAndShoot : Sequence
{
    public LineAndShoot(Blackboard bb) : base(bb)
    {
        children.Add(new InLineOfSight(this.blackboard));
        children.Add(new Shoot(this.blackboard));
    }

    public override bool Execute()
    {
        Sequence s = new Sequence(this.blackboard, this.children);
        return s.Execute();
    }
}



// TASKS
class Help : Task
{
    GameObject agent;

    public Help(Blackboard bb)
    {
        this.blackboard = bb;
        agent = (GameObject)this.blackboard.get("Agent");
    }

    public override bool Execute()
    {
        Collider[] hitColliders = Physics.OverlapSphere(agent.transform.position, 10);

        foreach (Collider c in hitColliders)
        {
            if (c.gameObject.tag == "enemy")
            {
                GameObject ally = c.gameObject;
                ShooterEnemy a = ally.GetComponent<ShooterEnemy>();
                if (a.InChase)
                {
                    a.NavAgent.destination = agent.transform.position;
                }
            }
        }
        return false;
    }
}

class Patrol : Task
{
    GameObject agent;
    ShooterEnemy agentClass;
    NavMeshAgent navAgent;
    Animator animator;
    Transform[] points;
    int destPoint = 0;

    public Patrol(Blackboard bb)
    {
        this.blackboard = bb;
        agent = (GameObject)this.blackboard.get("Agent");
        agentClass = agent.GetComponent<ShooterEnemy>();
        navAgent = agentClass.NavAgent;
        animator = agentClass.Anim;
        points = agentClass.points;
    }

    public override bool Execute()
    {
        if (points.Length == 0)
            return false;

        // Choose the next destination point when the agent gets
        // close to the current one.
        if (!navAgent.pathPending && navAgent.remainingDistance <= navAgent.stoppingDistance)
        {
            GotoNextPoint();
            return true;
        }

        return false;
    }

    void GotoNextPoint()
    {

        // Set the agent to go to the currently selected destination.
        navAgent.destination = points[destPoint].position;
        animator.SetFloat("Speed", 1);

        // Choose the next point in the array as the destination,
        // cycling to the start if necessary.
        destPoint = (destPoint + 1) % points.Length;
    }
}

class IsWithinRange : Task
{
    GameObject player;
    Transform playerTransform;
    Player playerClass;
    GameObject agent;
    Transform agentTransform;
    ShooterEnemy agentClass;
    NavMeshAgent agentNav;
    Transform tracePoint;
    SphereCollider sphere;
    bool playerSpotted;

    public IsWithinRange(Blackboard bb)
    {
        this.blackboard = bb;
        player = (GameObject)this.blackboard.get("Player");
        playerClass = player.GetComponent<Player>();
        agent = (GameObject)this.blackboard.get("Agent");
        playerTransform = player.transform;
        agentTransform = agent.transform;
        agentClass = agent.GetComponent<ShooterEnemy>();
        tracePoint = agentClass.tracePoint;
        agentNav = agentClass.NavAgent;
        sphere = agentClass.GetComponent<SphereCollider>();
    }

    public override bool Execute()
    {
        if (!playerClass.isDead && player != null)
        {
            //RaycastHit hit;
            //float d = Vector3.Distance(playerTransform.position, agentTransform.position);

            //if(d <= 8)
            //{

            //    tracePoint.LookAt(playerTransform);
            //    Ray ray = new Ray(tracePoint.position, tracePoint.forward.normalized);

            //    if (Physics.Raycast(ray, out hit, 100))
            //    {
            //        Debug.Log(hit.collider.tag);
            //        if (hit.collider.tag == "Player")
            //        {
            //            agentNav.path = null;
            //            Debug.DrawRay(tracePoint.position, tracePoint.forward * 8, Color.green, 2f);
            //            return true;
            //        }

            //    }
            //}
            if (agentClass.PlayerSpotted)
            {
                return true;
            }
        }
        agentClass.InChase = false;
        return false;
    }

    //void OnTriggerStay(Collider col)
    //{
    //    if (col.gameObject == player)
    //    {
    //        Vector3 dir = player.transform.position - agent.transform.position;
    //        float a = Vector3.Angle(dir, agent.transform.forward);
    //        Debug.DrawRay(tracePoint.position, tracePoint.forward * 8, Color.green, 2f);
    //        if (a < 55)
    //        {
    //            RaycastHit hit;
    //            Ray ray = new Ray(agentTransform.position + agentTransform.up, dir.normalized);
    //            if (Physics.Raycast(ray, out hit,  sphere.radius))
    //            {
    //                if (hit.collider.gameObject == player)
    //                {
    //                    playerSpotted = true;
    //                }
    //            }
    //        }
    //    }
    //}
}

class Chase : Task
{
    Vector3 destination = new Vector3(0, 0, 0);
    GameObject player;
    GameObject agent;
    ShooterEnemy agentClass;
    NavMeshAgent navAgent;
    Animator animator;
    Transform playerTransform;
    float time = 0f;
    float timeToCheck = 1f;

    public Chase(Blackboard bb)
    {
        this.blackboard = bb;
        player = (GameObject)this.blackboard.get("Player");
        agent = (GameObject)this.blackboard.get("Agent");
        agentClass = agent.GetComponent<ShooterEnemy>();
        navAgent = agentClass.NavAgent;
        animator = agentClass.Anim;
        playerTransform = player.transform;
    }

    public override bool Execute()
    {
        float dist = Vector2.Distance(destination, playerTransform.position);
        Debug.DrawRay(agent.transform.position, agent.transform.forward * dist, Color.white, 2f);
        float d = navAgent.stoppingDistance;
        if (playerTransform == null)
        {
            return false;
        }
        time += Time.deltaTime;
        if (dist>d)
        {
            destination = playerTransform.position;
            navAgent.destination = destination;
            time = 0f;
        }
        animator.SetBool("isFiring", false);
        animator.SetFloat("Speed", navAgent.velocity.magnitude);
        agentClass.InChase = true;

        return true;
    }
}

class Aim : Task
{
    GameObject player;
    GameObject agent;
    ShooterEnemy agentClass;
    NavMeshAgent navAgent;
    Player playerClass;

    public Aim(Blackboard bb)
    {
        this.blackboard = bb;
        player = (GameObject)this.blackboard.get("Player"); 
        agent = (GameObject)this.blackboard.get("Agent");
        agentClass = agent.GetComponent<ShooterEnemy>();
        navAgent = agentClass.NavAgent;
        playerClass = player.GetComponent<Player>();
    }

    public override bool Execute()
    {

        float dist = Vector3.Distance(agent.transform.position, player.transform.position);
        bool isStopped = navAgent.velocity.magnitude <= 3f;

        if (!playerClass.isDead && isStopped)
        {
            agent.transform.LookAt(player.transform);
            return true;
        }
        //Debug.DrawLine(agent.transform.position, agent.transform.right * 10, Color.blue, 2f);
        return false;
    }
}

class Shoot : Task
{
    GameObject agent;
    ShooterEnemy agentClass;
    NavMeshAgent navAgent;
    Transform firePoint;
    Animator animator;

    public Shoot(Blackboard bb)
    {
        this.blackboard = bb;
        agent = (GameObject)this.blackboard.get("Agent");
        agentClass = agent.GetComponent<ShooterEnemy>();
        navAgent = agentClass.NavAgent;
        animator = agentClass.Anim;
        firePoint = agentClass.firePoint;
    }

    public override bool Execute()
    {
        //navAgent.isStopped = true;
        GameObject instBullet = GameObject.Instantiate(agentClass.bullet, firePoint.position, firePoint.rotation) as GameObject;
        Rigidbody instBulletRigidbody = instBullet.GetComponent<Rigidbody>();
        instBulletRigidbody.AddForce(firePoint.forward * agentClass.fireSpeed);
        MonoBehaviour.Destroy(instBullet, 0.5f);
        animator.SetBool("isFiring", true);
        return true;
    }
}

class InLineOfSight : Task
{
    GameObject player;
    GameObject agent;
    ShooterEnemy agentClass;
    Transform rayPoint;
    Transform firePoint;

    public InLineOfSight(Blackboard bb)
    {
        this.blackboard = bb;
        agent = (GameObject)this.blackboard.get("Agent");
        player = (GameObject)this.blackboard.get("Player");
        agentClass = agent.GetComponent<ShooterEnemy>();
        rayPoint = agentClass.rayPoint;
        firePoint = agentClass.firePoint;
    }

    public override bool Execute()
    {
        //RaycastHit hit;
        //Ray ray = new Ray(rayPoint.position, rayPoint.forward);


        //if (Physics.Raycast(ray, out hit, 6))
        //{
        //    if (hit.collider.tag == player.tag)
        //    {
        //        Debug.DrawRay(rayPoint.position, rayPoint.forward * 10, Color.red, 2f);
        //        return true;
        //    }
        //}
        if (agentClass.PlayerInLineOfSight)
        {
            return true;
        }
        return false;
    }
}

abstract class Task
{
    public Blackboard blackboard;
    public abstract bool Execute();
}

class Blackboard
{
    Dictionary<string, Object> dict;

    public Blackboard()
    {
        dict = new Dictionary<string, Object>();
    }

    public Object get(string key)
    {
        return dict[key];
    }

    public void put(string key, Object val)
    {
        dict[key] = val;
    }
}

class Sequence : Task
{
    public List<Task> children = new List<Task>();

    public Sequence(Blackboard bb)
    {
        this.blackboard = bb;
    }

    public Sequence(Blackboard bb, List<Task> ts)
    {
        this.blackboard = bb;
        this.children = ts;
    }

    public override bool Execute()
    {
        foreach (Task t in children)
        {
            if (t.Execute() == false)
            {
                return false;
            }
        }
        return true;
    }
}

class Selector : Task
{
    public List<Task> children = new List<Task>();

    public Selector(Blackboard bb)
    {
        this.blackboard = bb;
    }

    public Selector(Blackboard bb, List<Task> ts)
    {
        this.blackboard = bb;
        this.children = ts;
    }

    public override bool Execute()
    {
        foreach (Task t in children)
        {
            if (t.Execute() == true)
            {
                return true;
            }
        }
        return false;
    }
}