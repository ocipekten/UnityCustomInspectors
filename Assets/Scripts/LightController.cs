using UnityEngine;

public class LightController : MonoBehaviour
{
    public float angle = 30f;
    public float range = 10f;

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;
        Vector3 forward = transform.forward;
        Vector3 endPoint = transform.position + forward * range;
        float radius = Mathf.Tan(angle * 0.5f * Mathf.Deg2Rad) * range;
        Gizmos.DrawWireSphere(endPoint, radius);
        Gizmos.DrawLine(transform.position, endPoint + transform.up * radius);
        Gizmos.DrawLine(transform.position, endPoint - transform.up * radius);
        Gizmos.DrawLine(transform.position, endPoint + transform.right * radius);
        Gizmos.DrawLine(transform.position, endPoint - transform.right * radius);
    }
}