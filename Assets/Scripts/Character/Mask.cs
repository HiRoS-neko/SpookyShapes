using System;
using Game;
using TMPro;
using UnityEngine;
using UnityEngine.Experimental.PlayerLoop;
using UnityEngine.InputSystem;

namespace Character
{
    public class Mask : MonoBehaviour
    {
        [SerializeField] private Character currentlyControlling;

        [SerializeField] private Rigidbody2D rgd;

        private Controls controls;
        [SerializeField] private float throwForce;

        [SerializeField] private LineRenderer line;

        private void Awake()
        {
            controls = new Controls();
            controls.Enable();

            controls.Player.Jump.performed += JumpOnPerformed;
            controls.Player.Attack.performed += AttackOnPerformed;

            controls.Player.Aim.performed += AimOnPerformed;
        }

        private void AimOnPerformed(InputAction.CallbackContext obj)
        {
            var camera = LevelManager.Instance.camera;
            var playerPos = (Vector2) camera.WorldToScreenPoint(currentlyControlling.transform.position);
            var mousePos = controls.Player.AttackDirection.ReadValue<Vector2>();
            var direction = (mousePos - playerPos).normalized;

            var notCollided = true;
            line.positionCount = 0;
            var newPosition = transform.position;
            var velocity = throwForce * direction;
            do
            {
                var positionCount = line.positionCount;
                positionCount += 1;

                line.positionCount = positionCount;
                line.SetPosition(positionCount - 1, newPosition);

                if (line.positionCount > 50) notCollided = false;
                velocity += Physics2D.gravity * Time.fixedDeltaTime;
                newPosition = newPosition + (Vector3) velocity * Time.fixedDeltaTime;
            } while (notCollided);
        }

        private void AttackOnPerformed(InputAction.CallbackContext obj)
        {
            if (currentlyControlling != null)
            {
                var camera = LevelManager.Instance.camera;
                var playerPos = (Vector2) camera.WorldToScreenPoint(currentlyControlling.transform.position);
                var mousePos = controls.Player.AttackDirection.ReadValue<Vector2>();
                var direction = (mousePos - playerPos).normalized;

                Throw(direction);
            }
        }

        private void Throw(Vector2 direction)
        {
            transform.parent = null;
            rgd.AddForce(direction * throwForce, ForceMode2D.Impulse);
        }

        private void JumpOnPerformed(InputAction.CallbackContext obj)
        {
            if (currentlyControlling != null)
            {
                currentlyControlling.Jump();
            }
        }

        private void FixedUpdate()
        {
            if (controls != null)
            {
                var movement = controls.Player.Movement.ReadValue<float>();
                if (currentlyControlling != null)
                {
                    currentlyControlling.Move(new Vector2(movement, 0));
                }
            }
        }
    }
}