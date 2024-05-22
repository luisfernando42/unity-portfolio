using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Platformer2DController : MonoBehaviour
{
    
    [SerializeField] private Platformer2DInputs platformer2DInputs;

    [Header("Movement")]
    [SerializeField] private float maxSpeed = 4f;
    [SerializeField] private float maxAcceleration = 35f;
    [SerializeField] private float maxAirAcceleration = 20f;

    private float acceleration;
    private float maxSpeedChange;

    [Header("Jump")]
    [SerializeField] private float jumpHeight = 3f;
    [SerializeField] private int maxAirJumps = 0;
    [SerializeField] private float downwarMovementMultiplier = 3f;
    [SerializeField] private float upwardMovementMultiplier = 1.7f;

    private int jumpPhase;
    private float defaultGravityScale;

    private Vector2 desiredVelocity;
    private Vector2 velocity;
    private Ground ground;
    
    private bool isGrounded;

    private Rigidbody2D _body;


    private void Awake()
    {
        _body = GetComponent<Rigidbody2D>();
        ground = GetComponent<Ground>();

        defaultGravityScale = 1f;
    }

    private void Start()
    {
        platformer2DInputs.OnJumpAction += Platformer2DInputs_OnJumpAction;
    }

    private void Update()
    {
        HandleMovementDirection();
    }
    private void FixedUpdate()
    {
        HandleMovement();
    }

    private void Platformer2DInputs_OnJumpAction(object sender, EventArgs e)
    {
       
    }

    private void HandleMovement()
    {
        isGrounded = ground.GetIsGrounded();
        velocity = _body.velocity;

        acceleration = isGrounded ? maxAcceleration : maxAirAcceleration;
        maxSpeedChange = acceleration * Time.deltaTime;
        velocity.x = Mathf.MoveTowards(velocity.x, desiredVelocity.x, maxSpeedChange);

        _body.velocity = velocity;
    }

    private void HandleJump()
    {

    }
    private void HandleMovementDirection()
    {
        Vector2 inputVector = platformer2DInputs.GetMovementVector2Normalized();
        desiredVelocity = new Vector2(inputVector.x, 0f) * Mathf.Max(maxSpeed - ground.GetFriction(), 0f);   
    }
}
