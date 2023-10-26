using UdonSharp;
using UnityEngine;
using UnityEngine.UI;
using VRC.SDKBase;

public class AdvancedWhitelistController : UdonSharpBehaviour
{
    [Header("Whitelist URLs")]
    public VRCUrl[] urls;

    [Header("Whitelist Toggle Checker")]
    public Toggle isWhitelisted;

    private string full;
    private string[] whitelisted = new string[0];
    private void Start()
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
}
