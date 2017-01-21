using UnityEngine;

public class MoodColorZone : MonoBehaviour
{
  public MoodColor MoodColor;
  
  [SerializeField]
  private Transform m_FocusTransform;

  private GameObject m_interactionPrompt;

  public void ShowInteractionPrompt()
  {
    m_interactionPrompt = Instantiate(GameGlobals.Instance.InteractPromptPrefab);
    m_interactionPrompt.transform.position = m_FocusTransform.position;
  }

  public void HideInteractionPrompt()
  {
    if (m_interactionPrompt != null)
    {
      Destroy(m_interactionPrompt);
      m_interactionPrompt = null;
    }
  }
}