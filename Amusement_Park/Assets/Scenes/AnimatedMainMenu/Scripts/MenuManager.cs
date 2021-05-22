using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

public class MenuManager : MonoBehaviour 
{
    public void GoTo(int scene)
    {
        SceneManager.LoadScene(scene);
    }

    public void Quit()
    {
        Application.Quit();
    }
    public void ToggleVisible(Animator anim)
    {
        if (anim.GetBool("isDisplayed"))
        {
            anim.SetBool("isDisplayed", false);
        }
        else
        {
            anim.SetBool("isDisplayed", true);
        }
    }
	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
