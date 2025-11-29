using System.Collections.Generic;
using UnityEngine;

public class PoolManager : MonoBehaviour
{
    public static PoolManager Instance;

    // 프리팹의 InstanceID를 키로 사용하여 풀 관리
    private Dictionary<int, Queue<GameObject>> _pools = new Dictionary<int, Queue<GameObject>>();
    
    // Hierarchy 정리를 위한 부모 트랜스폼들
    private Dictionary<int, Transform> _containers = new Dictionary<int, Transform>();

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            // 필요하다면 DontDestroyOnLoad(gameObject); 추가
        }
        else
        {
            Destroy(gameObject);
        }
    }
    
    // 풀에서 오브젝트를 가져옵니다. 없으면 새로 생성
    public GameObject Get(GameObject prefab, Vector3 position, Quaternion rotation)
    {
        int key = prefab.GetInstanceID();

        if (!_pools.ContainsKey(key))
        {
            InitPool(prefab);
        }

        GameObject obj;

        if (_pools[key].Count > 0)
        {
            obj = _pools[key].Dequeue();
        }
        else
        {
            obj = Instantiate(prefab, _containers[key]);
        }

        obj.transform.position = position;
        obj.transform.rotation = rotation;
        obj.SetActive(true);

        return obj;
    }
    
    // 사용이 끝난 오브젝트를 풀로 반환합니다.
    public void ReturnToPool(GameObject obj, GameObject prefab)
    {
        int key = prefab.GetInstanceID();

        if (!_pools.ContainsKey(key))
        {
            InitPool(prefab);
        }

        obj.SetActive(false);
        _pools[key].Enqueue(obj);
    }
    
    // 특정 프리팹을 미리 생성해 두기
    public void Preload(GameObject prefab, int count)
    {
        int key = prefab.GetInstanceID();
        if (!_pools.ContainsKey(key))
        {
            InitPool(prefab);
        }

        for (int i = 0; i < count; i++)
        {
            GameObject obj = Instantiate(prefab, _containers[key]);
            obj.SetActive(false);
            _pools[key].Enqueue(obj);
        }
    }

    private void InitPool(GameObject prefab)
    {
        int key = prefab.GetInstanceID();
        _pools.Add(key, new Queue<GameObject>());

        GameObject container = new GameObject($"Pool_{prefab.name}");
        container.transform.SetParent(transform);
        _containers.Add(key, container.transform);
    }
}