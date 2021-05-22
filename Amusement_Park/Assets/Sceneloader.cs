using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Sceneloader : MonoBehaviour
{
    public void NewGame()
    {
        SceneManager.LoadScene("Fresh");
    }
    public void LoadGame()
    {
        SceneManager.LoadScene("Main");
    }
    public void QuitGame()
    {
        Application.Quit();
        Debug.Log("Quit!");
    }

}
