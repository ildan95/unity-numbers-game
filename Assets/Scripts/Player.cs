using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

[Serializable]
public class Player : MonoBehaviour {
    int gold=1000;
    int lives;
    int maxLives = 5;
    float leftSecondsToRestoreLive=0;
    public float liveRestoreSeconds = 20 * 60;

    public List<PartData> partsData = new List<PartData>();
    public int Gold { get; private set; }
    public int Lives { get; private set; }

    private void Start()
    {

        lives = maxLives;
        StartCoroutine(LoadData());
    }

    void SetData(Player p)
    {
        gold = p.gold;
    }

    IEnumerator LoadData()
    {
        UnityWebRequest www = UnityWebRequest.Get("http://127.0.0.1:8000");
        yield return www.SendWebRequest();

        if (www.isNetworkError || www.isHttpError)
        {
            Debug.Log(www.error);
        }
        else
        {
            // Show results as text
            Debug.Log(www.downloadHandler.text);
            Player p = JsonUtility.FromJson<Player>(www.downloadHandler.text);


        }
    }

    IEnumerator LiveRestoringProcess()
    {
        while (lives < maxLives)
        {
            if (--leftSecondsToRestoreLive == 0)
            {
                AddLives(1);
                if (lives < maxLives)
                    leftSecondsToRestoreLive = liveRestoreSeconds;
            }
            yield return new WaitForSeconds(1);
        }
    }
    public void AddGold(int value)
    {
        gold += value;
    }

    public bool SubGold(int value)
    {
        if (gold < value)
            return false;
        else
        {
            gold -= value;
            return true;
        }
    }

    public void LooseLive()
    {
        if (!CoroutineExtension.GroupProcessing("LiveRestoringProcess"))
            LiveRestoringProcess().ParallelCoroutinesGroup(this, "LiveRestoringProcess");
        lives = Mathf.Max(lives - 1, 0);
    }

    public void AddLives(int count)
    {
        lives = Mathf.Min(lives + count, maxLives);
    }
}


[Serializable]
public class PartData
{
    public int partNum;
    int stars;
    bool closed;
    public List<LevelData> levelsData = new List<LevelData>();
}

[Serializable]
public class LevelData
{
    public int partNum;
    int levelNum;
    int stars;
    int maxScore;
}