using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrajectoryMarker : MonoBehaviour
{
    [SerializeField]
    private LayerMask groundLayer;

    private Rigidbody2D rb;

    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    private void OnDrawGizmos()
    {
        if (Application.isPlaying)
            PlotTrajectory(transform.position, rb.velocity, 0.1f, 2f);
    }

    public Vector2 PlotTrajectoryAtTime(Vector2 start, Vector2 startVelocity, float time)
    {
        return start + startVelocity * time + Physics2D.gravity * time * time * 0.5f;
    }

    public void PlotTrajectory(Vector2 start, Vector2 startVelocity, float timestep, float maxTime)
    {
        Vector2 prev = start;
        for (int i = 1; ; i++)
        {
            float t = timestep * i;
            if (t > maxTime) break;
            Vector2 pos = PlotTrajectoryAtTime(start, startVelocity, t);
            RaycastHit2D hit;
            hit = Physics2D.Linecast(prev, pos, groundLayer);
            if (hit.collider != null)
            {
                Gizmos.color = Color.blue;
                Gizmos.DrawSphere(hit.point, 1f);
                break;
            }
            Debug.DrawLine(prev, pos, Color.red);
            prev = pos;
        }
    }
}
