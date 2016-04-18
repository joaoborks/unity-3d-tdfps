using UnityEngine;
using System.Collections;
using System.Linq;

public abstract class Turret : Buildable 
{
	enum States
    {
        None,
        Idle,
        Engaging
    }
    States state = States.None;

    public LayerMask raycastTarget;
    public Transform turretBase;
    public Transform turretWeapon;

    Collider[] enemies;
    Collider enemy;
    Animator anim;
    Vector3 detectPos;
    Vector3 dir;
    float detectRadius = 8f;

    protected override void Awake()
    {
        base.Awake();
        anim = GetComponent<Animator>();
        detectPos = transform.position + Vector3.up * 2.5f;
    }

    public void Activate()
    {
        state = States.Idle;
    }

    void FixedUpdate()
    {
        switch (state)
        {
            case States.None:
                return;
            case States.Idle:
                Idle();
                break;
            case States.Engaging:
                Engaging();
                break;
        }
    }

    void Idle()
    {
        if (Physics.CheckSphere(detectPos, detectRadius, raycastTarget))
        {
            state = States.Engaging;
        }
    }

    void Engaging()
    {
        enemies = Physics.OverlapSphere(detectPos, detectRadius, raycastTarget);
        if (enemies.Length > 0)
        {
            float minDist = detectRadius;
            float dist;
            foreach (Collider e in enemies)
            {
                dist = Vector3.Distance(e.transform.position, detectPos);
                if (dist < minDist)
                {
                    minDist = dist;
                    enemy = e;
                }
            }
            dir = enemy.transform.position - turretBase.position;
            turretBase.rotation = Quaternion.Lerp(turretBase.rotation, Quaternion.LookRotation(dir), Time.deltaTime * 10);
            turretBase.localEulerAngles = new Vector3(0, turretBase.localEulerAngles.y);
            // Turret aligned to target
            if (Vector3.Angle(turretBase.forward, dir) < 5f)
            {
                float angle;
                dir = enemy.transform.position - turretWeapon.position;
                turretWeapon.LookAt(enemy.transform);
                angle = turretWeapon.localEulerAngles.x;
                if (angle > 180)
                    angle -= 360;
                turretWeapon.localEulerAngles = new Vector3(Mathf.Clamp(angle, -45, 45), 0, 0);
                print(Vector3.Angle(turretWeapon.forward, dir));
            }
        }
        else
        {
            state = States.Idle;
        }
    }
}