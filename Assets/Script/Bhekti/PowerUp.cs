using UnityEngine;
using ProjectTank; // Tambahkan ini jika RobotController ada dalam namespace ProjectTank

public class PowerUp : MonoBehaviour
{
    public enum PowerUpType { SpeedBoost, AmmoRefill }
    public PowerUpType powerUpType;
    public float effectDuration = 5.0f;
    public float speedMultiplier = 1.5f;
    public int ammoIncrease = 10;

    private void OnTriggerEnter(Collider other)
    {
        // Memastikan bahwa objek yang bersentuhan adalah pemain (robot)
        if (other.CompareTag("Player"))
        {
            RobotController robotController = other.GetComponent<RobotController>();
            if (robotController != null)
            {
                ApplyPowerUp(robotController);
            }

            // Menghancurkan power-up setelah digunakan
            Destroy(gameObject);
        }
    }

    private void ApplyPowerUp(RobotController robotController)
    {
        switch (powerUpType)
        {
            case PowerUpType.SpeedBoost:
                robotController.ApplySpeedBoost(speedMultiplier, effectDuration);
                break;
            case PowerUpType.AmmoRefill:
                robotController.RefillAmmo(ammoIncrease);
                break;
        }
    }
}
