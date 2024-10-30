using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
using Unity.Mathematics;

public class BulletPool : NetworkBehaviour
{
    public Bullet bulletPrefab;
    public int poolSize = 20;

    [SerializeField] private List<GameObject> bulletPool = new List<GameObject>();
    private int bulletPoolIndex = 0;

    private void Start()
    {
        if (!IsOwner) return;
        InitializePool();
    }

    private void InitializePool()
    {
        for (int i = 0; i < poolSize; i++)
        {
            GameObject bullet = Instantiate(bulletPrefab.gameObject, transform.position, quaternion.identity, transform);
            bullet.SetActive(false);
            bullet.transform.position = transform.position;
            bulletPool.Add(bullet);
        }
    }

    public void ShootBullet()
    {
        if (bulletPoolIndex >= bulletPool.Count)
        {
            Debug.Log("bullet empty");
            return;
        }
        else
        {
            bulletPool[bulletPoolIndex].SetActive(true);
            bulletPool[bulletPoolIndex].transform.SetParent(null);
        }

        bulletPoolIndex += 1;
        // ReturnAllBulletsToPlayer();
    }

    private void ReturnAllBulletsToPlayer() // TODO : perlu di perbaiki agar bisa kembalikan ke parent awal
    {
        foreach (GameObject bullet in bulletPool)
        {
            bullet.transform.position = transform.position;
            bullet.SetActive(false);
        }

        Debug.Log("Semua peluru dikembalikan ke pemain.");
    }
}
