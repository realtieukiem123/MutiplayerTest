using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class ProjectileLauncher : NetworkBehaviour
{
    [Header("Refer")]
    [SerializeField] private InputReader inputReader;
    [SerializeField] private CoinWallet wallet;
    [SerializeField] private Transform projectileSpawnPoint;
    [SerializeField] private GameObject serverProjectilePrefab;
    [SerializeField] private GameObject clientProjectilePrefab;
    [SerializeField] private GameObject muzzleFlash;
    [SerializeField] private Collider2D playerCollider;
    [Header("Setting")]
    [SerializeField] private float projectileSpeed = 30f;
    [SerializeField] private float fireRate;
    [SerializeField] private float muzzleFlashDur;
    [SerializeField] private int costToFire;

    private bool shouldFire;
    private float timer;
    private float muzzleFlashTimer;
    private void Update()
    {
        if (muzzleFlashTimer > 0f)
        {
            muzzleFlashTimer -= Time.deltaTime;
            if (muzzleFlashTimer <= 0f)
            {
                muzzleFlash.SetActive(false);
            }
        }
        if (!IsOwner) return;

        if (timer > 0) { timer -= Time.deltaTime; }

        if (!shouldFire) return;

        if (timer > 0) return;

        if (wallet.TotalCoins.Value < costToFire) { return; }

        PrimaryFireServerRpc(projectileSpawnPoint.position, projectileSpawnPoint.up);
        SpawnDummyProjectile(projectileSpawnPoint.position, projectileSpawnPoint.up);
        timer = 1 / fireRate;
    }

    public override void OnNetworkSpawn()
    {
        if (!IsOwner) return;
        inputReader.PrimaryFireEvent += HandPrimaryFire;

    }
    public override void OnNetworkDespawn()
    {
        if (!IsOwner) return;
        inputReader.PrimaryFireEvent -= HandPrimaryFire;
    }
    private void HandPrimaryFire(bool shouldFire)
    {
        this.shouldFire = shouldFire;
    }

    [ServerRpc]
    private void PrimaryFireServerRpc(Vector3 spawnPos, Vector3 dir)
    {
        if (wallet.TotalCoins.Value < costToFire) { return; }

        wallet.SpendCoins(costToFire);

        GameObject v = Instantiate(serverProjectilePrefab, spawnPos, Quaternion.identity);
        v.transform.up = dir;

        Physics2D.IgnoreCollision(playerCollider, v.GetComponent<Collider2D>());

        if (v.TryGetComponent<DealDamageOnContact>(out DealDamageOnContact dealDamage))
        {
            dealDamage.SetOwner(OwnerClientId);
        }

        if (v.TryGetComponent<Rigidbody2D>(out Rigidbody2D rb))
        {
            rb.velocity = rb.transform.up * projectileSpeed;
        }

        SpawnDummyProjectileClientRpc(spawnPos, dir);
    }
    [ClientRpc]
    private void SpawnDummyProjectileClientRpc(Vector3 spawnPos, Vector3 dir)
    {
        if (IsOwner) return;
        SpawnDummyProjectile(spawnPos, dir);
    }
    private void SpawnDummyProjectile(Vector3 pos, Vector3 dir)
    {
        muzzleFlash.SetActive(true);
        muzzleFlashTimer = muzzleFlashDur;

        GameObject v = Instantiate(clientProjectilePrefab, pos, Quaternion.identity);
        v.transform.up = dir;

        Physics2D.IgnoreCollision(playerCollider, v.GetComponent<Collider2D>());

        if (v.TryGetComponent<Rigidbody2D>(out Rigidbody2D rb))
        {
            rb.velocity = rb.transform.up * projectileSpeed;
        }
    }
}
