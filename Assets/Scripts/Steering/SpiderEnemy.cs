using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpiderEnemy : MonoBehaviour
{
    public float maxSpeed = 6;
    public float maxAccel = 2;
    public float maxRotation = 80;
    public float maxAngularAccel = 25;

    public float orientation;
    public float rotation;
    public Vector3 velocity;
    protected Steering steering;

    public Animator animator;


    void Start()
    {
        velocity = Vector3.zero;
        steering = new Steering();
    }

    public void SetSteering(Steering steering)
    {
        this.steering = steering;
    }

    private void Awake()
    {
        animator = GetComponentInChildren<Animator>();
    }

    void Update()
    {
        Vector3 displacement = velocity * Time.deltaTime;
        orientation += rotation * Time.deltaTime;
       
        if (orientation < 0.0f)
            orientation += 360.0f;
        else if (orientation > 360.0f)
            orientation -= 360.0f;
        transform.Translate(displacement, Space.World);
        transform.rotation = new Quaternion();
        transform.Rotate(Vector3.up, orientation);
    }

    void LateUpdate()
    {
        velocity += steering.linear * Time.deltaTime;
        rotation += steering.angular * Time.deltaTime;
        if (velocity.magnitude > maxSpeed)
        {
            velocity.Normalize();
            velocity = velocity * maxSpeed;
        }
        if (steering.angular == 0.0f)
        {
            rotation = 0.0f;
        }
        if (Math.Abs(steering.linear.sqrMagnitude - 0.0f) < 0.001)
        {
            velocity = Vector3.zero;
        }
        steering = new Steering();
    }

}

public class Steering
{
    public float angular;
    public Vector3 linear;
    public Steering()
    {
        angular = 0.0f;
        linear = new Vector3();
    }
}
