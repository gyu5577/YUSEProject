/*
 * [TestEquip.cs]
 * [패키지 2] 플레이어 로직 - 초기 장비
 * 플레이어 주변을 회전하며 충돌한 적에게 데미지를 입히는 장비입니다.
 */

using UnityEngine;

public class TestEquip : Equipment
{
    #region Serialized Fields
    [Header("TestEquip Specific")]
    [SerializeField] private float rotationSpeed = 100f; // 회전 속도
    [SerializeField] private float distanceFromPlayer = 2f; // 플레이어로부터의 거리
    [SerializeField] private Transform visualSprite; // 실제 회전할 스프라이트 객체
    #endregion

    #region Private Fields
    private float _currentAngle = 0f;
    #endregion

    #region Unity LifeCycle
    
    private void Start()
    {
        if (visualSprite != null)
            visualSprite.localPosition = new Vector3(distanceFromPlayer, 0, 0);
    }

    public override void Initialize(PlayerManager player)
    {
        base.Initialize(player);
        
        // 초기 위치 설정
        if (visualSprite != null)
        {
            visualSprite.localPosition = new Vector3(distanceFromPlayer, 0, 0);
        }
    }

    protected override void Update()
    {
        base.Update(); // 부모의 쿨다운 로직 등 실행
        
        RotateAroundPlayer();

        if (visualSprite != null)
            visualSprite.localRotation = Quaternion.identity;
    }
//     private void LateUpdate()
// {
//     if (visualSprite != null)
//         visualSprite.rotation = Quaternion.identity;
// }

    // 충돌 감지 (반드시 Collider2D가 있어야 함, IsTrigger 체크 권장)
    private void OnTriggerEnter2D(Collider2D collision)
    {
        // (패키지 3) Monster 태그를 가진 오브젝트인지 확인
        // (Monster 스크립트는 IDamageable 인터페이스를 구현하는 것이 좋음)
        if (collision.CompareTag("Enemy"))
        {
            // Monster 스크립트 가져오기
            // (컨벤션 1-4) GetComponent는 가급적 캐싱하면 좋지만, 
            // 동적으로 충돌하는 대상은 그때그때 가져와야 합니다.
            Monster monster = collision.GetComponentInParent<Monster>();
            
            if (monster != null)
            {
                // 데미지 입히기
                monster.TakeDamage(baseDamage);
                Debug.Log($"TestEquip hit {monster.name} for {baseDamage} damage.");
                
                // (선택) 타격 이펙트나 사운드 재생
                // AudioManager.Instance.PlaySFX("HitSound");
            }
        }
    }
    #endregion

    #region Private Methods
    private void RotateAroundPlayer()
    {
        // 플레이어 중심으로 회전 로직
        // 1. 단순히 부모(플레이어)를 따라다니며 Z축 회전만 시키는 방법
        // 2. 삼각함수로 위치를 직접 계산하는 방법
        
        // 여기서는 (1)번 방식: 이 스크립트가 붙은 오브젝트 자체를 회전시킵니다.
        // visualSprite는 (distanceFromPlayer, 0, 0)에 있으므로
        // 중심축이 회전하면 위성처럼 돕니다.
        
        float rotationStep = rotationSpeed * Time.deltaTime;
        transform.Rotate(Vector3.back * rotationStep);
        
        //transform.Rotate(Vector3.back * rotationSpeed * Time.deltaTime);
    }
    #endregion

    #region Equipment Overrides
    protected override void PerformAttack()
    {
        // 이 장비는 지속형(Passive)이라 특정 시점에 발사하지 않으므로 비워둡니다.
        // (만약 쿨다운마다 크기가 커졌다 작아지는 등의 로직을 넣고 싶다면 여기서 구현)
    }

    public override void LevelUp()
    {
        base.LevelUp();
        
        // 레벨 업 시 스탯 강화 (예: 회전 속도 증가, 데미지 증가)
        rotationSpeed += 20f;
        baseDamage += 5f;
        
        // (선택) 크기 증가
        if (visualSprite != null)
        {
            visualSprite.localScale *= 1.1f;
        }
    }
    #endregion
}