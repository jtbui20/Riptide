using UnityEngine;

public class GearSpinBehaviour : MonoBehaviour
{
    [SerializeField]
    private Vector3 _initialVelocity; // Remove once we get a launcher script ready
    public Rigidbody rb;

    public Transform centerOfFieldPosition;
    public float GravitationalForceMultiplier = 0.5f;
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        InitialLaunchVector(_initialVelocity);
    }


    public void InitialLaunchVector(Vector3 initialVelocity)
    {
        rb.linearVelocity = initialVelocity;
        rb.AddTorque(Vector3.up * 5000f, ForceMode.Impulse); // Add some initial torque for spinning
    }

    // Update is called once per frame
    void Update()
    {
        // Get the vector from the center of field position to the gear's position
        Vector3 direction = centerOfFieldPosition.position - transform.position;
        // Apply force to the gear in the direction of the vector that is a proportion to gravity


        // rb.AddForce(direction.normalized * Physics.gravity.magnitude * GravitationalForceMultiplier, ForceMode.Force);
    }

    public void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawLine(transform.position, transform.position + _initialVelocity);
    }
}
