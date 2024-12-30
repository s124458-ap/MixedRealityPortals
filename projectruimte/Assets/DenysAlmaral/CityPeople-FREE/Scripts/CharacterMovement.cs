using UnityEngine;

public class CharacterMovement : MonoBehaviour
{
    public float moveSpeed = 2f;

    // Path points
    public Vector3[] pathPoints;
    private int currentPointIndex = 0;
    public float reachDistance = 0.1f;

    void Start()
    {
        // Only initialize path points if none are set
        if (pathPoints == null || pathPoints.Length == 0)
        {
            // Define a simple rectangular path
            pathPoints = new Vector3[]
            {
                new Vector3(0, 0, 0),
                new Vector3(10, 0, 0),
                new Vector3(10, 0, 10),
                new Vector3(0, 0, 10)
            };
        }

        // Set initial position to first path point
        if (pathPoints.Length > 0)
        {
            transform.position = pathPoints[0];
        }
    }

    // Rest of the code remains the same
    void Update()
    {
        // Get the next target point
        Vector3 targetPoint = pathPoints[currentPointIndex];

        // Calculate direction to the target
        Vector3 moveDirection = (targetPoint - transform.position).normalized;

        // Check if we've reached the current point
        if (Vector3.Distance(transform.position, targetPoint) < reachDistance)
        {
            // Move to next point (loop back to start if at end)
            currentPointIndex = (currentPointIndex + 1) % pathPoints.Length;
        }
        else
        {
            // Rotate character to face movement direction
            transform.rotation = Quaternion.LookRotation(moveDirection);

            // Move character
            transform.position += moveDirection * moveSpeed * Time.deltaTime;
        }
    }

    void OnDrawGizmos()
    {
        if (pathPoints == null || pathPoints.Length == 0) return;

        Gizmos.color = Color.yellow;
        for (int i = 0; i < pathPoints.Length; i++)
        {
            Gizmos.DrawSphere(pathPoints[i], 0.3f);
            if (i < pathPoints.Length - 1)
                Gizmos.DrawLine(pathPoints[i], pathPoints[i + 1]);
        }
        // Connect last point to first point
        Gizmos.DrawLine(pathPoints[pathPoints.Length - 1], pathPoints[0]);
    }
}