using ProjectTank;
using TMPro;
using UnityEngine;
using Unity.Netcode;
using UnityEngine.UI;
using System.Collections;
using System;

public class RobotInterface : NetworkBehaviour
{
    [Header("Statistic")]
    private float maxHealth = 500;
    private float currentHealth;
    private int MaxAmmo = 10;
    public int CurrentAmmo { get; private set; }
    public bool PlayerIsDead { get; private set; }

    [Space]
    [SerializeField] private GameObject DeadModal;
    [SerializeField] private TextMeshProUGUI Timer;
    [SerializeField] private Inputs inputs;
    [SerializeField] private HealthBar healthBar;


    [Header("Interface")]
    [SerializeField] private TextMeshProUGUI ammo;

    private void Start()
    {
        if (!IsOwner)
        {
            gameObject.SetActive(false);
            return;
        }

        CurrentAmmo = MaxAmmo;
        currentHealth = maxHealth;
        healthBar.SetHealth(currentHealth);
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
    }

    public void PlayerDead()
    {
        PlayerIsDead = true;
        DeadModal.SetActive(true);
        StartCoroutine(RespawnPlayerAtSpawnPoint());
    }

    private IEnumerator RespawnPlayerAtSpawnPoint()
    {
        float delayDead = 5;
        while (true)
        {
            yield return new WaitForSecondsRealtime(1f);
            delayDead--;
            Timer.text = TimeSpan.FromSeconds(delayDead).ToString("mm\\:ss");

            if (delayDead < 0)
            {
                DeadModal.SetActive(false);
                GameManager.Instance.RespawnPlayer(OwnerClientId);
            }
        }
    }

    public void ResetStatus()
    {
        currentHealth = maxHealth;
        currentHealth = maxHealth;
    }

}