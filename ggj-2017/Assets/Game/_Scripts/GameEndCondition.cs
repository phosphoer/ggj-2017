using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections;

public class GameEndCondition : MonoBehaviour
{
  [SerializeField]
  private Image m_fadeToBlackImage;

  private void Start()
  {
    DateMood.MoodCentered += OnMoodCentered;
    DateMood.MoodOutOfControl += OnMoodOutOfControl;
  }

  private void OnDestroy()
  {
    DateMood.MoodCentered -= OnMoodCentered;
    DateMood.MoodOutOfControl -= OnMoodOutOfControl;
  }

  private void OnMoodCentered(DateMood dateMood)
  {
    
  }

  private void OnMoodOutOfControl(DateMood dateMood)
  {
    StartCoroutine(EndAnimation());
  }

  private IEnumerator EndAnimation()
  {
    const float duration = 1.0f;
    float startTime = Time.time;
    while (Time.time < startTime + duration)
    {
      float t = (Time.time - startTime) / duration;
      Color blackColor = m_fadeToBlackImage.color;
      blackColor.a = Mathf.Lerp(0.0f, 1.0f, t);
      m_fadeToBlackImage.color = blackColor;
      yield return null;
    }

    SceneManager.LoadScene(gameObject.scene.name);
  }
}