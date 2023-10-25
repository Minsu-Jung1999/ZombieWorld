using System;
using System.Collections;
using System.Collections.Generic;
using System.Timers;
using UnityEngine;
using UnityEngine.UI;

public class PlayerController : MonoBehaviour
{
    [SerializeField]
    Slider spSlider;
    [SerializeField]
    float moveSpeed = 0.0f;
    [SerializeField]
    float runSpeed = 0.0f;
    [SerializeField]
    float lookRotationSpeed= 10.0f;
    [SerializeField]
    float moveRotationSpeed = 10.0f;
    [SerializeField]
    GameObject cameraArm;
    [SerializeField]
    float cameraRotationLimitUP=90.0f;
    [SerializeField]
    float cameraRotationLimitDown=90.0f;

    private Animator anim;
    private float xRotation = 0;
    private float yRotation = 0;
    private float currentMoveSpeed=0;
    private bool istired = false;

    private Rigidbody rb;

    void Start()
    {
        spSlider.maxValue = 100;
        spSlider.value = spSlider.maxValue;
        rb = GetComponent<Rigidbody>();
        anim = GetComponent<Animator>();
    }
    private void FixedUpdate()
    {
        Move();
    }

   

    void Update()
    {
        LookAround();
        if (anim.GetBool("isRun"))
            spSlider.value -= 0.3f;
        else
            spSlider.value += 0.1f;
    }



    private void LookAround()
    {
        xRotation += Input.GetAxis("Mouse X");
        yRotation -= Input.GetAxis("Mouse Y");
        yRotation = Mathf.Clamp(yRotation, -cameraRotationLimitDown, cameraRotationLimitUP);

        cameraArm.transform.rotation = Quaternion.Euler(yRotation, xRotation, 0);
    }

    private void Move()
    {
        Vector2 moveInput = new Vector2(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical"));
        bool isMove = moveInput.magnitude != 0;     // 개 좋노
        bool isRun = false;
        anim.SetBool("isMove", isMove);
        if (isMove)
        {
            if (spSlider.value < 10)
                istired = true;
            else if (spSlider.value > 40)
                istired = false;
            if (Input.GetKey(KeyCode.LeftShift) && !istired)
            {
                currentMoveSpeed = runSpeed;
                isRun = true;
            }
            else
            {
                currentMoveSpeed = moveSpeed;
                isRun = false;
            }
            anim.SetBool("isRun", isRun);
            Vector3 lookForward = new Vector3(cameraArm.transform.forward.x, 0f, cameraArm.transform.forward.z).normalized;
            Vector3 lookRight = new Vector3(cameraArm.transform.right.x, 0f, cameraArm.transform.right.z).normalized;
            Vector3 moveDir = lookForward * moveInput.y + lookRight * moveInput.x;

            //transform.forward = moveDir;
            Rotate(moveDir);
            transform.position += moveDir * Time.deltaTime * currentMoveSpeed;

        }

    }
    private void Rotate(Vector3 moveDirection)
    {
        // 원하는 회전 각도를 계산합니다.
        Quaternion targetRotation = Quaternion.LookRotation(moveDirection);

        // 부드러운 회전을 위해 Slerp를 사용합니다.
        transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, moveRotationSpeed * Time.deltaTime);
    }

}
