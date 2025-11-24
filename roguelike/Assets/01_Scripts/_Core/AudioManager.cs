using UnityEngine;
using System.Collections.Generic; //딕셔너리 사용을 위해 추가

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance { get; private set; }
    
    #region Serialized Fields
    // 실행중인 음악,효과음
    [Header("Source")]
    [SerializeField] private AudioSource bgmSource;
    [SerializeField] private AudioSource sfxSource;
    // 음악, 효과음 리스트
    [Header("Clips")]
    [SerializeField] private AudioClip[] bgmClips; 
    [SerializeField] private AudioClip[] sfxClips;
    private Dictionary<string, AudioClip> bgmDictionary = new Dictionary<string, AudioClip>();
    private Dictionary<string, AudioClip> sfxDictionary = new Dictionary<string, AudioClip>();
    // 음악, 효과음 볼륨
    [Header("Volume")]
    [SerializeField] private float masterVolume = 1.0f;
    [SerializeField] private float bgmVolume = 1.0f;
    [SerializeField] private float sfxVolume = 1.0f;
    public float MasterVolume => masterVolume;
    public float BgmVolume => bgmVolume;
    public float SfxVolume => sfxVolume;
    #endregion
    
    #region Unity LifeCycle
    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            //볼륨 기본값 설정 (추후 저장된 볼륨사용기능?)
            if (bgmSource != null) bgmSource.volume = bgmVolume;
            if (sfxSource != null) sfxSource.volume = sfxVolume;
            //딕셔너리 초기화
            InitializeAudioDictionary();
        }
        else
        {
            Destroy(gameObject);
        }
    }
    #endregion
    
    #region Public methods
    /// <summary>
    /// 배열 딕셔너리 변환 (InitializeAudioKictionary() : void)
    /// </summary>
    private void InitializeAudioDictionary()
    {
        // BGM 클립 초기화
        foreach (var clip in bgmClips)
        {
            if (clip != null && !bgmDictionary.ContainsKey(clip.name))
            {
                bgmDictionary.Add(clip.name, clip);
            }
        }
        
        // SFX 클립 초기화
        foreach (var clip in sfxClips)
        {
            if (clip != null && !sfxDictionary.ContainsKey(clip.name))
            {
                sfxDictionary.Add(clip.name, clip);
            }
        }
    }
    /// <summary>
    /// 배경 음악 재생 (PlayBGM(clipName: string) : void)
    /// </summary>
    public void PlayBGM(string clipName)
    {
        AudioClip targetClip;
        
        // 💡 Dictionary를 사용하여 반복문 없이 클립 검색
        if (bgmDictionary.TryGetValue(clipName, out targetClip))
        {
            if (bgmSource.isPlaying)
            {
                bgmSource.Stop();
            }

            bgmSource.clip = targetClip;
            bgmSource.Play();
        }
        else
        {
            Debug.LogWarning($"BGM Clip not found: {clipName}");
        }
    }

    /// <summary>
    /// 효과음 재생 (PlaySFX(clipName: string) : void)
    /// </summary>
    public void PlaySfx(string clipName)
    {
        AudioClip targetClip;
        
        // 💡 Dictionary를 사용하여 반복문 없이 클립 검색
        if (sfxDictionary.TryGetValue(clipName, out targetClip))
        {
            sfxSource.PlayOneShot(targetClip);
        }
        else
        {
            Debug.LogWarning($"SFX Clip not found: {clipName}");
        }
    }
    
    /// <summary>
    /// 배경 음악 종료 (StopBGM() : void) 
    /// </summary>
  
    public void StopBGM()
    {
        if (bgmSource.isPlaying)
        {
            bgmSource.Stop();
        }
    }
    
    /// <summary>
    /// 마스터 볼륨 설정 (SetMasterVolume() : float)
    /// </summary>
    public void SetMasterVolume(float level)
    {
        masterVolume = Mathf.Clamp01(level);
        bgmSource.volume = bgmVolume*masterVolume;
        sfxSource.volume = sfxVolume*masterVolume;
    }
    
    /// <summary>
    /// 볼륨 설정 (SetBgmVolume() : float), (SetSfxVolume() : float)
    /// </summary>
    public void SetBgmVolume(float level)
    {
        bgmVolume = Mathf.Clamp01(level);
        bgmSource.volume = bgmVolume*masterVolume; 
    }

    public void SetSfxVolume(float level)
    {
        sfxVolume = Mathf.Clamp01(level);
        sfxSource.volume = sfxVolume*masterVolume; 
    }
    #endregion
}
