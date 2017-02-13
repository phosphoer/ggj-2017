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

  private void Update()
  {
    for (int i = 1; i < m_audioSources.Length; ++i)
    {
      m_audioSources[i].timeSamples = m_audioSources[0].timeSamples;
    }
  }
}