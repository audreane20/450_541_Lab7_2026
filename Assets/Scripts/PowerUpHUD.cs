using UnityEngine;
using TMPro;

// Reads the player's timer; the HUD never keeps a separate countdown.
public class PowerUpHUD : MonoBehaviour
{
    [SerializeField] private PlayerPowerUps powerUps;
    [SerializeField] private GameObject speedIndicator;
    [SerializeField] private TMP_Text speedTimeText;

    private void Start() => Refresh();
    private void LateUpdate() => Refresh();

    private void Refresh()
    {
        float timeLeft = powerUps != null ? powerUps.SpeedTimeLeft : 0f;
        if (speedIndicator != null) speedIndicator.SetActive(timeLeft > 0f);
        if (speedTimeText != null)
            speedTimeText.text = Mathf.CeilToInt(Mathf.Max(0f, timeLeft)) + "s";
    }
}
