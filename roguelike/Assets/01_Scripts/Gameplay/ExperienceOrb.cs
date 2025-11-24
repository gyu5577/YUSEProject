using UnityEngine;

public class ExperienceOrb : AcquireableObject
{
    [SerializeField] private int expAmount = 10;


    public override void OnAcquire(PlayerManager player)
    {
        player.GainExp(expAmount);
        player.GainGold(expAmount); //골드는 따로 오브젝트 안만들꺼면 킬을 했을때 오르는걸로 처리하는게 좋을듯함
        //일단 파괴 나중에는 비활성화로 처리
        Destroy(gameObject);
    }
}
