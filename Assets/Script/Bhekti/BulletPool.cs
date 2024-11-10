using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
using Unity.Mathematics;

public class BulletPool : MonoBehaviour
{
    public Bullet bulletPrefab;

    [SerializeField] private List<GameObject> bulletPool = new List<GameObject>();
    private int bulletPoolIndex = 0;
    public int CurrentAmmo { get; set; }
    public int MaxAmmo => bulletPool.Count;
    public void InitializePool(int maxAmmo)
    {
        for (int i = 0; i < maxAmmo; i++)
        {
            GameObject bullet = Instantiate(bulletPrefab.gameObject, transform.position, quaternion.identity, transform);
            bullet.SetActive(false);
            bullet.transform.position = transform.position;
            bulletPool.Add(bullet);
        }
        CurrentAmmo = maxAmmo;
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
            bulletPoolIndex += 1;
            CurrentAmmo -= 1;
        }

        // ReturnAllBulletsToPlayer();
    }

    public void ReturnAllBulletsToPlayer(int maxAmmo) //TODO perlu tambahkan delay
    {
        foreach (GameObject bullet in bulletPool)
        {
            bullet.transform.position = transform.position;
            bullet.SetActive(false);
            bullet.transform.SetParent(transform);
        }
        CurrentAmmo = maxAmmo;

        Debug.Log("Semua peluru dikembalikan ke pemain.");
    }
}
