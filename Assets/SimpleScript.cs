using System;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactables;
using UnityEngine.UI;
using UnityEngine.UIElements;

public class SimpleScript : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        var simple = GetComponent<XRSimpleInteractable>();
        simple.selectEntered.AddListener(OnSelectEntered);
    }

    private void OnSelectEntered(SelectEnterEventArgs arg0)
    {
        Pose p = arg0.interactorObject.GetAttachPoseOnSelect(arg0.interactorObject.firstInteractableSelected);
        string grabbedObjectName = arg0.interactorObject.firstInteractableSelected.transform.gameObject.name;
        Vector3 position = p.position;
        Debug.Log(grabbedObjectName);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
