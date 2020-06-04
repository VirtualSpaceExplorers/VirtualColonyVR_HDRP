using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Assets.Src.Controls.Command;
using UnityEngine;

namespace Assets.Src.Controls
{
    public class PcControlScheme : IControlScheme
    {
        private float _moveSpeed;
        private float _sensitivity;
        private float _xRotation = 0f;

        private Dictionary<KeyCode, Vector3> _movementControls = new Dictionary<KeyCode, Vector3>();

        public PcControlScheme(float moveSpeed, float mouseSensitivity)
        {
            _moveSpeed = moveSpeed;
            _sensitivity = mouseSensitivity;
        }

        public Action StartupActions()
        {
            return () => { Cursor.lockState = CursorLockMode.Locked; Debug.Log("Started Pc Controls"); };
        }

        public IMovementCommand MovePlayer(GameObject playerObject, Rigidbody rigidbody)
        {
            var verticalInput = Input.GetAxis("Vertical");
            var horizontalInput = Input.GetAxis("Horizontal");

            var currentSpeedVector = new Vector3(horizontalInput * _moveSpeed, 0, verticalInput * _moveSpeed);
            currentSpeedVector *= Time.deltaTime;
            currentSpeedVector = playerObject.transform.TransformDirection(currentSpeedVector);

            return new KeyboardMoveCommand
            {
                Acceleration = currentSpeedVector,
                AffectedGameObject = playerObject,
                RigidBody = rigidbody
            };
        }

        public IPlayerRotationCommand RotatePlayer(GameObject gameObject, Transform cameraTransform)
        {
            var mouseInput = new Vector2(Input.GetAxisRaw("Mouse X") * _sensitivity * Time.deltaTime, 
                                        Input.GetAxisRaw("Mouse Y") * _sensitivity * Time.deltaTime);

            _xRotation -= mouseInput.y;
            _xRotation = Mathf.Clamp(_xRotation, -90f, 90f);

            var cameraRotation = Quaternion.Euler(_xRotation, 0f, 0f);

            return new MouseRotateCommand
            {
                AffectedCamera = cameraTransform,
                AffectedGameObject = gameObject,
                CurrentMouseLook = mouseInput,
                CameraRotation = cameraRotation
            };
        }

        public void SetMove(KeyCode keyCode, Vector3 direction)
        {
            _movementControls.Add(keyCode, direction * _moveSpeed);
        }

    }
}
