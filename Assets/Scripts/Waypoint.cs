using UnityEngine;
using System.Collections.Generic;

public class Waypoint : MonoBehaviour
{
    public List<Waypoint> connectedWaypoints = new List<Waypoint>();

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.blue;
        Gizmos.DrawSphere(transform.position, 0.5f);

        Gizmos.color = Color.yellow;
        foreach (var connectedWaypoint in connectedWaypoints)
        {
            if (connectedWaypoint != null)
            {
                Gizmos.DrawLine(transform.position, connectedWaypoint.transform.position);
            }
        }
    }
}

[ExecuteInEditMode]
public class WaypointManager : MonoBehaviour
{
    public List<Waypoint> waypoints = new List<Waypoint>();

    private void Update()
    {
        if (!Application.isPlaying)
        {
            waypoints.Clear();
            waypoints.AddRange(FindObjectsOfType<Waypoint>());
        }
    }
}