using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    public float moveSpeed = 5f; // Kecepatan gerak
    public float rotateSpeed = 720f; // Kecepatan rotasi
    private Rigidbody rb;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        rb.freezeRotation = true; // Membekukan rotasi agar tidak terpengaruh fisika
    }

    void Update()
    {
        MovePlayer();
        RotatePlayer();
    }

    void MovePlayer()
    {
        // Input untuk pergerakan
        float moveX = Input.GetAxis("Horizontal"); // A dan D
        float moveZ = Input.GetAxis("Vertical");   // W dan S

        // Vektor gerakan berdasarkan input dan kecepatan
        Vector3 movement = transform.forward * moveZ + transform.right * moveX;
        movement *= moveSpeed * Time.deltaTime;

        // Menggerakkan karakter
        rb.MovePosition(rb.position + movement);
    }

    void RotatePlayer()
    {
        // Input rotasi berdasarkan input mouse
        float mouseX = Input.GetAxis("Mouse X");

        // Rotasi karakter
        Vector3 rotation = Vector3.up * mouseX * rotateSpeed * Time.deltaTime;
        rb.MoveRotation(rb.rotation * Quaternion.Euler(rotation));
    }
}
