using System.Collections.Generic;
using UnityEngine;

public class SpawnManager : MonoBehaviour
{
    #region Serialized Fields
    [Header("References")]
    [SerializeField] private Transform playerTransform; 
    // [삭제] poolContainer는 이제 PoolManager가 관리하므로 필요 없음

    [Header("Wave Settings")] 
    [SerializeField] private List<WaveData> waves;
    [SerializeField] private BossMonster bossPrefab; 

    [Header("Spawn Settings")] 
    [SerializeField] private float spawnInterval = 5.0f; 
    [SerializeField] private float spawnRadius = 10.0f;
    [SerializeField] private float bossSpawnCycle = 300f;
    [SerializeField] private int initialPerTypeSize = 10; 
    #endregion

    #region Private Fields
    private float _spawnTimer;
    private WaveData _currentWave;
    private int _bossLevel = 1;
    private bool _isBossActive;

    // 현재 필드 몬스터 리스트 (마릿수 제한용)
    private List<Monster> _activeMonsters = new List<Monster>();
    #endregion

    #region Unity LifeCycle
    private void Start()
    {
        if (waves == null || waves.Count == 0)
        {
            Debug.LogWarning("SpawnManager: WaveData가 비어있습니다!");
        }
        
        PreloadAllWaveMonsters();
    }

    private void Update()
    {
        if (GameManager.Instance.CurrentState != GameState.Playing) return;
        if (_isBossActive) return;

        float currentGameTime = GameManager.Instance.GameTime;

        // 1. 보스 스폰 체크
        if (currentGameTime >= bossSpawnCycle * _bossLevel)
        {
            SpawnBoss();
            return;
        }

        // 2. 웨이브 데이터 갱신
        UpdateWaveData(currentGameTime);

        // 3. 몬스터 스폰
        if (_currentWave != null)
        {
            ProcessWaveSpawning();
        }
    }
    #endregion

    #region Wave & Boss Logic
    // ... (UpdateWaveData, ProcessWaveSpawning, GetRandomPrefab, SpawnBoss 등은 로직 변경 없음) ...
    
    private void UpdateWaveData(float time)
    {
        if (_currentWave != null && time >= _currentWave.startTime && time < _currentWave.endTime) return;
        foreach (var wave in waves)
        {
            if (time >= wave.startTime && time < wave.endTime)
            {
                _currentWave = wave;
                return;
            }
        }
    }

    private void ProcessWaveSpawning()
    {
        _spawnTimer += Time.deltaTime;
        if (_spawnTimer >= _currentWave.spawnInterval)
        {
            if (_activeMonsters.Count < _currentWave.maxFieldMonsterCount)
            {
                Monster prefabToSpawn = GetRandomPrefab();
                if (prefabToSpawn != null)
                {
                    Vector2 pos = CalculateSpawnPosition();
                    SpawnMonster(prefabToSpawn, pos);
                }
            }
            _spawnTimer = 0f;
        }
    }

    private Monster GetRandomPrefab()
    {
        if (_currentWave.spawnablePrefabs.Count == 0) return null;
        int idx = Random.Range(0, _currentWave.spawnablePrefabs.Count);
        return _currentWave.spawnablePrefabs[idx];
    }

    private void SpawnBoss()
    {
        _isBossActive = true;
        GameManager.Instance.IsTimerStopped = true;

        Vector2 spawnPos = CalculateSpawnPosition();
        BossMonster boss = Instantiate(bossPrefab, spawnPos, Quaternion.identity);
        boss.Init(playerTransform, OnBossDied);

        Debug.Log($"BOSS APPEARED! Level: {_bossLevel}");
    }

    private void OnBossDied(Monster boss)
    {
        Debug.Log("BOSS DEFEATED!");
        _isBossActive = false;
        _bossLevel++; 
        GameManager.Instance.IsTimerStopped = false;
        Destroy(boss.gameObject);
    }
    #endregion

    #region Object Pooling Logic (Delegated to PoolManager)
    
    public void SpawnMonster(Monster prefab, Vector2 position)
    {
        // 1. PoolManager에게 GameObject 요청
        GameObject obj = PoolManager.Instance.Get(prefab.gameObject, position, Quaternion.identity);
        
        // 2. Monster 컴포넌트 가져오기
        Monster monster = obj.GetComponent<Monster>();

        // 3. 초기화 (콜백에서 prefab 정보를 캡처해서 넘겨줌)
        monster.Init(playerTransform, (m) => ReturnToPool(m, prefab));

        // 4. 활성 리스트에 추가
        _activeMonsters.Add(monster);
    }

    public void ReturnToPool(Monster monster, Monster prefab)
    {
        // 1. 활성 리스트에서 제거
        _activeMonsters.Remove(monster);
        
        // 2. PoolManager에게 반환 요청
        PoolManager.Instance.ReturnToPool(monster.gameObject, prefab.gameObject);
    }
    
    private void PreloadAllWaveMonsters()
    {
        // 중복 방지 (Instance ID 사용)
        HashSet<int> processedIDs = new HashSet<int>();

        foreach (var wave in waves)
        {
            // 1. waveData 객체 자체가 null인지 확인합니다.
            if (wave == null)
            {
                Debug.LogWarning("SpawnManager: Waves 리스트에 할당되지 않은 빈 슬롯(Null)이 있습니다. 건너뜁니다.");
                continue;
            }
                
            // 2. spawnablePrefabs 리스트 자체가 null인지 확인합니다.
            if (wave.spawnablePrefabs == null)
            {
                Debug.LogWarning($"SpawnManager: '{wave.name}' WaveData의 spawnablePrefabs 리스트가 Null입니다. 인스펙터에서 초기화하거나 몬스터를 할당해주세요. 건너뜁니다.");
                continue;
            }
            
            foreach (var prefab in wave.spawnablePrefabs)
            {
                if (prefab == null)
                {
                    continue;
                }
                
                int id = prefab.gameObject.GetInstanceID();
                if (processedIDs.Contains(id)) continue; 

                // PoolManager에게 미리 생성 요청
                PoolManager.Instance.Preload(prefab.gameObject, initialPerTypeSize);
                
                processedIDs.Add(id);
            }
        }
    }
    
    public Vector2 CalculateSpawnPosition()
    {
        Vector2 randomDir = Random.insideUnitCircle.normalized;
        Vector2 origin = playerTransform != null ? (Vector2)playerTransform.position : Vector2.zero;
        return origin + (randomDir * spawnRadius);
    }
    #endregion
}