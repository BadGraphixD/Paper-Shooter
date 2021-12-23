using UnityEngine;

public class Temporary : MonoBehaviour
{
    [SerializeField] private float selfDestroyTime = 1f;

    private void Awake()
    {
        Invoke("SelfDestroy", selfDestroyTime);
    }

    private void SelfDestroy()
    {
        Destroy(gameObject);
    }
}
