using UnityEngine;
using System.Collections;

public class PlayerCombat : MonoBehaviour
{
    public ParticleSystem ps;
    Animator anim;

    void Awake()
    {
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
        StartCoroutine(EndShoot());
    }

    IEnumerator EndShoot()
    {
        yield return new WaitForSeconds(ps.duration);
        ps.gameObject.SetActive(false);
    }
}
