using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
using Unity.Mathematics;
using ProjectTank.Utilities;
using ProjectTank;

public class BulletPool : SingletonNetworkBehaviour<BulletPool>
{
    public Bullet bulletPrefab;
    [SerializeField] private List<GameObject> bulletPool = new List<GameObject>();
    private int bulletPoolIndex = 0;
    public int CurrentAmmo { get; private set; }

    public override void OnNetworkSpawn()
    {
        if (IsServer || IsHost)
        {
            InitializePool(30);
        }
    }

    public void InitializePool(int maxAmmo)
    {
        if (!IsServer) { return; }

        for (int i = 0; i < maxAmmo; i++)
        {
            GameObject bullet = Instantiate(bulletPrefab.gameObject, transform.position, quaternion.identity, transform);
            bullet.SetActive(false);
            bullet.transform.position = transform.position;

            NetworkObject networkObject = bullet.GetComponent<NetworkObject>();
            if (networkObject == null)
            {
                networkObject = bullet.AddComponent<NetworkObject>();
            }

            networkObject.Spawn(); // Spawn objek tanpa mengubah kepemilikan
            bulletPool.Add(bullet);
        }

        CurrentAmmo = maxAmmo;
    }

    public void RequestShootBullet(Vector3 position, quaternion direction, RobotController shooter)
    {
        // Mengirim NetworkObjectId milik shooter ke server
        ShootBulletServerRpc(position, new ForceNetworkSerializeByMemcpy<quaternion>(direction), shooter.NetworkObjectId);
    }

    [ServerRpc(RequireOwnership = false)]
    public void ShootBulletServerRpc(Vector3 position, ForceNetworkSerializeByMemcpy<quaternion> direction, ulong shooterId)
    {
        if (bulletPoolIndex >= bulletPool.Count)
        {
            bulletPoolIndex = 0;
            return;
        }

        GameObject bullet = ShootBullet(position, direction.Value);

        if (bullet != null)
        {
            if (NetworkManager.Singleton.SpawnManager.SpawnedObjects.TryGetValue(shooterId, out NetworkObject shooterNetworkObject))
            {
                RobotController shooter = shooterNetworkObject.GetComponent<RobotController>();
                bullet.GetComponent<Bullet>().SetShooter(shooter);
            }
            SetBulletActiveClientRpc(bullet.GetComponent<NetworkObject>().NetworkObjectId, position, direction);
        }
    }

    private GameObject ShootBullet(Vector3 position, quaternion direction)
    {
        GameObject bullet = bulletPool[bulletPoolIndex];
        bullet.transform.position = position;
        bullet.transform.rotation = direction;
        bullet.SetActive(true);

        bulletPoolIndex += 1;
        CurrentAmmo -= 1;

        return bullet;
    }

    [ClientRpc]
    private void SetBulletActiveClientRpc(ulong bulletId, Vector3 position, ForceNetworkSerializeByMemcpy<quaternion> direction)
    {
        if (NetworkManager.Singleton.SpawnManager.SpawnedObjects.TryGetValue(bulletId, out NetworkObject bulletNetworkObject))
        {
            GameObject bullet = bulletNetworkObject.gameObject;
            bullet.transform.position = position;
            bullet.transform.rotation = direction.Value;
            bullet.SetActive(true);
        }
    }

    [ServerRpc(RequireOwnership = false)]
    public void ReturnAllBulletsToPoolServerRpc(int maxAmmo)
    {
        foreach (GameObject bullet in bulletPool)
        {
            bullet.transform.position = transform.position;
            bullet.SetActive(false);
            bullet.transform.SetParent(transform);
        }

        CurrentAmmo = maxAmmo;
        bulletPoolIndex = 0;

        ReturnAllBulletsClientRpc(maxAmmo);
    }

    [ClientRpc]
    private void ReturnAllBulletsClientRpc(int maxAmmo)
    {
        foreach (GameObject bullet in bulletPool)
        {
            bullet.transform.position = transform.position;
            bullet.SetActive(false);
            bullet.transform.SetParent(transform);
        }

        CurrentAmmo = maxAmmo;
        bulletPoolIndex = 0;
    }
}
