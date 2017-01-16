using UnityEngine;

public class PlayerRewiredLink : MonoBehaviour
{
  public Rewired.Player RewiredPlayer
  {
    get { return Rewired.ReInput.players.GetPlayer(RewiredPlayerID); }
  }

  public int RewiredPlayerID;
}