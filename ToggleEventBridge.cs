﻿using UnityEngine;
using UnityEngine.UI; // Required for the Toggle component

public class ToggleEventBridge : MonoBehaviour
{
    public AdvancedWhitelistTeleporter targetScript; // Assign the UdonSharpBehaviour script in the inspector

    public enum ToggleType
    {
        LockRoom,
        AllowAllUsers
    }

    public ToggleType toggleType;

    private Toggle toggle; // Reference to the Toggle component

    void Start()
    {
        Debug.Log("ToggleEventBridge: Start method called.");

        toggle = GetComponent<Toggle>(); // Get the Toggle component on the same GameObject

        if (toggle == null)
        {
            Debug.LogError("ToggleEventBridge: No Toggle component found on this GameObject.");
            return;
        }

        // Register the OnToggleChanged method to listen for the onValueChanged event
        toggle.onValueChanged.AddListener(OnToggleChanged);
    }

    // This method will be called whenever the Toggle's value changes
    public void OnToggleChanged(bool value)
    {
        Debug.Log($"ToggleEventBridge: OnToggleChanged called with value: {value}");
        targetScript.toggleType = toggleType.ToString();
        targetScript.toggleValue = value;
        targetScript.SendCustomEvent("OnToggleChanged");
    }
}
