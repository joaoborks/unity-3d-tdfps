using UnityEngine;
using System.Collections;

public class PlayerCombat : MonoBehaviour
{
    public ParticleSystem ps;

    Animator anim;
    Camera cam;
    float damage = 10f;
    int atkRange = 100;

    void Awake()
    {
        cam = Camera.main;
        anim = GetComponent<Animator>();
    }

    void Update()
    {
        if (Input.GetButtonDown("Fire1"))
            Shoot();

        if (Input.GetButtonDown("Fire2"))
            anim.SetBool("aiming", true);
        else if (Input.GetButtonUp("Fire2"))
            anim.SetBool("aiming", false);
    }

    void Shoot()
    {
        ps.gameObject.SetActive(true);
        ps.Play();
        RaycastHit hit;
        Vector3 screenPoint = cam.ScreenToWorldPoint(new Vector3(Screen.width / 2, Screen.height / 2, atkRange)),
            dir = screenPoint - ps.transform.position;
        if (Physics.Raycast(ps.transform.position, dir, out hit, atkRange))
        {
            IDamageable dmgbl = hit.collider.GetComponent<IDamageable>();
            if (dmgbl != null)
                dmgbl.TakeDamage(damage);
        }
        StartCoroutine(EndShoot());
    }

    IEnumerator EndShoot()
    {
        yield return new WaitForSeconds(ps.duration);
        ps.gameObject.SetActive(false);
    }
}
