using UnityEngine;

public class SplitscreenAudioListener : MonoBehaviour
{
  private void OnEnable()
  {
    Splitscreen3DAudio.AddListener(this);
  }

  private void OnDisable()
  {
    Splitscreen3DAudio.RemoveListener(this);
  }
}