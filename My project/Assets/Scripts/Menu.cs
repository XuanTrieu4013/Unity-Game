using UnityEngine.SceneManagement; 
using UnityEngine; 

public class Menu : MonoBehaviour 
{ 
    public void Play() 
    { 
        SceneManager.LoadScene(1); 
    } 
    
    public void ExitToMenu() 
    { 
        SceneManager.LoadScene(0); 
    } 
    
    public void Exit() 
    { 
        Application.Quit(); 
    } 
}