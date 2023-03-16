using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ComputerInteractable : MonoBehaviour, IInteractable
{

    public bool thisOne = false;

    public void Interact()
    {
        CallEndGame();
    }

    private void CallEndGame() 
    {
        if (!PlayerScript.instance.stopTimer) 
        {
            if (thisOne)
            {
                PlayerScript.instance.SetMessage("This is the right computer well done", Color.green);
                PlayerScript.instance.SetMessage("You are free to finish your job now", Color.green);
                PlayerScript.instance.stopTimer = true;
                PlayerScript.instance.gameMessage = "You did it you found the computer!!";
            }
            else
            {
                PlayerScript.instance.SetMessage("This is the wrong computer try another one", Color.red);
            }
        }
       
    }
    
}
