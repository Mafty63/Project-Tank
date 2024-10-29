using UnityEngine;

public class Bullet : MonoBehaviour
{     
    public Rigidbody rb;
    public float bulletSpeed = 20f;   
    public float bulletLifeTime = 5f;

    void Update()
    {
        Shoot();
    }

    void Shoot()
    {
        if (rb != null)
        {
            rb.velocity = transform.forward * bulletSpeed;
        }

        Destroy(this.gameObject, bulletLifeTime);
    }
}
