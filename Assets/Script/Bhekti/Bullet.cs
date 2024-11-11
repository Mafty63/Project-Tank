using ProjectTank;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class Bullet : MonoBehaviour
{
    public Rigidbody rb;
    public float bulletSpeed = 20f;
    public float bulletLifeTime = 5f;
    public int bulletDamage = 10;
    private RobotController shooter;

    private void OnEnable()
    {
        shooter = null;

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

    public void SetShooter(RobotController shooter)
    {
        this.shooter = shooter;
    }

    private void DeactivateBullet()
    {
        gameObject.SetActive(false);
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.transform.CompareTag("Player"))
        {
            RobotController colli = collision.gameObject.GetComponent<RobotController>();
            if (colli != shooter)
            {
                colli.TakeDamage(10); //TODO sementara bullet damage static
                DeactivateBullet();
            }
        }
    }

}
