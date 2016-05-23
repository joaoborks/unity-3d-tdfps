using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class PlayerCombat : MonoBehaviour, IDamageable
{
    public ParticleSystem ps;
    public Slider hp;
    public Text tAmmo;

    PauseMenu menu;
    Animator anim;
    Camera cam;
    float damage = 10f,
        maxHealth = 100f,
        health;
    int atkRange = 100,
        maxAmmo = 150,
        ammo;

    void Awake()
    {
        cam = Camera.main;
        menu = FindObjectOfType<PauseMenu>();
        anim = GetComponent<Animator>();
        ChangeAmmo(150);
        ChangeHealth(maxHealth);
    }

    void Update()
    {
        if (ammo > 1 && Input.GetButtonDown("Fire1"))
            Shoot();

        if (Input.GetButtonDown("Fire2"))
            anim.SetBool("aiming", true);
        else if (Input.GetButtonUp("Fire2"))
            anim.SetBool("aiming", false);
    }

    void Shoot()
    {
        ChangeAmmo(-1);
        ps.gameObject.SetActive(true);
        ps.Play();
        RaycastHit hit;
        Vector3 screenPoint = cam.ScreenToWorldPoint(new Vector3(Screen.width / 2, Screen.height / 2, atkRange)),
            dir = screenPoint - ps.transform.position;
        if (Physics.Raycast(ps.transform.position, dir, out hit, atkRange))
        {
            if (hit.collider.gameObject.tag != "Nuleum")
            {
                IDamageable dmgbl = hit.collider.GetComponent<IDamageable>();
                if (dmgbl != null)
                    dmgbl.TakeDamage(damage);
            }
        }
        StartCoroutine(EndShoot());
    }

    void OnParticleCollision(GameObject other)
    {
        if (other.tag == "Shot")
        {
            float damage = float.Parse(other.name.Split('_')[1]);
            TakeDamage(damage);
        }
    }

    public void TakeDamage(float damage)
    {
        ChangeHealth(-damage);
    }

    public void Die()
    {
        menu.SetActive(2);
    }

    public void ChangeHealth(float value)
    {
        health += value;
        if (health > maxHealth)
            health = maxHealth;
        else if (health <= 0)
        {
            health = 0;
            Die();
        }
        hp.value = health / maxHealth;
    }

    public void ChangeAmmo(int value)
    {
        ammo += value;
        if (ammo > maxAmmo)
            ammo = maxAmmo;
        tAmmo.text = "Ammo: " + ammo;
    }

    IEnumerator EndShoot()
    {
        yield return new WaitForSeconds(ps.duration);
        ps.gameObject.SetActive(false);
    }
}