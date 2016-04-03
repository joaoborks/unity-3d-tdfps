using UnityEngine;
using System.Collections;

public class Base : Buildable
{
    static GameObject wallPrefab;
    static GameObject dWallPrefab;
    static int baseMask;
    static int wallMask;

    protected override void Awake()
    {
        base.Awake();

        if (wallPrefab == null)
        {
            wallPrefab = Resources.Load<GameObject>("Prefabs/Buildables/Wall");
            dWallPrefab = Resources.Load<GameObject>("Prefabs/Buildables/WallD");
            baseMask = LayerMask.NameToLayer("Base");
            wallMask = LayerMask.NameToLayer("Wall");
        }
    }

	public void Attach()
    {
        Collider[] neighbours = Physics.OverlapBox(transform.position + Vector3.up, new Vector3(3, 0.1f, 3), Quaternion.identity, 1 << baseMask);

        float dist;
        Vector3 pos = transform.position + Vector3.up;
        Vector3 target;
        Vector3 dir;
        Quaternion rotation;
        foreach (Collider col in neighbours)
        {
            if (col.transform != transform)
            {
                target = col.transform.position + Vector3.up;
                dist = Vector3.Distance(pos, target);
                dir = target - pos;

                // Check for wall
                if (!Physics.Raycast(pos, dir, dist, 1 << wallMask))
                {
                    // Check for incoming wall
                    if (!Physics.Raycast(pos + dir / 2, Vector3.up, 50, 1 << wallMask))
                    {
                        // Diagonal Case
                        if (dir.x != 0 && dir.z != 0)
                        {
                            // Must rotate
                            if (dir.x != dir.z)
                                rotation = Quaternion.Euler(0, 90, 0);
                            else
                                rotation = Quaternion.identity;
                            Instantiate(dWallPrefab, transform.position + dir / 2, rotation);
                        }
                        else
                        {
                            // Must rotate
                            if (Mathf.Abs(dir.z) > Mathf.Abs(dir.x))
                                rotation = Quaternion.Euler(0, 90, 0);
                            else
                                rotation = Quaternion.identity;
                            Instantiate(wallPrefab, transform.position + dir.normalized * 2, rotation);
                        }
                    }
                }
            }
        }
    }
}
