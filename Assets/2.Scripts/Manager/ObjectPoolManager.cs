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
    /// Ư�� ������ ��ο��� Ǯ�� �����մϴ�.
    /// </summary>
    public void CreatePool(string prefabPath, int count)
    {
        GameObject prefab = Resources.Load<GameObject>(prefabPath);
        if (prefab == null)
        {
            Debug.LogError($"�������� ã�� �� �����ϴ�: {prefabPath}");
            return;
        }

        string poolKey = System.IO.Path.GetFileNameWithoutExtension(prefabPath); // ��ο��� ���ϸ� ����
        if (!pools.ContainsKey(poolKey))
        {
            pools[poolKey] = new Pool();

            // �θ� ������Ʈ ����
            GameObject poolContainer = new GameObject(poolKey);
            poolContainer.transform.SetParent(transform);
            pools[poolKey].parent = poolContainer.transform;

            pools[poolKey].prefabPath = prefabPath;
        }
        else
        {
            Debug.LogError($"���� �̸��� ���� Ǯ�� �����մϴ�: {poolKey}");
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
    /// Ǯ���� ������Ʈ�� �����ɴϴ�.
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
            Debug.LogError($"�������� �ʴ� Ǯ�Դϴ�: {poolKey}");
            return null;
        }
    }

    /// <summary>
    /// ������Ʈ�� Ǯ�� �ǵ����ϴ�.
    /// </summary>
    public void ReleaseObject(string poolKey, GameObject obj)
    {
        if (!pools.ContainsKey(poolKey))
        {
            Debug.LogError($"�������� �ʴ� Ǯ�Դϴ�: {poolKey}");
            Destroy(obj);
            return;
        }

        obj.SetActive(false);
        pools[poolKey].queue.Enqueue(obj);
    }

    /// <summary>
    /// Ư�� Ǯ�� �����մϴ�.
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
