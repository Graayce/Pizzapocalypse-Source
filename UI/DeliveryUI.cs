using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class DeliveryUI : MonoBehaviour
{
    // References
    private Transform mainCamera;
    private TextMeshProUGUI successfulDeliveriesText;
    private TextMeshProUGUI timeLeft;
    private RectTransform directionImg;
    private DeliveryManager deliveryManager;
    private Transform gameOverScreen;
    private TextMeshProUGUI highScoreText;
    private TextMeshProUGUI endDeliveriesText;

    // Attributes/Data
    private float timeForDelivery;

    void Start()
    {
        mainCamera = Camera.main.transform;
        Transform totalDeliveries = transform.Find("TotalDeliveries");

        successfulDeliveriesText = totalDeliveries.Find("Total").GetComponent<TextMeshProUGUI>();
        timeLeft = totalDeliveries.Find("TimeLeft").GetComponent<TextMeshProUGUI>();
        directionImg = transform.Find("Direction").GetComponent<RectTransform>();
        gameOverScreen = transform.Find("GameOver");

        endDeliveriesText = gameOverScreen.Find("Deliveries").Find("Num").GetComponent<TextMeshProUGUI>();
        highScoreText = gameOverScreen.Find("HighScore").Find("Num").GetComponent<TextMeshProUGUI>();

        deliveryManager = DeliveryManager.instance;

        deliveryManager.NewLocation += UpdateUI;
        deliveryManager.gameFailed += GameFailed;
        timeForDelivery = deliveryManager.TimeForDelivery;
    }

    void Update()
    {
        if (DeliveryManager.instance.Failed) { return; }
        AimArrow();
        timeLeft.text = Mathf.FloorToInt(deliveryManager.CurrentTime) + "";
    }

    private void AimArrow()
    {
        Vector3 direction = deliveryManager.CurrentLocation.position - mainCamera.position;
        Vector3 forward = mainCamera.forward;

        direction.Normalize();

        direction.y = 0;
        forward.y = 0;

        float angle = Vector3.SignedAngle(direction, forward, Vector3.up);

        directionImg.localRotation = Quaternion.Euler(0, 0, angle);
    }

    private void UpdateUI()
    {
        successfulDeliveriesText.text = deliveryManager.DeliveriesCompleted + "";
    }

    private void GameFailed()
    {
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        gameOverScreen.gameObject.SetActive(true);
        gameOverScreen.GetComponent<Animator>().Play("GameOver_Open");
        
        int score = DeliveryManager.instance.DeliveriesCompleted;
        int highScore = 0;
        
        if (PlayerPrefs.HasKey("High Score"))
        {
            int oldHighScore = PlayerPrefs.GetInt("High Score");
            if (oldHighScore < score)
                PlayerPrefs.SetInt("High Score", score);
        }
        else
            PlayerPrefs.SetInt("High Score", DeliveryManager.instance.DeliveriesCompleted);

        highScore = PlayerPrefs.GetInt("High Score");

        endDeliveriesText.text = score + "";
        highScoreText.text = highScore + "";
    }
}
