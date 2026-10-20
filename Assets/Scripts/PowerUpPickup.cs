using UnityEngine;

// A speed boost pickup. When the player touches it, it gives the boost,
// plays a sound and particles, hides, and comes back a few seconds later.
[RequireComponent(typeof(AudioSource), typeof(SphereCollider))]
public class PowerUpPickup : MonoBehaviour
{
    [SerializeField] private float duration = 10f;
    [SerializeField] private float respawnDelay = 5f;
    [SerializeField] private GameObject visuals;
    [SerializeField] private AudioClip pickupSound;
    [SerializeField] private GameObject pickupEffect;

    private bool collected;

    private void OnTriggerEnter(Collider other)
    {
        // Only the player has a PlayerPowerUps component
        if (collected) return;
        PlayerPowerUps powerUps = other.GetComponentInParent<PlayerPowerUps>();

        if (powerUps == null)
        {
            return;
        }

        collected = true;
        // Give the player the speed boost
        powerUps.ActivateSpeedBoost(duration);

        // Play sound
        if (pickupSound != null) GetComponent<AudioSource>().PlayOneShot(pickupSound);

        // Spawn particle effect
        if (pickupEffect != null) Instantiate(
            pickupEffect,
            transform.position,
            Quaternion.identity
        );

        // Hide the pickup
        visuals.SetActive(false);

        // Disable collider so it cannot be collected again immediately
        GetComponent<Collider>().enabled = false;

        // Bring it back after a few seconds
        Invoke(nameof(Reappear), Mathf.Max(0f, respawnDelay));
    }

    private void Reappear()
    {
        visuals.SetActive(true);
        GetComponent<Collider>().enabled = true;
        collected = false;
    }
}
