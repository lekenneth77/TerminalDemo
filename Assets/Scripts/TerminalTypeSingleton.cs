using System;
using UnityEngine;

public class TerminalTypeSingleton : MonoBehaviour
{
    public TerminalType terminalType = TerminalType.Mac;
    public static TerminalTypeSingleton Get;
    public event Action ChangedType;
    void Awake() 
    {
        if (Get) {
            Destroy(gameObject);
            return;
        } else {
            Get = this;
        }
        DontDestroyOnLoad(gameObject);
    }

    public void SetTerminalType(TerminalType type) {
        terminalType = type;
        ChangedType?.Invoke();
    } 
}
