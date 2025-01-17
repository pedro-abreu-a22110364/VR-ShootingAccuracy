using TMPro;
using UnityEngine;

public class GameInfoManager : MonoBehaviour
{
    public TextMeshProUGUI targetsLeftText, shotsFiredText, shotsOnTargetText, accuracyText, precisionText, timeElapsedText;

    public int totalTargets;
    public int shotsOnTarget = 0;
    public int shotsFired = 0;
    public int bullseyeHits = 0;
    public float startTime;
    public float endTime;

    public int targetsLeft;

    private bool isTimerRunning = false;

    void Start()
    {
        // Initialize startTime to the current time when the game starts
        startTime = Time.time;
        isTimerRunning = true;
    }

    void Update()
    {
        UpdateStatsUI();
    }

    void UpdateStatsUI()
    {
        if (isTimerRunning)
        {
            float timeElapsed = Time.time - startTime;
            timeElapsedText.text = $"Tempo: {timeElapsed:0.0} segundos";
        }

        targetsLeft = totalTargets - shotsOnTarget;
        float accuracy = shotsFired > 0 ? ((float)shotsOnTarget / shotsFired) * 100 : 0;
        float precision = shotsOnTarget > 0 ? ((float)bullseyeHits / shotsOnTarget) * 100 : 0;

        targetsLeftText.text = $"Alvos Restantes: {targetsLeft}";
        shotsFiredText.text = $"Disparos Realizados: {shotsFired}";
        shotsOnTargetText.text = $"Disparos no Alvo: {shotsOnTarget}";
        accuracyText.text = $"Precisão: {accuracy:0.0}%";
        precisionText.text = $"Exatidão: {precision:0.0}%";
    }

    public void ResetStats()
    {
        shotsOnTarget = 0;
        shotsFired = 0;
        bullseyeHits = 0;
        startTime = 0;
        endTime = 0;
        UpdateStatsUI();
    }

    public void StopTimer()
    {
        isTimerRunning = false; // Stop the timer if needed
    }
}
