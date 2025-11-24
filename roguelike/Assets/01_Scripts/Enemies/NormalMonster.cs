using UnityEngine;

/// 일반 몬스터 클래스
/// 플레이어 방향으로 단순히 이동
public class NormalMonster : Monster
{
    #region Public Methods
    public override void Move(Vector2 targetPosition)
    {
        // 현재 위치에서 타겟 위치로 이동
        transform.position = Vector2.MoveTowards(
            transform.position, 
            targetPosition, 
            moveSpeed * Time.deltaTime
        );

        // 스프라이트 좌우 반전 로직
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