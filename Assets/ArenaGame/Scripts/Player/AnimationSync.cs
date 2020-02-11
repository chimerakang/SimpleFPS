using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// This component is responsible for *most* of the animations synced across the network.
/// </summary>
public class AnimationSync : MonoBehaviour
{
    [SerializeField]
    //Get a refrence to the main network object script
    private NetworkedPlayer np;

    //The view model animator
    [SerializeField]
    private Animator viewModelAnimator;

    //the world model animator
    [SerializeField]
    private Animator worldModelAnimator;

    void Update()
    {
        //if we are the owner
        if (np.networkObject.IsOwner)
        {
            var horizontal = Input.GetAxis("Horizontal");
            var vertical = Input.GetAxis("Vertical");

            //set the "IsMoving" variable on the world & view model
            if (vertical != 0 || horizontal != 0)
            {
                if (worldModelAnimator.gameObject.activeInHierarchy || viewModelAnimator.gameObject.activeInHierarchy)
                {
                    worldModelAnimator.SetBool("IsMoving", true);
                    //set the view model
                    viewModelAnimator.SetBool("IsMoving", true);
                }                
                //Set the bool across the network
                np.networkObject.isMoving = true;
            }
            else //we aren't moving
            {
                if (worldModelAnimator.gameObject.activeInHierarchy || viewModelAnimator.gameObject.activeInHierarchy)
                {
                    worldModelAnimator.SetBool("IsMoving", false);
                    viewModelAnimator.SetBool("IsMoving", false);
                }

                np.networkObject.isMoving = false;
            }

            if (worldModelAnimator.gameObject.activeInHierarchy)
            {
                //set the vertical and horizontal floats
                worldModelAnimator.SetFloat("vertical", vertical);
                worldModelAnimator.SetFloat("horizontal", horizontal);
            }

            //sync the vertial and horizontal floats on the network
            np.networkObject.horizontal = horizontal;
            np.networkObject.vertical = vertical;
        }
        else //if we aren't the owner
        {
            if (worldModelAnimator.gameObject.activeInHierarchy)
            {
                //Use the values set by the owner
                worldModelAnimator.SetBool("IsMoving", np.networkObject.isMoving);
                worldModelAnimator.SetFloat("vertical", np.networkObject.vertical);
                worldModelAnimator.SetFloat("horizontal", np.networkObject.horizontal);
            }

        }
    }
}
