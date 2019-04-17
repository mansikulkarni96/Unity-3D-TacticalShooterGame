using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Align : SpiderBehavior
{
    //public float targetRadius = Mathf.Rad2Deg * Mathf.PI / 60;
    //public float slowRadius = Mathf.Rad2Deg * Mathf.PI / 15;
    //public float timeToTarget = 0.1f;

    public override Steering GetSteering()
    {
        Steering steering = new Steering();

        float targetOrientation = target.transform.rotation.eulerAngles.y;
        float rotation = targetOrientation - agent.orientation;

        rotation = MapToRange(rotation);
        Debug.Log(rotation);
        float rotationSize = Mathf.Abs(rotation);

        if (rotationSize < targetRadius)
            return steering;

        float targetRotation;
        if (rotationSize > slowRadius)
            targetRotation = agent.maxRotation;

        else
            targetRotation = agent.maxRotation * rotationSize / slowRadius;

        targetRotation *= rotation / rotationSize;
        steering.angular = targetRotation - agent.rotation;
        //steering.angular /= timeToTarget;

        float angularAccel = Mathf.Abs(steering.angular);
        Debug.Log(steering.angular);
        if (angularAccel > agent.maxAngularAccel)
        {
            steering.angular /= angularAccel;
            steering.angular *= agent.maxAngularAccel;
        }

        return steering;
    }
}
