using System;
using UnityEngine;
using UnityEngine.Experimental.PlayerLoop;

namespace Character
{
    public enum State
    {
        OnGround,
        JumpedOnce,
        JumpedTwice
    }

    [ExecuteInEditMode]
    public class Character : MonoBehaviour
    {
        [SerializeField] private MeshRenderer mesh;
        [SerializeField] private Rigidbody2D rgd;


        [SerializeField] private Texture text;

        private MaterialPropertyBlock prop;
        private static readonly int MainTex = Shader.PropertyToID("_MainTex");
        [SerializeField] private float maxSpeed;
        [SerializeField] private float movementSpeed;
        [SerializeField] private float jumpForce;

        [SerializeField] private State canJump;
        private Collider2D coll2D;

        Animator anim;
        float moving = 0;
        public Vector3 facePos;

        private void Start()
        {
            prop = new MaterialPropertyBlock();
            coll2D = GetComponent<Collider2D>();
            anim = GetComponent<Animator>();
        }

        private void OnDrawGizmos()
        {
            Gizmos.color = Color.red;
            Gizmos.DrawSphere(facePos + transform.position, 0.1f);
        }

        private void Update()
        {
            UpdateTexture();
            anim.SetFloat("move", moving);
        }


        private void UpdateTexture()
        {
            if (prop == null) prop = new MaterialPropertyBlock();
            mesh.GetPropertyBlock(prop);
            prop.SetTexture(MainTex, text);
            mesh.SetPropertyBlock(prop);
        }

        private void Reset()
        {
            mesh = GetComponent<MeshRenderer>();
        }


        public void Jump()
        {
            var ground = Physics2D.OverlapCircle(
                (Vector2)(coll2D.bounds.center) - new Vector2(0, coll2D.bounds.extents.y),
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

        private void OnCollisionEnter2D(Collision2D other)
        {
            var ground = Physics2D.OverlapCircle(
                (Vector2)(coll2D.bounds.center) - new Vector2(0, coll2D.bounds.extents.y),
                0.001f, 1 << 9);
            if (ground != null)
            {
                canJump = State.OnGround;
            }
        }

        public void Move(Vector2 movement)
        {
            //if (rgd.velocity.magnitude < maxSpeed)
            var vel = rgd.velocity;
            moving = Mathf.Abs( movement.x);

            if (movement.x != 0)
            {
                var localScale = transform.localScale;
                localScale = new Vector3(Mathf.Sign(movement.x) * Mathf.Abs(localScale.x),
                    localScale.y, localScale.z);
                transform.localScale = localScale;
            }

            if (Mathf.Abs((vel + movement * movementSpeed).x) < maxSpeed) //less then max speed, just add it
                vel += movement * movementSpeed;
            else //it is greater, so set to max speed
                vel.x = maxSpeed * Mathf.Sign(movement.x);

            if (Math.Abs(movement.x) < 0.001f)
                vel.x = 0;

            rgd.velocity = vel;
        }
    }
}