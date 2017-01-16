using UnityEngine;

public class Singleton<T> : MonoBehaviour where T : MonoBehaviour
{
  private static T instance;

  public static T Instance
  {
    get
    {
      return instance;
    }
    set
    {
      instance = value;
    }
  }
}