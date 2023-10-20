using UdonSharp;
using UnityEngine;
using VRC.SDK3.StringLoading;
using VRC.SDKBase;
using VRC.Udon;
using VRC.Udon.Common.Interfaces;

public class AdvancedWhitelistTeleporter : UdonSharpBehaviour
{
    [Header("Teleportation Button")]
    public GameObject teleportButton; // This is the button that triggers teleportation, which you already have set up.

    [Header("Objects to Enable/Disable")]
    public GameObject[] objectsToEnableWhenLocked;
    public GameObject[] objectsToDisableWhenLocked;

    [Header("Whitelist URLs")]
    public VRCUrl[] urls;

    private string full;
    private string[] whitelisted = new string[0];
    private bool isRoomLocked = false;
    private bool allowAllUsers = false;

    [HideInInspector]
    public string toggleType; // Will be set by the bridge script
    [HideInInspector]
    public bool toggleValue; // Will be set by the bridge script

    void Start()
    {
        Debug.Log("AdvancedWhitelistTeleporter: Start method called.");
        // Existing setup for whitelist
        foreach (VRCUrl url in urls)
        {
            VRCStringDownloader.LoadUrl(url, (IUdonEventReceiver)this);
        }

        UpdateRoomLockState();
        UpdateTeleportButtonState();
    }

    public void OnToggleChanged() // No parameters here, we'll use the public properties instead
    {
        Debug.Log($"AdvancedWhitelistTeleporter: OnToggleChanged called with toggleType: {toggleType}, toggleValue: {toggleValue}");
        
        if (toggleType == "LockRoom")
        {
            isRoomLocked = toggleValue;
            UpdateRoomLockState();
        }
        else if (toggleType == "AllowAllUsers")
        {
            allowAllUsers = toggleValue;
            UpdateTeleportButtonState();
        }
        else {
            Debug.LogError("Error: Sent toggleType does not match an appropriate value.");
        }
    }

    private void UpdateRoomLockState()
    {
        Debug.Log($"AdvancedWhitelistTeleporter: UpdateRoomLockState called. isRoomLocked: {isRoomLocked}");
        foreach (GameObject obj in objectsToEnableWhenLocked)
        {
            obj.SetActive(isRoomLocked);
        }

        foreach (GameObject obj in objectsToDisableWhenLocked)
        {
            obj.SetActive(!isRoomLocked);
        }

        UpdateTeleportButtonState();
    }

    private void UpdateTeleportButtonState()
    {
        Debug.Log($"AdvancedWhitelistTeleporter: UpdateTeleportButtonState called. allowAllUsers: {allowAllUsers}, isRoomLocked: {isRoomLocked}");

        if (isRoomLocked)
        {
            teleportButton.SetActive(false);
        }
        else
        {

            Debug.Log(IsNameInWhitelist(Networking.LocalPlayer.displayName));
            ParseWhitelist();
            // teleportButton.SetActive(allowAllUsers);
            // teleportButton.SetActive(IsNameInWhitelist(Networking.LocalPlayer.displayName));
            teleportButton.SetActive(allowAllUsers || IsNameInWhitelist(Networking.LocalPlayer.displayName));
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

        UpdateTeleportButtonState();
    }

    bool IsNameInWhitelist(string name)
    {
        Debug.Log("Name brought in: " + name);
        for (int i = 0; i < whitelisted.Length; i++)
        {
            Debug.Log("Name to compare to: " + whitelisted[i]);
            Debug.Log("Does Name match?: " + (whitelisted[i] == name));
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
}
