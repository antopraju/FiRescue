using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MLAgents;

public class PlayerAcademy : Academy
{
    private PlayerArea[] playerAreas;
    public override void AcademyReset()
    {
        //Get the penguin Areas
        if (playerAreas == null)
        {
            playerAreas = FindObjectsOfType<PlayerArea>();
        }
        
        //Set up areas
        foreach (PlayerArea playerArea in playerAreas)
        {
            playerArea.level = resetParameters["level"];
            playerArea.ResetArea();
        }
        
    }
}