using UnityEngine;
using System.Collections;

public class CanvasBillboard : MonoBehaviour
{
    Transform cam;

    void Awake()
    {
        cam = Camera.main.transform;
    }

    void Update()
    {
        transform.LookAt(2 * transform.position - cam.position);
    }
}
