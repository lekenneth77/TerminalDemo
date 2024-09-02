using UnityEngine;

public class CH2Event : MonoBehaviour
{
    [SerializeField] private FileSystem fs;
    // Start is called before the first frame update
    void Start()
    {
        fs.ForceCD("\\Users\\baggu\\Documents");
    }

}
