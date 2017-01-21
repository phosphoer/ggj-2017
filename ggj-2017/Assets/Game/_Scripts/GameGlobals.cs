using UnityEngine;

public class GameGlobals : Singleton<GameGlobals>
{
  public GameObject InteractPromptPrefab;
  public PlayerController PlayerController { get; set; }

  private void Awake()
  {
    Instance = this;
  }
}