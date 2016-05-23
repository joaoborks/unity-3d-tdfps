using UnityEngine;
using System.Collections;

[RequireComponent(typeof(CharacterController))]
public class PlayerPhysics : MonoBehaviour
{
    const float SPEED = 6f;
    const float JUMP_FORCE = 13f;
    const int MASS = 6;    
    const int MIN_ANGLE = -45;
    const int MAX_ANGLE = 80;

    // Input variables
    float horizontal;
    float vertical;
    bool sprint;
    bool jump;
    // Movement variables
    Vector3 moveDirection;    
    float gravity = Physics.gravity.y;    
    // Camera Variables
    Vector3 camRotation;
    int lookSens = 200;

    CharacterController controller;
    PlayerCombat pc;
    Transform cam;

    void Awake()
    {
        controller = GetComponent<CharacterController>();
        pc = GetComponent<PlayerCombat>();
        cam = GetComponentInChildren<Camera>().transform;
    }

    void Update()
    {
        // Input Handling
        if (Input.GetButtonDown("Jump"))
            jump = true;
        else if (Input.GetButtonUp("Jump"))
            jump = false;
        horizontal = Input.GetAxis("Horizontal");
        vertical = Input.GetAxis("Vertical");
        sprint = Input.GetButton("Sprint");

        if (horizontal != 0 || vertical < 0)
            sprint = false;

        // Movement Control
        CollisionFlags flag = controller.collisionFlags;

        if (controller.isGrounded)
        {
            moveDirection = new Vector3(horizontal, 0, vertical);
            moveDirection = transform.TransformDirection(moveDirection);
            moveDirection *= sprint ? 1.5f * SPEED : SPEED;

            if (jump)
            {
                moveDirection.y = JUMP_FORCE;
                
                if ((flag & CollisionFlags.Sides) == CollisionFlags.Sides)
                    moveDirection.x = moveDirection.z = 0;
            }
        }

        moveDirection.y += gravity * MASS * Time.deltaTime;

        controller.Move(moveDirection * Time.deltaTime);

        if (transform.position.y < -5)
        {
            pc.ChangeHealth(-100);
            enabled = false;
        }
    }

    void LateUpdate()
    {
        // Camera Controlling
        transform.Rotate(Vector3.up * lookSens * Time.deltaTime * Input.GetAxis("Mouse X"));

        camRotation.x -= Input.GetAxis("Mouse Y") * lookSens * Time.deltaTime;
        camRotation.x = Mathf.Clamp(camRotation.x, MIN_ANGLE, MAX_ANGLE);

        cam.localEulerAngles = camRotation;
    }
}