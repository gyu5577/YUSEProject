using System.Collections;
using UnityEngine;

public class FoxBoss : BossMonster
{
    [Header("Pattern Settings")]
    [SerializeField] private float patternInterval = 5f; // 패턴 사이 간격
    [SerializeField] private float patternWarningTime = 1.5f; // 패턴 전조 시간

    [Header("Pattern 1: 3-Way Shot")]
    [SerializeField] private GameObject projectilePrefab;
    [SerializeField] private float projectileSpeed = 7f; // 투사체 속도
    [SerializeField] private float projectileSize = 1.0f; // 투사체 크기
    [SerializeField] private float projectileDamage = 5f; // 투사체 데미지

    [Header("Pattern 2: Dash")]
    [SerializeField] private float dashDistance = 8f; // 대시 거리
    [SerializeField] private float dashDuration = 0.5f; // 대시 지속 시간
    [SerializeField] private float dashDamage = 10f; // 대시 데미지

    [Header("Pattern 3: AOE Damage")]
    [SerializeField] private float aoeDamage = 10f; // 범위공격 데미지
    [SerializeField] private float aoeRange = 5f; // 범위공격 범위

    private bool _isPatternActive = false;
    private float _patternTimer = 0f;
    private LineRenderer _lineRenderer;
    private SpriteRenderer _rangeIndicator; // 원형 범위 표시용

    protected override void Awake()
    {
        base.Awake();
        
        // 패턴 범위 표시용 ( 추후 스프라이트로 변경 가능)
        
        // LineRenderer 설정 (대시 경로 표시용)
        _lineRenderer = gameObject.AddComponent<LineRenderer>();
        _lineRenderer.startWidth = 0.1f;
        _lineRenderer.endWidth = 0.1f;
        _lineRenderer.material = new Material(Shader.Find("Sprites/Default"));
        _lineRenderer.startColor = new Color(1, 0, 0, 0.5f);
        _lineRenderer.endColor = new Color(1, 0, 0, 0.5f);
        _lineRenderer.enabled = false;

        // 범위 표시용 SpriteRenderer 
        GameObject indicatorObj = new GameObject("RangeIndicator");
        indicatorObj.transform.SetParent(transform);
        indicatorObj.transform.localPosition = Vector3.zero;
        _rangeIndicator = indicatorObj.AddComponent<SpriteRenderer>();
        Destroy(indicatorObj); 
    }

    protected override void Start()
    {
        base.Start();
        _patternTimer = patternInterval;
    }

    protected override void Update()
    {
        if (_isPatternActive) return;

        // 평소에는 플레이어 추적
        base.Update();

        _patternTimer -= Time.deltaTime;
        if (_patternTimer <= 0)
        {
            StartCoroutine(ExecuteRandomPattern());
        }
    }

    private IEnumerator ExecuteRandomPattern()
    {
        _isPatternActive = true;
        
        // 패턴 랜덤 선택 
        int patternIndex = Random.Range(0, 3);
        
        // 패턴 시작 전 멈춤
        // Move()가 호출되지 않으므로 제자리에 멈춤 (Rigidbody velocity가 있다면 0으로 초기화 필요)
        Rigidbody2D rb = GetComponent<Rigidbody2D>();
        if (rb != null) rb.linearVelocity = Vector2.zero;

        switch (patternIndex)
        {
            case 0:
                yield return StartCoroutine(Pattern_ThreeWayShot());
                break;
            case 1:
                yield return StartCoroutine(Pattern_Dash());
                break;
            case 2:
                yield return StartCoroutine(Pattern_AOEDamage());
                break;
        }

        _isPatternActive = false;
        _patternTimer = patternInterval;
    }

    // 패턴 1: 3갈래 투사체 발사
    private IEnumerator Pattern_ThreeWayShot()
    {
        Debug.Log("FoxBoss: 3-Way Shot Pattern Start");

        // 전조: 플레이어 방향으로 3갈래 선 표시
        Vector2 savedDirection = Vector2.right; 
        if (_target != null)
        {
            savedDirection = (_target.position - transform.position).normalized;
            ShowTelegraphLines(savedDirection, 3, 30f); 
        }

        yield return new WaitForSeconds(patternWarningTime);

        HideTelegraph();

        // 저장된 방향으로 발사 
        if (projectilePrefab != null)
        {
            FireProjectile(savedDirection); 
            FireProjectile(Quaternion.Euler(0, 0, 30) * savedDirection); 
            FireProjectile(Quaternion.Euler(0, 0, -30) * savedDirection); 
        }
    }

