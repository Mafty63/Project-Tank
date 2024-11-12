using ProjectTank;
using TMPro;
using UnityEngine;
using Unity.Netcode;
using UnityEngine.UI;
using System.Collections;
using System;

public class RobotInterface : NetworkBehaviour
{
    [SerializeField] private NetworkObject networkObject;
    [Header("Statistic")]
    private float maxHealth = 500;
    private float currentHealth;
    private int MaxAmmo = 10;
    public int CurrentAmmo { get; private set; }
    public bool PlayerIsDead { get; private set; }

    [Space]
    [SerializeField] private Animator animator;
    [SerializeField] private GameObject DeadModal;
    [SerializeField] private TextMeshProUGUI Timer;
    [SerializeField] private Inputs inputs;
    [SerializeField] private HealthBar healthBar;

    [Header("Interface")]
    [SerializeField] private TextMeshProUGUI ammo;

    private void Start()
    {
        if (!networkObject.IsOwner)
        {
            gameObject.SetActive(false);
            return;
        }

        CurrentAmmo = MaxAmmo;
        currentHealth = maxHealth;
        healthBar.SetMaxHealth(currentHealth);
        ammo.text = CurrentAmmo.ToString();
    }

    public void ReloadAmmo(int ammo)
    {
        CurrentAmmo = ammo;
    }

    public void UpdateBulletAmmo()
    {
        CurrentAmmo--;
        ammo.text = CurrentAmmo.ToString();
    }

    public void TakeDamage(int damage)
    {
        currentHealth -= damage;
        healthBar.SetHealth(currentHealth);

        if (currentHealth <= 0)
        {
            PlayerDead();
        }
    }

    public void PlayerDead()
    {
        if (!networkObject.IsOwner) return;

        PlayerIsDead = true;
        DeadModal.SetActive(true);
        animator.Play("Death");

        // Kirim RPC ke client yang relevan (hanya diri sendiri) untuk memulai respawn coroutine.
        StartRespawnCoroutineClientRpc(networkObject.OwnerClientId);
    }

    [ClientRpc]
    private void StartRespawnCoroutineClientRpc(ulong clientId)
    {
        // Pastikan coroutine hanya dijalankan di client yang sesuai.
        if (NetworkManager.Singleton.LocalClientId == clientId)
        {
            StartCoroutine(RespawnPlayerAtSpawnPoint());
        }
    }

    private IEnumerator RespawnPlayerAtSpawnPoint()
    {
        float delayDead = 5;
        while (delayDead > 0)
        {
            yield return new WaitForSecondsRealtime(1f);
            delayDead--;
            Timer.text = TimeSpan.FromSeconds(delayDead).ToString("mm\\:ss");
        }

        // Reset status dan respawn setelah waktu habis
        animator.Play("Idle");
        DeadModal.SetActive(false);
        PlayerIsDead = false;
        ResetStatus();
        GameManager.Instance.RespawnPlayer(networkObject.OwnerClientId);
    }

    public void ResetStatus()
    {
        currentHealth = maxHealth;
        CurrentAmmo = MaxAmmo;
        healthBar.SetMaxHealth(currentHealth);
        ammo.text = CurrentAmmo.ToString();
    }
}
