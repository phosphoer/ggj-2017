using UnityEngine;

public class SpawnOnLevelStart : MonoBehaviour
{
  public SpawnPoints SpawnPoints;

  private void OnGameStateStart(GameState state)
  {
    for (int i = 0; i < Player.PlayerCount; ++i)
    {
      Player p = Player.GetPlayer(i);
      p.Spawn(SpawnPoints.ClaimSpawn(p));
    }
  }

  private void Awake()
  {
    GameState.GameStateStart += OnGameStateStart;
  }

  private void OnDestroy()
  {
    GameState.GameStateStart -= OnGameStateStart;
  }
}