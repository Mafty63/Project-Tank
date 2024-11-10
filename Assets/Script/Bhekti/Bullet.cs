using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class Bullet : MonoBehaviour
{
    public Rigidbody rb;
    public float bulletSpeed = 20f;
    public float bulletLifeTime = 5f;
    public int bulletDamage = 10;

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

private void OnCollisionEnter(Collision collision)
{
    // Check if bullet collides with an enemy
    Enemy enemy = collision.gameObject.GetComponent<Enemy>();
    if (enemy != null)
    {
        // Reduce enemy health by bullet damage
        enemy.TakeDamage(100); // Sets the damage to 100
        DeactivateBullet(); // Deactivate bullet after hitting the enemy
    }
}

}
