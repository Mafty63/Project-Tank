using UnityEngine;
using UnityEngine.InputSystem;

public class TankController : MonoBehaviour, SimpleCharacterControlls.IActionActions
{
    private SimpleCharacterControlls controls;
    private Vector2 movementInput;

    [SerializeField] private Rigidbody rb; // Ensure Rigidbody is attached to the tank
    [SerializeField] private float moveSpeed = 5f; // Movement speed of the tank
    [SerializeField] private GameObject projectilePrefab; // Projectile prefab
    [SerializeField] private Transform firePoint; // Point to fire projectiles
    [SerializeField] private float projectileSpeed = 10f; // Projectile speed

    [SerializeField] private Animator animator; // Animator component
    [SerializeField] private Avatar avatar; // Avatar for the tank (optional, depending on your setup)

    private void Awake()
    {
        controls = new SimpleCharacterControlls();
        controls.Action.SetCallbacks(this);
    }

    private void OnEnable() => controls.Enable();
    private void OnDisable() => controls.Disable();

    public void OnMovement(InputAction.CallbackContext context)
    {
        movementInput = context.ReadValue<Vector2>();
        // Update animator parameters for movement
        animator.SetFloat("MoveSpeed", movementInput.magnitude); // Assuming you have a parameter named MoveSpeed
    }

    public void OnAttack(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            Attack();
            animator.SetTrigger("Attack"); // Assuming you have a trigger parameter named Attack
        }
    }

    private void Update()
    {
        Move();
    }

    private void Move()
    {
        // Calculate movement direction based on input
        Vector3 direction = new Vector3(movementInput.x, 0, movementInput.y).normalized;

        // Move the Rigidbody at a certain speed
        rb.MovePosition(transform.position + direction * moveSpeed * Time.deltaTime);

        // Rotate the tank to face the movement direction
        if (direction != Vector3.zero)
        {
            Quaternion targetRotation = Quaternion.LookRotation(direction);
            rb.MoveRotation(Quaternion.Slerp(transform.rotation, targetRotation, 0.1f));
        }
    }

    private void Attack()
    {
        // Instantiate a new projectile from prefab at the firePoint position
        GameObject projectile = Instantiate(projectilePrefab, firePoint.position, firePoint.rotation);

        // Give speed to the projectile
        Rigidbody projectileRb = projectile.GetComponent<Rigidbody>();
        if (projectileRb != null)
        {
            projectileRb.velocity = firePoint.forward * projectileSpeed;
        }
    }
}
