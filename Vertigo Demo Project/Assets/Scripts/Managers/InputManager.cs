using UnityEngine;



public class InputManager : MonoBehaviour
{
    public static InputManager instance;

    private void Awake()
    {
        instance = this;
    }


 
}
