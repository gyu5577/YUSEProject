using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class MainMenuPanelManager : MonoBehaviour
{
    #region Panel
    [Header("Panel 및 text 변수")]
    public GameObject MainPanel;
    public GameObject LobbyPanel;
    public GameObject UpgradePanel;
    public GameObject CodexPanel;
    public GameObject OptionPanel;

    public Text press_Anykey;
    #endregion
    private static bool PanelShowKey = false;


    #region life Cycle
    void Start()
    {
        AudioManager.Instance.PlayBGM("BGM");
      CheckAndShowTitlePanel();
    }

    // Update is called once per frame
    void Update()
    {

        if (press_Anykey == null) return;
        float newAlpha = AlphaChange();
        BlinkText(newAlpha);

        ShowLobbyPanel();
    }
    #endregion

    #region Button Action

    //처음 실행시만 타이틀 화면 보여주기
    private void CheckAndShowTitlePanel()
    {
        if (PanelShowKey == false)
        {

            MainPanel.SetActive(true);
            PanelShowKey = true;
            Debug.Log(PanelShowKey);
        }
        else
        {

            MainPanel.SetActive(false);

        }
    }

    //아무키나 누르면 로비로 넘어가기
    public void ShowLobbyPanel()
    {
        if(Input.anyKeyDown && MainPanel.activeSelf && !LobbyPanel.activeSelf)
        {
            MainPanel.SetActive(false);
            LobbyPanel.SetActive(true);
        }
    }

    //강화 패널 토글
    public void ToggleUpgradePanel()
    { 
        AudioManager.Instance.PlaySfx("Select");
       UpgradePanel.SetActive(!UpgradePanel.activeSelf);
    }

    
    //옵션 패널 토글
    public void ToggleOptionPanel()
    {
        AudioManager.Instance.PlaySfx("Select");
        OptionPanel.SetActive(!OptionPanel.activeSelf);  
    }

    // 도감 패널 토글
    public void ToggleCodexPanel()
    {
        AudioManager.Instance.PlaySfx("Select");
        CodexPanel.SetActive(!CodexPanel.activeSelf);
    }
    #endregion


    #region Title Blink method
    //text blink 함수
    private float AlphaChange()
    {
        float normalizedAlpha = Mathf.PingPong(Time.time * 0.5f, 1f);  
        return normalizedAlpha;
    }

    //알파값 바꿔주는 함수
    private void BlinkText(float alpha)
    {
        Color newColor = press_Anykey.color;
        newColor.a = alpha;
        press_Anykey.color = newColor;
    }
    #endregion


}
