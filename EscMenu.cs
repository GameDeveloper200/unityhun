using UnityEngine;
using UnityEngine.SceneManagement;

public class EscMenu : MonoBehaviour
{

    public string MenuName = "MenuScene";


    public void ExitMenu()
    {
        SceneManager.LoadScene(MenuName);
    }
}
