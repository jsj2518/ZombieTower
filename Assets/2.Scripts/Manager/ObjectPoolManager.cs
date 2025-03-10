using System.Collections.Generic;
using UnityEngine;


public class ObjectPoolManager : MonoBehaviour
{
    private class Pool
    {
        public Queue<GameObject> queue = new Queue<GameObject>();
        public Transform parent = null;
        public string prefabPath = string.Empty;
    }

    private Dictionary<string, Pool> pools = new Dictionary<string, Pool>();

    /// <summary>
    /// 특정 프리팹 경로에서 풀을 생성합니다.
    /// </summary>
    public void CreatePool(string prefabPath, int count)
    {
        GameObject prefab = Resources.Load<GameObject>(prefabPath);
        if (prefab == null)
        {
            Debug.LogError($"프리팹을 찾을 수 없습니다: {prefabPath}");
            return;
        }

        string poolKey = System.IO.Path.GetFileNameWithoutExtension(prefabPath); // 경로에서 파일명만 추출
        if (!pools.ContainsKey(poolKey))
        {
            pools[poolKey] = new Pool();

            // 부모 오브젝트 생성
            GameObject poolContainer = new GameObject(poolKey);
            poolContainer.transform.SetParent(transform);
            pools[poolKey].parent = poolContainer.transform;

            pools[poolKey].prefabPath = prefabPath;
        }
        else
        {
            Debug.LogError($"같은 이름을 가진 풀이 존재합니다: {poolKey}");
            return;
        }

        for (int i = 0; i < count; i++)
        {
            GameObject obj = Instantiate(prefab);
            obj.name = prefab.name;
            obj.transform.SetParent(pools[poolKey].parent);
            obj.SetActive(false);
            pools[poolKey].queue.Enqueue(obj);
        }
    }

    /// <summary>
    /// 풀에서 오브젝트를 가져옵니다.
    /// </summary>
    public GameObject GetObject(string poolKey)
    {
        if (pools.TryGetValue(poolKey, out Pool pool))
        {
            if (pool.queue.Count > 0)
            {
                return pool.queue.Dequeue();
            }
            else
            {
                GameObject prefab = Resources.Load<GameObject>(pools[poolKey].prefabPath);

                GameObject newObj = Instantiate(prefab);
                newObj.name = prefab.name;
                newObj.transform.SetParent(pools[poolKey].parent);
                return newObj;
            }
        }
        else
        {
            Debug.LogError($"존재하지 않는 풀입니다: {poolKey}");
            return null;
        }
    }

    /// <summary>
    /// 오브젝트를 풀에 되돌립니다.
    /// </summary>
    public void ReleaseObject(string poolKey, GameObject obj)
    {
        if (!pools.ContainsKey(poolKey))
        {
            Debug.LogError($"존재하지 않는 풀입니다: {poolKey}");
            Destroy(obj);
            return;
        }

        obj.SetActive(false);
        pools[poolKey].queue.Enqueue(obj);
    }

    /// <summary>
    /// 특정 풀을 삭제합니다.
    /// </summary>
    public void DeletePool(string poolKey)
    {
        if (pools.TryGetValue(poolKey, out Pool pool))
        {
            while (pool.queue.Count > 0)
            {
                Destroy(pool.queue.Dequeue());
            }
            Destroy(pool.parent.gameObject);
            pools.Remove(poolKey);
        }
    }
}
