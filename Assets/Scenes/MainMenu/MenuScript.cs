using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class MenuScript : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField] private bool isDemo = false;

    [Header("Objects")]
    [SerializeField] private GameObject[] objectsToHideInDemo;
    [SerializeField] private Button[] buttonsToDeactivateInDemo;

    void OnValidate()
    {
        DemoChecker();
    }

    void Awake()
    {
        DemoChecker();
    }

    void DemoChecker()
    {
        foreach (GameObject g in objectsToHideInDemo)
        {
            g.SetActive(isDemo);
        }
        foreach (Button b in buttonsToDeactivateInDemo)
        {
            b.enabled = isDemo;
        }
    }
}
