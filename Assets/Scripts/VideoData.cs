using UnityEngine;
using System.Collections.Generic;

[System.Serializable]
public class FavoriteData
{
    public List<string> favorites = new List<string>();
}

public class VideoData : MonoBehaviour
{
    private string userKey;

    void Start()
    {
        userKey = PlayerPrefs.GetString("authToken", "default");
    }

    public void AddFavorite(string source)
    {
        FavoriteData data = LoadFavorites();
        if (!data.favorites.Contains(source))
        {
            data.favorites.Add(source);
            SaveFavorites(data);
        }
    }

    public void RemoveFavorite(string source)
    {
        FavoriteData data = LoadFavorites();
        data.favorites.RemoveAll(f => f == source);
        SaveFavorites(data);
    }

    public List<string> GetFavorites()
    {
        return LoadFavorites().favorites;
    }

    private FavoriteData LoadFavorites()
    {
        string json = PlayerPrefs.GetString(userKey + "_favorites", "{}");
        FavoriteData data = JsonUtility.FromJson<FavoriteData>(json);
        if (data == null)
        {
            data = new FavoriteData();
        }
        return data;
    }

    private void SaveFavorites(FavoriteData data)
    {
        string json = JsonUtility.ToJson(data);
        PlayerPrefs.SetString(userKey + "_favorites", json);
        PlayerPrefs.Save();
    }
}