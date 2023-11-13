using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TextLoader
{
    private List<string[]> TextMatrix;
    private bool _safety = true;

    public string FirstCell {get; private set;}

    public int TextMatrixLength => TextMatrix.Count;

    public TextLoader (string folder, string fileName, int additiveID=0) // additiveID =0 => можно пустые клетки
    {
        TextMatrix = new List<string[]>();

        if (additiveID == -1)
            _safety = false;
        
        switch (folder)
        {
            case "Localization":
                if (additiveID == 1)
                {
                    Debug.Log("Special english load (additiveID == 1)");
                    GetText("Localization/" + "EN-" + fileName);
                    break;
                }   
                else
                {
                    GetText("Localization/" + PlayerPrefs.GetString("Language", "RU-") + fileName);
                    break;
                }

            default:
                GetText(folder + "/" + fileName);
                break;
        }
    }

    public TextLoader (string path, int row, int col, bool localized = false)
    {
        if (localized)
        {
            FirstCell = new CSV().GetCell("Localization/" + PlayerPrefs.GetString("Language", "RU-") + path, row, col);   
        }

        else
        {
            FirstCell = new CSV().GetCell(path, row, col);   
        }
    }

    public TextLoader (string path, int row, bool localized = false)
    {
        TextMatrix = new List<string[]>();
        if (localized)
        {
            TextMatrix.Add(GetText("Localization/" + PlayerPrefs.GetString("Language", "RU-") + path, row));
            FirstCell = TextMatrix[0][0];
        }

        else
        {
            TextMatrix.Add(GetText(path, row));
            FirstCell = TextMatrix[0][0];
        }
    }

    public void GetText(string path)
    {
        CSV csv = new CSV();
        csv.LoadFile(path);
        TextMatrix = csv.GetMatrix();
        FirstCell = TextMatrix[0][0];
    } 

    public string[] GetText(string path, int row)
    {
        CSV csv = new CSV();
        csv.LoadFile(path);
        List<string[]> tempTextMatrix = csv.GetMatrix();
        return tempTextMatrix[row];
    }

    public string GetCell(int row, int col)
    {
        if (row >= TextMatrix.Count || col >= TextMatrix[row].Length)
            return null;
        
        if(TextMatrix[row][col].Length == 0 && !_safety){
            Debug.Log("Null Cell (not safety load : string cell < 2 chars)");
            return null;
        }
        return TextMatrix[row][col];
    }
    public string[] GetRow(int row)
    {
        if (row >= TextMatrix.Count)
            return null;
        
        return TextMatrix[row];
    }

}
