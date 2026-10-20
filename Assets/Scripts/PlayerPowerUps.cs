using UnityEngine;

// Gives the player a speed boost for a limited time.
public class PlayerPowerUps : MonoBehaviour
{
    [SerializeField] private float boostMultiplier = 1.5f;

    // How many seconds of speed boost remain.
    // Other scripts can read this, but only this script can change it.
    public float SpeedTimeLeft { get; private set; }

    private PlayerMovement movement;

    private void Start()
    {
        movement = GetComponent<PlayerMovement>();
    }

    // Called when the player collects a speed boost.
    public void ActivateSpeedBoost(float duration)
    {
        SpeedTimeLeft = duration;

        movement.SpeedMultiplier = boostMultiplier;
    }

    private void Update()
    {
        // No boost is active.
        if (SpeedTimeLeft <= 0f)
        {
            return;
        }

        // Count down using real elapsed time.
        SpeedTimeLeft -= Time.deltaTime;

        // Boost has finished.
        if (SpeedTimeLeft <= 0f)
        {
            SpeedTimeLeft = 0f;
            movement.SpeedMultiplier = 1f;
        }
    }
}