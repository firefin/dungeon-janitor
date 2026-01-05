using UnityEngine;
using System.Collections.Generic;

public class RoomController : MonoBehaviour
{
    public List<Enemy> enemiesInRoom = new List<Enemy>();
    
    private void Start()
    {
        if (enemiesInRoom.Count == 0)
        {
            FindEnemiesInRoom();
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {        
        if (collision.CompareTag("Player"))
        {
            ActivateEnemies();
        }
    }
    
    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            DeactivateEnemies();
        }
    }

    private void ActivateEnemies()
    {        
        foreach (Enemy enemy in enemiesInRoom)
        {
            if (enemy != null)
            {
                enemy.SetRoomActive(true);
            }
        }
    }
    
    private void DeactivateEnemies()
    {
        foreach (Enemy enemy in enemiesInRoom)
        {
            if (enemy != null)
            {
                enemy.SetRoomActive(false);
            }
        }
    }
    
    [ContextMenu("Find Enemies In Room")]
    public void FindEnemiesInRoom()
    {
        enemiesInRoom.Clear();
        
        Enemy[] allEnemies = FindObjectsByType<Enemy>(FindObjectsSortMode.None);
        BoxCollider2D roomCollider = GetComponent<BoxCollider2D>();
        
        if (roomCollider != null)
        {
            // Get the actual world-space bounds
            Bounds bounds = roomCollider.bounds;
           
            foreach (Enemy enemy in allEnemies)
            {
                Vector2 enemyPos2D = new Vector2(enemy.transform.position.x, enemy.transform.position.y);
                
                // Manual bounds check (more reliable)
                bool isInsideX = enemyPos2D.x >= bounds.min.x && enemyPos2D.x <= bounds.max.x;
                bool isInsideY = enemyPos2D.y >= bounds.min.y && enemyPos2D.y <= bounds.max.y;
                bool isInside = isInsideX && isInsideY;
                
                if (isInside)
                {
                    enemiesInRoom.Add(enemy);
                }
            }
        }
    }
}