using System;
//using System.Collections.Generic;
//using System.Net.Http;
//using System.Net.Http.Headers;
//using System.Text;

public static class Scores
{
    public static string username = "Unknown_Prerelease2";
    public static void UploadScores(string username, string levelName, int score)
    {
    }

    public static void UploadStart(string username, string levelName)
    {
    }

    public static void UploadDeath(string username, string levelName, int time)
    {
    }
    /*

    public static List<KeyValuePair<string, int>> GetScores(string level, int count = 10)
    {
        const string URL = "https://firestore.googleapis.com/v1beta1/projects/wgjweek161/databases/(default)/documents:runQuery";
        string queryJson = "{\"structuredQuery\": {\"from\": [{\"collectionId\": \"scores\",\"allDescendants\": false}],\"where\":{\"fieldFilter\" : {\"field\":{\"fieldPath\":\"Level\"},\"op\":\"EQUAL\",\"value\":{\"stringValue\":\""+level+"\"}}},\"orderBy\":{\"field\":{\"fieldPath\":\"Score\"}},\"limit\": 10},}";

        HttpClient httpClient = new HttpClient();
        httpClient.BaseAddress = new Uri(URL);
        httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
        var content = new StringContent(queryJson, Encoding.UTF8, "application/json");

        HttpResponseMessage response = httpClient.PostAsync(URL, content).Result;

        var json = SimpleJSON.JSON.Parse( response.Content.ReadAsStringAsync().Result );
        var scores = new List<KeyValuePair<string, int>>();
        for(int i = 0; i < json.Count; i++)
        {
            int score = json[i]["document"]["fields"]["Score"]["integerValue"].AsInt;
            string name = json[i]["document"]["fields"]["Name"]["stringValue"];
            scores.Add(new KeyValuePair<string, int>(name, score));
        }

        return scores;
    }

    public static void UploadScores(string username, string levelName, int score)
    {
        const string URL = "https://firestore.googleapis.com/v1beta1/projects/wgjweek161/databases/(default)/documents/scores/";
        string queryJson = "{\"fields\": {\"Score\": {\"integerValue\": \"" + score + "\"},\"Name\": {\"stringValue\": \"" + username + "\"}, \"Level\":{\"stringValue\":\""+ levelName+"\"}}}";
        PostQuery(URL, queryJson);
    }

    public static void UploadStart(string username, string levelName)
    {
        const string URL = "https://firestore.googleapis.com/v1beta1/projects/wgjweek161/databases/(default)/documents/starts/";
        string queryJson = "{\"fields\": {\"Name\": {\"stringValue\": \"" + username + "\"}, \"Level\":{\"stringValue\":\"" + levelName + "\"}}}";
        PostQuery(URL, queryJson);
    }

    public static void UploadDeath(string username, string levelName, int time)
    {
        const string URL = "https://firestore.googleapis.com/v1beta1/projects/wgjweek161/databases/(default)/documents/deaths/";
        string queryJson = "{\"fields\": {\"Time\": {\"integerValue\": \"" + time + "\"},\"Name\": {\"stringValue\": \"" + username + "\"}, \"Level\":{\"stringValue\":\"" + levelName + "\"}}}";
        PostQuery(URL, queryJson);
    }

    public static void PostQuery(string URL,string queryJson)
    {
        HttpClient httpClient = new HttpClient();
        httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
        var content = new StringContent(queryJson, Encoding.UTF8, "application/json");

        httpClient.PostAsync(URL, content);
    }*/
}
