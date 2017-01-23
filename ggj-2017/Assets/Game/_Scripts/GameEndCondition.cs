using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections;

public class GameEndCondition : MonoBehaviour
{
  [System.SerializableAttribute]
  public class MessageList
  {
    public string[] Messages;
  }

  public static event System.Action MinuteElapsed;

  [SerializeField]
  private Image m_fadeToBlackImage;

  [SerializeField]
  private GameObject m_loseText;

  [SerializeField]
  private Text m_winText;

  [SerializeField]
  private Image m_fadeToWhiteImage;

  [SerializeField]
  private MessageList[] m_endMessages;

  private int m_score;
  private float m_gameStartTime;
  private bool m_ending;

  private const int kMaxScore = 10;
  private const float kGameLength = 5 * 60.0f;

  private void Start()
  {
    m_gameStartTime = Time.time;
    m_loseText.SetActive(false);

    StartCoroutine(MinuteCounter());

    DateMood.MoodCentered += OnMoodCentered;
    DateMood.MoodOutOfControl += OnMoodOutOfControl;
    DateMood.GoodMood += OnGoodMood;
  }

  private void OnDestroy()
  {
    DateMood.MoodCentered -= OnMoodCentered;
    DateMood.MoodOutOfControl -= OnMoodOutOfControl;
    DateMood.GoodMood -= OnGoodMood;
  }

  private void Update()
  {
    if (!m_ending && Time.time > m_gameStartTime + kGameLength)
    {
      m_ending = true;
      StartCoroutine(WinAnimation());
    }
  }

  private void OnGoodMood(DateMood dateMood)
  {
    ++m_score;
  }

  private void OnMoodCentered(DateMood dateMood)
  {
    
  }

  private void OnMoodOutOfControl(DateMood dateMood)
  {
    StartCoroutine(EndAnimation());
  }

  private IEnumerator MinuteCounter()
  {
    while (!m_ending)
    {
      yield return new WaitForSeconds(60.0f);

      if (MinuteElapsed != null)
      {
        MinuteElapsed();
      }
    }
  }

  private IEnumerator WinAnimation()
  {
    int messageListIndex = Mathf.RoundToInt(((float)m_score / kMaxScore) * (m_endMessages .Length - 1));
    MessageList messageList = m_endMessages[messageListIndex];

    int messageIndex = Random.Range(0, messageList.Messages.Length);
    string message = messageList.Messages[messageIndex];

    m_winText.text = message;
    m_winText.gameObject.SetActive(true);

    const float duration = 3.0f;
    float startTime = Time.time;
    while (Time.time < startTime + duration)
    {
      float t = (Time.time - startTime) / duration;
      Color whiteColor = m_fadeToWhiteImage.color;
      whiteColor.a = Mathf.Lerp(0.0f, 1.0f, t);
      m_fadeToWhiteImage.color = whiteColor;
      yield return null;
    }


    SceneManager.LoadScene(gameObject.scene.name);
  }

  private IEnumerator EndAnimation()
  {
    m_loseText.gameObject.SetActive(true);
    
    const float duration = 3.0f;
    float startTime = Time.time;
    while (Time.time < startTime + duration)
    {
      float t = (Time.time - startTime) / duration;
      Color blackColor = m_fadeToBlackImage.color;
      blackColor.a = Mathf.Lerp(0.0f, 1.0f, t);
      m_fadeToBlackImage.color = blackColor;
      yield return null;
    }

    SceneManager.UnloadSceneAsync(gameObject.scene.name);
    SceneManager.LoadScene(gameObject.scene.name);
  }
}