using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WheelController : MonoBehaviour
{

    [SerializeField] WheelCollider leftFront;
    [SerializeField] WheelCollider rightFront;
    [SerializeField] WheelCollider leftRear;
    [SerializeField] WheelCollider rightRear;

    [SerializeField] Transform leftFrontTransform;
    [SerializeField] Transform rightFrontTransform;
    [SerializeField] Transform leftRearTransform;
    [SerializeField] Transform rightRearTransform;

    [SerializeField] Rigidbody rigidbody;

    public float acceleration = 500f;
    public float breakingForce = 300f;
    public float maxTurnAngle = 15f;

    private float currentAcceleration = 0f;
    private float currentBrakeForce = 0f;
    private float currentTurnAngle = 0f;

    private void FixedUpdate() {

        rigidbody.centerOfMass = new Vector3(0, -0.5f, 0);

        currentAcceleration = acceleration * Input.GetAxis("Vertical");

        if (Input.GetKey(KeyCode.Space))
            currentBrakeForce = breakingForce;
        else {
            currentBrakeForce = 0f;
            leftRear.motorTorque = currentAcceleration;
            rightRear.motorTorque = currentAcceleration;
        }

        leftFront.brakeTorque = currentBrakeForce;
        rightFront.brakeTorque = currentBrakeForce;
        leftRear.brakeTorque = currentBrakeForce;
        rightRear.brakeTorque = currentBrakeForce;

        currentTurnAngle = maxTurnAngle * Input.GetAxis("Horizontal");
        leftFront.steerAngle = currentTurnAngle;
        rightFront.steerAngle = currentTurnAngle;

        UpdateWheel(leftFront, leftFrontTransform);
        UpdateWheel(rightFront, rightFrontTransform);
        UpdateWheel(leftRear, leftRearTransform);
        UpdateWheel(rightRear, rightRearTransform);

    }

    void UpdateWheel(WheelCollider col, Transform trans) {

        Vector3 position;
        Quaternion rotation;
        col.GetWorldPose(out position, out rotation);

        trans.position = position;
        trans.rotation = rotation;

    }

}
