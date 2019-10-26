﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Patrol : MonoBehaviour
{
    [SerializeField] public int patrolPathIndex;
    private int patrolPathPositionIndex = -1;


    private Character.Character enemy;

    // Update is called once per frame
    void FixedUpdate()
    {
        Vector3 dir;

        if (patrolPathIndex < LevelManager.levelManager.patrolPoints.Count && patrolPathIndex >= 0)
        {
            //no target found, move to closest patrol point
            if (patrolPathPositionIndex == -1)
            {
                patrolPathPositionIndex = LevelManager.levelManager.patrolPoints[patrolPathIndex]
                    .GetClosestPoint(transform.position);
            }
            else
            {
                //get direction to next patrol point
                dir = GetDirection(LevelManager.levelManager.patrolPoints[patrolPathIndex]
                    .points[patrolPathPositionIndex]);
                //check if we are pretty close, and change to the next patrol point
                if (dir.magnitude < 0.5f)
                {
                    //update the patrol point
                    patrolPathPositionIndex = (patrolPathPositionIndex + 1) %
                                              LevelManager.levelManager.patrolPoints[patrolPathIndex].points
                                                  .Count;
                    //get direction to new patrol point
                    dir = GetDirection(LevelManager.levelManager.patrolPoints[patrolPathIndex]
                        .points[patrolPathPositionIndex]);
                }

                //move to direction
                enemy.Move(dir);
            }
        }

    }

    private Vector3 GetDirection(Vector3 newTarget)
    {
      


        var dir = newTarget - transform.position;
        if (dir.magnitude > 1)
            dir.Normalize();

        return dir;
    }


}
