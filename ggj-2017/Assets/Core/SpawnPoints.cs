using UnityEngine;

public class SpawnPoints : MonoBehaviour
{
  public Transform[] Spawns { get { return _spawns; } }

  [SerializeField]
  private Transform[] _spawns;

  private bool[] _spawnClaimedStates;

  public Transform ClaimSpawn(Player forPlayer, int requestedIndex = -1)
  {
    if (requestedIndex < 0)
      requestedIndex = Random.Range(0, Spawns.Length);

    requestedIndex = requestedIndex % Spawns.Length;

    if (_spawnClaimedStates[requestedIndex])
    {
      for (int i = 0; i < Spawns.Length; ++i)
      {
        if (!_spawnClaimedStates[i])
          return ClaimSpawn(forPlayer, i);
      }

      return null;
    }

    _spawnClaimedStates[requestedIndex] = true;
    return Spawns[requestedIndex];
  }

  public void ReleaseSpawn(Transform spawn)
  {
    for (int i = 0; i < Spawns.Length; ++i)
    {
      if (Spawns[i] == spawn)
      {
        _spawnClaimedStates[i] = false;
        return;
      }
    }
  }

  private void OnPlayerDespawned(Player player, Transform spawn)
  {
    ReleaseSpawn(spawn);
  }

  private void Awake()
  {
    Player.PlayerDespawned += OnPlayerDespawned;

    _spawnClaimedStates = new bool[Spawns.Length + 1];
  }

  private void OnDestroy()
  {
    Player.PlayerDespawned -= OnPlayerDespawned;
  }
}