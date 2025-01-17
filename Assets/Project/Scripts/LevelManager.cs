using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using UnityEngine.XR;
using UnityEngine.XR.Interaction.Toolkit.Inputs.Readers;
using UnityEngine.XR.Interaction.Toolkit.Interactors;

public class LevelManager : MonoBehaviour
{
    [SerializeField] private Logger logger;

    public GameObject[] targets;
    public GameObject menuCanvas;
    public GameObject gameinfoCanvas;
    public GameObject practiceCanvas;
    public GameObject aimCanvas;
    public GameObject postTaskCanvas;
    public TextMeshProUGUI textMeshPro1;
    public TextMeshProUGUI textMeshPro2;
    public GameObject outroCanvas;
    public GameObject handManager;
    public GameObject gun;
    public Button practiceButton;
    public Button playButton;
    public GameInfoManager gameInfoManager;

    [SerializeField] private List<InputActionReference> inputActionReferences;
    [SerializeField] private NearFarInteractor nearFarInteractor;
    [SerializeField] private List<GameObject> canvasShooting;

    private GameObject[] practiceTargets;

    private int currentLevel = 0;
    public int playCyclesCompleted = 0;
    private int inputActionIndex;
    private int maxPlayCycles = 3;
    private bool isPracticeMode = false;
    private bool isPlayMode = false;
    private bool canSkipAim = false;
    private bool canSkipPostTask = false;
    private int targetId = 1;

    void Start()
    {
        practiceTargets = new GameObject[]
        {
            InstantiateTarget(targets[0]),
            InstantiateTarget(targets[3]),
            InstantiateTarget(targets[0]),
            InstantiateTarget(targets[3]),
            InstantiateTarget(targets[0]),
            InstantiateTarget(targets[3]),
            InstantiateTarget(targets[0])
        };

        for (int i = 1; i < targets.Length; i++)
        {
            targets[i].SetActive(false);
        }

        practiceCanvas.SetActive(false);
        postTaskCanvas.SetActive(false);
        outroCanvas.SetActive(false);
        menuCanvas.SetActive(true);
    }

    private GameObject InstantiateTarget(GameObject originalTarget)
    {
        GameObject clone = Instantiate(originalTarget, originalTarget.transform.position, originalTarget.transform.rotation);
        clone.SetActive(false);
        return clone;
    }

    public void StartPracticeMode()
    {
        isPracticeMode = true;
        currentLevel = 0;
        gameInfoManager.totalTargets = practiceTargets.Length;
        gameInfoManager.shotsOnTarget = 0;
        AssignShootingModePractice(currentLevel);
        //nearFarInteractor.activateInput.inputActionReferencePerformed = inputActionReferences[playCyclesCompleted];
        //canvasShooting[playCyclesCompleted].SetActive(true);

        foreach (var target in targets)
        {
            target.SetActive(false);
        }

        if (practiceTargets.Length > 0)
        {
            practiceTargets[currentLevel].SetActive(true);
        }

        menuCanvas.SetActive(false);
        handManager.SetActive(true);

        gameInfoManager.startTime = Time.time;
    }

    public void StartPlayMode()
    {
        isPlayMode = true;
        gun.SetActive(true);
        currentLevel = 0;
        gameInfoManager.totalTargets = targets.Length;
        gameInfoManager.shotsOnTarget = 0;
        playCyclesCompleted = 0;
        nearFarInteractor.activateInput.inputActionReferencePerformed = inputActionReferences[playCyclesCompleted];
        canvasShooting[playCyclesCompleted].SetActive(true);
        gameinfoCanvas.SetActive(true);

        targets[currentLevel].SetActive(true);

        logger.startTime = Time.time;
        gameInfoManager.startTime = Time.time;

        menuCanvas.SetActive(false);
        handManager.SetActive(true); //???
    }

