﻿using System;
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

        private void Start()
        {
            prop = new MaterialPropertyBlock();
            coll2D = GetComponent<Collider2D>();
        }

        private void Update()
        {
            UpdateTexture();
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

        public void Attack()
        {
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
            else
                canJump = State.JumpedOnce;

            switch (canJump)
            {
                case State.OnGround:
                    rgd.AddForce(new Vector2(0, jumpForce), ForceMode2D.Force);
                    canJump = State.JumpedOnce;
                    break;
                case State.JumpedOnce:
                    rgd.AddForce(new Vector2(0, jumpForce), ForceMode2D.Force);
                    canJump = State.JumpedTwice;
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
                (Vector2) (coll2D.bounds.center) - new Vector2(0, coll2D.bounds.extents.y),
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