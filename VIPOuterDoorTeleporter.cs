using UdonSharp;
using UnityEngine;
using UnityEngine.UI;
using VRC.SDK3.StringLoading;
using VRC.SDKBase;
using VRC.Udon;
using VRC.Udon.Common.Interfaces;

public class VIPOuterDoorTeleporter : UdonSharpBehaviour
{
    [Header("Toggles")]
    public Toggle isWhitelistedToggle;
    public Toggle isLockedToggle;
    public Toggle allowAllToggle;

    [Header("Teleportation Target")]
    public GameObject teleportTarget;

    private void Start()
    {
    }
    
    public override void Interact()
    {
        // Check if the collider is a player
        Debug.Log("Event was triggered VIPOuterDoorTeleporter.");
        if (!Networking.LocalPlayer.isLocal)
        {
            return;
        }

        // Check the lock toggle
        if (isLockedToggle.isOn)
        {
            Debug.Log("Door is locked. No teleportation.");
            return;
        }

        // Check the whitelist or allow all toggles
        if (isWhitelistedToggle.isOn || allowAllToggle.isOn)
        {
            TeleportPlayerToTarget(Networking.LocalPlayer);
        }
        else
        {
            Debug.Log("Player is not whitelisted and general entry is not allowed.");
        }
    }

    private void TeleportPlayerToTarget(VRCPlayerApi player)
    {
        player.TeleportTo(teleportTarget.transform.position, teleportTarget.transform.rotation);
    }
}
