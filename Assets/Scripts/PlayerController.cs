using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;

public class PlayerController : MonoBehaviour
{
    [SerializeField] Transform playerCamera = null;
    [SerializeField] float mouseSensitivity = 3.5f;
    [SerializeField] private const float walkSpeed = 6.0f;
    [SerializeField] private const float runSpeed = 10.0f;
    [SerializeField] float gravity = -13.0f;
    [SerializeField] [Range(0.0f, 0.5f)] float moveSmoothTime = 0.3f;
    [SerializeField] [Range(0.0f, 0.5f)] float mouseSmoothTime = 0.03f;
    [SerializeField] float moveSpeed = walkSpeed;    

    [SerializeField] bool isRunning;
    [SerializeField] bool isCrouched;
    [SerializeField] int runDuration = 1200;

    [SerializeField] Slider slider;

    float cameraPitch = 0.0f;
    float velocityY = 0.0f;

    CharacterController controller = null;

    Vector2 currentDir = Vector2.zero;
    Vector2 currentDirVelocity = Vector2.zero;

    Vector2 currentMouseDelta = Vector2.zero;
    Vector2 currentMouseDeltaVelocity = Vector2.zero;

    [SerializeField] bool lockCursor = true;

    // Start is called before the first frame update
    void Start()
    {
        controller = GetComponent<CharacterController>();
        if (lockCursor)
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }
        isRunning = false;
        isCrouched = false;
    }

    // Update is called once per frame
    void Update()
    {
        UpdateMouseLook();
        UpdateMovement();
    }

    void UpdateMouseLook()
    {
        Vector2 targetMouseDelta = new Vector2(Input.GetAxis("Mouse X"), Input.GetAxis("Mouse Y"));

        currentMouseDelta = Vector2.SmoothDamp(currentMouseDelta, targetMouseDelta, ref currentMouseDeltaVelocity, mouseSmoothTime);

        cameraPitch -= currentMouseDelta.y * mouseSensitivity;
        cameraPitch = Mathf.Clamp(cameraPitch, -80.0f, 80.0f);

        playerCamera.localEulerAngles = Vector3.right * cameraPitch;
        transform.Rotate(Vector3.up * currentMouseDelta.x * mouseSensitivity);

    }

    void UpdateMovement()
    {
        // Crouch
        if ((Input.GetKey(KeyCode.LeftControl)) || Input.GetKey(KeyCode.C))
        {
            isRunning = false;
            moveSpeed = walkSpeed;
            runDuration += 1;
            if (runDuration >= 1200)
            {
                runDuration = 1200;
            }
            isCrouched = true;
        }
        else
        {
                RaycastHit hit;
                if (Physics.Raycast(playerCamera.transform.position, transform.TransformDirection(Vector3.up), out hit, 1f))
                {
                    isCrouched = true;
                }
                else if (!Physics.Raycast(playerCamera.transform.position, transform.TransformDirection(Vector3.up), out hit, 1f))
                {
                    isCrouched = false;
                }

        }

        if (isCrouched)
        {
            transform.localScale = new Vector3(1, 0.5f, 1);
        }
        if (!isCrouched)
        {
            transform.localScale = new Vector3(1, 1, 1);
        }

        if ((Input.GetKey(KeyCode.LeftShift)) && ((!Input.GetKey(KeyCode.LeftControl))) && (!isCrouched)){
            // Set current speed to run if shift is down
                isRunning = true;
                moveSpeed = runSpeed;
                runDuration -= 1;
                if (runDuration <=0)
                {
                    isRunning = false;
                    moveSpeed = walkSpeed;
                    runDuration = 0;
                }
        }
        else
        {
            // Otherwise set current speed to walking speed
            isRunning = false;
            moveSpeed = walkSpeed;
            runDuration += 1;
            if (runDuration >= 1200)
            {
                runDuration = 1200;
            }
        }

        slider.value = runDuration;
        

        Vector2 targetDir = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));
        targetDir.Normalize();

        currentDir = Vector2.SmoothDamp(currentDir, targetDir, ref currentDirVelocity, moveSmoothTime);

        if (controller.isGrounded)
            velocityY = 0.0f;

        velocityY += gravity * Time.deltaTime;

        Vector3 velocity = (transform.forward * currentDir.y + transform.right * currentDir.x) * moveSpeed + Vector3.up * velocityY;

        controller.Move(velocity * Time.deltaTime);
    }

}
