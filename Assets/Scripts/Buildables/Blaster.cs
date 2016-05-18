using UnityEngine;
using System.Collections;

public class Blaster : Turret 
{
    public Transform origin,
        turretWeapon;
    public Transform[] cannons;

    ParticleSystem[] ps = new ParticleSystem[2];
    Collider enemy;
    Vector3 dir;
    float cannonMoveTime = 1f,
        minCannonZ = -0.4f,
        maxCannonZ = -0.9f,
        damage = 10f;
    bool alternate;
    int rotationSens = 10,
        minCannonX = -45,
        maxCannonX = 45;

    protected override void Awake()
    {
        base.Awake();
        for (int i = 0; i < cannons.Length; i++)
            ps[i] = cannons[i].GetComponentInChildren<ParticleSystem>(true);
        detectRadius = 8f;
        atkCooldown = 1.5f;
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
            turretWeapon.localEulerAngles = new Vector3(Mathf.Clamp(angle, minCannonX, maxCannonX), 0);            
        }
        else
            state = States.Idle;
    }

    protected override void Attack()
    {
        int i = alternate ? 0 : 1;
        alternate = !alternate;
        ps[i].gameObject.SetActive(true);
        ps[i].Play();
        if (Physics.Raycast(origin.position, enemy.transform.position - origin.position, detectRadius, raycastTarget))
        {
            IDamageable dmgbl = enemy.GetComponent<IDamageable>();
            if (dmgbl != null)
                dmgbl.TakeDamage(damage);
        }
        StartCoroutine(EndAttack(i));
        StartCoroutine(MoveCannon(i));
    }

    IEnumerator EndAttack(int index)
    {
        yield return new WaitForSeconds(ps[index].duration);
        ps[index].gameObject.SetActive(false);
    }

    IEnumerator MoveCannon(int index)
    {
        Transform t = cannons[index];
        Vector3 pos = t.localPosition;
        float amount = -(maxCannonZ - minCannonZ) / cannonMoveTime;
        pos.z = maxCannonZ;
        t.localPosition = pos;
        float time = 0;
        while (time < cannonMoveTime)
        {
            time += Time.fixedDeltaTime;
            yield return new WaitForFixedUpdate();
            t.Translate(0, 0, amount * Time.fixedDeltaTime);
        }
        pos.z = minCannonZ;
        t.localPosition = pos;
    }
}