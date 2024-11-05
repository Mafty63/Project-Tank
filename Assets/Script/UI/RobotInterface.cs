using ProjectTank;
using TMPro;
using UnityEngine;
using Unity.Netcode;
using UnityEngine.UI;

public class RobotInterface : NetworkBehaviour
{
    [Header("Statistic")]
    [SerializeField] private float maxHealth;
    private float currentHealth;
    [SerializeField] private int maxAmmo;

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

        foreach (var pool in robotController.BulletPools)
        {
            pool.InitializePool(maxAmmo);
        }

        healthBar.SetHealth(maxHealth);
        ammo.text = maxAmmo.ToString();
    }

    private void Update()
    {
        ReloadAmmo();
        UpdateBulletAmmo();
    }


    private void ReloadAmmo()
    {
        if (!inputs.reload) return;

        foreach (var pool in robotController.BulletPools)
        {
            pool.ReturnAllBulletsToPlayer(maxAmmo);
        }

        inputs.reload = false;
    }

    public void UpdateBulletAmmo()
    {
        if (robotController.BulletPools[0].CurrentAmmo != maxAmmo)
        {
            ammo.text = robotController.BulletPools[0].CurrentAmmo.ToString();
        }
    }

}