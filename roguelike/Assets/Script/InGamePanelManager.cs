/*
 * [InGamePanelManager.cs]
 * [패키지 4] 인게임 UI/UX
 * (더미 스크립트: Sprint 1~3에 걸쳐 구현)
 *
 * GameManager(묶음 1)가 컴파일 오류를 일으키지 않도록
 * 필수 함수들을 임시로 정의합니다. (SDS 3.2.3)
 */

using UnityEngine;

public class InGamePanelManager : MonoBehaviour
{
    #region Serialized Fields
    // (컨벤션 1-1) 
    // Sprint 1에서 UI 아티스트가 만든 프리팹을 이곳에 연결합니다.
    [Header("UI Panels")]
    [SerializeField]
    private GameObject pausePanel;
    [SerializeField]
    private GameObject rewardPanel;
    [SerializeField]
    private GameObject gameOverPanel;
    [SerializeField]
    private GameObject gameClearPanel;
    
    // (Sprint 2)
    // [SerializeField] private GameObject settingPanel;
    #endregion

    #region Public Methods
    /// <summary>
    /// (S2, D-2.a) GameManager가 호출할 더미 함수
    /// </summary>
    public void ShowPausePanel(bool show)
    {
        Debug.Log("InGamePanelManager: 일시정지 패널 " + (show ? "표시" : "숨김"));
        if (pausePanel != null)
            pausePanel.SetActive(show);
    }

    /// <summary>
    /// (S2, D-2.b) GameManager가 호출할 더미 함수
    /// </summary>
    public void ShowRewardPanel(bool show)
    {
        Debug.Log("InGamePanelManager: 보상 패널 " + (show ? "표시" : "숨김"));
        if (rewardPanel != null)
            rewardPanel.SetActive(show);
    }

    /// <summary>
    /// (S3, D-2.c) GameManager가 호출할 더미 함수
    /// </summary>
    public void ShowGameOverPanel(bool show)
    {
        Debug.Log("InGamePanelManager: 게임 오버 패널 " + (show ? "표시" : "숨김"));
        if (gameOverPanel != null)
            gameOverPanel.SetActive(show);
    }

    /// <summary>
    /// (S3, D-2.c) GameManager가 호출할 더미 함수
    /// </summary>
    public void ShowGameClearPanel(bool show)
    {
        Debug.Log("InGamePanelManager: 게임 클리어 패널 " + (show ? "표시" : "숨김"));
        if (gameClearPanel != null)
            gameClearPanel.SetActive(show);
    }

    // (Sprint 2)
    // 패널의 '계속하기', '메인으로' 버튼이
    // GameManager.Instance.ResumeGame(), GameManager.Instance.GoToMainMenu()를
    // 호출하도록 OnClick 이벤트를 인스펙터에서 연결해야 합니다.
    #endregion
}