using UnityEngine;
using System.Collections;

public class Building : MonoBehaviour
{
    MeshRenderer[] renderers;

    protected virtual void Awake()
    {
        renderers = GetComponentsInChildren<MeshRenderer>();
        Hide();
    }

    public void Hide()
    {
        foreach (MeshRenderer r in renderers)
            r.enabled = false;
    }

    public void Show()
    {
        foreach (MeshRenderer r in renderers)
            r.enabled = true;
    }

    public void Arrive()
    {
        ParticleSystem arrive = transform.FindChild("Arrive").GetComponent<ParticleSystem>();
        arrive.Play();
        Destroy(arrive.gameObject, arrive.startLifetime);
    }
}
