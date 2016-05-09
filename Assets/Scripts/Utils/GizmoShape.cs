using UnityEngine;
using System.Collections;

public class GizmoShape : MonoBehaviour
{
    public GizmoShapes gizmoShape;
    public Color gizmoColor;
    public Transform referenceTransform;
    public Vector3 offset;
    public Vector3 size;

    public enum GizmoShapes
    {
        Cube,
        Sphere,
        WireCube,
        WireSphere
    }

    void OnDrawGizmos()
    {
        Gizmos.color = gizmoColor;
        if (referenceTransform != null)
            Gizmos.matrix = Matrix4x4.TRS(referenceTransform.position, referenceTransform.rotation, Vector3.one);
        switch (gizmoShape)
        {
            case GizmoShapes.Cube:
                Gizmos.DrawCube(offset, size);
                break;
            case GizmoShapes.Sphere:
                Gizmos.DrawSphere(offset, Mathf.Max(size.x, size.y, size.z));
                break;
            case GizmoShapes.WireCube:
                Gizmos.DrawWireCube(offset, size);
                break;
            case GizmoShapes.WireSphere:
                Gizmos.DrawWireSphere(offset, Mathf.Max(size.x, size.y, size.z));
                break;
        }
    }
}