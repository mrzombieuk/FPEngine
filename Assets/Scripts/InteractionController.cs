using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;

public class InteractionController : MonoBehaviour
{
    public GameObject intObject;
    public GameObject actObject;
    public Transform playerLook;
    public GameObject actionTextUI;
    public Text actionText;
    public string actionTextString;

    // Update is called once per frame
    void Update()
    {
        var ray = new Ray(playerLook.transform.position, playerLook.transform.forward);
        RaycastHit hit;
        if (Physics.Raycast(ray, out hit, 2.5f))
        {
            if (hit.transform.CompareTag(intObject.tag))
            {
                
                actionText.text = actionTextString;
                actionTextUI.SetActive(true);
                actObject.SetActive(false);
                
                switch (actionTextString)
                {
                    case "E":
                        print("actionTextString is set to E");
                        break;

                    case "F":
                        print("actionTextString is set to F");
                        break;

                    default:
                        break;
                }
            }

        }
        else
        {
            actionText.text = null;
            actionTextUI.SetActive(false);
            actObject.SetActive(true);
        }
    }


}
