using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class DrawerInteractable : XRGrabInteractable
{
    [SerializeField] Transform drawerTransform;
    [SerializeField] XRSocketInteractor keySocket;
    [SerializeField] bool isLocked;
    [SerializeField] GameObject keyIndicatorLight;

    private Transform parentTransform;
    private const string defaultLayer = "Default";
    private const string grabLayer = "Grab";
    private bool isGrabbed;
    private Vector3 limitPositions;
    [SerializeField] float drawerLimitZ = 0.8f;

    [SerializeField] private Vector3 limitDistances = new Vector3 (0.2f, 0.2f, 0);

    void Start()
    {
        if (keySocket != null)
        {
            keySocket.selectEntered.AddListener(onDrawerUnlocked);
            keySocket.selectExited.AddListener(onDrawerLocked);
        }
        parentTransform = transform.parent.transform;
        limitPositions = drawerTransform.localPosition;
    }

    void onDrawerLocked(SelectExitEventArgs arg0)
    {
        isLocked = true;
        Debug.Log("***Drawer Locked***");
    }

    void onDrawerUnlocked(SelectEnterEventArgs arg0)
    {
        isLocked = false;
        if (keyIndicatorLight != null)
        {
            keyIndicatorLight.SetActive(false);
        }
        Debug.Log("***Drawer Unlocked***");
    }

    protected override void OnSelectEntered (SelectEnterEventArgs args)
    {
        base.OnSelectEntered (args);
        
        if (!isLocked)
        {
            transform.SetParent(parentTransform);
            isGrabbed = true;
        }
        else
        {
            ChangeLayerMask(defaultLayer);
        }
    }

    protected override void OnSelectExited (SelectExitEventArgs args)
    {
        base.OnSelectExited (args);
        ChangeLayerMask(grabLayer);
        isGrabbed = false;
        transform.localPosition = drawerTransform.localPosition;
    }

    void Update()
    {
        if (isGrabbed && drawerTransform != null)
        {
            drawerTransform.localPosition =  new Vector3 (drawerTransform.localPosition.x,
            drawerTransform.localPosition.y, transform.localPosition.z);

            CheckLimits();
        }
    }

    private void CheckLimits()
    {
        if (transform.localPosition.x >= limitPositions.x + limitDistances.x ||
            transform.localPosition.x <= limitPositions.x - limitDistances.x)
        {
            ChangeLayerMask(defaultLayer);
        }
        else if (transform.localPosition.y >= limitPositions.y + limitDistances.y ||
            transform.localPosition.y <= limitPositions.y - limitDistances.y)
        {
            ChangeLayerMask(defaultLayer);
        }
        else if (drawerTransform.localPosition.z <= limitPositions.z - limitDistances.z)
        {
            isGrabbed = false;
            drawerTransform.localPosition = limitPositions;
            ChangeLayerMask(defaultLayer);
        }
        else if (drawerTransform.localPosition.z >= drawerLimitZ + limitDistances.z)
        {
            isGrabbed = false;
            drawerTransform.localPosition = new Vector3(
            drawerTransform.localPosition.x,
            drawerTransform.localPosition.y,
            drawerLimitZ
            );
            ChangeLayerMask(defaultLayer);
        }
    }

    void ChangeLayerMask(string mask)
    {
        interactionLayers = InteractionLayerMask.GetMask(mask);    
    }
}
