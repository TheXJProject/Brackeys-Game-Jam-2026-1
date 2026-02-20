using System;
using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

public class StartGame : MonoBehaviour
{
    public AnimationCurve curve;
    [SerializeField] GameObject cameraObject;
    [SerializeField] Vector3 cameraFinalPos;
    public float size;
    public float playerCanStartTime;
    public float cameraMoveTime;
    static float time = 0;
    float timeTransition = 0;
    bool inStartTransistion = true;
    bool startedCameraMove = false;
    float currentCameraSize = 0;
    float donePercentage = 0;
    Vector3 cameraStartPos;
    float startSize;

    public static event Action startGameNow;

    InputController playerControls;
    InputAction interact;

    private void Awake()
    {
        playerControls = new InputController();

        cameraStartPos = cameraObject.transform.position;
        startSize = cameraObject.GetComponent<Camera>().orthographicSize;
    }

    private void OnEnable()
    {
        interact = playerControls.Player.ToggleSleep;
        interact.Enable();

        interact.started += DoAdamsthing;
    }

    private void OnDisable()
    {
        interact.Disable();
        interact.started -= DoAdamsthing;
    }

    private void Start()
    {
        startedCameraMove = false;
    }

    private void DoAdamsthing(InputAction.CallbackContext context)
    {
        if (!inStartTransistion && !startedCameraMove)
        {
            startedCameraMove = true;
            StartCoroutine(MoveCamera());
        }
    }

    void Update()
    {
        if (time > playerCanStartTime)
        {
            // Show press Space to play
            gameObject.GetComponent<TextMeshPro>().enabled = true;
            inStartTransistion = false;
        }
        else
        {
            time += Time.deltaTime;
        }
    }

    IEnumerator MoveCamera()
    {
        if (startedCameraMove)
        {
            // Is the camera in the correct position
            while (!((Vector3.Distance(cameraFinalPos, cameraObject.transform.position) < 0.005f) && (size - currentCameraSize) < 0.005f))
            {
                timeTransition += Time.deltaTime;
                donePercentage = curve.Evaluate(timeTransition / cameraMoveTime);

                // Move camera a bit
                ChangeCameraPositionAndSize();
                yield return null;
            }
        }
        else
        {
            Debug.LogWarning("Error, how heree??");
        }

        cameraObject.transform.position = cameraFinalPos;
        cameraObject.GetComponent<Camera>().orthographicSize = size;
        startGameNow?.Invoke();
        this.enabled = false;
    }

    void ChangeCameraPositionAndSize()
    {
        Camera camera = cameraObject.GetComponent<Camera>();

        currentCameraSize = camera.orthographicSize;
        camera.orthographicSize = Mathf.Lerp(startSize, size, donePercentage);

        cameraObject.transform.position = Vector3.Lerp(cameraStartPos, cameraFinalPos, donePercentage);
    }
}
