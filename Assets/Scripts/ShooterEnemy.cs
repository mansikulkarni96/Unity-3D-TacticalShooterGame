using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using Unity.Collections;
using Unity.Jobs;
using UnityEngine;
using UnityEngine.AI;

public class ShooterEnemy : Agent
{
    const int MAX_HEALTH = 1000;
    public GameObject player;
    public GameObject agent;
    public Transform[] points;
    public Transform firePoint;
    public Transform rayPoint;
    public Transform tracePoint;
    public Transform exitPoint;
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
    bool isShooting;
    bool playerBehindWall;

    AudioSource source;
    public AudioClip fireSound;

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

    public bool IsShooting
    {
        get { return isShooting; }
        set { isShooting = value; }
    }
    public bool PlayerBehindWall
    {
        get { return playerBehindWall; }
        set { playerBehindWall = value; }
    }

    void Start()
    {
        currentHealth = MAX_HEALTH;
        agent = this.gameObject;
        bb.put("Player", player);
        bb.put("Agent", this.gameObject);
        chp = new ChaseHelpPatrol(bb);
        sphere = agent.GetComponent<SphereCollider>();
    }

    void Awake()
    {
        animator = GetComponentInChildren<Animator>();
        navAgent = GetComponent<NavMeshAgent>();
        source = GetComponent<AudioSource>();
    }

    void Update()
    {
        if (isShooting)
        {
            StartCoroutine("playAudio");
        }
        if (!isDead)
        {
            chp.Execute();
            animator.SetFloat("Speed", navAgent.velocity.magnitude);
        }
    }

    void OnTriggerStay(Collider col)
    {
        if (col.gameObject == player)
        {
            Vector3 dir = player.transform.position - agent.transform.position;
            float a = Vector3.Angle(dir, agent.transform.forward);

            RaycastHit hit0;


            // Angle between agent forward and player is < 90
            if (a < 90)
            {
                Debug.Log("< 90");
                RaycastHit hit;
                Vector3 v = new Vector3(0, 1f, 0);
                Ray ray = new Ray(rayPoint.position, dir.normalized);
                Debug.DrawRay(rayPoint.position, dir.normalized * 8, Color.red, 2f);
 
                // Player is not behind a wall
                if (Physics.Raycast(ray, out hit, sphere.radius))
                {

                    //Debug.Log("SHOOTING RAY");
                    if (hit.collider.gameObject == player)
                    {
                        playerBehindWall = false;
                        playerSpotted = true;
                        if (a < 1f)
                        {
                            playerInLineOfSight = true;
                        }
                        else
                        {
                            playerInLineOfSight = false;
                        }
                    }
                    else
                    {
                        playerBehindWall = true;
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

    public override void CheckHealth()
    {
        if (currentHealth <= 0 && !isDead)
        {
            isDead = true;
            animator.SetTrigger("isDead");
        }
    }

    // Co-routine used to play sound in parallel with shooting
    IEnumerator playAudio()
    {
        source.PlayOneShot(fireSound);
        yield return new WaitForSeconds(.3f);

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
    ShooterEnemy agentClass;

    public Help(Blackboard bb)
    {
        this.blackboard = bb;
        agent = (GameObject)this.blackboard.get("Agent");
        agentClass = agent.GetComponent<ShooterEnemy>();
    }

    public override bool Execute()
    {
        Collider[] hitColliders = Physics.OverlapSphere(agent.transform.position, 10);

        if (agentClass.InChase)
        {
            return false;
        }
        foreach (Collider c in hitColliders)
        {
            if (c.gameObject.tag == "enemy" && !c.gameObject.Equals(agent.gameObject))
            {
                ShooterEnemy ally = c.gameObject.GetComponent<ShooterEnemy>();
                if (ally.InChase)
                {
                    Debug.Log("HELPING");
                    agentClass.NavAgent.destination = ally.transform.position;
                    return true;
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
        if (!navAgent.pathPending
        && navAgent.remainingDistance <= navAgent.stoppingDistance
            && !agentClass.PlayerSpotted)
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
            if (agentClass.PlayerSpotted)
            {
                return true;
            }
        }
        agentClass.InChase = false;
        return false;
    }
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
        Debug.Log("CHASING");

        if (playerTransform == null)
        {
            return false;
        }

        if (agentClass.PlayerBehindWall || !agentClass.PlayerInLineOfSight)
        {
            navAgent.stoppingDistance = 2.5f;
        }
        else
        {
            navAgent.stoppingDistance = 4.0f;
        }

        Vector3 a = navAgent.pathEndPosition - playerTransform.position;
        bool largeRange = a.magnitude > navAgent.stoppingDistance;
        if ((navAgent.velocity.magnitude < 1 && largeRange) || !agentClass.InChase)
        {
            Debug.Log("CALC PATH");
            destination = playerTransform.position;
            navAgent.destination = destination;

        }
        animator.SetBool("isFiring", false);
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
    float r = 0;

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
        if (!playerClass.isDead && !agentClass.PlayerBehindWall)
        {
            Vector3 d = (player.transform.position - agent.transform.position).normalized;
            Quaternion newDirection = Quaternion.LookRotation(d);
            agent.transform.rotation = Quaternion.Slerp(agent.transform.rotation, newDirection, 10f * Time.deltaTime);
            return true;
        }
        return false;
    }

    float MapToRange(float rotation)
    {
        rotation %= 360.0f;
        if (Mathf.Abs(rotation) > 180.0f)
        {
            if (rotation < 0.0f)
                rotation += 360.0f;
            else
                rotation -= 360.0f;
        }
        return rotation;
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
        Debug.Log("SHOOTING");
        this.blackboard = bb;
        agent = (GameObject)this.blackboard.get("Agent");
        agentClass = agent.GetComponent<ShooterEnemy>();
        navAgent = agentClass.NavAgent;
        animator = agentClass.Anim;
        firePoint = agentClass.firePoint;
    }

    public override bool Execute()
    {

        agentClass.IsShooting = true;
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
    NavMeshAgent navAgent;
    Transform rayPoint;
    Transform firePoint;

    public InLineOfSight(Blackboard bb)
    {
        this.blackboard = bb;
        agent = (GameObject)this.blackboard.get("Agent");
        player = (GameObject)this.blackboard.get("Player");
        agentClass = agent.GetComponent<ShooterEnemy>();
        navAgent = agentClass.NavAgent;
        rayPoint = agentClass.rayPoint;
        firePoint = agentClass.firePoint;
    }

    public override bool Execute()
    {
        if (agentClass.PlayerInLineOfSight)
        {
            return true;
        }
        agentClass.IsShooting = false;
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
    Dictionary<string, UnityEngine.Object> dict;

    public Blackboard()
    {
        dict = new Dictionary<string, UnityEngine.Object>();
    }

    public UnityEngine.Object get(string key)
    {
        return dict[key];
    }

    public void put(string key, UnityEngine.Object val)
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