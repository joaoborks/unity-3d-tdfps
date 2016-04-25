using UnityEngine;
using System.Collections;

public class Blaster : Turret 
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
    public Transform[] cannons;

    ParticleSystem[] ps = new ParticleSystem[2];
    Collider[] enemies;
    Collider enemy;
    Animator anim;
    Vector3 detectPos;
    Vector3 dir;
    float detectRadius = 8f;
    float shotCooldown = 1.5f;
    float cannonMoveTime = 1f;
    float minCannonZ = -0.4f;
    float maxCannonZ = -0.9f;
    bool alternate;
    bool attackable = true;

    protected override void Awake()
    {
        base.Awake();
        for (int i = 0; i < cannons.Length; i++)
            ps[i] = cannons[i].GetComponentInChildren<ParticleSystem>(true);
        anim = GetComponent<Animator>();
        detectPos = transform.position + Vector3.up * 2.5f;
    }

    public void Activate()
    {
        state = States.Idle;
        anim.enabled = false;
    }

    void FixedUpdate()
    {
        switch (state)
        {
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
            state = States.Engaging;
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
            turretBase.rotation = Quaternion.Slerp(turretBase.rotation, Quaternion.LookRotation(dir), Time.deltaTime * 10);
            turretBase.localEulerAngles = new Vector3(0, turretBase.localEulerAngles.y);
            if (attackable && Vector3.Angle(turretBase.forward, dir) < 5f)
            {
                StartCoroutine(ShotCooldown());
                Shoot();
            }
            // Turret aligned to target
            dir = enemy.transform.position - turretWeapon.position;
            turretWeapon.rotation = Quaternion.Slerp(turretWeapon.rotation, Quaternion.LookRotation(dir), Time.deltaTime * 10);
            float angle = turretWeapon.localEulerAngles.x;
            turretWeapon.localEulerAngles = new Vector3(Mathf.Clamp(angle, -45, 45), 0, 0);            
        }
        else
        {
            state = States.Idle;
        }
    }

    void Shoot()
    {
        int i = alternate ? 0 : 1;
        alternate = !alternate;
        ps[i].gameObject.SetActive(true);
        ps[i].Play();
        StartCoroutine(EndShoot(i));
        StartCoroutine(MoveCannon(i));
    }

    IEnumerator ShotCooldown()
    {
        attackable = false;
        yield return new WaitForSeconds(shotCooldown);
        attackable = true;
    }

    IEnumerator EndShoot(int index)
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