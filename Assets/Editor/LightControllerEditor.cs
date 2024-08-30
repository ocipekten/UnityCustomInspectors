using static UnityEngine.GraphicsBuffer;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(LightController))]
public class LightControllerEditor : Editor
{
    private void OnSceneGUI()
    {
        LightController lightController = (LightController)target;

        // Creates a handle for adjusting the light's angle
        EditorGUI.BeginChangeCheck();
        float newAngle = Handles.RadiusHandle(Quaternion.identity, lightController.transform.position, lightController.angle);
        if (EditorGUI.EndChangeCheck())
        {
            Undo.RecordObject(lightController, "Change Light Angle");
            lightController.angle = newAngle;
        }

        // Creates a handle for adjusting the light's range
        EditorGUI.BeginChangeCheck();
        Vector3 rangePoint = lightController.transform.position + lightController.transform.forward * lightController.range;
        Vector3 newRangePoint = Handles.PositionHandle(rangePoint, Quaternion.identity);
        if (EditorGUI.EndChangeCheck())
        {
            Undo.RecordObject(lightController, "Change Light Range");
            lightController.range = Vector3.Distance(lightController.transform.position, newRangePoint);
        }
    }
}