    public void OnTargetDestroyed()
    {
        if (isPracticeMode)
        {
            if (currentLevel + 1 < practiceTargets.Length)
            {
                practiceTargets[currentLevel].SetActive(false);
                currentLevel++;

                if (currentLevel == 6) // After the sixth target
                {
                    ShowAimCanvas();
                }
                else
                {
                    AssignShootingModePractice(currentLevel);
                    practiceTargets[currentLevel].SetActive(true);
                }
            }
            else
            {
                EndPracticeMode();
            }
        }
        else
        {
            logger.targetId = targetId.ToString();
            targetId++;

            if (currentLevel + 1 == 4 || currentLevel + 1 == 5 || currentLevel + 1 == 6)
            {
                logger.movement = "Moving";
            } 
            else
            {
                logger.movement = "Stationary";
            }

            logger.shootingMode = GetShootingMode(false);

            logger.endTime = Time.time;

            logger.LogTarget();

            logger.shotsFiredPerTarget = 0;

            if (currentLevel + 1 < targets.Length)
            {
                targets[currentLevel].SetActive(false);
                currentLevel++;
                targets[currentLevel].SetActive(true);

                logger.startTime = Time.time;
            }
            else
            {
                EndPlayCycle();
            }
        }
    }

    private void AssignShootingModePractice(int targetIndex)
    {
        inputActionIndex = 0;

        if (targetIndex == 0 || targetIndex == 1 || targetIndex == 6)
            inputActionIndex = 0;
        else if (targetIndex == 2 || targetIndex == 3)
            inputActionIndex = 1;
        else if (targetIndex == 4 || targetIndex == 5)
            inputActionIndex = 2;

        nearFarInteractor.activateInput.inputActionReferencePerformed = inputActionReferences[inputActionIndex];

        foreach (var canvas in canvasShooting)
        {
            canvas.SetActive(false);
        }

        if (targetIndex > 0)
        {
            canvasShooting[inputActionIndex].SetActive(true);
            gameinfoCanvas.SetActive(true);
        }
    }

    private void ShowAimCanvas()
    {
        aimCanvas.SetActive(true);
        canSkipAim = true; // Reuse the flag to enable skipping

        gameinfoCanvas.SetActive(false);
        canvasShooting[inputActionIndex].SetActive(false);
        gun.SetActive(false);
    }

    private void SkipAimCanvas()
    {
        aimCanvas.SetActive(false);
        canSkipAim = false;

        gameinfoCanvas.SetActive(true);
        canvasShooting[inputActionIndex].SetActive(true);
        gun.SetActive(true);

        // Continue with the next actions after skipping the canvas
        AssignShootingModePractice(currentLevel);
        practiceTargets[currentLevel].SetActive(true);
    }

    private void EndPracticeMode()
    {
        gameInfoManager.endTime = Time.time;

        isPracticeMode = false;
        gun.SetActive(false);
        canvasShooting[playCyclesCompleted].SetActive(false);
        gameinfoCanvas.SetActive(false);
        gameInfoManager.ResetStats();

        for (int i = 0; i < practiceTargets.Length; i++)
        {
            practiceTargets[i].SetActive(false);
        }

        practiceButton.interactable = false;
        playButton.interactable = true;

        if (practiceCanvas != null)
        {
            practiceCanvas.SetActive(true);
            Invoke(nameof(HidePracticeCanvas), 3f);
        }

    }

    private void EndPlayCycle()
    {
        gun.SetActive(false);
        gameInfoManager.endTime = Time.time;
        playCyclesCompleted++;

        if (playCyclesCompleted < maxPlayCycles)
        {
            nearFarInteractor.activateInput.inputActionReferencePerformed = inputActionReferences[playCyclesCompleted];
            canvasShooting[playCyclesCompleted - 1].SetActive(false);
            gameinfoCanvas.SetActive(false);
            ShowPostTaskCanvas();
        }
        else
        {
            canvasShooting[playCyclesCompleted - 1].SetActive(false);
            gameinfoCanvas.SetActive(false);
            EndPlayMode();
            ShowPostTaskCanvas();
        }
    }

