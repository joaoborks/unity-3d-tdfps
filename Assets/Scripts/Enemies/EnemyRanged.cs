using UnityEngine;
using System.Collections;

public class EnemyRanged : Enemy
{
    protected override void Awake()
    {
        maxHealth = 80;
        defSpeed = 0.08f;
        atkCd = 0.5f;
        sightRange = 10f;
        rotSens = 0.05f;
        base.Awake();
    }

    protected override void CheckDisturbed()
    {
        cols = Physics.OverlapSphere(transform.position, 8, mask);
        if (cols.Length > 0)
        {
            float minDist = 15,
                dist;
            foreach (Collider c in cols)
            {
                dist = Vector3.Distance(transform.position, c.transform.position);
                if (dist < minDist)
                {
                    minDist = dist;
                    enemy = c;
                }
            }
            transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(enemy.transform.position - transform.position), rotSens);
            if (!disturbed)
            {
                disturbed = true;
                StartCoroutine(Attack());
            }
        }
        else
            disturbed = false;
    }
}