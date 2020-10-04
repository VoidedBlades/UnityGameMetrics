using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System;

#region Value listing
// Serializable multivalue List/Dictionary classes
[Serializable]
public class VisualizeDictionary
{
    public int ID;
    public List<Location> Locations = new List<Location>();
    public Color Color;
    public VisualizeDictionary(int _ID, Location _Loc, Color32 _color = new Color32())
    {
        this.ID = _ID;
        this.Locations.Add(_Loc);
        
        this.Color = new Color(UnityEngine.Random.Range(1f, 255f) / 255f, UnityEngine.Random.Range(1f, 255f)/ 255f, _color.b/255f);
    }

}
[Serializable]
public class Location
{
    public Vector3 Position;
    public Vector3 LookAt;

    public Location(Vector3 _Pos, Vector3 _LA)
    {
        this.Position = _Pos;
        this.LookAt = _LA;
    }
}
#endregion
public class Metrics : MonoBehaviour
{
    #region Values & Debug
    [Header("Metric Settings")]
    [SerializeField, Range(1,100)] private int ShowMetricID;
    [SerializeField] private bool All;
    [SerializeField, Tooltip("Shows wire spheres on the saved positions")] private bool PlayerSpheres;
    [SerializeField, Tooltip("Shows where the player was looking at at the current time")] private bool LookatLine;
    [SerializeField, Tooltip("Clears either the selected metric file or all saved metric files depending on the \"All\" Boolean")] private bool Clear;

    [Header("Debug")]
    [SerializeField] private List<VisualizeDictionary> storage;
    private List<VisualizeDictionary> PreloadStorage = new List<VisualizeDictionary>();
    #endregion
    #region FrontEnd
    /// <summary>
    /// Saves the temporairy Storage of the VisualizeDictionary.ID
    /// </summary>
    /// <param name="_ID"></param>
    public void Save(int _ID)
    {
        foreach(VisualizeDictionary dir in storage) if(dir.ID == _ID) WriteToFile(_ID);
        Debug.LogWarning("Saved");
    }

    /// <summary>
    /// Stores the given values temporairly before saving. 
    /// </summary>
    /// <param name="_New"></param>
    /// <param name="_ID"></param>
    public void Store(Location _New, int _ID)
    {
        foreach (VisualizeDictionary Section in storage)
        {
            if (Section.ID == _ID)
            {
                Section.Locations.Add(_New);
                return;
            }
        }
        Color32 c = new Color32((byte)(255 / _ID), (byte)(255 / _ID), (byte)(255 / _ID), 1);
        storage.Add(new VisualizeDictionary(_ID, _New, c));

    }
    #endregion
    #region BackEnd
    private void Start()
    {
        storage = new List<VisualizeDictionary>();
        PreloadStorage = new List<VisualizeDictionary>();
    }
    /// <summary>
    /// Load in the metrics of the specified ID
    /// </summary>
    /// <param name="_ID"></param>
    private void Load(int _ID)
    {
        VisualizeDictionary Data = new VisualizeDictionary(0, new Location(new Vector3(), new Vector3()));
        string json = ReadFromFile(_ID);
        JsonUtility.FromJsonOverwrite(json, Data);
        if (Data != new VisualizeDictionary(0, new Location(new Vector3(), new Vector3())))
        {
            foreach(VisualizeDictionary data in PreloadStorage)
                if (data.ID == _ID) return;
            if (Data.ID != 0)
            {
                PreloadStorage.Add(Data);
            }
        }
    }
    /// <summary>
    /// Write the storage with the bound ID to a json file
    /// </summary>
    /// <param name="ID"></param>
    private void WriteToFile(int ID)
    {
        string filepath = GetFilePath(ID.ToString()+"_Save.txt");
        string json = "";

        foreach(VisualizeDictionary stor in storage)
            if (stor.ID == ID)
                json = JsonUtility.ToJson(stor);
        if (json == "") return;

        FileStream fileStream = new FileStream(filepath, FileMode.Create);
        using (StreamWriter writer = new StreamWriter(fileStream))
        {
            writer.Write(json);
        };
    }
    /// <summary>
    /// read the file of the specified ID
    /// </summary>
    /// <param name="ID"></param>
    /// <returns></returns>
    private string ReadFromFile(int ID)
    {
        string path = GetFilePath(ID.ToString()+"_Save.txt");
        if (File.Exists(path))
        {
            using (StreamReader reader = new StreamReader(path))
            {
                string json = reader.ReadToEnd();
                return json;
            }
        }
        else
        {
            return "";
        }
    }
    /// <summary>
    /// Clear the specified files or all of them.
    /// </summary>
    private void ClearFiles()
    {
        if (All)
        {
            for (int i = 1; i <= 100; i++)
            {
                string filepath = GetFilePath(i.ToString() + "_Save.txt");
                if (File.Exists(filepath))
                {
                    File.Delete(filepath);
                }
            }
        }
        else
        {
            string filepath = GetFilePath(ShowMetricID.ToString() + "_Save.txt");
            if (File.Exists(filepath))
            {
                File.Delete(filepath);
            }
        }
    }

    private string GetFilePath(string fileName)
    {
        return Application.persistentDataPath + "/" + fileName;
    }
    #endregion
    #region Visuals
    private void OnDrawGizmos()
    {
        if (Application.isEditor && !Application.isPlaying)
        {
            if (Clear)
            {
                Clear = false;
                ClearFiles();
            }
            PreloadStorage = new List<VisualizeDictionary>();
            if (All)
            {
                for(int i = 1; i <= 100; i++)
                {
                    Load(i);
                }
            }
            else
            {
                Load(ShowMetricID);
            }
            foreach (VisualizeDictionary storage in PreloadStorage)
            {
                Vector3 previous = new Vector3();
                foreach (Location loc in storage.Locations)
                {
                    if (previous != new Vector3())
                    {
                        Gizmos.color = storage.Color;
                        if(PlayerSpheres)
                            Gizmos.DrawWireSphere(loc.Position, .3f); // location sphere
                        Gizmos.DrawLine(previous, loc.Position); // line from previous to the next position
                        Gizmos.color = Color.red;
                        if(LookatLine)
                            Gizmos.DrawLine(loc.Position, loc.LookAt); // lookat line
                        Gizmos.color = storage.Color;
                        if (PlayerSpheres && LookatLine)
                            Gizmos.DrawWireSphere(loc.LookAt, .1f); // lookat sphere
                    }
                    else
                    {
                        Gizmos.color = storage.Color;
                        if(PlayerSpheres)
                            Gizmos.DrawWireSphere(loc.Position, .3f);
                        Gizmos.color = Color.red;
                        if(LookatLine)
                            Gizmos.DrawLine(loc.Position, loc.LookAt);
                        Gizmos.color = storage.Color;
                        if (PlayerSpheres && LookatLine)
                            Gizmos.DrawWireSphere(loc.LookAt, .1f);

                        Gizmos.color = Color.green;
                        Gizmos.DrawLine(loc.Position, loc.Position + new Vector3(0,5,0)); // start position


                    }
                    previous = loc.Position;
                }
            }
        }
    }
    #endregion
}
