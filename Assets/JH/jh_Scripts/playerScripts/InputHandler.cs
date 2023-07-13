using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace JH
{
    public class InputHandler : MonoBehaviour
    {
        public float horizontal;
        public float vertical;
        public float moveAmount;
        public float mouseX;
        public float mouseY;
        public float rotSpeed = 20f;

        PlayerControlls inputActions;
        CameraHandler cameraHandler;

        Vector2 movementInput;
        Vector2 cameraInput;

        private void Start()
        {
            cameraHandler = CameraHandler.instance;
        }

        private void Update()
        {
            float delta = Time.deltaTime;

            float mx = Input.GetAxis("Mouse X");
            float my = Input.GetAxis("Mouse Y");

            mouseX += my * rotSpeed * Time.deltaTime;
            mouseY += mx * rotSpeed * Time.deltaTime;

            mouseX = Mathf.Clamp(mouseX, -75, 75);

            transform.eulerAngles = new Vector3(-mouseX, mouseY, 0);

            if (cameraHandler != null)
            {
                print("camera");
                cameraHandler.FollowTarget(delta);
                cameraHandler.HandleCameraRotation(delta, mouseX, mouseY);
            }
        }
        public void OnEnable()
        {
            if(inputActions == null)
            {
                inputActions = new PlayerControlls();
                inputActions.PlayerMovement.Movement.performed += inputActions => movementInput = inputActions.ReadValue<Vector2>();
                inputActions.PlayerMovement.Camera.performed += i => cameraInput = i.ReadValue<Vector2>();
            }

            inputActions.Enable();
        }

        private void OnDisable()
        {
            inputActions.Disable();
        }

        public void TickInput(float delta)
        {
            MoveInput(delta);
        }

        private void MoveInput(float delta)
        {
            horizontal = movementInput.x;
            vertical = movementInput.y;
            moveAmount = Mathf.Clamp01(Mathf.Abs(horizontal) + Mathf.Abs(vertical));
            mouseX = cameraInput.x;
            mouseY = cameraInput.y;
        }
    }
}
