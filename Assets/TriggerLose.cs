using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TriggerLose : MonoBehaviour
{
    public GameObject door0;
    private OpenDoor openDoorScript;

    public GameObject Ben;

    private void OnTriggerEnter(Collider p)
    {

        //void Start()
        //{
        //    openDoorScript = door0.GetComponent<OpenDoor>();
        //}

        //// Check if the object entering the trigger is the player
        //if (p.CompareTag("Player"))
        //{
        //    openDoorScript = door0.GetComponent<OpenDoor>();
        //    //EndGame();
        //    //Endtext.SetActive(true);
        //    if (GameData.wrongDoorChosen) {
        //        openDoorScript.openDoor();
        //        Ben.SetActive(true);
        //        Debug.Log("You Lost");
        //    }
            

        //}
    }
}
