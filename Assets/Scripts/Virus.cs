using UnityEngine;

public class Virus : MonoBehaviour
{
    [SerializeField] private GameObject deathEffect;

    [SerializeField] private Vector2Int bulletAmountRange;
    [SerializeField] private Vector2Int toiletPaperAmountRange;

    [SerializeField] private float speed;

    [SerializeField] private int damage;

    [SerializeField] private int health;
    [SerializeField] private int maxHealth;

    [SerializeField] private bool dieOnImpact;

    private float dir = 1f;
    private PlayerController player;

    private void Start()
    {
        player = FindObjectOfType<PlayerController>();
    }

    private void FixedUpdate()
    {
        MoveToPlayer();
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.collider.CompareTag("Player"))
        {
            DamagePlayer();

            if (dieOnImpact)
                Die();
        }
    }

    private void DamagePlayer()
    {
        player.TakeDamage(damage);
    }

    private void MoveToPlayer()
    {
        if (player != null)
        {
            if (player.transform.position.x < transform.position.x)
                dir = -1f;
            else dir = 1f;

            transform.position += new Vector3(speed * dir, 0f, 0f);
        }
    }

    public void TakeDamage(int amount)
    {
        health -= amount;

        if (health <= 0f)
            Die();
    }

    public void Die()
    {
        SpawnDropItems();
        HandleDeathEffect();
        Destroy(gameObject);
    }

    private void HandleDeathEffect()
    {
        Vector3 effectSpawnPos = transform.position;
        effectSpawnPos.z = -1;

        if (deathEffect != null)
            Instantiate(deathEffect, effectSpawnPos, Quaternion.identity);
        else Debug.LogError("ERROR: No Virus Death Effect assigned!");
    }

    private void SpawnDropItems()
    {
        int bulletAmount = Random.Range(bulletAmountRange.x, bulletAmountRange.y + 1);
        int toiletPaperAmount = Random.Range(toiletPaperAmountRange.x, toiletPaperAmountRange.y + 1);

        GameManager manager = GameManager.instance;

        for (int i = 0; i < bulletAmount; i++)
            manager.SpawnBulletItem(transform.position);

        for (int i = 0; i < toiletPaperAmount; i++)
            manager.SpawnToiletPaperItem(transform.position);

    }
}
