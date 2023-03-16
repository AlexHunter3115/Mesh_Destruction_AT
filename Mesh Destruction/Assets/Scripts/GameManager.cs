using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public List<ComputerInteractable> computers =  new List<ComputerInteractable>();


    private void Start()
    {
        int rand = Random.Range(0, computers.Count);

        computers[rand].thisOne = true;
        
    }

}
