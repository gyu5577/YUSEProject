using UnityEngine;

public static class SaveManager
{
    #region Constants (Key Definitions)
    // 상수
    private const string KEY_GOLD = "Player_Gold";
    private const string KEY_MASTER_VOLUME = "Setting_MasterVolume";
    private const string KEY_BGM_VOLUME = "Setting_BgmVolume";
    private const string KEY_SFX_VOLUME = "Setting_SfxVolume";
    
    // 업그레이드 키 접두사 미리 만들어둔것.
    private const string KEY_UPGRADE_PREFIX = "Upgrade_";
    #endregion
    
    //저장부분
    #region Public Methods (General)
    public static void Save()
    {
        //저장
        PlayerPrefs.Save();
    }
    
    public static void DeleteAll()
    {
        //삭제
        PlayerPrefs.DeleteAll();
        Save();
    }
    
    //조회
    public static bool HasKey(string key)
    {
        return PlayerPrefs.HasKey(key);
    }
    #endregion
    
    //골-드
    #region Public Methods (Gold)
    public static void SaveGold(int amount)
    {
        PlayerPrefs.SetInt(KEY_GOLD, amount);
    }

    public static int LoadGold()
    {
        // 저장된 값이 없으면 0
        return PlayerPrefs.GetInt(KEY_GOLD, 0);
    }
    #endregion
    
    #region Public Methods (Upgrade Levels)
    /// <summary>
    /// 특정 능력치 강화 레벨 저장
    /// </summary>
    public static void SaveUpgradeLevel(UpgradeType upgradeType, int level)
    {
        string key = GetUpgradeKey(upgradeType);
        PlayerPrefs.SetInt(key, level);
    }
    
    /// <summary>
    /// 특정 능력치 강화 레벨 불러옴
    /// </summary>
    public static int LoadUpgradeLevel(UpgradeType upgradeType)
    {
        string key = GetUpgradeKey(upgradeType);
        // 기본 레벨은 0
        return PlayerPrefs.GetInt(key, 0);
    }
    
    private static string GetUpgradeKey(UpgradeType upgradeType)
    {
        return $"{KEY_UPGRADE_PREFIX}{upgradeType}";
    }
    #endregion

    #region Public Methods (Settings)
    public static void SaveVolume(string volumeType, float value)
    {
        switch (volumeType)
        {
            case "Master": PlayerPrefs.SetFloat(KEY_MASTER_VOLUME, value); break;
            case "BGM": PlayerPrefs.SetFloat(KEY_BGM_VOLUME, value); break;
            case "SFX": PlayerPrefs.SetFloat(KEY_SFX_VOLUME, value); break;
        }
    }

    public static float LoadVolume(string volumeType, float defaultValue = 1.0f)
    {
        switch (volumeType)
        {
            case "Master": return PlayerPrefs.GetFloat(KEY_MASTER_VOLUME, defaultValue);
            case "BGM": return PlayerPrefs.GetFloat(KEY_BGM_VOLUME, defaultValue);
            case "SFX": return PlayerPrefs.GetFloat(KEY_SFX_VOLUME, defaultValue);
            default: return defaultValue;
        }
    }
    #endregion
}
