using UnityEngine;
using System.IO;

public class LevelManager : MonoBehaviour
{

    public static LevelManager Instance;

    private const string SAVE_FILE_NAME = "game_save.json";
    private const string LEVEL_PREF_KEY = "CurrentLevel";

    public void Awake(){
        if(Instance != null){
            Destroy(gameObject);
        }
        Instance = this;
        currentLevel = PlayerPrefs.GetInt(LEVEL_PREF_KEY, 0);
    }

    public LevelScriptable levelScriptable;
    public int currentLevel = 0;

    public void SaveLevelProgress() {
        PlayerPrefs.SetInt(LEVEL_PREF_KEY, currentLevel);
        PlayerPrefs.Save();
    }

    public LevelData GetCurrentLevelData(){
        return levelScriptable.levelDataArray[currentLevel % levelScriptable.levelDataArray.Length];
    }

    // ─── Save / Load ───────────────────────────────────────────

    private string GetSavePath()
    {
        return Path.Combine(Application.persistentDataPath, SAVE_FILE_NAME);
    }

    public void SaveGame(GameSaveData saveData)
    {
        string json = JsonUtility.ToJson(saveData, true);
        string path = GetSavePath();

        try
        {
            File.WriteAllText(path, json);
            Debug.Log($"[LevelManager] Game saved successfully to: {path}");
        }
        catch (System.Exception e)
        {
            Debug.LogError($"[LevelManager] Failed to save game: {e.Message}");
        }
    }

    public GameSaveData LoadGame()
    {
        string path = GetSavePath();

        if (!File.Exists(path))
        {
            Debug.LogWarning("[LevelManager] No save file found.");
            return null;
        }

        try
        {
            string json = File.ReadAllText(path);
            GameSaveData saveData = JsonUtility.FromJson<GameSaveData>(json);
            Debug.Log("[LevelManager] Game loaded successfully.");
            return saveData;
        }
        catch (System.Exception e)
        {
            Debug.LogError($"[LevelManager] Failed to load game: {e.Message}");
            return null;
        }
    }

    public bool HasSaveFile()
    {
        bool hasSaveFile = File.Exists(GetSavePath());
        Debug.Log("[LevelManager] hasSaveFile: " + hasSaveFile);
        return hasSaveFile;
    }

    public void DeleteSave()
    {
        string path = GetSavePath();
        if (File.Exists(path))
        {
            File.Delete(path);
            Debug.Log("[LevelManager] Save file deleted.");
        }
    }
}
