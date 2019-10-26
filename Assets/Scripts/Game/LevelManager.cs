using System;
using System.Collections.Generic;
using UnityEngine;

namespace Game
{
    public class LevelManager : MonoBehaviour
    {
        public static LevelManager Instance;

        public List<PatrolPoints> patrolPoints;

        [SerializeField] public Camera camera;

        private void Awake()
        {
            Instance = this;
        }
    }

    [Serializable]
    public struct PatrolPoints
    {
        public string name;
        public List<Vector3> points;

        public int GetClosestPoint(Vector3 transformPosition)
        {
            //todo find closest point to transform position
            throw new NotImplementedException();
        }
    }
}