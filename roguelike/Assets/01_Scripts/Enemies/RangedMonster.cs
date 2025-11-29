using UnityEngine;

/// <summary>
/// 원거리 공격을 하는 몬스터
/// 일정 거리를 유지하며 투사체를 발사합니다.
/// </summary>
public class RangedMonster : Monster
{
    #region Serialized Fields
    
    [Header("Ranged Settings")]
    [SerializeField] private float attackRange = 8.0f; // 공격을 시작할 거리
    [SerializeField] private float fireRate = 3.0f; // 공격 주기
    [SerializeField] private GameObject projectilePrefab; // 투사체 프리팹
    
    #endregion
    
    #region Private Fields
    
    private float _attackTimer;
    
    #endregion
    
    #region Unity LifeCycle
    
    protected override void Update()
    {
        if (_target != null)
        {
            // 이동 처리: 목표 거리보다 멀면 접근, 가까우면 멈춤
            Move(_target.position);
            
            // 공격 처리: 공격 주기마다 투사체 발사
            HandleAttack();
        }
    }
    
    #endregion

    #region Public Methods
    
    /// <summary>
    /// 타겟 위치로 이동 (공격 범위 밖이면 접근)
    /// </summary>
    /// <param name="targetPosition">목표 위치</param>
    public override void Move(Vector2 targetPosition)
    {
        Vector2 currentPosition = transform.position;
        float distance = Vector2.Distance(currentPosition, targetPosition);

        // 플레이어와의 거리가 공격 범위보다 크면 접근
        if (distance > attackRange)
        {
            transform.position = Vector2.MoveTowards(
                currentPosition, 
                targetPosition, 
                moveSpeed * Time.deltaTime
            );
        }

        // 스프라이트 좌우 반전
        UpdateSpriteDirection(targetPosition);
    }
    
    #endregion
    
    #region Private Methods
    
    /// <summary>
    /// 공격 타이머를 관리하고 사격 가능 시 투사체 발사
    /// </summary>
    private void HandleAttack()
    {
        _attackTimer += Time.deltaTime;
        
        if (_target != null && 
            Vector2.Distance(transform.position, _target.position) <= attackRange && 
            _attackTimer >= fireRate)
        {
            ShootProjectile();
            _attackTimer = 0f;
        }
    }

    /// <summary>
    /// 투사체를 생성하고 초기화
    /// </summary>
    private void ShootProjectile()
    {
        if (projectilePrefab == null) 
        {
            return;
        }

        // 투사체 생성
        GameObject projObj = Instantiate(projectilePrefab, transform.position, Quaternion.identity);
        Projectile2 projectile = projObj.GetComponent<Projectile2>();

        if (projectile != null)
        {
            // 투사체 초기화
            projectile.Init(_target); 
        }
    }
    
    /// <summary>
    /// 타겟 위치에 따라 스프라이트 방향 변경
    /// </summary>
    /// <param name="targetPosition">타겟 위치</param>
    private void UpdateSpriteDirection(Vector2 targetPosition)
    {
        if (targetPosition.x < transform.position.x)
        {
            transform.localScale = new Vector3(-1, 1, 1);
        }
        else
        {
            transform.localScale = new Vector3(1, 1, 1);
        }
    }
    
    #endregion
}