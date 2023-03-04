using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

/// <summary>
/// Class for UI that is specific to in-level gameplay.
/// </summary>
public class GameplayUI : MonoBehaviour
{
    public static GameplayUI Instance { get { return _instance; } }
    private static GameplayUI _instance;

    [SerializeField] private RectTransform reloadBarTransform;
    [SerializeField] private TMP_Text ammoTxt;
    [SerializeField] private TMP_Text waveTxt;
    [SerializeField] private TMP_Text pointsTxt;

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

        GameManager.OnPrepForNextWave.AddListener(StartNewWaveCoroutine);
    }

    private void Update()
    {
        (int loaded, int total) = GameManager.Instance.GetPlayerScript().GetAmmo();
        ammoTxt.text = "Ammo: " + loaded + "/" + total;
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

    private void OnDestroy()
    {
        GameManager.OnPrepForNextWave.RemoveListener(StartNewWaveCoroutine);
    }
}
