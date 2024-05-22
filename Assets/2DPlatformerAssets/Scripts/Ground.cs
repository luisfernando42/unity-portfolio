using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ground : MonoBehaviour
{
    private bool onGround;
    private float friction;

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.layer == 3)
        {
            CheckCollision(collision);
            SetFriction(collision);
        }
    }

    private void OnCollisionStay2D(Collision2D collision)
    {
        if (collision.gameObject.layer == 3)
        {
            CheckCollision(collision);
            SetFriction(collision);
        }

    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        onGround = false;
        friction = 0;
    }

    private void CheckCollision(Collision2D collision)
    {
        for (int i = 0; i < collision.contactCount; i++)
        {
            Vector2 normal = collision.GetContact(i).normal;
            onGround |= normal.y >= 0.9f;
        }
    }

    private void SetFriction(Collision2D collision)
    {

        PhysicsMaterial2D material = collision.rigidbody.sharedMaterial;
        friction = 0;
        if (material != null)
        {
            friction = material.friction;
        }
    }

    public bool GetIsGrounded()
    {
        return onGround;
    }
    public float GetFriction()
    {
        return friction;
    }
}
