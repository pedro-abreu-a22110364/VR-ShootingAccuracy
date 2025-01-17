using System;
using System.IO;
using UnityEngine;
using UnityEngine.Events;

public class Logger : MonoBehaviour
{
    private string filePathLevel;
    private string filePathTarget;

    public string shootingMode;

    public string targetId;
    public string distance;
    public string movement;
    public int shotsFiredPerTarget;
    public string accuracy;
    public float startTime;
    public float endTime;
    public string time;

    private readonly string[] headers = new string[]
    {
        "Shooting Mode", "Shots Fired", "Shots On Target", "Accuracy (%)", "Precision (%)", "Time Elapsed (s)"
    };

    private readonly string[] headersTarget = new string[]
    {
        "Target ID", "Distance", "Movement", "Shooting Mode", "Shots Fired", "Accuracy (%)", "Time Elapsed (s)"
    };

    public GameInfoManager gameInfoManager;

    void Awake()
    {
        // Initialize the log file
        filePathLevel = Path.Combine(Application.persistentDataPath, "levelinfo.csv");
        filePathTarget = Path.Combine(Application.persistentDataPath, "targetinfo.csv");

        // Check if the file already exists
        if (File.Exists(filePathLevel))
        {
            File.Delete(filePathLevel);
        }
        File.WriteAllText(filePathLevel, string.Join(",", headers) + "\n");

        if (File.Exists(filePathTarget))
        {
            File.Delete(filePathTarget);
        }
        File.WriteAllText(filePathTarget, string.Join(",", headersTarget) + "\n");
    }

    public void LogLevel(string shotsFired, string shotsOnTarget, string accuracy, string precision, string timeElapsed)
    {
        string logEntry = $"{shootingMode},{shotsFired},{shotsOnTarget},{accuracy},{precision},{timeElapsed}\n";
        File.AppendAllText(filePathLevel, logEntry);
        Debug.Log($"Logged: {logEntry}");
    }

    public void LogTarget()
    {
        accuracy = (shotsFiredPerTarget > 0 ? ((float)1 / shotsFiredPerTarget) * 100 : 0).ToString("0.0");
        time = (endTime - startTime).ToString("0.0");

        string logEntry = $"{targetId},{distance},{movement},{shootingMode},{shotsFiredPerTarget},{accuracy},{time}\n";
        File.AppendAllText(filePathTarget, logEntry);
        Debug.Log($"Logged: {logEntry}");
    }

    public void LogLevelInfo()
    {
        if (gameInfoManager == null)
        {
            Debug.LogWarning("GameInfoManager is not assigned.");
            return;
        }

        // Get data from GameInfoManager
        int shotsFired = gameInfoManager.shotsFired;
        int shotsOnTarget = gameInfoManager.shotsOnTarget;
        float accuracy = shotsFired > 0 ? ((float)shotsOnTarget / shotsFired) * 100 : 0;
        int bullseyeHits = gameInfoManager.bullseyeHits;
        float precision = shotsOnTarget > 0 ? ((float)bullseyeHits / shotsOnTarget) * 100 : 0;
        float timeElapsed = gameInfoManager.endTime - gameInfoManager.startTime;

        // Log each piece of information
        LogLevel(shotsFired.ToString(), shotsOnTarget.ToString(), accuracy.ToString("0.0"), precision.ToString("0.0"), timeElapsed.ToString("0.0"));
    }
}
