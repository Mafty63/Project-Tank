using UnityEngine;

public class Enemy : MonoBehaviour
{
    public int health = 50;

    public void TakeDamage(int damage)
    {
        health -= damage;
        Debug.Log("Enemy health: " + health);

        // Cek jika health musuh habis
        if (health <= 0)
        {
            Die();
        }
    }

    private void Die()
    {
        // Tambahkan efek kematian atau deaktivasi musuh
        gameObject.SetActive(false);
        Debug.Log("Enemy defeated!");
    }
}
