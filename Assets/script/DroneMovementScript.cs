using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DroneMovementScript : MonoBehaviour
{

    Rigidbody testDrone;

    void Awake()
    {
        testDrone = GetComponent<Rigidbody>();
        dronesound = gameObject.transform.Find("drone_sound").GetComponent<AudioSource>();

    }

    void FixedUpdate()
    {
        //MovementUpDown();
        MovementForward();
        Rotation();
        ClampingSpeedValue();
        Swerving();
        DroneSound();


        testDrone.AddRelativeForce(Vector3.up * upForce);
        testDrone.rotation = Quaternion.Euler(
                new Vector3(tiltAmountForward,currentYRotation, tiltAmountSideways)
            );
    }

    public float upForce;

    public Joystick joystick;

  
    
    void MovementUpDown()
    {
        if((Mathf.Abs(Input.GetAxis("Vertical"))>0.2f||Mathf.Abs(Input.GetAxis("Horizontal"))>0.2f))
        {
            if((Input.GetKey(KeyCode.I))||Input.GetKey(KeyCode.K))
            {
                testDrone.velocity = testDrone.velocity;
            }
            if(!Input.GetKey(KeyCode.I)&&!Input.GetKey(KeyCode.K)&&!Input.GetKey(KeyCode.J)&&!Input.GetKey(KeyCode.L))
            {
                testDrone.velocity = new Vector3(testDrone.velocity.x, Mathf.Lerp(testDrone.velocity.y, 0, Time.deltaTime * 5), testDrone.velocity.z);
                upForce = 281;
            }
            if(!Input.GetKey(KeyCode.I)&&!Input.GetKey(KeyCode.K)&&(Input.GetKey(KeyCode.J)||Input.GetKey(KeyCode.L)))
            {
                testDrone.velocity = new Vector3(testDrone.velocity.x, Mathf.Lerp(testDrone.velocity.y, 0, Time.deltaTime * 5), testDrone.velocity.z);
                upForce = 110;
            }
            if(Input.GetKey(KeyCode.J)||Input.GetKey(KeyCode.L))
            {
                upForce = 410;
            }
        }

        if(Mathf.Abs(Input.GetAxis("Vertical"))<0.2f&&Mathf.Abs(Input.GetAxis("Horizontal"))>0.2f)
        {
            upForce = 135;
        }

        if(Input.GetKey(KeyCode.I))
        {
            upForce = 450;
            if (Mathf.Abs(Input.GetAxis("Horizontal")) > 0.2f)
            {
                upForce = 500;
            }
        }
        else if(Input.GetKey(KeyCode.K))
        {
            upForce = -200;
        }
        else if(!Input.GetKey(KeyCode.I)&& !Input.GetKey(KeyCode.K)&& (Mathf.Abs(Input.GetAxis("Vertical"))<0.2f&&Mathf.Abs(Input.GetAxis("Horizontal"))<0.2f))
        {
             upForce = 98.1f;
        }
    }

    private float movementForwardSpeed = 500.0f;
    private float tiltAmountForward = 0;
    private float tiltVelocityForward;

    void MovementForward()
    {
        if(joystick.Vertical!=0)
        {
            testDrone.AddRelativeForce(Vector3.forward * joystick.Vertical * movementForwardSpeed);
            tiltAmountForward = Mathf.SmoothDamp(tiltAmountForward, 20 * joystick.Vertical, ref tiltVelocityForward, 0.1f);
        }
    }

    private float wantedYRotation;
    [HideInInspector]public float currentYRotation;
    private float rotateAmountByKeys = 2.5f;
    private float rotationYVelocity;

    void Rotation()
    {
        if(Input.GetKey(KeyCode.J))
        {
            wantedYRotation -= rotateAmountByKeys;
        }
        if(Input.GetKey(KeyCode.L))
        {
            wantedYRotation += rotateAmountByKeys;
        }
        currentYRotation = Mathf.SmoothDamp(currentYRotation, wantedYRotation, ref rotationYVelocity, 0.25f);
    }

    private Vector3 velocityToSmoothDampToZero;

    void ClampingSpeedValue()
    {
        if(Mathf.Abs(joystick.Vertical)>0.2f&&Mathf.Abs(joystick.Horizontal)>0.2f)
        {
            testDrone.velocity=Vector3.ClampMagnitude(testDrone.velocity,Mathf.Lerp(testDrone.velocity.magnitude,0.0f,Time.deltaTime*5f));
        }
        if(Mathf.Abs(joystick.Vertical) > 0.2f && Mathf.Abs(joystick.Horizontal) < 0.2f)
        {
            testDrone.velocity = Vector3.ClampMagnitude(testDrone.velocity, Mathf.Lerp(testDrone.velocity.magnitude, 0.0f, Time.deltaTime * 5f));
        }
        if (Mathf.Abs(joystick.Vertical) < 0.2f && Mathf.Abs(joystick.Horizontal) > 0.2f)
        {
            testDrone.velocity = Vector3.ClampMagnitude(testDrone.velocity, Mathf.Lerp(testDrone.velocity.magnitude, 0.0f, Time.deltaTime * 5f));
        }
        if(Mathf.Abs(joystick.Vertical) < 0.2f && Mathf.Abs(joystick.Horizontal) < 0.2f)
        {
            testDrone.velocity = Vector3.SmoothDamp(testDrone.velocity, Vector3.zero, ref velocityToSmoothDampToZero, 0.95f);
        }
    }


    private float sideMovementAmount = 300.0f;
    private float tiltAmountSideways;
    private float tiltAmountVelocity;

    void Swerving()
    {
        if(Mathf.Abs(joystick.Horizontal)>0.2f)
        {
            testDrone.AddRelativeForce(Vector3.right * joystick.Horizontal * sideMovementAmount);
            tiltAmountSideways = Mathf.SmoothDamp(tiltAmountSideways, -20 * joystick.Horizontal, ref tiltAmountVelocity, 0.1f);
        }
        else
        {
            tiltAmountSideways = Mathf.SmoothDamp(tiltAmountSideways, 0, ref tiltAmountVelocity, 0.1f);
        }
    }

    private AudioSource dronesound;
    void DroneSound()
    {
        dronesound.pitch = 1 + (testDrone.velocity.magnitude / 100);
    }

}
