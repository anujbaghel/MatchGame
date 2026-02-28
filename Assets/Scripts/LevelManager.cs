using UnityEngine;

public class LevelManager : MonoBehaviour
{

    public static LevelManager Instance;

    public void Awake(){
        if(Instance != null){
            Destroy(gameObject);
        }
        Instance = this;
    }

    public LevelScriptable levelScriptable;
    public int currentLevel = 0;

    public LevelData GetCurrentLevelData(){
        return levelScriptable.levelDataArray[currentLevel];
    }
}
