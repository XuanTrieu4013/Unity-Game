using System.Collections;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public bool FacingLeft { get { return facingLeft; } }
    public static PlayerController Instance;

    [SerializeField] private float moveSpeed = 4f;
    [SerializeField] private float dashForce = 10f;       // lực dash
    [SerializeField] private float dashDuration = 0.2f;   // thời gian dash
    [SerializeField] private float dashCooldown = 0.5f;   // thời gian hồi
    [SerializeField] private TrailRenderer myTrailRender;

    private PlayerControls playerControls;
    private Vector2 movement;
    private Rigidbody2D rb;
    private Animator myAnimator;
    private SpriteRenderer mySpriteRender;
    private float startingMoveSpeed;

    private bool facingLeft = false;
    private bool isDashing = false;
    private bool canDash = true;

    private void Awake()
    {
        Instance = this;
        playerControls = new PlayerControls();
        rb = GetComponent<Rigidbody2D>();
        myAnimator = GetComponent<Animator>();
        mySpriteRender = GetComponent<SpriteRenderer>();
    }

    private void Start()
    {
        playerControls.Combat.Dash.performed += _ => TryDash();
        startingMoveSpeed = moveSpeed;
    }

    private void OnEnable() => playerControls.Enable();
    private void OnDisable() => playerControls.Disable();

    private void Update() => PlayerInput();

    private void FixedUpdate()
    {
        AdjustPlayerFacingDirection();
        if (!isDashing) Move();
    }

    private void PlayerInput()
    {
        movement = playerControls.Movement.Move.ReadValue<Vector2>();
        myAnimator.SetFloat("moveX", movement.x);
        myAnimator.SetFloat("moveY", movement.y);
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
            mySpriteRender.flipX = true;
            facingLeft = true;
        }
        else
        {
            mySpriteRender.flipX = false;
            facingLeft = false;
        }
    }

    private void TryDash()
    {
        if (canDash && !isDashing)
        {
            StartCoroutine(DashRoutine());
        }
    }

    private IEnumerator DashRoutine()
    {
        isDashing = true;
        canDash = false;

        if (myTrailRender != null)
            myTrailRender.emitting = true;

        // Dash theo hướng di chuyển hiện tại
        Vector2 dashDir = movement.normalized;
        rb.AddForce(dashDir * dashForce, ForceMode2D.Impulse);

        yield return new WaitForSeconds(dashDuration);

        if (myTrailRender != null)
            myTrailRender.emitting = false;

        isDashing = false;

        yield return new WaitForSeconds(dashCooldown);
        canDash = true;
    }
}