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
        private Character oldHost;
        [SerializeField] private Character currentlyControlling;

        [SerializeField] private Rigidbody2D rgd;

        private Controls controls;
        [SerializeField] private float throwForce;

        [SerializeField] private LineRenderer line;
        private bool thrown;
        [SerializeField] private float movementSpeed;
        [SerializeField] private float maxSpeed;
        [SerializeField] private Collider2D coll2D;
        private void OnEnable()
        {
            controls = new Controls();
            controls.Enable();

            controls.Player.Jump.performed += JumpOnPerformed;
            controls.Player.Attack.performed += AttackOnPerformed;
        }

        private void OnDisable()
        {
            controls.Player.Jump.performed -= JumpOnPerformed;
            controls.Player.Attack.performed -= AttackOnPerformed;

            controls.Disable();
            controls.Dispose();

        }

        private void AimOnPerformed()
        {
            var camera = LevelManager.Instance.camera;
            var playerPos = (Vector2) camera.WorldToScreenPoint(currentlyControlling.transform.position);
            var mousePos = controls.Player.AttackDirection.ReadValue<Vector2>();
            var direction = (mousePos - playerPos).normalized;

            var notCollided = true;
            var reflected = false;
            line.positionCount = 0;
            var newPosition = transform.position;
            var velocity = (throwForce * direction) / rgd.mass;
            do
            {
                var positionCount = line.positionCount;
                positionCount += 1;

                line.positionCount = positionCount;
                line.SetPosition(positionCount - 1, newPosition);


                velocity += Physics2D.gravity * Time.fixedDeltaTime;

                var oldPosition = newPosition;
                newPosition = newPosition + (Vector3) velocity * Time.fixedDeltaTime;
                var dir = velocity.normalized;

                if (line.positionCount > 50 || reflected) notCollided = false;

                RaycastHit2D hit = Physics2D.Raycast(oldPosition, dir,
                    (velocity.magnitude * Time.fixedDeltaTime), 1 << 9);
                if (hit.collider != null)
                {
                    newPosition = hit.point - velocity.normalized * 0.001f;
                    velocity = Vector2.Reflect(dir, hit.normal) * velocity.magnitude;
                    reflected = true;
                }
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
            currentlyControlling.anim.SetFloat("throw");
            oldHost = currentlyControlling;
            //if (oldHost.patrol != null)
            //    oldHost.patrol.enabled = true;
            currentlyControlling = null;
            rgd.simulated = true;
            rgd.bodyType = RigidbodyType2D.Dynamic;
            transform.parent = null;
            transform.localScale = Vector3.one * 0.5f;
            rgd.position = transform.position;
            rgd.velocity = Vector2.zero;
            rgd.rotation = 0;
            transform.localRotation = Quaternion.identity;
            rgd.AddForce(direction * throwForce, ForceMode2D.Impulse);
            thrown = true;
        }

        [SerializeField] private State canJump;
        [SerializeField] private float jumpForce;


        private void JumpOnPerformed(InputAction.CallbackContext obj)
        {
            if (currentlyControlling != null)
            {
                currentlyControlling.Jump();
            }
            else if (!thrown) Jump();
        }

        private void OnCollisionEnter2D(Collision2D other)
        {
            if (thrown)
            {
                thrown = false;
            }
            oldHost = null;
        }

        public void Jump()
        {
            var ground = Physics2D.OverlapCircle(
                (Vector2) (coll2D.bounds.center) - new Vector2(0, coll2D.bounds.extents.y),
                0.001f, 1 << 9);
            if (ground != null)
            {
                canJump = State.OnGround;
            }
            else if (canJump == State.OnGround)
            {
                canJump = State.JumpedOnce;
            }


            switch (canJump)
            {
                case State.OnGround:
                    rgd.AddForce(new Vector2(0, jumpForce), ForceMode2D.Impulse);
                    canJump = State.JumpedOnce;
                    break;
                case State.JumpedOnce:
                    //rgd.AddForce(new Vector2(0, jumpForce), ForceMode2D.Impulse);
                    //canJump = State.JumpedTwice;
                    break;
                case State.JumpedTwice:
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        public void Attach(Character parent)
        {
            if (parent == oldHost) return;
            if (parent.patrol != null)
                parent.patrol.enabled = false;
            rgd.simulated = false;
            rgd.bodyType = RigidbodyType2D.Kinematic;

            thrown = false;
            currentlyControlling = parent;
            transform.parent = parent.transform;
            rgd.velocity = Vector2.zero;

            rgd.rotation = 0;
            transform.localRotation = Quaternion.identity;


            //transform.position = parent.transform.position + parent.facePos;


            var scale = currentlyControlling.transform.localScale;
            transform.localPosition = new Vector3(currentlyControlling.facePos.x / scale.x,
                currentlyControlling.facePos.y / scale.y, -1);

            rgd.position = transform.position;
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
                else if (!thrown)
                {
                    Move(new Vector2(movement, 0));
                }

                if (Mouse.current.rightButton.isPressed && currentlyControlling != null)
                {
                    AimOnPerformed();
                }
                else
                {
                    line.positionCount = 0;
                }

                if (Keyboard.current.rKey.wasPressedThisFrame)
                {
                    GameManager.ResetLevel();
                }
            }
        }

        private void Move(Vector2 movement)
        {
            //if (rgd.velocity.magnitude < maxSpeed)
            var vel = rgd.velocity;

            if (movement.x != 0)
            {
                var localScale = transform.localScale;
                localScale = new Vector3(Mathf.Sign(movement.x) * Mathf.Abs(localScale.x),
                    localScale.y, localScale.z);
                transform.localScale = localScale;
            }

            if (Mathf.Abs((vel + movement * movementSpeed).x) < maxSpeed) //less then max speed, just add it
                vel += movement * movementSpeed;
            else if (movement.x != 0) //it is greater, so set to max speed
                vel.x = maxSpeed * Mathf.Sign(movement.x);

            //if (Math.Abs(movement.x) < 0.001f)
            //vel.x = 0;

            rgd.velocity = vel;
        }
    }
}