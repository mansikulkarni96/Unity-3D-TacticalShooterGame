﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CollisionAvoidance : SpiderBehavior
{
    public float collisionRadius = 0.4f;
    GameObject[] spiders;

    void Start()
    {
        spiders = GameObject.FindGameObjectsWithTag("Spider");
    }

    public override Steering GetSteering()
    {
        Steering steering = new Steering();

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

            //timeToCollision /= relativeSpeed * relativeSpeed * -1;

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
