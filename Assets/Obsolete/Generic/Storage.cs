using System.IO;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;
using System.Threading.Tasks;

public class Storage
{
    private const string copy_prefix = "copy_";

    private string filePath;
    private string filename;
    private BinaryFormatter formatter;

    public Storage(string path){
        filename = path;

        var directory = Application.persistentDataPath + "/saves";
        if (!Directory.Exists(directory))
            Directory.CreateDirectory(directory);
        filePath = directory + $"/{filename}.save";
        Debug.Log(filePath);
        InitBinaryFormatter();
    }

    private void InitBinaryFormatter(){
        formatter = new BinaryFormatter();
        var selector = new SurrogateSelector();

        var v3Surrogate = new Vector3SerializationSurrogate();
        var qSurrogate = new QuaternionSerializationSurrogate();

        selector.AddSurrogate(typeof(Vector3), new StreamingContext(StreamingContextStates.All), v3Surrogate);
        selector.AddSurrogate(typeof(Quaternion), new StreamingContext(StreamingContextStates.All), qSurrogate);

        formatter.SurrogateSelector = selector;
    }

    public object Load(object saveDataByDefault){
        if (!File.Exists(filePath)){
            if (saveDataByDefault != null) Save(saveDataByDefault);
            return saveDataByDefault;
        }

        var file = File.Open(filePath, FileMode.Open);
        var savedData = formatter.Deserialize(file);
        file.Close();
        return savedData;

    }

    public object LoadCopy(object saveDataByDefault, int id = 1)
    {
        var directory = Application.persistentDataPath + "/saves";
        if (!Directory.Exists(directory))
            Directory.CreateDirectory(directory);

        string copyFilePath = directory + $"/{copy_prefix}{id}_{filename}.save";

        if (!File.Exists(copyFilePath)){
            if (saveDataByDefault != null) SaveCopy(saveDataByDefault, id);
            return saveDataByDefault;
        }

        var file = File.Open(copyFilePath, FileMode.Open);
        var savedData = formatter.Deserialize(file);
        file.Close();

        Debug.Log($"<color=magenta>LOAD COPY {copyFilePath}</color>");
        return savedData;
    }

    public void SaveCopy(object saveData, int id = 1)
    {
        var directory = Application.persistentDataPath + "/saves";
        if (!Directory.Exists(directory))
            Directory.CreateDirectory(directory);
        string copyFilePath = directory + $"/{copy_prefix}{id}_{filename}.save";

        var file = File.Create(copyFilePath);
        formatter.Serialize(file, saveData);
        file.Close();

        Debug.Log($"<color=magenta>SAVE COPY {copyFilePath}</color>");
    }

    async public void SaveAsync(object saveData){
        await Task.Run(() => Save(saveData));
        // var file = File.Create(filePath);
        // formatter.Serialize(file, saveData);
        // file.Close();
    }

    public void Save(object saveData){
        // var file = File.Create(filePath);
        // formatter.Serialize(file, saveData);
        // file.Close();
    }
}