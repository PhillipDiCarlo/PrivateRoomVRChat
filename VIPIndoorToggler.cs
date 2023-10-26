using UdonSharp;
using UnityEngine;
using UnityEngine.UI;
using VRC.SDKBase;

public enum ToggleType
    {
        LockRoom,
        AllowAllUsers
    }
public class VIPIndoorToggler : UdonSharpBehaviour
{
    
    [Header("Settings")]
    public ToggleType toggleType;
    
    [Header("Link Toggle to Self")]
    public Toggle toggle; // The toggle UI component defined by the user

    [Header("Objects to Enable/Disable")]
    public GameObject[] objectsToEnableWhenLocked;
    public GameObject[] objectsToDisableWhenLocked;

    [Header("AllowAll/RoomLock Toggle")]
    public Toggle checkedToggle;
    private bool previousToggleState = false;
    
    private void Start()
    {
        if (toggle == null)
        {
            Debug.LogError("VIPIndoorToggler: No user-defined toggle has been assigned.");
            return;
        }

        previousToggleState = toggle.isOn;
        OnToggleChanged(previousToggleState);
    }

    private void Update()
    {
        if (toggle.isOn != previousToggleState)
        {
            OnToggleChanged(toggle.isOn);
            previousToggleState = toggle.isOn;
        }
    }

    private void OnToggleChanged(bool isOn)
    {
        // If the toggle type matches, do the corresponding action
        if ((toggleType == ToggleType.LockRoom)) {
            LockedRoom(isOn);
        }
        else if ((toggleType == ToggleType.AllowAllUsers)) {
            AllowUsers(isOn);
        }

    }
    private void LockedRoom(bool isOn) {
        if (isOn) {
            checkedToggle.isOn = true;
            foreach (GameObject obj in objectsToEnableWhenLocked) {
                obj.SetActive(true);
            }

            foreach (GameObject obj in objectsToDisableWhenLocked) {
                obj.SetActive(false);
            }
        }
        else {
            checkedToggle.isOn = false;
            // Here we revert the objects to their original state when the room is not locked
            foreach (GameObject obj in objectsToEnableWhenLocked) {
                obj.SetActive(false); // objects that were enabled when locked are now disabled
            }

            foreach (GameObject obj in objectsToDisableWhenLocked) {
                obj.SetActive(true); // objects that were disabled when locked are now enabled
            }
        }
    }

    private void AllowUsers(bool isOn) {
        if (isOn) {
            checkedToggle.isOn = true;
        }
        else {
            checkedToggle.isOn = false;
        }
    }

    // private void EnableObjects()
    // {
    //     foreach (GameObject obj in objectsToControl)
    //     {
    //         obj.SetActive(true);
    //     }
    // }

    // private void DisableObjects()
    // {
    //     foreach (GameObject obj in objectsToControl)
    //     {
    //         obj.SetActive(false);
    //     }
    // }
}
