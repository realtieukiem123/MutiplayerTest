using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;

public class HealingZone : NetworkBehaviour
{
    [Header("Referrences")]
    [SerializeField] private Image healPowerBar;
    [Header("Setting")]
    [SerializeField] private int maxHealPower = 30;
    [SerializeField] private float healCooldown = 60f;
    [SerializeField] private float healTickRate = 1f;
    [SerializeField] private int coinsPerTick = 10;
    [SerializeField] private int healthPerTick = 10;
    float remainingCooldown;
    float tickTimer;

    private List<TankPlayer> playerInZone = new List<TankPlayer>();
    private NetworkVariable<int> HealPower = new NetworkVariable<int>();

    public override void OnNetworkSpawn()
    {
        if (IsClient)
        {
            HealPower.OnValueChanged += HandleHealPowerChanged;
            HandleHealPowerChanged(0, HealPower.Value);
        }
        if (IsServer)
        {
            HealPower.Value = maxHealPower;
        }
    }
    public override void OnNetworkDespawn()
    {
        if (IsClient)
        {
            HealPower.OnValueChanged -= HandleHealPowerChanged;
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (!IsServer) return;
        if (!collision.attachedRigidbody.TryGetComponent<TankPlayer>(out TankPlayer player)) return;

        playerInZone.Add(player);
    }
    private void OnTriggerExit2D(Collider2D collision)
    {
        if (!IsServer) return;
        if (!collision.attachedRigidbody.TryGetComponent<TankPlayer>(out TankPlayer player)) return;
        playerInZone.Remove(player);
    }
    private void Update()
    {
        if (!IsServer) return;
        if (remainingCooldown > 0f)
        {
            remainingCooldown -= Time.deltaTime;
            if (remainingCooldown < 0f)
            {
                HealPower.Value = maxHealPower;
            }
            else
            {
                return;
            }
        }
        tickTimer += Time.deltaTime;
        if (tickTimer >= 1/healTickRate)
        {
            foreach (TankPlayer player in playerInZone)
            {
                if (HealPower.Value == 0) break;
                if (player.Health.CurrentHealth.Value == player.Health.MaxHealth) continue;
                if (player.Wallet.TotalCoins.Value < coinsPerTick) continue;

                player.Wallet.SpendCoins(coinsPerTick);
                player.Health.RestoreHealth(healthPerTick);
                HealPower.Value -= 1;
                if (HealPower.Value == 0)
                {
                    remainingCooldown = healCooldown;
                }
            }
            tickTimer = tickTimer % (1/healTickRate);
        }
    }
    private void HandleHealPowerChanged(int oldHealPower, int newHealPower)
    {
        healPowerBar.fillAmount = (float)newHealPower / maxHealPower;
    }
}
