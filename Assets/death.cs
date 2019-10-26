using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class death : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player")) Game.GameManager.ResetLevel();
        if (collision.CompareTag("Enemy"))
            if (collision.transform.childCount != 0)
                Game.GameManager.ResetLevel();
    }
}
