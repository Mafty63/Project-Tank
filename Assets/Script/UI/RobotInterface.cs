using ProjectTank;
using TMPro;
using UnityEngine;
using Unity.Netcode;
using UnityEngine.UI;

public class RobotInterface : NetworkBehaviour
{
    [Header("Statistic")]
    [SerializeField] private float maxHealth = 500;
    private float currentHealth;
    public int MaxAmmo;
    public int CurrentAmmo { get; private set; }

    [Space]
    [SerializeField] private RobotController robotController;
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
        healthBar.SetHealth(maxHealth);
        ammo.text = MaxAmmo.ToString();
    }


    public void ReloadAmmo()
    {
        if (!inputs.reload) return;

        CurrentAmmo = MaxAmmo;

        inputs.reload = false;
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

}