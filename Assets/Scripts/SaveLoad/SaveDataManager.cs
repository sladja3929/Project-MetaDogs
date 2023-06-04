using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.IO;

[Serializable]
public class GestureVectorData
{
    public List<float> x;
    public List<float> y;
    public List<float> z;

    public GestureVectorData (List<float> x, List<float> y, List<float> z)
    {
        this.x = x;
        this.y = y;
        this.z = z;
    }
}

[Serializable]
public class TrainingData
{
    public List<int> behavior;
    public List<GestureVectorData> gesture;
}

public class SaveDataManager : Singleton<SaveDataManager>
{
    public void SaveTrainingData(Dictionary<BehaviorType, List<Vector3>> saveData)
    {
        Debug.Log("저장하기");
        TrainingData trainingData = new();

        int i = 0;
        trainingData.behavior = new(saveData.Count);
        trainingData.gesture = new(saveData.Count);
        foreach (KeyValuePair<BehaviorType, List<Vector3>> item in saveData)
        {
            trainingData.behavior.Add((int)item.Key);

            List<float> x = new(item.Value.Count);
            List<float> y = new(item.Value.Count);
            List<float> z = new(item.Value.Count);
            for (int j = 0; j < item.Value.Count; ++j)
            {
                x.Add(item.Value[j].x);
                y.Add(item.Value[j].y);
                z.Add(item.Value[j].z);
            }

            trainingData.gesture.Add(new GestureVectorData(x, y, z));

            ++i;
        }

        var textdata = SaveLoad.ObjectToJson(trainingData);
        //var AEStextdata = AES256.Encrypt256(textdata, "aes256=32CharA49AScdg5135=48Fk63");
        SaveLoad.CreateJsonFile(Application.streamingAssetsPath, "TrainingData", textdata);
    }

    public void LoadTrainingData(out Dictionary<BehaviorType, List<Vector3>> loadData)
    {
        try
        {
            var data = SaveLoad.LoadJsonFile<TrainingData>(Application.streamingAssetsPath, "TrainingData");
            
            loadData = new();
            for (int i = 0; i < data.behavior.Count; ++i)
            {
                List<Vector3> gesture = new();
                for (int j = 0; j < data.gesture[i].x.Count; ++j)
                {
                    gesture.Add(new(data.gesture[i].x[j], data.gesture[i].y[j], data.gesture[i].z[j]));
                }
                loadData[(BehaviorType)data.behavior[i]] = gesture;
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
