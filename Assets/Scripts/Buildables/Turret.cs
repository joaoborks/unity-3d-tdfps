using UnityEngine;
using System.Collections;
using System.Linq;

public abstract class Turret : Building 
{
	protected enum States
    {
        None,
        Idle,
        Engaging
    }
    protected States state = States.None;

    public static LayerMask raycastTarget;

    protected Collider[] enemies;
    protected Transform turretBase;
    protected Animator anim;
    protected Vector3 detectPos;
    protected float detectRadius,
        atkCooldown;
    protected bool attackable = true;

    protected override void Awake()
    {
        base.Awake();
        raycastTarget = 1 << LayerMask.NameToLayer("Enemy");
        anim = GetComponent<Animator>();
        turretBase = transform.GetChild(0).GetChild(0);
        detectPos = turretBase.position;
    }

    protected virtual void FixedUpdate()
    {
        if (state == States.Idle)
            Idle();
        else if (state == States.Engaging)
            Engaging();
    }

    protected virtual void Idle()
    {
        if (Physics.CheckSphere(detectPos, detectRadius, raycastTarget))
            state = States.Engaging;
    }

    protected abstract void Engaging();
    protected abstract void Attack();

    protected IEnumerator AttackCooldown()
    {
        attackable = false;
        yield return new WaitForSeconds(atkCooldown);
        attackable = true;
    }
}