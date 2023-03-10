using System;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using UnityEngine.Events;

public class ShopItemButton : MonoBehaviour
{
    /// <summary>
    /// Param is rewardKey to compare. Used for updating button visuals/interactability when another button is selected.
    /// </summary>
    private static UnityEvent<string> OnAShopButtonClicked = new UnityEvent<string>();

    [SerializeField] private TMP_Text label;
    [SerializeField] private TMP_Text cost;

    private ShopItem item;
    private Button button;
    private Color originalLabelColor;
    private Color originalCostColor;
    private bool tooExpensive;

    private void Awake()
    {
        button = GetComponent<Button>();
        button.onClick.AddListener(UpdateReward);
        OnAShopButtonClicked.AddListener(UpdateVisuals);
    }

    private void OnDestroy()
    {
        button.onClick.RemoveListener(UpdateReward);
        OnAShopButtonClicked.RemoveListener(UpdateVisuals);
    }

    /// <summary>
    /// Set the shopitem for this button.
    /// </summary>
    /// <param name="item"></param>
    public void SetItem(ShopItem item)
    {
        label.text = item.displayName;
        cost.text = $"${item.cost}";
        this.item = item;

        originalLabelColor = label.color;
        originalCostColor = cost.color;

        tooExpensive = Scoreboard.totalScore - item.cost < 0;
        // disable the button if the item is too expensive
        if (tooExpensive)
        {
            UpdateVisuals();
        }
    }

    /// <summary>
    /// Informs the system of the current reward choice.
    /// </summary>
    private void UpdateReward()
    {
        GameplayUI.Instance.HandleRewardPicked(item);
        OnAShopButtonClicked.Invoke(item.rewardKey);
    }

    /// <summary>
    /// Updates button interactability.
    /// </summary>
    /// <param name="key">Currently selected rewardKey, for comparison.</param>
    private void UpdateVisuals(string key = "")
    {
        if (tooExpensive)
        {
            cost.color = Color.red;
            button.interactable = false;
        }
        else if (key == item.rewardKey)
        {
            //button.interactable = key != item.rewardKey;
            label.color = label.color != Color.gray ? Color.grey : originalLabelColor;
            cost.color = cost.color != Color.gray ? Color.grey : originalCostColor;
        }
        else if (label.color == Color.gray)
        {
            label.color = originalLabelColor;
            cost.color = originalCostColor;
        }
        
    }
}
