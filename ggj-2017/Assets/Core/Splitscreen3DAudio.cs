using UnityEngine;
using System.Collections.Generic;

public class Splitscreen3DAudio : MonoBehaviour
{
  public float Range = 10.0f;
  public float Volume = 1.0f;
  public bool GetAudioSourcesOnStart = true;

  [SerializeField]
  private AudioSource[] _audioSources;

  private static List<SplitscreenAudioListener> listeners = new List<SplitscreenAudioListener>();

  public static void AddListener(SplitscreenAudioListener listener)
  {
    listeners.Add(listener);
  }

  public static void RemoveListener(SplitscreenAudioListener listener)
  {
    listeners.Remove(listener);
  }

  private void Start()
  {
    if (GetAudioSourcesOnStart)
      _audioSources = GetComponents<AudioSource>(); 

    Update();
  }

  private void Update()
  {
    var minDistance = Mathf.Infinity;
    for (var i = 0; i < listeners.Count; ++i)
    {
      var dist = Vector3.Distance(listeners[i].transform.position, transform.position);
      minDistance = Mathf.Min(minDistance, dist);
    }

    for (var i = 0; i < _audioSources.Length; ++i)
    {
      var t = 1.0f - Mathf.Clamp01(minDistance / Range);
      float desiredVolume = Mathfx.Hermite(0, Volume, t);
      _audioSources[i].volume = desiredVolume;
    }
  }

  private void OnDrawGizmos()
  {
    Gizmos.color = Color.cyan;
    Gizmos.DrawWireSphere(transform.position, Range);
  }
}