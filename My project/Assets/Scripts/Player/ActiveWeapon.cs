using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActiveWeapon : Singleton<ActiveWeapon>
{
    public MonoBehaviour CurrentActiveWeapon { get; private set; }

    private PlayerControls playerControls;
    private float timeBetweenAttacks;

    private bool attackButtonDown, isAttacking = false;

    protected override void Awake() {
        base.Awake();

        playerControls = new PlayerControls();
    }

    private void OnEnable()
    {
        playerControls.Enable();
    }

    private void OnDisable()
    {
        if (playerControls != null)
        {
            playerControls.Combat.Attack.started -= StartAttackingCallback;
            playerControls.Combat.Attack.canceled -= StopAttackingCallback;
            playerControls.Disable();
        }
    }

    private void Start()
    {
        playerControls.Combat.Attack.started += StartAttackingCallback;
        playerControls.Combat.Attack.canceled += StopAttackingCallback;

        AttackCooldown();
    }

    private void StartAttackingCallback(UnityEngine.InputSystem.InputAction.CallbackContext ctx)
    {
        if (this == null) return;
        StartAttacking();
    }

    private void StopAttackingCallback(UnityEngine.InputSystem.InputAction.CallbackContext ctx)
    {
        if (this == null) return;
        StopAttacking();
    }

    private void Update() {
        Attack();
    }

    public void NewWeapon(MonoBehaviour newWeapon) {
        CurrentActiveWeapon = newWeapon;

        AttackCooldown();
        timeBetweenAttacks = (CurrentActiveWeapon as IWeapon).GetWeaponInfo().weaponCooldown;
    }

    public void WeaponNull() {
        CurrentActiveWeapon = null;
    }

    private void AttackCooldown() {
        isAttacking = true;
        StopAllCoroutines();
        StartCoroutine(TimeBetweenAttacksRoutine());
    }

    private IEnumerator TimeBetweenAttacksRoutine() {
        yield return new WaitForSeconds(timeBetweenAttacks);
        isAttacking = false;
    }

    private void StartAttacking()
    {
        if (PauseMenu.GameIsPaused) return;
        attackButtonDown = true;
    }

    private void StopAttacking()
    {
        attackButtonDown = false;
    }

    private void Attack() {
        if (PauseMenu.GameIsPaused) {
            attackButtonDown = false;
            return;
        }
        if (attackButtonDown && !isAttacking && CurrentActiveWeapon) {
            AttackCooldown();
            (CurrentActiveWeapon as IWeapon).Attack();
        }
    }
}
