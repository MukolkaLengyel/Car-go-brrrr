using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI; // For Image
using TMPro; // For TextMeshPro

public class WheelController : MonoBehaviour

{

    [SerializeField] private Transform spawnPoint;
    [SerializeField] private float deathForceThreshold = 10f;
    [SerializeField] private float respawnDelay = 2f;

    [SerializeField] private Image tintImage;           // Assign Tint Image
    [SerializeField] private TextMeshProUGUI wastedText; // Assign WastedText
    [SerializeField] private float fadeDuration = 1f;    // Time to fade in/out

    [SerializeField] WheelCollider leftFront;
    [SerializeField] WheelCollider rightFront;
    [SerializeField] WheelCollider leftRear;
    [SerializeField] WheelCollider rightRear;

    [SerializeField] Transform leftFrontTransform;
    [SerializeField] Transform rightFrontTransform;
    [SerializeField] Transform leftRearTransform;
    [SerializeField] Transform rightRearTransform;

    [SerializeField] Rigidbody rigidbody;

    private bool isDead = false;

    public float acceleration = 500f;
    public float breakingForce = 300f;
    public float maxTurnAngle = 15f;

    private float currentAcceleration = 0f;
    private float currentBrakeForce = 0f;
    private float currentTurnAngle = 0f;


    private void Start()
    {
        isDead = false;
        if (tintImage != null) tintImage.color = new Color(tintImage.color.r, tintImage.color.g, tintImage.color.b, 0f); // Ensure tint starts transparent
        if (wastedText != null) wastedText.gameObject.SetActive(false); // Ensure text is hidden
    }

    private void FixedUpdate() {

        rigidbody.centerOfMass = new Vector3(0, -0.6f, 0);

        currentAcceleration = acceleration * Input.GetAxis("Vertical");

        if (Input.GetKey(KeyCode.Space))
        {
            currentBrakeForce = breakingForce;
            leftFront.motorTorque = 0f;
            rightFront.motorTorque = 0f;
            leftRear.motorTorque = 0f;
            rightRear.motorTorque = 0f;
        }
        else {
            currentBrakeForce = 0f;
            leftFront.motorTorque = currentAcceleration;
            rightFront.motorTorque = currentAcceleration;
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

    private void OnCollisionEnter(Collision collision)
    {
        // Check collision impact force
        float impactForce = collision.relativeVelocity.magnitude;
        if (impactForce > deathForceThreshold && !isDead)
        {
            Die();
        }
    }

    private void Die()
    {
        if (isDead) return;

        isDead = true;
        leftRear.motorTorque = 0f;
        rightRear.motorTorque = 0f;
        leftFront.brakeTorque = breakingForce;
        rightFront.brakeTorque = breakingForce;
        leftRear.brakeTorque = breakingForce;
        rightRear.brakeTorque = breakingForce;

        StartCoroutine(ShowDeathScreen());
    }

    private System.Collections.IEnumerator ShowDeathScreen()
    {
        // Fade in tint and show text
        if (tintImage != null && wastedText != null)
        {
            wastedText.gameObject.SetActive(true);
            tintImage.gameObject.SetActive(true);
            yield return StartCoroutine(FadeTint(0f, 0.7f)); // Fade to 70% opacity

            // Hold for a moment
            yield return new WaitForSeconds(respawnDelay - fadeDuration * 2); // Adjust total time

            // Fade out tint and hide text
            yield return StartCoroutine(FadeTint(0.7f, 0f));
            wastedText.gameObject.SetActive(false);
        }

        // Respawn after screen effect
        transform.position = spawnPoint.position;
        transform.rotation = spawnPoint.rotation;
        rigidbody.velocity = Vector3.zero;
        rigidbody.angularVelocity = Vector3.zero;
        leftFront.motorTorque = 0f;
        rightFront.motorTorque = 0f;
        leftRear.motorTorque = 0f;
        rightRear.motorTorque = 0f;
        leftFront.brakeTorque = breakingForce;
        rightFront.brakeTorque = breakingForce;
        leftRear.brakeTorque = breakingForce;
        rightRear.brakeTorque = breakingForce;
        isDead = false;
    }

    private System.Collections.IEnumerator FadeTint(float startAlpha, float endAlpha)
    {
        float elapsedTime = 0f;
        Color color = tintImage.color;

        while (elapsedTime < fadeDuration)
        {
            elapsedTime += Time.deltaTime;
            float alpha = Mathf.Lerp(startAlpha, endAlpha, elapsedTime / fadeDuration);
            tintImage.color = new Color(color.r, color.g, color.b, alpha);
            yield return null;
        }
        tintImage.color = new Color(color.r, color.g, color.b, endAlpha);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") && !isDead)
            Die();
    }

}
