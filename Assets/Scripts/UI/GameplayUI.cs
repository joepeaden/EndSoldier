using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using UnityEngine.Events;

/// <summary>
/// Class for UI that is specific to in-level gameplay.
/// </summary>
public class GameplayUI : MonoBehaviour
{
    public static GameplayUI Instance { get { return _instance; } }
    private static GameplayUI _instance;

    public static UnityEvent<ShopItem> OnRewardsPicked = new UnityEvent<ShopItem>();

    #region BattleUI vars
    [Header("Battle UI")]
    [SerializeField] private GameObject battleUI;
    [SerializeField] private TMP_Text curntWpnTxt;
    [SerializeField] private TMP_Text ammoTxt;
    [SerializeField] private TMP_Text waveTxt;
    [SerializeField] private TMP_Text pointsTxt;
    [SerializeField] private RectTransform reloadBarTransform;
    #endregion

    #region RewardUI vars
    [Header("Reward UI")]
    [SerializeField] private GameObject rewardUI;
    [SerializeField] private Button confirmRewardButton;
    [SerializeField] private Button shopItemButtonPrefab;
    [SerializeField] private Transform shopItemParent;
    private ShopItem pickedRewardItem;
    #endregion

    private Player player;

    private void Awake()
    {
        if (_instance != null && _instance != this)
        {
            Debug.Log("More than one Gameplay UI, deleting one.");
            Destroy(this.gameObject);
        }
        else
        {
            _instance = this;
        }

        WaveManager.OnPrepForNextWave.AddListener(StartNewWaveCoroutine);
        WaveManager.OnWaveEnd.AddListener(ShowRewardUI);
        Scoreboard.OnScoreUpdated.AddListener(UpdateScore);
        GameManager.OnGameOver.AddListener(ShowMouse);

        confirmRewardButton.onClick.AddListener(HandleRewardConfirm);

        Cursor.visible = false;
    }

    private void Start()
    {
        player = GameManager.Instance.GetPlayerScript();
        player.OnSwitchWeapons.AddListener(UpdateCurrentWeapon);

        // missed the UpdateCurrentWeapon initial event, so just update it.
        UpdateCurrentWeapon(player.GetInventory().GetEquippedWeapon());
    }

    private void Update()
    {
        if (player)
        {
            (int loaded, int total) = player.GetAmmo();

            // int.MinValue means that the gun has infinite backup ammo (pistol)
            string totalAmmoString = (total == int.MinValue) ? "INF" : total.ToString();

            ammoTxt.text = "Ammo: " + loaded + "/" + totalAmmoString;
        }
    }

    private void OnDestroy()
    {
        WaveManager.OnPrepForNextWave.RemoveListener(StartNewWaveCoroutine);
        WaveManager.OnWaveEnd.RemoveListener(ShowRewardUI);
        player.OnSwitchWeapons.RemoveListener(UpdateCurrentWeapon);
        Scoreboard.OnScoreUpdated.RemoveListener(UpdateScore);
        GameManager.OnGameOver.RemoveListener(ShowMouse);
        confirmRewardButton.onClick.RemoveListener(HandleRewardConfirm);
    }

    public bool InMenu()
    {
        return rewardUI.activeInHierarchy;
    }

    private void ShowMouse()
    {
        Cursor.visible = true;
    }

    /// <summary>
    /// Handle a reward choice.
    /// </summary>
    /// <param name="rewardKey">String code for the inventory item chosen.</param>
    public void HandleRewardPicked(ShopItem shopItem)
    {
        // if it's the same button, deselect the reward by making a blank one.
        if (shopItem.rewardKey == pickedRewardItem.rewardKey)
        {
            // this could be better. Instantiating a new object every time isn't really necessary.
            // I don't care though. But now you know that I know. Bro.
            shopItem = new ShopItem();
        }

        pickedRewardItem = shopItem;
    }    

    private void HandleRewardConfirm()
    {
        OnRewardsPicked.Invoke(pickedRewardItem);
        ShowBattleUI();
    }

    /// <summary>
    /// Activates reward UI, shows mouse, and populates shop items
    /// </summary>
    private void ShowRewardUI()
    {
        ShowMouse();

        rewardUI.SetActive(true);
        battleUI.SetActive(false);

        // clear children
        for (int i = 0; i < shopItemParent.childCount; i++)
        {
            Destroy(shopItemParent.GetChild(i).gameObject);
        }

        // add reward buttons
        foreach (ShopItem item in Rewards.Instance.GetRewardShopItems())
        {
            ShopItemButton shopButtonScript = Instantiate(shopItemButtonPrefab, shopItemParent).GetComponent<ShopItemButton>();
            shopButtonScript.SetItem(item);
        }
    }

    private void ShowBattleUI()
    {
        rewardUI.SetActive(false);
        battleUI.SetActive(true);
    }

    private void UpdateScore(int totalPoints)
    {
        pointsTxt.text = "Points: $" + totalPoints + ".00";
    }

    private void UpdateCurrentWeapon(InventoryWeapon weapon)
    {
        curntWpnTxt.text = weapon.data.displayName;
    }

    private void StartNewWaveCoroutine()
    {
        StartCoroutine(WaveTextFade());
    }

    private IEnumerator WaveTextFade()
    {
        float timePassed = 0f;
        waveTxt.alpha = 0f;
        waveTxt.gameObject.SetActive(true);

        while (timePassed <  1f)
        {
            float percent = timePassed / 1f;
            waveTxt.alpha = percent;

            timePassed += Time.deltaTime;
            yield return null;
        }

        yield return new WaitForSeconds(1f);

        while (timePassed > 0f)
        {
            float percent = timePassed / 1f;
            waveTxt.alpha = percent;

            timePassed -= Time.deltaTime;
            yield return null;
        }

        waveTxt.gameObject.SetActive(false);
    }

    /// <summary>
    /// Start the reload bar animation.
    /// </summary>
    /// <param name="time">Time over which the bar should stretch.</param>
    public void StartReloadBarAnimation(float time)
    {
        StartCoroutine(ReloadBarStretch(time));
    }

    /// <summary>
    /// Stretches the reload bar from 0 to some value to show feedback for reloading.
    /// </summary>
    /// <param name="time">Time over which the bar should stretch.</param>
    public IEnumerator ReloadBarStretch(float time)
    {
        float timePassed = 0f;

        float origSize = reloadBarTransform.sizeDelta.x;
        reloadBarTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, 0f);
        reloadBarTransform.gameObject.SetActive(true);

        while (timePassed < time)
        {
            float percent = timePassed / time;
            reloadBarTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, (origSize * percent));

            timePassed += Time.deltaTime;
            yield return null;
        }

        reloadBarTransform.gameObject.SetActive(false);
    }
}
