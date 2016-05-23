using UnityEngine;
using System.Collections;

public class EnemyRanged : MonoBehaviour, IDamageable, ISlowable, IPathWalkable
{
    public LayerMask mask;

    ParticleSystem ps;
    Transform[] path;
    Transform target,
        nucleum;
    Coroutine slowCooldown;
    Collider[] cols;
    Collider enemy;
    Vector3 curWaypoint;
    float maxHealth = 70,
        health,
        defSpeed = 0.08f,
        speed,
        sightRange = 10f,
        rotSens = 0.05f,
        atkCd = 0.5f;
    bool disturbed;
    int curWPIndex;

    void Awake()
    {
        ps = GetComponentInChildren<ParticleSystem>();
        target = FindObjectOfType<PlayerPhysics>().transform;
        nucleum = FindObjectOfType<Nucleum>().transform;
        health = maxHealth;
        speed = defSpeed;
    }

    void FixedUpdate()
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

        if (!disturbed && path != null)
        {
            curWaypoint.y = transform.position.y;
            if (Vector3.Distance(transform.position, curWaypoint) < 1)
            {
                curWPIndex++;
                if (curWPIndex >= path.Length)
                    path = null;
                else
                    curWaypoint = path[curWPIndex].position;
            }
            transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(curWaypoint - transform.position), rotSens);
            transform.localEulerAngles = new Vector3(0, transform.localEulerAngles.y);
            transform.Translate(Vector3.forward * speed);
        }
    }

    public void SetPath(Transform[] path)
    {
        this.path = path;
        curWPIndex = 0;
        curWaypoint = path[curWPIndex].position;
    }

    public void TakeDamage(float damage)
    {
        health -= damage;
        if (health <= 0)
            Die();
    }

    public void Die()
    {
        WaveController.curEnemies--;
        Destroy(gameObject);
    }

    public void GetSlowed(float dur, float ratio)
    {
        speed = defSpeed * ratio;
        if (slowCooldown != null)
            StopCoroutine(slowCooldown);
        slowCooldown = StartCoroutine(SlowCooldown(dur));
    }

    public IEnumerator SlowCooldown(float time)
    {
        yield return new WaitForSeconds(time);
        speed = defSpeed;
    }

    IEnumerator Attack()
    {
        while (disturbed)
        {
            yield return new WaitForSeconds(atkCd);
            ps.Play();
        }
    }
}