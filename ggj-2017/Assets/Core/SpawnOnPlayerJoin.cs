using UnityEngine;

public class SpawnOnPlayerJoin : MonoBehaviour
{
  public SpawnPoints SpawnPoints;

  private void OnPlayerJoined(Player player)
  {
    player.Spawn(SpawnPoints.ClaimSpawn(player));
  }

  private void Awake()
  {
    Player.PlayerJoined += OnPlayerJoined;
  }

  private void OnDestroy()
  {
    Player.PlayerJoined -= OnPlayerJoined;
  }
}