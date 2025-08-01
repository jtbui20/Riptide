using System.Collections;
using UnityEngine;

public class TrailAreaBehaviour : MonoBehaviour
{
    LineRenderer lineRenderer;

    public float TrailLingeringTime = 1;

    public bool isSafe = false;
    public bool shouldRemove = false;

    void Start()
    {
        lineRenderer = GetComponent<LineRenderer>();
    }

    public void IsConfirmed()
    {
        // Start the lingering timer
        SetClosed();
        StartCoroutine(LingeringTimer());
    }

    IEnumerator LingeringTimer()
    {
        yield return new WaitForSeconds(TrailLingeringTime);

        if (isSafe)
        {
            shouldRemove = true;
        }
    }

    void Update()
    {
        // Custom physics behaviour to check if an object has collided with the trail area

    }

    void SetClosed()
    {
        lineRenderer.startColor = Color.green;
        lineRenderer.endColor = Color.green;
        isSafe = true;
    }

    void SetBroken()
    {
        lineRenderer.startColor = Color.red;
        lineRenderer.endColor = Color.red;
        isSafe = false;
    }
}
