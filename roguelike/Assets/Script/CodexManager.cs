using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class CodexManager : MonoBehaviour
{
    [Header("panel")]
    [SerializeField] private GameObject[] allCodexPanels;

    






    #region Button Action

    //ui tab키 구현
    public void OpenPanel(GameObject targetPanel)
    {
        foreach (GameObject panel in allCodexPanels)
        {
            if(panel==targetPanel)
            {
                panel.SetActive(true);
            }
            else
            {
                panel.SetActive(false);
            }
        }
    }


    #endregion


    void Start()
    {
        
    }

    
    void Update()
    {
        
    }
}
