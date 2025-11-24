using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// 메인 메뉴의 패널 전환 및 타이틀 화면 연출을 관리하는 클래스
/// </summary>
public class MainMenuPanelManager : MonoBehaviour
{
    #region Constants
    private const string BGM_NAME = "BGM";
    private const string SFX_SELECT = "Select";
    #endregion

    #region Serialized Fields
    [Header("Panels")]
    [SerializeField] private GameObject mainPanel;
    [SerializeField] private GameObject lobbyPanel;
    [SerializeField] private GameObject upgradePanel;
    [SerializeField] private GameObject codexPanel;
    [SerializeField] private GameObject settingPanel;

    [Header("UI Elements")]
    [SerializeField] private Text pressAnyKeyText;
    #endregion

    #region Private Fields
    // static이 굳이 필요한지 점검 필요하나, 원본 유지 차원에서 static 유지
    private static bool _isPanelShown = false; 
    #endregion

    #region Unity LifeCycle
    private void Start()
    {
        AudioManager.Instance.PlayBGM(BGM_NAME);
        CheckAndShowTitlePanel();
    }

    private void Update()
    {
        if (!pressAnyKeyText) return;

        float newAlpha = CalculateAlpha();
        UpdateBlinkText(newAlpha);

        HandleLobbyInput();
    }
    #endregion

    #region Public Methods
    /// <summary>
    /// 아무 키나 눌렀을 때 로비 패널로 전환합니다.
    /// </summary>
    public void HandleLobbyInput() // 이름 변경: ShowLobbyPanel -> HandleLobbyInput (Update에서 호출되므로)
    {
        if (Input.anyKeyDown && mainPanel.activeSelf && !lobbyPanel.activeSelf)
        {
            mainPanel.SetActive(false);
            lobbyPanel.SetActive(true);
        }
    }

    /// <summary>
    /// 업그레이드 패널을 켜거나 끕니다.
    /// </summary>
    public void ToggleUpgradePanel()
    {
        AudioManager.Instance.PlaySfx(SFX_SELECT);
        upgradePanel.SetActive(!upgradePanel.activeSelf);
    }

    /// <summary>
    /// 옵션 패널을 켜거나 끕니다.
    /// </summary>
    public void ToggleSettingPanel()
    {
        AudioManager.Instance.PlaySfx(SFX_SELECT);
        settingPanel.SetActive(!settingPanel.activeSelf);
    }

    /// <summary>
    /// 도감 패널을 켜거나 끕니다.
    /// </summary>
    public void ToggleCodexPanel()
    {
        AudioManager.Instance.PlaySfx(SFX_SELECT);
        codexPanel.SetActive(!codexPanel.activeSelf);
    }
    #endregion

    #region Private Methods
    /// <summary>
    /// 게임 최초 실행 시에만 타이틀 패널을 보여줍니다.
    /// </summary>
    private void CheckAndShowTitlePanel()
    {
        if (!_isPanelShown)
        {
            mainPanel.SetActive(true);
            _isPanelShown = true;
            Debug.Log(_isPanelShown);
        }
        else
        {
            mainPanel.SetActive(false);
        }
    }

    /// <summary>
    /// 텍스트 깜빡임을 위한 알파값을 계산합니다.
    /// </summary>
    private float CalculateAlpha() // AlphaChange -> CalculateAlpha (동사형)
    {
        return Mathf.PingPong(Time.time * 0.5f, 1f);
    }

    /// <summary>
    /// 계산된 알파값을 텍스트에 적용합니다.
    /// </summary>
    /// <param name="alpha">적용할 알파값</param>
    private void UpdateBlinkText(float alpha) // BlinkText -> UpdateBlinkText (동사형)
    {
        Color newColor = pressAnyKeyText.color;
        newColor.a = alpha;
        pressAnyKeyText.color = newColor;
    }
    #endregion
}