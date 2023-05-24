using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.IO;

[System.Serializable]
public class TrainingData
{
    public List<int> behavior;
    public List<List<float>> gestureX;
    public List<List<float>> gestureY;
    public List<List<float>> gestureZ;
}

public class SaveDataManager : Singleton<SaveDataManager>
{
    public void SaveTrainingData(Dictionary<BehaviorType, List<Vector3>> saveData)
    {
        Debug.Log("저장하기");
        TrainingData trainingData = new();

        int i = 0;
        trainingData.behavior = new(saveData.Count);
        trainingData.gestureX = new(saveData.Count);
        trainingData.gestureY = new(saveData.Count);
        trainingData.gestureZ = new(saveData.Count);
        foreach (KeyValuePair<BehaviorType, List<Vector3>> item in saveData)
        {
            trainingData.behavior.Add((int)item.Key);
            trainingData.gestureX.Add(new List<float>(item.Value.Count));
            trainingData.gestureY.Add(new List<float>(item.Value.Count));
            trainingData.gestureZ.Add(new List<float>(item.Value.Count));
            for (int j = 0; j < item.Value.Count; ++j)
            {
                trainingData.gestureX[i].Add(item.Value[j].x);
                trainingData.gestureY[i].Add(item.Value[j].y);
                trainingData.gestureZ[i].Add(item.Value[j].z);
            }

            Debug.Log(trainingData.behavior[i] + "저장");
            ++i;
        }

        var textdata = SaveLoad.ObjectToJson(trainingData);
        var AEStextdata = AES256.Encrypt256(textdata, "aes256=32CharA49AScdg5135=48Fk63");

        SaveLoad.CreateJsonFile(Application.streamingAssetsPath, "TrainingData", AEStextdata);
    }

    public void LoadTrainingData(out Dictionary<BehaviorType, List<Vector3>> loadData)
    {
        try
        {
            Debug.Log("불러오기");
            var data = SaveLoad.LoadJsonFileAES<TrainingData>(Application.streamingAssetsPath, "TrainingData", "aes256=32CharA49AScdg5135=48Fk63");
            loadData = new(data.behavior.Count);
            Debug.Log($"{data.behavior.Count}");

            for (int i = 0; i < data.behavior.Count; ++i)
            {
                List<Vector3> gesture = new();
                Debug.Log($"{data.gestureX.Count}");
                Debug.Log($"{data.gestureX[0].Count}");
                for (int j = 0; j < data.gestureX[i].Count; ++j)
                {
                    gesture.Add(new(data.gestureX[i][j], data.gestureY[i][j], data.gestureZ[i][j]));
                }
                loadData[(BehaviorType)data.behavior[i]] = gesture;
                Debug.Log($"{loadData[(BehaviorType)data.behavior[i]]}");
            }
        }
        catch (IOException e)
        {
            loadData = new();
        }
    }

    public bool FileExist(string fileName)
    {
        return File.Exists(Path.Combine(Application.streamingAssetsPath, $"{fileName}.json"));
    }
}
