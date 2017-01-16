using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections.Generic;

public class PrefabStore : MonoBehaviour
{
  public static System.Action<PrefabStore> PrefabStoreLoaded;

  public GameObject[] Prefabs;
  public bool BuildListOnStart;

  [SerializeField]
  private Dictionary<string, GameObject> _prefabLookup = new Dictionary<string, GameObject>();

  private static Dictionary<string, PrefabStore> _storeInstances = new Dictionary<string, PrefabStore>();

  public static void Load(string sceneName)
  {
    SceneManager.LoadSceneAsync(sceneName, LoadSceneMode.Additive);
  }

  public static PrefabStore GetInstance(string sceneName)
  {
    PrefabStore store = null;
    _storeInstances.TryGetValue(sceneName, out store);
    return store;
  }

  public GameObject GetPrefabByName(string name)
  {
    GameObject prefab = null;
    _prefabLookup.TryGetValue(name, out prefab);
    return prefab;
  }

  public GameObject GetRandomPrefab()
  {
    return Prefabs[Random.Range(0, Prefabs.Length)];
  }

  public void Unload()
  {
    SceneManager.UnloadSceneAsync(gameObject.scene.name);
  }

  private void Awake()
  {
    if (BuildListOnStart)
    {
      Prefabs = new GameObject[transform.childCount];
      for (int i = 0; i < transform.childCount; ++i)
      {
        Prefabs[i] = transform.GetChild(i).gameObject;
        Prefabs[i].SetActive(false);
      }
    }

    for (int i = 0; i < Prefabs.Length; ++i)
    {
      _prefabLookup.Add(Prefabs[i].name, Prefabs[i]);
    }
  }

  private void Start()
  {
    _storeInstances.Add(gameObject.scene.name, this);

    if (PrefabStoreLoaded != null)
    {
      PrefabStoreLoaded(this);
    }
  }
}