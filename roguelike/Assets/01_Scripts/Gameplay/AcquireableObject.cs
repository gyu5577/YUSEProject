using UnityEngine;

public abstract class AcquireableObject : MonoBehaviour
{
    
    public Vector2 Position =>transform.position;
    private PlayerManager currentTarget = null;

    #region life Cycle

    private void Update()
    {
        if (currentTarget != null)
        {
            MoveToPlayer(currentTarget);
        }
    }


    private void OnTriggerEnter2D(Collider2D collision)
    {

      
        PlayerManager player = collision.GetComponent<PlayerManager>();

        if(player != null)
        {
            currentTarget = player;
            Debug.Log("감지");
        }
    }


    #endregion



    #region public method
    [Header("Settings")]
    public float moveSpeed = 3.0f; //날라가는 속도
    
    public void MoveToPlayer(PlayerManager target)
    {
        if (target == null) return;


        transform.position = Vector2.MoveTowards(transform.position, target.Player_Position, moveSpeed*Time.deltaTime); //플레이어쪽으로

        // 거리가 가까우면 획득
        if(Vector2.Distance(transform.position, target.Player_Position) <0.5f)
        {
            OnAcquire(target);
        }
    }

    public abstract void OnAcquire(PlayerManager player);

    #endregion
}
