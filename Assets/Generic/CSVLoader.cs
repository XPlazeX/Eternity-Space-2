using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

public class CSV
{
    private List<string[]> MatrixData;

    public List<string[]> GetMatrix() => MatrixData;

    public CSV() {MatrixData = new List<string[]>();}

    public void LoadFile(string path)
    {
        MatrixData.Clear();
        var dataset = Resources.Load<TextAsset>(path);
        var splitDataset = dataset.text.Split(new char[] {'\n'});
        for (int i = 0; i < splitDataset.Length; i++)
        {
            MatrixData.Add(splitDataset[i].Split(';'));
        }
    }

    public string GetCell(string path, int row, int col)
    {
        var dataset = Resources.Load<TextAsset>(path);
        var splitDataset = dataset.text.Split(new char[] {'\n'});
        for (int i = 0; i < splitDataset.Length; i++)
        {
            if (i == row)
            {
                string[] splitRows = splitDataset[i].Split(';');

                for (int j = 0; j < splitRows.Length; j++)
                {
                    if (j == col)
                        return splitRows[j];
                }
            }
        }

        return "Not Loaded";
    }
}
