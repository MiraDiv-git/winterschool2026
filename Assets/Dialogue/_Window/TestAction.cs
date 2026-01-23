using UnityEngine;

public class TestAction : MonoBehaviour
{
    public string message = "The dialogue was ended";
    public void PrintMessage()
    {
        Debug.LogWarning(message);
    }
}
