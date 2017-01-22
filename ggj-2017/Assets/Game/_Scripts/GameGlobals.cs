using UnityEngine;
using UnityEngine.SceneManagement;

public class GameGlobals : Singleton<GameGlobals>
{
  public Color[] MoodColors;
  public int MaxIntensity = 5;
  public GameObject InteractPromptPrefab;
  public PlayerController PlayerController { get; set; }

  private void Awake()
  {
    Instance = this;
  }

  private void Start()
  {
    SceneManager.SetActiveScene(gameObject.scene);
  }
}