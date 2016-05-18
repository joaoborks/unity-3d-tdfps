using UnityEngine;
using System.Collections;

public class Tumper : Turret
{
    float damage = 5f,
        slowRatio = 0.4f,
        slowTime = 2f;

    protected override void Awake()
    {
        base.Awake();
        detectRadius = 8f;
        atkCooldown = 1f;
    }

    protected override void Engaging()
    {
        enemies = Physics.OverlapSphere(detectPos, detectRadius, raycastTarget);
        if (enemies.Length > 0)
        {
            if (attackable)
                anim.SetTrigger("atk");
        }
        else
            state = States.Idle;
    }

    protected override void Attack()
    {
        IDamageable dmgbl;
        ISlowable slwbl;
        foreach (Collider col in enemies)
        {
            dmgbl = col.GetComponent<IDamageable>();
            slwbl = col.GetComponent<ISlowable>();
            if (dmgbl != null)
                dmgbl.TakeDamage(damage);
            if (slwbl != null)
                slwbl.GetSlowed(slowTime, slowRatio);
        }
    }

    void Activate()
    {
        state = States.Idle;
    }

    void Execute()
    {
        Attack();
    }

    void EndAttack()
    {
        StartCoroutine(AttackCooldown());
    }
}
