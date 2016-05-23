using UnityEngine;
using System.Collections;

public class EnemyMelee : Enemy
{
    protected override void Awake()
    {
        maxHealth = 180;
        defSpeed = 0.06f;
        atkCd = 1f;
        sightRange = 3f;
        rotSens = 0.05f;
        base.Awake();
    }

    protected override void CheckDisturbed()
    {
        cols = Physics.OverlapSphere(transform.position, 3, mask);
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
            transform.localEulerAngles = new Vector3(0, transform.localEulerAngles.y);
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
