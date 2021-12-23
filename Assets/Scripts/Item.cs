using UnityEngine;

public class Item : MonoBehaviour
{
    public enum ItemType
    {
        Bullet,
        ToiletPaper,
        FastFirePill,
        Disinfectant
    }

    [SerializeField] private ItemType type = ItemType.Bullet;
    [SerializeField] private GameObject collectEffect;

    [SerializeField] private GameObject collectSound0;
    [SerializeField] private GameObject collectSound1;

    private void OnTriggerEnter2D(Collider2D collider)
    {
        if (collider.CompareTag("Player"))
            Collect();
    }

    private void Collect()
    {
        if (type == ItemType.Bullet)
            GameManager.instance.CollectBullet(3);
        else if (type == ItemType.ToiletPaper)
            GameManager.instance.CollectToiletPaper(1);
        else if (type == ItemType.FastFirePill)
            GameManager.instance.ActivateFastFire();
        else if (type == ItemType.Disinfectant)
            GameManager.instance.DisinfectantArea();

        HandleCollectEffect();
        HandleCollectSoundEffect();

        Destroy(transform.parent.gameObject);
    }

    private void HandleCollectEffect()
    {
        Vector3 effectSpawnPos = transform.position;
        effectSpawnPos.z = -1;

        if (collectEffect != null)
            Instantiate(collectEffect, effectSpawnPos, Quaternion.identity);
        else Debug.LogError("ERROR: No Item Collection Effect assigned!");
    }

    private void HandleCollectSoundEffect()
    {
        GameObject collectSound = Random.value > .5f ? collectSound0 : collectSound1;

        if (collectSound != null)
            Instantiate(collectSound);
        else Debug.LogError("ERROR: No Collection Sound assigned!");
    }
}
