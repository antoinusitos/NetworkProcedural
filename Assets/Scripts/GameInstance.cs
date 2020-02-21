using UnityEngine;

public class GameInstance : MonoBehaviour
{
    public int mySeed = -1;

    private void Awake()
    {
        DontDestroyOnLoad(this);
    }
}
