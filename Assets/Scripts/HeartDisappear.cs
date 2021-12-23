using UnityEngine;

public class HeartDisappear : MonoBehaviour
{
    public void SelfDestroy()
    {
        Destroy(gameObject);
    }
}
