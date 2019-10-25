using System;
using UnityEngine;
using UnityEngine.Experimental.PlayerLoop;

namespace Character
{
    [ExecuteInEditMode]
    public class Character : MonoBehaviour
    {
        [SerializeField] private MeshRenderer mesh;
        [SerializeField] private Rigidbody2D rgd;


        [SerializeField] private Texture text;

        private MaterialPropertyBlock prop;
        private static readonly int MainTex = Shader.PropertyToID("_MainTex");
        [SerializeField] private float maxSpeed;

        private void Start()
        {
            prop = new MaterialPropertyBlock();
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
        }

        public void Move(Vector2 movement)
        {
            if (rgd.velocity.magnitude < maxSpeed)
                rgd.AddForce(movement);
        }
    }
}