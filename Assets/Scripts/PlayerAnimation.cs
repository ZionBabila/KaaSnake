using System;
using NUnit.Framework;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem.Controls;

public class PlayerAnimation : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public Animator animator;
    public SimplePlayer simplePlayer;
   public Rigidbody2D rb;
    public SpriteRenderer spriteRenderer;
    public float speed = 0;
public float up =0;
    private void Start()
    {

        rb = GetComponent<Rigidbody2D>();
    }
    private void Update()
    {
        speed = Mathf.Abs(rb.linearVelocity.x);
        animator.SetFloat("speed", speed);
        up = simplePlayer.up;
        animator.SetFloat("up", up);
        if (speed > 0.1f)
        {
            if (simplePlayer.V > 0)
            {
                spriteRenderer.flipX = false;
            }
            if (simplePlayer.V < 0)
            {
                spriteRenderer.flipX = true;
            }
        }
        if(simplePlayer.Grounded)
        {
            animator.SetBool("jump", false);
        }
        else
        {
            animator.SetBool("jump", true);
        }

    }
}
