using UnityEngine;
using System.Collections.Generic;

public class Player : MonoBehaviour
{
  public static event System.Action<Player> PlayerJoined;
  public static event System.Action<Player> PlayerLeft;
  public static event System.Action<Player, Transform> PlayerSpawned;
  public static event System.Action<Player, Transform> PlayerDespawned;

  public static Player GetPlayer(int index)
  {
    return _sPlayersJoined[index];
  }

  public static int PlayerCount { get { return _sPlayersJoined.Count; } }

  public event System.Action Joined;
  public event System.Action Left;
  public event System.Action<Transform> Spawned;
  public event System.Action<Transform> Despawned;

  public string ExclusiveLayerName;

  private static List<Player> _sPlayersJoined = new List<Player>();

  private Transform _lastSpawnPoint;

  public void Spawn(Transform spawnPoint)
  {
    _lastSpawnPoint = spawnPoint;

    if (Spawned != null)
      Spawned(spawnPoint);
    if (PlayerSpawned != null)
      PlayerSpawned(this, spawnPoint);
  }

  public void Despawn()
  {
    if (Despawned != null)
      Despawned(_lastSpawnPoint);
    if (PlayerDespawned != null)
      PlayerDespawned(this, _lastSpawnPoint);
  }

  public void Respawn()
  {
    Despawn();
    Spawn(_lastSpawnPoint);
  }

  public void Join()
  {
    if (!_sPlayersJoined.Contains(this))
    {
      _sPlayersJoined.Add(this);

      if (string.IsNullOrEmpty(ExclusiveLayerName))
        ExclusiveLayerName = string.Format("Player{0}", _sPlayersJoined.Count);

      if (PlayerJoined != null)
        PlayerJoined(this);
      if (Joined != null)
        Joined();
    }
  }

  public void Leave()
  {
    if (_sPlayersJoined.Contains(this))
    {
      _sPlayersJoined.Remove(this);
      if (PlayerLeft != null)
        PlayerLeft(this);
      if (Left != null)
        Left();
    }
  }
}