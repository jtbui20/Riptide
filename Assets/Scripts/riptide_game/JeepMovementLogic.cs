using System.Collections;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Splines;

public class JeepMovementLogic : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created

    private Rigidbody rb;
    private NavMeshAgent agent;

    private SplineContainer destinationSpline;
    private SplineContainer exitSpline;

    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    /// <summary>
    /// Pass in 2 splines to setup the route for the jeep
    /// </summary>
    /// <param name="destinationSpline"></param>
    /// <param name="exitSpline"></param>
    public void SetupRoute(SplineContainer destinationSpline, SplineContainer exitSpline)
    {
        this.destinationSpline = destinationSpline;
        this.exitSpline = exitSpline;
    }

    public void EnterArea()
    {
        // Start a coroutine that will move the jeep along the destination spline
        if (destinationSpline != null)
        {
            StartCoroutine(MoveAlongSpline(destinationSpline));
        }
    }

    IEnumerator MoveAlongSpline(SplineContainer spline)
    {
        float duration = 5f; // Duration to complete the spline traversal
        float elapsedTime = 0f;
        while (elapsedTime < duration)
        {
            float t = elapsedTime / duration;
            Vector3 position = spline.EvaluatePosition(t);
            rb.MovePosition(position);
            elapsedTime += Time.deltaTime;
            yield return null;
        }
        rb.MovePosition(spline.EvaluatePosition(1f)); // Ensure final position is set
    }
}