    private void FireProjectile(Vector2 direction)
    {
        GameObject projObj = Instantiate(projectilePrefab, transform.position, Quaternion.identity);
        Projectile2 proj = projObj.GetComponent<Projectile2>();
        if (proj != null)
        {
            proj.SetSize(projectileSize);
            proj.SetSpeed(projectileSpeed);
            proj.SetDamage(projectileDamage);
            proj.Init(direction);
        }
    }

    // 패턴 2: 돌진
    private IEnumerator Pattern_Dash()
    {
        Debug.Log("FoxBoss: Dash Pattern Start");

        // 플레이어 방향으로 일정 거리만큼 대시
        Vector2 dashDirection = Vector2.right; 
        if (_target != null)
        {
            dashDirection = (_target.position - transform.position).normalized;
        }
        
        Vector2 startPos = transform.position;
        Vector2 dashTargetPos = startPos + dashDirection * dashDistance;

        // 전조: 돌진 경로 표시
        _lineRenderer.enabled = true;
        _lineRenderer.positionCount = 2;
        _lineRenderer.SetPosition(0, startPos);
        _lineRenderer.SetPosition(1, dashTargetPos);

        yield return new WaitForSeconds(patternWarningTime);

        _lineRenderer.enabled = false;

        // 돌진 실행
        float elapsedTime = 0f;
        
        while (elapsedTime < dashDuration)
        {
            transform.position = Vector2.Lerp(startPos, dashTargetPos, elapsedTime / dashDuration);
            elapsedTime += Time.deltaTime;
            yield return null;
        }
        transform.position = dashTargetPos;
        
        // 대시 중 플레이어와 충돌했는지 체크 (범위 내에 있으면 데미지)
        if (_target != null)
        {
            float distance = Vector2.Distance(transform.position, _target.position);
            if (distance <= 1.5f) // 대시 충돌 범위
            {
                PlayerManager player = _target.GetComponent<PlayerManager>();
                if (player != null)
                {
                    player.TakeDamage(dashDamage);
                    Debug.Log($"FoxBoss: Player hit by Dash for {dashDamage} damage!");
                }
            }
        }
    }



    // 여러 갈래 선 그리기
    private void ShowTelegraphLines(Vector2 centerDir, int count, float angleSpacing)
    {
        _lineRenderer.enabled = true;
        _lineRenderer.positionCount = 7; 

        float lineLength = 10f;
        Vector3 bossPos = transform.position;

        // 중앙 선
        _lineRenderer.SetPosition(0, bossPos);
        _lineRenderer.SetPosition(1, bossPos + (Vector3)centerDir * lineLength);
        
        // 보스 위치로 돌아옴
        _lineRenderer.SetPosition(2, bossPos);

        // 왼쪽 선 (+30도)
        Vector2 leftDir = Quaternion.Euler(0, 0, angleSpacing) * centerDir;
        _lineRenderer.SetPosition(3, bossPos + (Vector3)leftDir * lineLength);
        
        // 보스 위치로 돌아옴
        _lineRenderer.SetPosition(4, bossPos);

        // 오른쪽 선 (-30도)
        Vector2 rightDir = Quaternion.Euler(0, 0, -angleSpacing) * centerDir;
        _lineRenderer.SetPosition(5, bossPos + (Vector3)rightDir * lineLength);
        
        // 다시 보스 위치로 
        _lineRenderer.SetPosition(6, bossPos);
    }

    private void HideTelegraph()
    {
        _lineRenderer.enabled = false;
    }

    // 패턴 3: 범위 피해
    private IEnumerator Pattern_AOEDamage()
    {
        Debug.Log("FoxBoss: AOE Damage Pattern Start");

        // 전조: 주변 원형 범위 표시
        DrawCircle(aoeRange);

        yield return new WaitForSeconds(patternWarningTime);

        _lineRenderer.enabled = false;

        // 범위 내 플레이어 체크 및 피해
        if (_target != null)
        {
            float distance = Vector2.Distance(transform.position, _target.position);
            if (distance <= aoeRange)
            {
                PlayerManager player = _target.GetComponent<PlayerManager>();
                if (player != null)
                {
                    player.TakeDamage(aoeDamage);
                    Debug.Log($"FoxBoss: Player hit by AOE for {aoeDamage} damage!");
                }
            }
        }
    }

    private void DrawCircle(float radius)
    {
        _lineRenderer.enabled = true;
        int segments = 50;
        _lineRenderer.positionCount = segments + 1;
        float angle = 0f;

        for (int i = 0; i < segments + 1; i++)
        {
            float x = Mathf.Sin(Mathf.Deg2Rad * angle) * radius;
            float y = Mathf.Cos(Mathf.Deg2Rad * angle) * radius;

            _lineRenderer.SetPosition(i, transform.position + new Vector3(x, y, 0));

            angle += (360f / segments);
        }
    }
}
