using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RWVR_InteractionController : MonoBehaviour
{
    public Transform snapColliderOrigin; // 1 a reference to the tip of the controller. how far you can reach

    public GameObject ControllerModel; // 2

    [HideInInspector]
    public Vector3 velocity; // 3 speed and direction of the controller. You’ll use this to calculate how objects should fly when you throw them.
    [HideInInspector]
    public Vector3 angularVelocity; // 4 used when calculating the motion of thrown objects.

    private RWVR_InteractionObject objectBeingInteractedWith; // 5 This is the InteractionObject this controller is currently interacting with. You use it to send events to the active object.

    private SteamVR_TrackedObject trackedObj; // 6

    private SteamVR_Controller.Device Controller // 1
    {
        get { return SteamVR_Controller.Input((int)trackedObj.index); }
    }

    public RWVR_InteractionObject InteractionObject // 2  returns the InteractionObject this controller is currently interacting with.
    {
        get { return objectBeingInteractedWith; }
    }

    void Awake() // 3
    {
        trackedObj = GetComponent<SteamVR_TrackedObject>();
    }

    private void CheckForInteractionObject()
    {
        Collider[] overlappedColliders = Physics.OverlapSphere(snapColliderOrigin.position, snapColliderOrigin.lossyScale.x / 2f); // 1

        foreach (Collider overlappedCollider in overlappedColliders) // 2
        {
            if (overlappedCollider.CompareTag("InteractionObject") && overlappedCollider.GetComponent<RWVR_InteractionObject>().IsFree()) // 3
            {
                objectBeingInteractedWith = overlappedCollider.GetComponent<RWVR_InteractionObject>(); // 4
                objectBeingInteractedWith.OnTriggerWasPressed(this); // 5
                return; // 6
            }
        }
    }

    void Update()
    {
        if (Controller.GetHairTriggerDown()) // 1
        {
            CheckForInteractionObject();
        }

        if (Controller.GetHairTrigger()) // 2
        {
            if (objectBeingInteractedWith)
            {
                objectBeingInteractedWith.OnTriggerIsBeingPressed(this);
            }
        }

        if (Controller.GetHairTriggerUp()) // 3
        {
            if (objectBeingInteractedWith)
            {
                objectBeingInteractedWith.OnTriggerWasReleased(this);
                objectBeingInteractedWith = null;
            }
        }
    }

    private void UpdateVelocity()
    {
        velocity = Controller.velocity;
        angularVelocity = Controller.angularVelocity;
    }

    void FixedUpdate()
    {
        UpdateVelocity();
    }

    //hide controllers to make the experience more real
    public void HideControllerModel()
    {
        ControllerModel.SetActive(false);
    }

    public void ShowControllerModel()
    {
        ControllerModel.SetActive(true);
    }

    public void Vibrate(ushort strength) // 1 Its range is between 1 and 3999.
    {
        Controller.TriggerHapticPulse(strength);
    }

    public void SwitchInteractionObjectTo(RWVR_InteractionObject interactionObject) // 2
    {
        objectBeingInteractedWith = interactionObject; // 3
        objectBeingInteractedWith.OnTriggerWasPressed(this); // 4
    }

}
