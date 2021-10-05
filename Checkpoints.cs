using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Checkpoints : MonoBehaviour
{
    public List<GameObject> wall = new List<GameObject>();

    private void Start()
    {
        for (int i = 0; i < this.transform.childCount; i++)
        {
            if (this.transform.GetChild(i).CompareTag("Checkpoint") == true)
            {
                wall.Add(this.transform.GetChild(i).gameObject);
            }
        }
    }
    public int ExtractNumberFromString(string s1)
    {
        return System.Convert.ToInt32(System.Text.RegularExpressions.Regex.Replace(s1, "[^0-9]", ""));
    }
    public string GetNextCheckpointName(string CurrentCheckpointName)
    {
        int CurrentNumber = ExtractNumberFromString(CurrentCheckpointName);
        int NextNumber = (CurrentNumber + 1 > wall.Count - 1) ? 0 : CurrentNumber + 1;
        string NextCheckpointName = string.Format("wall ({0})", NextNumber);
        return NextCheckpointName;
    }
}
