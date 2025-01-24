using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    // Static var which contains the single instance of the Singleton
    public static GameManager gm;
    
    void Awake()
    {   
        // Saves the current instance of GameManager to the static var.
        if (gm == null)
        {
            gm = this;
            DontDestroyOnLoad(gameObject); // Avoids the GO will be destroyed when the Scene changes
        }            
        // If does already exists an instance of the GameManager then we'll destroy
        // this GO
        else
            Destroy(gameObject);
    }
    // GameManager methods
    public void Saludar()
    {
        Debug.Log("¡Hola desde el GameManager Singleton!");
    }

    // Private constructor in order to avoid create new instances through 'new'
    private GameManager() { }
}
