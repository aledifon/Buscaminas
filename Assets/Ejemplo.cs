using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ejemplo : MonoBehaviour
{
    //public GameManager gmInstance = GameManager.gm;

    // Start is called before the first frame update
    void Start()
    {
        GameManager.gm.Saludar();
    }
}
