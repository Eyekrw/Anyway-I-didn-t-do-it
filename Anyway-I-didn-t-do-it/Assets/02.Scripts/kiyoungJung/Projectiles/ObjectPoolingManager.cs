using UnityEngine;
using UnityEngine.Pool;
using Photon.Pun;

public class ObjectPoolingManager : MonoBehaviour
{
    public GameObject objectPrefab;
    private ObjectPool<GameObject> _objectPool;
    private GameObject _arrowPrefab;
    void Start()
    {
        _objectPool = new ObjectPool<GameObject>(
            CreatePooledObject,
            obj => obj.SetActive(true),
            obj => obj.SetActive(false),
            DestroyPooledObject,
            true, 470, 480
        );
        _arrowPrefab = Resources.Load<GameObject>("Projectiles/Arrow");
    }

    private GameObject CreatePooledObject()
    {
        // 포톤을 통해 객체 생성
        GameObject obj = PhotonNetwork.Instantiate("Projectiles/Arrow", Vector3.zero, Quaternion.identity);
        //GameObject obj = Instantiate(_arrowPrefab, Vector3.zero, Quaternion.identity);
        
        obj.SetActive(false);  // 풀에 생성된 오브젝트는 기본적으로 비활성화
        return obj;
    }

    private void DestroyPooledObject(GameObject obj)
    {
        // 포톤을 통해 오브젝트 제거
        PhotonNetwork.Destroy(obj);
    }

    public GameObject GetPooledObject()
    {
        return _objectPool.Get();
    }

    public void ReturnToPool(GameObject obj)
    {
        _objectPool.Release(obj);
    }
}