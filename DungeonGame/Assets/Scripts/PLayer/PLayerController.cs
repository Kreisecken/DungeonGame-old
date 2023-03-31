using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public Rigidbody2D rigid;
    public Vector2 movement;
    public float speed;
    void Update()
    {
        movement = (Input.GetAxis("Horizontal") * Vector2.right + Input.GetAxis("Vertical") * Vector2.up);
    }

        void FixedUpdate()
    {
        rigid.MovePosition(rigid.position + movement * Time.fixedDeltaTime * speed);
    }
}
