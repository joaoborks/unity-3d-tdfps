using UnityEngine;
using System.Collections;

public class Laser : Turret
{
    public Transform turretWeapon;

    Collider enemy;
    Vector3 dir;
    int rotationSens = 10;
    int minGunX = -25;
    int maxGunX = 30;

    protected override void Awake()
    {
        base.Awake();
        detectRadius = 8f;
        atkCooldown = 2f;
    }

    void Activate()
    {
        state = States.Idle;
        anim.enabled = false;
    }

    protected override void Engaging()
    {
        enemies = Physics.OverlapSphere(detectPos, detectRadius, raycastTarget);
        if (enemies.Length > 0)
        {
            float maxDist = 0;
            float dist;
            foreach (Collider e in enemies)
            {
                dist = Vector3.Distance(e.transform.position, detectPos);
                if (dist > maxDist)
                {
                    maxDist = dist;
                    enemy = e;
                }
            }
            // Horizontal Rotation
            dir = enemy.transform.position - turretBase.position;
            dir.y = 0;
            turretBase.rotation = Quaternion.Slerp(turretBase.rotation, Quaternion.LookRotation(dir), Time.deltaTime * rotationSens);
            if (attackable && Vector3.Angle(turretBase.forward, dir) < 1)
            {
                StartCoroutine(AttackCooldown());
                Attack();
            }
            // Turret aligned to target
            dir = enemy.transform.position - turretWeapon.position;
            turretWeapon.rotation = Quaternion.Slerp(turretWeapon.rotation, Quaternion.LookRotation(dir), Time.deltaTime * rotationSens);
            float angle = turretWeapon.localEulerAngles.x;
            if (angle > 180)
                angle -= 360;
            turretWeapon.localEulerAngles = new Vector3(Mathf.Clamp(angle, minGunX, maxGunX), 0);
        }
        else
        {
            state = States.Idle;
        }
    }

    protected override void Attack()
    {
        print("<color=purple>Laser Attack!</color>");
    }
}