    private void ShowPostTaskCanvas()
    {
        postTaskCanvas.SetActive(true);
        textMeshPro1.gameObject.SetActive(true);
        textMeshPro2.gameObject.SetActive(false);

        Invoke(nameof(ActivateSecondText), 15f);
        Invoke(nameof(EnableSkip), 15f);
    }

    private void ActivateSecondText()
    {
        textMeshPro2.gameObject.SetActive(true);
    }

    private void EnableSkip()
    {
        canSkipPostTask = true;
    }

    private void StartNextPlayCycle()
    {
        string shootingMode = GetShootingMode(true);
        logger.shootingMode = shootingMode;

        logger.LogLevelInfo();
        gameInfoManager.ResetStats();

        postTaskCanvas.SetActive(false);
        gun.SetActive(true);
        canvasShooting[playCyclesCompleted].SetActive(true);
        gameinfoCanvas.SetActive(true);
        currentLevel = 0;
        gameInfoManager.totalTargets = targets.Length;
        gameInfoManager.shotsOnTarget = 0;

        foreach (var target in targets)
        {
            Target targetScript = target.GetComponent<Target>();

            if (targetScript != null)
            {
                targetScript.ResetTarget();
            }

            target.SetActive(false);
        }

        targets[currentLevel].SetActive(true);

        logger.startTime = Time.time;
        gameInfoManager.startTime = Time.time;
    }

    private void EndPlayMode()
    {
        isPlayMode = false;
        gun.SetActive(false);

        for (int i = 0; i < targets.Length; i++)
        {
            targets[i].SetActive(false);
        }
    }

    private void HidePracticeCanvas()
    {
        if (practiceCanvas != null)
        {
            practiceCanvas.SetActive(false);
        }

        menuCanvas.SetActive(true);
    }

    private void Update()
    {
        if (canSkipPostTask && IsPrimaryButtonPressed())
        {
            SkipPostTaskCanvas();
        }

        if (canSkipAim && IsPrimaryButtonPressed())
        {
            SkipAimCanvas();
        }
    }

    private bool IsPrimaryButtonPressed()
    {
        UnityEngine.XR.InputDevice rightHandDevice = InputDevices.GetDeviceAtXRNode(XRNode.RightHand);

        if (rightHandDevice.isValid)
        {
            if (rightHandDevice.TryGetFeatureValue(UnityEngine.XR.CommonUsages.primaryButton, out bool primaryButtonValue))
            {
                return primaryButtonValue;
            }
        }
        return false;
    }

    private void SkipPostTaskCanvas()
    {
        postTaskCanvas.SetActive(false);
        canSkipPostTask = false;

        if (playCyclesCompleted >= maxPlayCycles)
        {
            ShowOutroCanvas();
        }
        else
        {
            StartNextPlayCycle();
        }
    }

    private void ShowOutroCanvas()
    {
        outroCanvas.SetActive(true);

        string shootingMode = GetShootingMode(true);
        logger.shootingMode = shootingMode;

        logger.LogLevelInfo();
        gameInfoManager.ResetStats();
    }

    private string GetShootingMode(bool after)
    {
        if (after)
        {
            switch (playCyclesCompleted)
            {
                case 1:
                    return "Press button in shooting hand";  // Customize based on your requirement
                case 2:
                    return "Hold and release button in shooting hand";  // Customize based on your requirement
                case 3:
                    return "Press button in non-shooting hand";  // Customize based on your requirement
                default:
                    return "Unknown shooting mode";
            }
        }
        else
        {
            switch (playCyclesCompleted)
            {
                case 0:
                    return "Press button in shooting hand";  // Customize based on your requirement
                case 1:
                    return "Hold and release button in shooting hand";  // Customize based on your requirement
                case 2:
                    return "Press button in non-shooting hand";  // Customize based on your requirement
                default:
                    return "Unknown shooting mode";
            }
        }
    }

}
