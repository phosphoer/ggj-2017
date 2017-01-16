using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class PlaySoundOnStart : MonoBehaviour
{
  public AudioClip Sound;

  private void Start()
  {
    GetComponent<AudioSource>().PlayOneShot(Sound);
  }
}