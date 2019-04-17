using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpiderBehavior : MonoBehaviour
{
    public GameObject target;
    protected SpiderEnemy agent;

    private void Start()
    {
        spiders = GameObject.FindGameObjectsWithTag("Spider");
    }

    public virtual void Awake()
    {
        agent = gameObject.GetComponentInChildren<SpiderEnemy>();
    }

    public void Update()
    {
        agent.SetSteering(Steer());
    }

    public virtual Steering GetSteering()
    {
        return new Steering();
    }

    public float MapToRange(float rotation)
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

    Steering Steer()
    {
        Steering steering = new Steering();
        steering.angular = Align().angular;
        steering.linear = Arrive().linear;
        //CollisionAvoidance(steering);
        return steering;
    }

    public float rotTargetRadius = Mathf.Rad2Deg * Mathf.PI / 60;
    public float rotSlowRadius = Mathf.Rad2Deg * Mathf.PI / 15;
    public float timeToTarget = 0.1f;

    Steering Align()
    {
        Steering steering = new Steering();

        float targetOrientation = target.transform.rotation.eulerAngles.y;
        float rotation = targetOrientation - agent.orientation;

        rotation = MapToRange(rotation);
        float rotationSize = Mathf.Abs(rotation);

        if (rotationSize < rotTargetRadius)
            return steering;

        float targetRotation;
        if (rotationSize > rotSlowRadius)
            targetRotation = agent.maxRotation;

        else
            targetRotation = agent.maxRotation * rotationSize / rotSlowRadius;

        targetRotation *= rotation / rotationSize;
        steering.angular = targetRotation - agent.rotation;
        steering.angular /= timeToTarget;

        float angularAccel = Mathf.Abs(steering.angular);
        if (angularAccel > agent.maxAngularAccel)
        {
            steering.angular /= angularAccel;
            steering.angular *= agent.maxAngularAccel;
        }

        return steering;
    }


    public float targetRadius = 4;
    public float slowRadius = 10;

    Steering Arrive()
    {
        Steering steering = new Steering();

        Vector3 direction = target.transform.position - transform.position;
        float distance = direction.magnitude;

        float targetSpeed;
        if (distance < targetRadius)
            return steering;
        if (distance > slowRadius)
            targetSpeed = agent.maxSpeed;
        else
            targetSpeed = agent.maxSpeed * distance / slowRadius;

        Vector3 desiredVelocity = direction;
        desiredVelocity.Normalize();
        desiredVelocity *= targetSpeed;
        steering.linear = desiredVelocity - agent.velocity;
        steering.linear /= timeToTarget;
        if (steering.linear.magnitude > agent.maxAccel)
        {
            steering.linear.Normalize();
            steering.linear *= agent.maxAccel;
        }
        agent.animator.SetFloat("Speed", Mathf.Abs(steering.linear.magnitude));
        return steering;
    }

    public float collisionRadius = 2.0f;
    GameObject[] spiders;

    Steering CollisionAvoidance(Steering steering)
    {
        float shortestTime = Mathf.Infinity;
        GameObject firstTarget = null;
        float firstMinSeparation = 0.0f;
        float firstDistance = 0.0f;
        Vector3 firstRelativePos = Vector3.zero;
        Vector3 firstRelativeVel = Vector3.zero;


        foreach (GameObject s in spiders)
        {
            Vector3 relativePos = s.transform.position - transform.position;
            SpiderEnemy spider = s.GetComponent<SpiderEnemy>();
            Vector3 relativeVel = spider.velocity - agent.velocity;

            float relativeSpeed = relativeVel.magnitude;

            float timeToCollision = Vector3.Dot(relativePos, relativeVel);

            timeToCollision /= relativeSpeed * relativeSpeed * -1;

            float distance = relativePos.magnitude;

            float minSeparation = distance - relativeSpeed * timeToCollision;

            if (minSeparation > 2 * collisionRadius)
                continue;

            if (timeToCollision > 0.0f && timeToCollision < shortestTime)
            {
                shortestTime = timeToCollision;
                firstTarget = s;
                firstMinSeparation = minSeparation;
                firstRelativePos = relativePos;
                firstRelativeVel = relativeVel;
            }
        }

        if (firstTarget == null)
            return steering;

        if (firstMinSeparation <= 0.0f || firstDistance < 2 * collisionRadius)
            firstRelativePos = firstTarget.transform.position;

        else
            firstRelativePos += firstRelativeVel * shortestTime;

        firstRelativePos.Normalize();
        steering.linear = -firstRelativePos * agent.maxAccel;
        return steering;
    }
}
