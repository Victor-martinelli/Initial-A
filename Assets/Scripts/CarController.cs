using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CarController : MonoBehaviour
{

    [Header("Car Settings")]
    public float driftFactor = 0.95f;
    public float accelerationFactor = 30.0f;
    public float turnFactor = 3.5f;
    public float maxSpeed = 20;

    //Local variables
    float accelerationInput = 0;
    float steeringInput = 0;

    float rotationAngle = 0;

    float velocityVsUp = 0;

    //Components
    Rigidbody2D carRigidBody2D;

    //Awake is called when the script instance is being loaded
    void Awake()
    {
        carRigidBody2D = GetComponent<Rigidbody2D>();
    }

    //Frame-rate independent for physics calculations
    void FixedUpdate()
    {
        ApplyEngineForce();

        killOrthogonalVelocity();

        ApplySteering();

    }

    void ApplyEngineForce()
    {

        //Calculate how forward we are going in terms of the direction of our velocity

        velocityVsUp = Vector2.Dot(transform.up, carRigidBody2D.velocity);

        //Limit so that we cannot go anymore forward than the limit set

        if (velocityVsUp > maxSpeed && accelerationInput > 0)
            return; //Don't apply anymore force

        //Limit so that we cannot fo faster than the 50% of the max speed in the reverse direction

        if (velocityVsUp < -maxSpeed * 0.5f && accelerationInput < 0)
            return; //Don't apply anymore force


        //Limit so that we cannot fo faster while accelerating

        if (carRigidBody2D.velocity.sqrMagnitude > maxSpeed * maxSpeed && accelerationInput > 0)
            return; //Don't apply anymore force

        //Apply drag if there is not accelerationInput so the car stops when the player let's go of the key

        if (accelerationInput == 0)
        {
            carRigidBody2D.drag = Mathf.Lerp(carRigidBody2D.drag, 3.0f, Time.fixedDeltaTime * 3);
        } else 
            carRigidBody2D.drag = 0;


        //Create a force for the engine
        Vector2 engineForceVector = transform.up * accelerationInput * accelerationFactor;

        //Apply force and push the car forward
        carRigidBody2D.AddForce(engineForceVector,ForceMode2D.Force);

    }

    void ApplySteering()
    {

        //Limit the car's ability to move when turning slowly
        float minSpeedBeforeAllowingTurningFactor = (carRigidBody2D.velocity.magnitude / 8);

        minSpeedBeforeAllowingTurningFactor = Mathf.Clamp01(minSpeedBeforeAllowingTurningFactor);

        //Update the rotation angle based on input
        rotationAngle -= steeringInput * turnFactor * minSpeedBeforeAllowingTurningFactor;

        //Apply steering by rotating the car object
        carRigidBody2D.MoveRotation(rotationAngle);
    }

    void killOrthogonalVelocity()
    {
        Vector2 forwardVelocity = transform.up * Vector2.Dot(carRigidBody2D.velocity,transform.up);
        Vector2 rightVelocity = transform.right * Vector2.Dot(carRigidBody2D.velocity, transform.right);

        carRigidBody2D.velocity = forwardVelocity + rightVelocity * driftFactor;
    }

    float getLateralVelocity()
    {
        //Returns how fast the car is moving sideways
        return Vector2.Dot(transform.right, carRigidBody2D.velocity);
    }

    public void setInputVector(Vector2 inputVector)
    {
        steeringInput = inputVector.x;
        accelerationInput = inputVector.y;

    }

    public bool isDrifting(out float lateralVelocity, out bool isBraking)
    {
        lateralVelocity = getLateralVelocity();
        isBraking = false;

        //Check if we are moving forward and the player is hitting the brakes. In this case they are drifting
        if(accelerationInput <0 && velocityVsUp > 0)
        {
            isBraking = true;
            return true;
        }


        //If we have a lot of side movement then we are drifting as well
        if(Mathf.Abs(getLateralVelocity()) > 4.0f)
        {
            return true;
        }

        return false;
    }

}
