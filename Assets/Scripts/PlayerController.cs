using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [SerializeField] private CharacterController2D controller;
    [SerializeField] private Animator animator;

    [SerializeField] private GameObject deathEffect;
    [SerializeField] private Transform bulletSpawnPos;
    [SerializeField] private Animator playerDamageEffectAnimator;

    [SerializeField] private float deathEffectExplosionForce = 100f;

    [SerializeField] private GameObject jumpSoundEffect;
    [SerializeField] private GameObject playerDamageSoundEffect;
    [SerializeField] private GameObject shootSoundEffect;
    [SerializeField] private GameObject emptyShootSoundEffect;

    [SerializeField] private int health;
    [SerializeField] private int maxHealth;

    private GameManager manager;

    private float horizontal = 0f;
    private bool jump = false;
    private bool shoot = false;
    private bool fastFire = false;
    private bool fastFire2nd = false;

    private float timeSinceLastFastFireActivation = 0f;

    private void Awake()
    {
        if (controller == null)
            controller = FindObjectOfType<CharacterController2D>();
    }

    private void Start()
    {
        manager = GameManager.instance;
    }

    private void Update()
    {
        RecievePlayerInput();
        SetAnimatorSpeedValue();
        SetAnimatorOnGroundBool();
    }

    private void FixedUpdate()
    {
        if (!manager.gamePaused)
        {
            controller.Move(horizontal, jump);

            if (jump) Jump();

            if (timeSinceLastFastFireActivation > 0f && fastFire && fastFire2nd)
                       Shoot(false);
            if (shoot) Shoot(true);

            fastFire2nd = !fastFire2nd;
            timeSinceLastFastFireActivation -= Time.fixedDeltaTime;
        }
    }

    private void RecievePlayerInput()
    {
        horizontal = Input.GetAxisRaw("Horizontal");
        fastFire = Input.GetButton("Shoot");

        if (Input.GetButtonDown("Jump"))
            jump = true;
        if (Input.GetButtonDown("Shoot"))
            shoot = true;
    }

    private void SetAnimatorSpeedValue()
    {
        animator.SetFloat("speed", controller.GetSpeedPercent());
    }

    private void SetAnimatorOnGroundBool()
    {
        animator.SetBool("onGround", controller.GetOnGround());
    }

    private void SetAnimatorJumpTrigger()
    {
        animator.SetTrigger("jump");
    }

    private void SetAnimatorShootTrigger()
    {
        animator.SetTrigger("shoot");
    }

    private void Jump()
    {
        jump = false;
        SetAnimatorJumpTrigger();
        HandleJumpSoundEffect();
    }

    private void Shoot(bool reduceBulletAmount)
    {
        shoot = false;

        if (reduceBulletAmount)
            if (!manager.Shoot())
            {
                HandleEmptyShootSoundEffect();
                return;
            }

        SetAnimatorShootTrigger();
        HandleShootSoundEffect();
        manager.SpawnBullet(bulletSpawnPos.position, transform.localScale);
    }

    public void TakeDamage(int amount)
    {
        HandlePlayerDamageEffect();
        HandlePlayerDamageSoundEffect();

        int oldHealth = health;
        health = Mathf.Max(0, health - amount);

        manager.ChangeHealth(health, oldHealth);

        if (health == 0)
            Die();
    }

    private void Die()
    {
        HandleDeathEffect();
        Destroy(gameObject);

        manager.PlayerDied();
    }

    private void HandleDeathEffect()
    {
        Vector3 effectSpawnPos = transform.position;
        effectSpawnPos.z = -1;

        if (deathEffect != null)
        {
            GameObject effect = Instantiate(deathEffect, effectSpawnPos, Quaternion.identity);

            foreach (Rigidbody2D rb in effect.GetComponentsInChildren<Rigidbody2D>())
            {
                Vector2 dir = rb.position - new Vector2(effectSpawnPos.x, effectSpawnPos.y);
                Vector2 force = dir.normalized * deathEffectExplosionForce;

                rb.AddForce(force);
            }
        }

        else Debug.LogError("ERROR: No Player Death Effect assigned!");

    }

    private void HandlePlayerDamageEffect()
    {
        if (playerDamageEffectAnimator != null)
            playerDamageEffectAnimator.SetTrigger("damage");
        else Debug.LogError("ERROR: No Player-Hurt-Effect Animator assigned!");
    }

    private void HandleJumpSoundEffect()
    {
        if (jumpSoundEffect != null)
            Instantiate(jumpSoundEffect);
        else Debug.LogError("ERROR: No Jump Sound assigned!");
    }

    private void HandlePlayerDamageSoundEffect()
    {
        if (playerDamageSoundEffect != null)
            Instantiate(playerDamageSoundEffect);
        else Debug.LogError("ERROR: No Player Damage Sound assigned!");
    }

    private void HandleShootSoundEffect()
    {
        if (shootSoundEffect != null)
            Instantiate(shootSoundEffect);
        else Debug.LogError("ERROR: No Shoot Sound assigned!");
    }

    private void HandleEmptyShootSoundEffect()
    {
        if (emptyShootSoundEffect != null)
            Instantiate(emptyShootSoundEffect);
        else Debug.LogError("ERROR: No Empty Shoot Sound assigned!");
    }

    public void AddFastFireTime(float amount)
    {
        timeSinceLastFastFireActivation = Mathf.Max(0f, timeSinceLastFastFireActivation) + amount;
    }
}
