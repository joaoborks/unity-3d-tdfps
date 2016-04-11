using UnityEngine;
using System.Collections;

// For the range Trigger
[RequireComponent(typeof(SphereCollider))]
public abstract class Turret : Buildable 
{
	enum States
    {
        None,
        Idle,
        Engaging
    }
    States state = States.None;

    SphereCollider rangeArea;
    Animator anim;

    protected override void Awake()
    {
        base.Awake();
        rangeArea = GetComponent<SphereCollider>();
        anim = GetComponent<Animator>();
    }
}