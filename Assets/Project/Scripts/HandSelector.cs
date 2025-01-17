using UnityEngine;
using UnityEngine.XR;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactables;

public class HandSelector : MonoBehaviour
{
    public GameObject weapon; // Assign the weapon GameObject in the inspector
    public Transform leftHandAnchor; // Assign the left-hand anchor (controller transform)
    public Transform rightHandAnchor; // Assign the right-hand anchor (controller transform)
    public Canvas selectionCanvas; // Assign the canvas with the UI

    public GameObject shootingCanvas;
    public GameObject gameinfoCanvas;

    private XRGrabInteractable grabInteractable; // XRGrabInteractable component on the weapon
    private bool weaponAssigned = false; // To ensure we only assign once

    void Start()
    {
        // Get the XRGrabInteractable component on the weapon
        grabInteractable = weapon.GetComponent<XRGrabInteractable>();
    }

    void Update()
    {
        if (!weaponAssigned)
        {
            CheckForInput();
        }
    }

    private void CheckForInput()
    {
        // Get input from the left-hand controller
        InputDevice leftHandDevice = InputDevices.GetDeviceAtXRNode(XRNode.LeftHand);
        if (leftHandDevice.TryGetFeatureValue(CommonUsages.primaryButton, out bool leftPressed) && leftPressed)
        {
            SimulateGrab(leftHandAnchor);
        }

        // Get input from the right-hand controller
        InputDevice rightHandDevice = InputDevices.GetDeviceAtXRNode(XRNode.RightHand);
        if (rightHandDevice.TryGetFeatureValue(CommonUsages.primaryButton, out bool rightPressed) && rightPressed)
        {
            SimulateGrab(rightHandAnchor);
        }
    }

    private void SimulateGrab(Transform handAnchor)
    {
        weapon.SetActive(true);
        shootingCanvas.SetActive(true);
        gameinfoCanvas.SetActive(true);

        //IF ITS POSSIBLE SET THE WEAPON TO THE HAND

        selectionCanvas.enabled = false;

        weaponAssigned = true;
    }
}
