using UnityEngine;

public class AudioSync : MonoBehaviour
{
  [SerializeField]
  private AudioSource[] m_audioSources;

  private void Start()
  {
    foreach (AudioSource source in m_audioSources)
    {
      source.Play();
    }
  }
}