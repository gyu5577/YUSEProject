using UnityEngine;

/// <summary>
/// 적 몬스터가 발사하는 투사체
/// 플레이어를 향해 직선으로 날아가며 충돌 시 데미지를 줍니다.
/// </summary>
public class Projectile : MonoBehaviour
{
    #region Serialized Fields
    
    [Header("Projectile Settings")]
    [SerializeField] private float speed = 5f;
    [SerializeField] private float damage = 5f;
    [SerializeField] private float lifetime = 3f; // 투사체 생존 시간
    
    #endregion
    
    #region Private Fields
    
    private Transform _target;
    private Vector2 _moveDirection;
    
    #endregion
    
    #region Unity LifeCycle
    
    /// <summary>
    /// 일정 시간 후에 스스로 파괴
    /// </summary>
    private void Start()
    {
        Destroy(gameObject, lifetime);
    }

    /// <summary>
    /// 이동 실시간 조정
    /// </summary>
    private void Update()
    {
        transform.Translate(_moveDirection * (speed * Time.deltaTime), Space.World);
    }
    
    #endregion
    
    #region Public Methods
    
    /// <summary>
    /// 투사체를 발사할 때 호출, 목표 방향 설정
    /// </summary>
    /// <param name="target">추적할 타겟 (일반적으로 플레이어)</param>
    public void Init(Transform target)
    {
        _target = target;
        
        if (_target != null)
        {
            Vector2 targetPos = _target.position;
            Vector2 currentPos = transform.position;
            
            _moveDirection = (targetPos - currentPos).normalized;
            
            // 투사체 스프라이트 방향
            float angle = Mathf.Atan2(_moveDirection.y, _moveDirection.x) * Mathf.Rad2Deg;
            transform.rotation = Quaternion.Euler(0f, 0f, angle + 270);
        }
        else
        {
            Destroy(gameObject);
        }
    }
    
    #endregion
    
    #region Private Methods
    
    /// <summary>
    /// 플레이어와 충돌시 데미지 후 파괴
    /// </summary>
    /// <param name="other">충돌한 콜라이더</param>
    private void OnTriggerEnter2D(Collider2D other)
    {
        PlayerManager player = other.GetComponent<PlayerManager>(); 

        if (player != null)
        {
            player.TakeDamage(damage); 
            Destroy(gameObject); 
        }
    }
    
    #endregion
}