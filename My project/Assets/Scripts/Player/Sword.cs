using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Sword : MonoBehaviour
{
    [SerializeField] private GameObject slashAnimPrefab;
    [SerializeField] private Transform slashAnimSpawnPoint;
    [SerializeField] private Transform weaponCollider;
    [SerializeField] private float swordAttackCD = .5f;

    private PlayerControls playerControls;
    private Animator myAnimator;
    private PlayerController playerController;
    private ActiveWeapon activeWeapon;
    private bool attackButtonDown, isAttacking = false;
    private GameObject slashAnim;

    // callback lưu lại để hủy khi disable
    private System.Action<UnityEngine.InputSystem.InputAction.CallbackContext> attackCallback;

    private void Awake()
    {
        playerController = GetComponentInParent<PlayerController>();
        activeWeapon = GetComponentInParent<ActiveWeapon>();
        myAnimator = GetComponent<Animator>();
        playerControls = new PlayerControls();
    }

    private void OnEnable()
    {
        playerControls.Enable();

        // đăng ký callback
        attackCallback = ctx => Attack();
        playerControls.Combat.Attack.started += attackCallback;
    }

    private void Start()
    {
        playerControls.Combat.Attack.started += _ => StartAttacking();
        playerControls.Combat.Attack.canceled += _ => StopAttacking();
    }
    private void OnDisable()
    {
        // hủy callback khi object disable/destroy
        if (attackCallback != null)
        {
            playerControls.Combat.Attack.started -= attackCallback;
        }
        playerControls.Disable();
    }

    private void Update()
    {
        MouseFollowWithOffset();
        Attack();
    }

    private void StartAttacking()
    {
        attackButtonDown = true;
    }

    private void StopAttacking()
    {
        attackButtonDown = false;
    }

    private void Attack()
    {
        if (attackButtonDown && !isAttacking )
        {
            isAttacking = true;
            myAnimator.SetTrigger("Attack");
            weaponCollider.gameObject.SetActive(true);
            slashAnim = Instantiate(slashAnimPrefab, slashAnimSpawnPoint.position, Quaternion.identity);
            slashAnim.transform.parent = this.transform.parent;
            StartCoroutine(AttackCDRoutine());
        }
    }

    private IEnumerator AttackCDRoutine()
    {
        yield return new WaitForSeconds(swordAttackCD);
        isAttacking = false;
    }

    // Animation Events
    public void DoneAttackingAnimEvent()
    {
        if (weaponCollider != null)
        {
            weaponCollider.gameObject.SetActive(false);
        }
    }

    public void SwingUpFlipAnimEvent()
    {
        if (slashAnim != null)
        {
            slashAnim.transform.rotation = Quaternion.Euler(-180, 0, 0);

            if (playerController != null && playerController.FacingLeft)
            {
                slashAnim.GetComponent<SpriteRenderer>().flipX = true;
            }
        }
    }

    public void SwingDownFlipAnimEvent()
    {
        if (slashAnim != null)
        {
            slashAnim.transform.rotation = Quaternion.Euler(0, 0, 0);

            if (playerController != null && playerController.FacingLeft)
            {
                slashAnim.GetComponent<SpriteRenderer>().flipX = true;
            }
        }
    }

    private void MouseFollowWithOffset()
    {
        if (playerController == null || activeWeapon == null || weaponCollider == null) return;

        Vector3 mousePos = Input.mousePosition;
        Vector3 playerScreenPoint = Camera.main.WorldToScreenPoint(playerController.transform.position);

        float angle = Mathf.Atan2(mousePos.y, mousePos.x) * Mathf.Rad2Deg;

        if (mousePos.x < playerScreenPoint.x)
        {
            activeWeapon.transform.rotation = Quaternion.Euler(0, -180, angle);
            weaponCollider.transform.rotation = Quaternion.Euler(0, -180, 0);
        }
        else
        {
            activeWeapon.transform.rotation = Quaternion.Euler(0, 0, angle);
            weaponCollider.transform.rotation = Quaternion.Euler(0, 0, 0);
        }
    }
}