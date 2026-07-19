using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public enum PauseReason
{
    PowerupTransition, 
    PauseMenu, 
    Cutscene, 
    HitStop,
}

public class GamePauseManager : MonoBehaviour
{
    public static GamePauseManager Instance { get; private set; }

    public static bool IsPaused => Instance != null && Instance.pauseCounts.Count > 0;

    public static event Action<bool> PauseStateChanged;

    public readonly Dictionary<PauseReason, int> pauseCounts = new();

    private void Awake()
    {
        if(Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
    }

    public static void RequestPause(PauseReason reason)
    {
        if(Instance == null)
        {
            Debug.LogError("No Game Pause Manager exists");
            return;
        }

        Instance.AddPause(reason);
    }

    public static void ReleasePause(PauseReason reason)
    {
        if (Instance == null)
        {
            Debug.LogError("No Game Pause Manager exists");
            return;
        }

        Instance.RemovePause(reason);
    }

    public static bool IsPausedFor(PauseReason reason)
    {
        if (Instance == null)
        {
            Debug.LogError("No Game Pause Manager exists");
            return false;
        }

        return Instance.CheckPauseCount(reason);
    }

    private void AddPause(PauseReason reason)
    {
        bool wasPaused = IsPaused;

        if (!pauseCounts.ContainsKey(reason))
        {
            pauseCounts[reason] = 0;
        }

        pauseCounts[reason]++;

        if(!wasPaused && IsPaused)
        {
            Time.timeScale = 0.0f;
            PauseStateChanged?.Invoke(true);
        }
    }

    private void RemovePause(PauseReason reason) 
    {
        if (!pauseCounts.ContainsKey(reason))
        {
            Debug.LogError($"Tried to release pause reason {reason}, but it was not active");
            return;
        }

        pauseCounts[reason]--;

        if(pauseCounts[reason] <= 0)
        {
            pauseCounts.Remove(reason);
        }

        if (!IsPaused)
        {
            Time.timeScale = 1f;
            PauseStateChanged?.Invoke(false);
        }
    }

    private bool CheckPauseCount(PauseReason reason)
    {
        return pauseCounts.ContainsKey(reason) && pauseCounts[reason] > 0;
    }
}
