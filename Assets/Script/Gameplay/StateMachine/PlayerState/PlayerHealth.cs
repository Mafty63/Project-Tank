using UnityEngine;
using UnityEngine.Events;
using System;

public class PlayerHealth : MonoBehaviour
{
    public float maxHealth = 100;
    private float currentHealth;
    public HealthBar healthBar;

    // Event untuk memperbarui UI health bar
    public UnityEvent<float> OnHealthChanged;
    public Action OnPlayerDeath;  // Event untuk memberitahu LevelInitializer

    private Transform spawnPoint; // Menyimpan titik spawn pemain

    void Start()
    {
        currentHealth = maxHealth;
        healthBar.SetMaxHealth(maxHealth);
    }

    void Update()
    {
        // Deteksi klik kanan (mouse button 1)
        if (Input.GetKeyDown(KeyCode.Space))
        {
            TakeDamage(10f); // Misalnya, kurangi 10 HP setiap kali klik kanan
        }
    }

    public void TakeDamage(float amount)
    {
        currentHealth -= amount;
        currentHealth = Mathf.Clamp(currentHealth, 0, maxHealth); // Batasan nilai kesehatan

        healthBar.SetHealth(currentHealth);

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    private void Die()
    {
        OnPlayerDeath?.Invoke();  // Memanggil event kematian
    }

    public void Respawn()
    {
        currentHealth = maxHealth;  // Mengembalikan kesehatan penuh saat respawn
        healthBar.SetHealth(currentHealth); // Memperbarui health bar
        transform.position = spawnPoint.position; // Kembali ke posisi spawn
    }

    public void SetSpawnPoint(Transform point)
    {
        spawnPoint = point; // Menyimpan titik spawn
    }
}
