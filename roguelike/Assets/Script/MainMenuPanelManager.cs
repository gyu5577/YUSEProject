using UnityEngine;
using UnityEngine.UI;
using System.Collections;
public class MainMenuPanelManager : MonoBehaviour
{
    #region Panel
    [Header("Panel �� text ����")]
    public GameObject MainPanel;
    public GameObject LobbyPanel;
    public GameObject UpgradePanel;
    public GameObject CodexPanel;
    public GameObject OptionPanel;

    public Text press_Anykey;
    #endregion



    #region life Cycle
    void Start()
    {
        AudioManager.Instance.PlayBGM("BGM");
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
    //�ƹ�Ű�� ������ �κ�� �Ѿ��
    public void ShowLobbyPanel()
    {
        if(Input.anyKeyDown && MainPanel.activeSelf && !LobbyPanel.activeSelf)
        {
            MainPanel.SetActive(false);
            LobbyPanel.SetActive(true);
        }
    }

    //��ȭ �г� ���
    public void ToggleUpgradePanel()
    { 
        AudioManager.Instance.PlaySfx("Select");
       UpgradePanel.SetActive(!UpgradePanel.activeSelf);
    }

    
    //�ɼ� �г� ���
    public void ToggleOptionPanel()
    {
        AudioManager.Instance.PlaySfx("Select");
        OptionPanel.SetActive(!OptionPanel.activeSelf);  
    }

    // ���� �г� ���
    public void ToggleCodexPanel()
    {
        AudioManager.Instance.PlaySfx("Select");
        CodexPanel.SetActive(!CodexPanel.activeSelf);
    }
    #endregion


    #region Title Blink method
    //text blink �Լ�
    private float AlphaChange()
    {
        float normalizedAlpha = Mathf.PingPong(Time.time * 0.5f, 1f);  
        return normalizedAlpha;
    }

    //���İ� �ٲ��ִ� �Լ�
    private void BlinkText(float alpha)
    {
        Color newColor = press_Anykey.color;
        newColor.a = alpha;
        press_Anykey.color = newColor;
    }
    #endregion


}
