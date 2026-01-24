using UnityEngine;
using UnityEngine.SceneManagement;

public class DevButton : MonoBehaviour
{
    public void ChangeScene(string scene)
    {
        SceneManager.LoadScene(scene);
    }
}
