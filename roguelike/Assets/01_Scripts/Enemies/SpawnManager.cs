using UnityEngine;

/// 몬스터 스폰을 담당하는 매니저
/// 일정 시간마다 화면 밖에서 몬스터를 생성
public class SpawnManager : MonoBehaviour
{
    #region Serialized Fields
    [Header("References")]
    [SerializeField] private NormalMonster normalMonsterPrefab; // 생성할 프리팹
    [SerializeField] private Transform playerTransform; // 플레이어 위치 참조

    [Header("Settings")]
    [SerializeField] private float spawnInterval = 5.0f; // 스폰 주기
    [SerializeField] private float spawnRadius = 10.0f; // 플레이어 기준 스폰 거리
    #endregion

    #region Private Fields
    private float _timer;
    #endregion

    #region Unity LifeCycle
    private void Update()
    {
        UpdateSpawning(Time.time);
    }
    #endregion

    #region Public Methods
    /// 시간에 맞는 몬스터 생성 하기
    public void UpdateSpawning(float gameTime)
    {
        _timer += Time.deltaTime;

        if (_timer >= spawnInterval)
        {
            Vector2 spawnPos = CalculateSpawnPosition();
            SpawnMonster(normalMonsterPrefab, spawnPos);
            _timer = 0f;
        }
    }
    
    // 실제 몬스터 생성 및 초기화
    public void SpawnMonster(Monster prefab, Vector2 position)
    {
        Monster monster = Instantiate(prefab, position, Quaternion.identity);
        // 생성된 몬스터에게 플레이어 위치 정보 주입 (FindObjectOfType 방지)
        monster.Init(playerTransform); 
    }
    
    /// 몬스터 생성 위치 계산 ,화면 밖 랜덤으로
    public Vector2 CalculateSpawnPosition()
    {
        // 플레이어 위치를 기준으로 반지름(spawnRadius) 만큼 떨어진 랜덤 위치 계산
        // 단순히 Random.insideUnitCircle.normalized를 사용하면 원의 둘레 위치를 얻음
        Vector2 randomDir = Random.insideUnitCircle.normalized;
        
        // 플레이어가 없다면(0,0) 기준, 있다면 플레이어 기준
        Vector2 origin = playerTransform != null ? (Vector2)playerTransform.position : Vector2.zero;

        return origin + (randomDir * spawnRadius);
    }
    #endregion
}