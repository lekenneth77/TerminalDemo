using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum GameMode {
    EASYCD,
    HARDCD,
    REDIRECTORS,
    ALL

}

public class PracticeController : MonoBehaviour
{
    public static PracticeController Get;
    public PracticeFilesys FS;

    void Awake()
    {
        if (Get) {
            Destroy(gameObject);
            return; 
        } else {
            Get = this;
        }


    }

    
}
