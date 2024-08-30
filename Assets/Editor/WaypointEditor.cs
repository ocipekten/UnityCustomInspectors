using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(Waypoint))]
public class WaypointEditor : Editor
{
    private Waypoint waypoint;

    private void OnEnable()
    {
        waypoint = (Waypoint)target;
    }

    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        if (GUILayout.Button("Connect to Nearest Waypoint"))
        {
            ConnectToNearestWaypoint();
        }
    }

    private void ConnectToNearestWaypoint()
    {
        Waypoint[] allWaypoints = FindObjectsOfType<Waypoint>();
        Waypoint nearest = null;
        float minDistance = float.MaxValue;

        foreach (var other in allWaypoints)
        {
            if (other != waypoint)
            {
                float distance = Vector3.Distance(waypoint.transform.position, other.transform.position);
                if (distance < minDistance)
                {
                    minDistance = distance;
                    nearest = other;
                }
            }
        }

        if (nearest != null)
        {
            if (!waypoint.connectedWaypoints.Contains(nearest))
            {
                waypoint.connectedWaypoints.Add(nearest);
                if (!nearest.connectedWaypoints.Contains(waypoint))
                {
                    nearest.connectedWaypoints.Add(waypoint);
                }
                Debug.Log($"Connected {waypoint.name} to {nearest.name}");
            }
            else
            {
                Debug.Log("Already connected to the nearest waypoint.");
            }
        }
        else
        {
            Debug.Log("No other waypoints found.");
        }
    }

    private void OnSceneGUI()
    {
        Handles.color = Color.red;
        foreach (var connectedWaypoint in waypoint.connectedWaypoints)
        {
            if (connectedWaypoint != null)
            {
                Handles.DrawDottedLine(waypoint.transform.position, connectedWaypoint.transform.position, 2f);
            }
        }
    }
}