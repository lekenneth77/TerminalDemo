using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Directory : MonoBehaviour
{
    [SerializeField] private GameObject _fileExplorerScreen;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    public GameObject GetFileExplorerScreen() {
        return _fileExplorerScreen;
    }
}
