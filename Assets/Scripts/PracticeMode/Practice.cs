using UnityEngine;

public class Practice : MonoBehaviour
{
    public static Practice Get;
    public PracticeTerminal Terminal;
    public PracticeFilesys Filesys;
    public FilesystemView View;
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
