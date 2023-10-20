using UdonSharp;
using UnityEngine;
using UnityEngine.UI;
using VRC.SDK3.StringLoading;
using VRC.SDKBase;
using VRC.Udon;
using VRC.Udon.Common.Interfaces;

public class AdvancedWhitelistTeleporter : UdonSharpBehaviour
{
    [Header("Teleportation Button")]
    public GameObject teleportButton; // This is the button that triggers teleportation.

    [Header("Objects to Enable/Disable")]
    public GameObject[] objectsToEnableWhenLocked;
    public GameObject[] objectsToDisableWhenLocked;

    [Header("Whitelist URLs")]
    public VRCUrl[] urls;

    // UI Toggle for whitelist status
    [Header("Whitelist Toggle Checker")]
    public Toggle isWhitelisted; // Assign your toggle in the inspector

    private bool lockRoom = false;
    private bool allowAllUsers = false;

    private string full;
    private string[] whitelisted = new string[0];

    void Start()
    {
        foreach (VRCUrl url in urls)
        {
            VRCStringDownloader.LoadUrl(url, (IUdonEventReceiver)this);
        }
    }

    public override void OnStringLoadSuccess(IVRCStringDownload result)
    {
        full = result.Result;
        ParseWhitelist();
    }

    public override void OnStringLoadError(IVRCStringDownload result)
    {
        Debug.LogError($"Whitelist Error: Failed to load string - {result.Result}");
    }

    void ParseWhitelist()
    {
        string[] names = full.Split('\n');

        foreach (string rawName in names)
        {
            string trimmedName = rawName.Trim();
            if (!IsNameInWhitelist(trimmedName))
            {
                AddToWhitelist(trimmedName);
            }
        }

        // Check if the local player is in the whitelist and update the toggle accordingly
        if (IsNameInWhitelist(Networking.LocalPlayer.displayName))
        {
            isWhitelisted.isOn = true;
        }
        else
        {
            isWhitelisted.isOn = false;
        }

        UpdateTeleporterStatus();
    }

    bool IsNameInWhitelist(string name)
    {
        for (int i = 0; i < whitelisted.Length; i++)
        {
            if (whitelisted[i] == name)
            {
                return true;
            }
        }
        return false;
    }

    void AddToWhitelist(string name)
    {
        string[] newWhitelist = new string[whitelisted.Length + 1];
        for (int i = 0; i < whitelisted.Length; i++)
        {
            newWhitelist[i] = whitelisted[i];
        }
        newWhitelist[whitelisted.Length] = name;
        whitelisted = newWhitelist;
    }

    public void HandleToggleChanged(ToggleEventBridge.ToggleType toggleType, bool value)
    {
        switch (toggleType)
        {
            case ToggleEventBridge.ToggleType.LockRoom:
                lockRoom = value;
                break;
            case ToggleEventBridge.ToggleType.AllowAllUsers:
                allowAllUsers = value;
                break;
        }

        UpdateTeleporterStatus();
    }

    private void UpdateTeleporterStatus()
{
    // If the room is locked, we enable/disable the respective objects and disable the teleporter.
    if (lockRoom)
    {
        teleportButton.SetActive(false); // disable the teleporter
        foreach (GameObject obj in objectsToEnableWhenLocked)
        {
            obj.SetActive(true);
        }
        foreach (GameObject obj in objectsToDisableWhenLocked)
        {
            obj.SetActive(false);
        }
    }
    else // this else corresponds to the room not being locked
    {
        // If the room isn't locked and either everyone is allowed or the player is whitelisted.
        if (allowAllUsers || isWhitelisted.isOn) // checking the status of the toggle
        {
            teleportButton.SetActive(true); // enable the teleporter
        }
        else
        {
            teleportButton.SetActive(false); // ensure the teleporter is disabled
        }

        // Here we revert the objects to their original state when the room is not locked
        foreach (GameObject obj in objectsToEnableWhenLocked)
        {
            obj.SetActive(false); // objects that were enabled when locked are now disabled
        }
        foreach (GameObject obj in objectsToDisableWhenLocked)
        {
            obj.SetActive(true); // objects that were disabled when locked are now enabled
        }
    }
}

}
