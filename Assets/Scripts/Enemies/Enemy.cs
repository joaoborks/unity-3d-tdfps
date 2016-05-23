using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public abstract class Enemy : MonoBehaviour, IDamageable, ISlowable, IPathWalkable
{
    public LayerMask mask;
    public Slider hp;

    protected Transform target,
        nucleum;
    protected Collider[] cols;
    protected Collider enemy;
    protected float maxHealth,
        health,
        defSpeed,
        speed,
        sightRange,
        atkCd,
        rotSens;
    protected bool disturbed;

    ParticleSystem ps;
    Transform[] path;
    Coroutine slowCooldown;
    Vector3 curWaypoint;
    int curWPIndex;

    protected virtual void Awake()
    {
        ps = GetComponentInChildren<ParticleSystem>();
        target = FindObjectOfType<PlayerPhysics>().transform;
        nucleum = FindObjectOfType<Nucleum>().transform;
        health = maxHealth;
        speed = defSpeed;
        hp.value = health / maxHealth;
    }

    void FixedUpdate()
    {
        CheckDisturbed();

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

    protected abstract void CheckDisturbed();

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

        hp.value = health / maxHealth;
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

    protected IEnumerator Attack()
    {
        while (disturbed)
        {
            yield return new WaitForSeconds(atkCd);
            ps.Play();
        }
    }
}
