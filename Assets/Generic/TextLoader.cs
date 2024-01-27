using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TextLoader
{
    private List<string[]> TextMatrix;
    private bool _safety = true;

    public string FirstCell {get; private set;}

    public int TextMatrixLength => TextMatrix.Count;

    public TextLoader (string folder, string filename, bool applyEmptyCells = true, bool localized = true) // additiveID =0 => можно пустые клетки
    {
        TextMatrix = new List<string[]>();

        if (!applyEmptyCells)
            _safety = false;

        if (localized)
        {
            GetText("Localization/" + PlayerPrefs.GetString("Language", "RU") + $"/{folder}/{filename}");
        } else
        {
            GetText(folder + "/" + filename);
        }
    }

    public TextLoader (string uifilename, int row, int col)
    {
        FirstCell = new CSV().GetCell("Localization/" + PlayerPrefs.GetString("Language", "RU") + "/UI/" + uifilename, row, col);   
    }

    public TextLoader (string uifilename, int row)
    {
        TextMatrix = new List<string[]>();
        TextMatrix.Add(GetText("Localization/" + PlayerPrefs.GetString("Language", "RU") + "/UI/" + uifilename, row));
        FirstCell = TextMatrix[0][0];
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
