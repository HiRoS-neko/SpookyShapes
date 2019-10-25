using System;
using UnityEngine;
using UnityEngine.Experimental.PlayerLoop;
using UnityEngine.InputSystem;

namespace Character
{
    public class Mask : MonoBehaviour
    {
        [SerializeField] private Character currentlyControlling;

        private Controls controls;

        private void Awake()
        {
            controls = new Controls();
            controls.Enable();

            controls.Player.Jump.performed += JumpOnPerformed;
            controls.Player.Attack.performed += AttackOnPerformed;
        }

        private void AttackOnPerformed(InputAction.CallbackContext obj)
        {
            if (currentlyControlling != null)
            {
                currentlyControlling.Jump();
            }
        }

        private void JumpOnPerformed(InputAction.CallbackContext obj)
        {
            if (currentlyControlling != null)
            {
                currentlyControlling.Attack();
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