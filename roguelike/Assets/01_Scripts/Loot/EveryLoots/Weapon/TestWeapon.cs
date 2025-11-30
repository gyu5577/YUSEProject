/*
 * [TestWeapon.cs]
 * [패키지 2] 플레이어 로직 - 무기 시스템
 * 플레이어가 바라보는 방향으로 투사체를 발사하는 테스트용 무기입니다.
 */

using UnityEngine;

/// <summary>
/// 테스트용 무기 클래스
/// </summary>
public class TestWeapon : Weapon
{
    #region Abstract Methods Implementation
    /// <summary>
    /// 무기 공격 로직을 수행합니다.
    /// 투사체를 생성하고 플레이어가 바라보는 방향으로 발사합니다.
    /// </summary>
    protected override void PerformAttack()
    {
        // 1. 투사체 생성
        if (WeaponData.Prefab != null)
        {
            // 플레이어 위치에서 생성
            GameObject projectileObj = Instantiate(WeaponData.Prefab, transform.position, Quaternion.identity);
            
            // 2. 투사체 초기화
            Projectile projectile = projectileObj.GetComponent<Projectile>();
            if (projectile != null)
            {
                // 플레이어가 바라보는 방향으로 발사
                Vector2 direction = _player.FacingDirection;
                
                // 만약 플레이어가 정지해 있어서 (0,0)이라면 기본값(오른쪽) 사용
                if (direction == Vector2.zero)
                {
                    direction = Vector2.right;
                }

                projectile.Initialize(
                    WeaponData.ProjectileSpeed,
                    WeaponData.BaseDamage,
                    direction,
                    WeaponData.Penetration
                );
            }
        }
    }
    #endregion
}