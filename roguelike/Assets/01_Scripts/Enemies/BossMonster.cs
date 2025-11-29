using UnityEngine;
//spawnManager 동작 테스트를 위해 만들어 두었습니다!
public class BossMonster : Monster
{
    protected override void Start()
    {
        base.Start();

        // 테스트로 알아보기 쉽게 크기를 키우고 빨간색으로 변경
        transform.localScale = Vector3.one * 2.0f; // 2배 커짐
        
        SpriteRenderer sr = GetComponent<SpriteRenderer>();
        if (sr != null)
        {
            sr.color = Color.red; // 빨간색
        }
    }
    
    public override void Move(Vector2 targetPosition)
    {
        transform.position = Vector2.MoveTowards(
            transform.position, 
            targetPosition, 
            moveSpeed * Time.deltaTime
        );

        // 플레이어 바라보기 (좌우 반전)
        if (targetPosition.x < transform.position.x)
        {
            transform.localScale = new Vector3(-2, 2, 1); // 왼쪽 (크기 2배 유지)
        }
        else
        {
            transform.localScale = new Vector3(2, 2, 1);  // 오른쪽
        }
    }

    // 선택 구현 (오버라이드)
    public override void Die()
    {
        Debug.Log("💀 BOSS: 응기잇 (사망)");
        
        // base.Die()를 호출해야 SpawnManager가 넘겨준 콜백(OnBossDied)이 실행됨
        // 이 콜백이 실행되어야 타이머가 다시 돌아가고 일반 몬스터가 스폰됨
        base.Die(); 
    }
}
