using UnityEngine;
using UnityEngine.SceneManagement;

public class ShopUI : MonoBehaviour
{
    public void OpenProgressionTree()
    {
        SceneManager.LoadScene("ProgressionTreeScene");
    }

    public void LoadShopScene()
    {
        SceneManager.LoadScene("ShopScene");
    }
}
