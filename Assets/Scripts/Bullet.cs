using UnityEngine;

public class Bullet : MonoBehaviour
{
    [SerializeField] private float speed = .5f;
    [SerializeField] private GameObject hitEffect;
    [SerializeField] private float maxLifeTime = 2f;
    [SerializeField] private int damage = 1;

    [SerializeField] private GameObject hitSound;

    private float dir = 1f;
    private float lifeTime = 0f;

    private void FixedUpdate()
    {
        UpdatePosition();
        CheckLifeTime();
    }

    private void UpdatePosition()
    {
        transform.position += new Vector3(speed * dir, 0f, 0f);
    }

    private void CheckLifeTime()
    {
        lifeTime += Time.fixedDeltaTime;
        if (lifeTime >= maxLifeTime)
            Despawn(false);
    }

    private void OnTriggerEnter2D(Collider2D collider)
    {
        if (collider.CompareTag("Wall"))
            Despawn(true);

        else if(collider.CompareTag("Enemy"))
        {
            Virus enemy = collider.GetComponent<Virus>();

            if (enemy != null)
                enemy.TakeDamage(damage);
            else Debug.LogError("ERROR: No Virus Component found on Enemy!");

            Despawn(true);
        }
    }

    private void Despawn(bool effect)
    {
        if (effect)
        {
            HandleHitEffect();
            HandleHitSoundEffect();
        }

        Destroy(gameObject);
        return;
    }

    private void HandleHitEffect()
    {
        Vector3 effectSpawnPos = transform.position;
        effectSpawnPos.z = -1;

        if (hitEffect != null)
             Instantiate(hitEffect, effectSpawnPos, Quaternion.identity);
        else Debug.LogError("ERROR: No hit effect assigned!");
    }

    private void HandleHitSoundEffect()
    {
        if (hitSound != null)
            Instantiate(hitSound);
        else Debug.LogError("ERROR: No Hit Sound assigned!");
    }

    public void SetDirection(float d) { dir = d; }
}
