using UnityEngine;

public class GameGlobals : Singleton<GameGlobals>
{
  public Color[] MoodColors;
  public GameObject InteractPromptPrefab;
  public PlayerController PlayerController { get; set; }

  private void Awake()
  {
    Instance = this;
  }
}