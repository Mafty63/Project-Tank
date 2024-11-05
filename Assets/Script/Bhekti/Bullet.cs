using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class Bullet : MonoBehaviour
{
    public Rigidbody rb;
    public float bulletSpeed = 20f;
    public float bulletLifeTime = 5f;

    private void OnEnable()
    {
        if (rb != null)
        {
            rb.velocity = transform.forward * bulletSpeed;
        }

        Invoke(nameof(DeactivateBullet), bulletLifeTime);
    }

    private void OnDisable()
    {
        CancelInvoke(nameof(DeactivateBullet));
    }

    void DeactivateBullet()
    {
        gameObject.SetActive(false);
    }
}
