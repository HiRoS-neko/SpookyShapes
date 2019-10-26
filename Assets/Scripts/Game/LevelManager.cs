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

            var closest = 0;
            for (var i = 1; i < points.Count; i++)
            {
                if (Vector3.Distance(points[i], transformPosition) < Vector3.Distance(points[closest], transformPosition))
                {
                    closest = i;
                }
            }

            return closest;

        }

        public void SetName(string name)
        {
            this.name = name;
        }
    }
}