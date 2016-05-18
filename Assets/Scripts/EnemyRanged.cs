using UnityEngine;
using System.Collections;

public class EnemyRanged : MonoBehaviour, IDamageable, ISlowable 
{
    Coroutine slowCooldown;
    float maxHealth = 100,
        health,
        defSpeed = 6,
        speed;

    void Awake()
    {
        health = maxHealth;
        speed = defSpeed;
    }

    public void TakeDamage(float damage)
    {
        health -= damage;
        print("<color=red>I took " + damage + " damage! My health is: " + health + "</color>");
        if (health <= 0)
            Die();
    }

    public void Die()
    {
        Destroy(gameObject);
    }

    public void GetSlowed(float dur, float ratio)
    {
        speed = defSpeed * ratio;
        print("<color=green>I got slowed by " + (ratio * 100) + "%! My speed is: " + speed + "</color>");
        slowCooldown = StartCoroutine(SlowCooldown(dur));
    }

    public IEnumerator SlowCooldown(float time)
    {
        yield return new WaitForSeconds(time);
        speed = defSpeed;
    }
}