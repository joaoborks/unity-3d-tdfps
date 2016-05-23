using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class Nucleum : MonoBehaviour, IDamageable
{
    public Text text;

    PauseMenu menu;
    float maxHealth = 1000,
        health;
    bool done;

    void Awake()
    {
        ChangeHealth(maxHealth);
        menu = FindObjectOfType<PauseMenu>();
    }

    void OnParticleCollision(GameObject other)
    {
        if (other.tag == "Shot")
        {
            float damage = float.Parse(other.name.Split('_')[1]);
            TakeDamage(damage);
        }
    }

    void ChangeHealth(float value)
    {
        health += value;
        if (health > maxHealth)
            health = maxHealth;
        else if (health <= 0)
        {
            health = 0;
            Die();
        }
        text.text = "Nucleum: " + (health / maxHealth * 100).ToString() + "%";
    }

    public void TakeDamage(float damage)
    {
        ChangeHealth(-damage);
    }

    public void Die()
    {
        if (done)
            return;
        done = true;
        menu.SetActive(2);
    }
}