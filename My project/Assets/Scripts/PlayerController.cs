using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
     public bool FacingLeft
{
    get { return facingLeft; }
    set { facingLeft = value; }
}


    [SerializeField] private float moveSpeed = 1f;

    // Dash
    public float dashBoost;
    public float dashTime;
    private float _dashTime;
    bool isDashing = false;
    // dash
    
    private PlayerControls playerControls;
    private Vector2 movement;
    private Rigidbody2D rb; 
    private Animator myAnimator;
    private SpriteRenderer mySpriteRenderer;
    private bool facingLeft = false;
    private void Awake()
    {
        playerControls = new PlayerControls();
        rb = GetComponent<Rigidbody2D>();
        myAnimator = GetComponent<Animator>();
        mySpriteRenderer = GetComponent<SpriteRenderer>();
    }

    private void OnEnable()
    {
        playerControls.Enable();       
    }

    private void Update()
    {
        //dash
        if (Input.GetKeyDown(KeyCode.LeftShift) && _dashTime <= 0 && isDashing == false)
        {
            moveSpeed += dashBoost;
            _dashTime = dashTime;
            isDashing = true;
        }

        if (_dashTime <= 0 && isDashing == true)
        {
            moveSpeed -= dashBoost;
            isDashing = false;
        }
        else if (isDashing == true)
        {
            _dashTime -= Time.deltaTime;
        }
        //dash

        PlayerInput();
    }

    private void FixedUpdate()
    {
        AdjustPlayerFacingDirection();
        Move();
    }

    private void PlayerInput()
    {
        movement = playerControls.Movement.Move.ReadValue<Vector2>();
        myAnimator.SetFloat("moveX",movement.x);
        myAnimator.SetFloat("moveY",movement.y);
    }

    private void Move()
    {
        rb.MovePosition(rb.position + movement * (moveSpeed * Time.fixedDeltaTime));
    }

    private void AdjustPlayerFacingDirection()
    {
        Vector3 mousePos = Input.mousePosition;
        Vector3 playerScreenPoint = Camera.main.WorldToScreenPoint(transform.position);

        if (mousePos.x < playerScreenPoint.x)
        {
            mySpriteRenderer.flipX = true;
            FacingLeft = true;
        }
        else
        {
            mySpriteRenderer.flipX = false;
            FacingLeft = false;
        }
    }
}
