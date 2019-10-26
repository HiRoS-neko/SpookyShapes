using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace Game
{
    [UnityEditor.CustomEditor(typeof(LevelManager))]
    public class LevelManagerEditor : Editor
    {
        #region Hack

        Tool LastTool = Tool.None;

        void OnEnable()
        {
            LastTool = Tools.current;
            Tools.current = Tool.None;
        }

        void OnDisable()
        {
            Tools.current = LastTool;
        }

        #endregion

        public override void OnInspectorGUI()
        {
            var levelManager = target as LevelManager;

            EditorGUILayout.LabelField("Patrol Paths");
            if (GUILayout.Button("Add Path"))
            {
                var patrolPoint = new PatrolPoints {points = new List<Vector3>()};

                levelManager.patrolPoints.Add(patrolPoint);
                EditorUtility.SetDirty(target);
            }

            for (int i = 0; i < levelManager.patrolPoints.Count; i++)
            {
                var patrolPath = levelManager.patrolPoints[i];

                var name = EditorGUILayout.TextField("Path: " + (i + 1), patrolPath.name);
                if (name != patrolPath.name)
                {
                    patrolPath.SetName(name);
                    EditorUtility.SetDirty(target);
                }

                if (GUILayout.Button("Add Point"))
                {
                    patrolPath.points.Add(Vector3.zero);
                    EditorUtility.SetDirty(target);
                }

                for (int j = 0; j < patrolPath.points.Count; j++)
                {
                    EditorGUILayout.BeginHorizontal();

                    Vector3 point = patrolPath.points[j];

                    point.x = EditorGUILayout.FloatField(point.x);
                    point.y = EditorGUILayout.FloatField(point.y);
                    point.z = EditorGUILayout.FloatField(point.z);

                    patrolPath.points[j] = point;

                    levelManager.patrolPoints[i] = patrolPath;

                    if (GUILayout.Button("Remove Point"))
                    {
                        levelManager.patrolPoints[i].points.RemoveAt(j);
                        EditorUtility.SetDirty(target);
                    }

                    EditorGUILayout.EndHorizontal();
                }

                if (GUILayout.Button("Remove Path"))
                {
                    levelManager.patrolPoints.RemoveAt(i);
                    EditorUtility.SetDirty(target);
                }
            }
        }

        private void OnSceneGUI()
        {
            Tools.current = Tool.None;

            var levelManager = target as LevelManager;
            for (int i = 0; i < levelManager.patrolPoints.Count; i++)
            {
                for (int j = 0; j < levelManager.patrolPoints[i].points.Count; j++)
                {
                    EditorGUI.BeginChangeCheck();
                    Vector3 newTargetPosition =
                        Handles.PositionHandle(levelManager.patrolPoints[i].points[j], Quaternion.identity);
                    if (EditorGUI.EndChangeCheck())
                    {
                        Undo.RecordObject(levelManager, "Change Look At Target Position");
                        levelManager.patrolPoints[i].points[j] = newTargetPosition;
                        EditorUtility.SetDirty(target);
                    }
                }
            }
        }
    }
}