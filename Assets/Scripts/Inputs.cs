using UnityEngine;

[RequireComponent(typeof(Game))]
public class Inputs : MonoBehaviour
{
    private Game game;
    private void Start()
    {
        game = GetComponent<Game>();
    }

    public void PlacePawn(int buttonIndex)
    {
        if (buttonIndex >= 0 && buttonIndex < 9)
            game.PlacePlayerPawn(buttonIndex);
        else Debug.Log("OnClick calls are faulty."); 
    }

}
