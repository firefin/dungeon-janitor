using UnityEngine;

public class MissionSign : MonoBehaviour
{
    public MissionMenu menu;

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            menu.ShowMenu();
        }
    }

    void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            menu.HideMenu();
        }
    }
}
