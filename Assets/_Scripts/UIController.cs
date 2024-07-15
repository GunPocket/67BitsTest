using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIController : MonoBehaviour {
    [Header("UI Elements")]
    [SerializeField] private TMP_Text moneyText;
    [SerializeField] private Button buyButton;

    [Header("References")]
    [SerializeField] private CharacterMovement characterMovement;

    private int money;

    private void Start() {
        buyButton.onClick.AddListener(OnBuyButtonClicked);
        buyButton.interactable = false;
        UpdateMoneyUI();
    }

    public void UpdateMoney(int value) {
        money += value;
        UpdateMoneyUI();
        CheckBuyButtonInteractable();
    }

    private void UpdateMoneyUI() {
        moneyText.text = $"Money: {money}";
    }

    private void CheckBuyButtonInteractable() {
        buyButton.interactable = money >= 100;
    }

    private void OnBuyButtonClicked() {
        if (money >= 100) {
            money -= 100;
            characterMovement.IncreaseMaxStack();
            UpdateMoneyUI();
            CheckBuyButtonInteractable();
        }
    }
}
