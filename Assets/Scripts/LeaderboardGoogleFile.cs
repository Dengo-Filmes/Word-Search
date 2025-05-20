using System.Collections;
using UnityEngine;
using UnityEngine.Networking;

public class LeaderboardGoogleFile : MonoBehaviour
{
    public string webAppUrl = "https://script.google.com/macros/s/AKfycbzowVLzX4otWdF6oTx8cuWRYaTn0biqNTYpZTctF6MJ9j-1fxznlQpPkH1knCc_q-VcCA/exec";
    string data;

    public void UploadLeaderboard(string content)
    {
        StartCoroutine(UploadCoroutine(content));
    }

    IEnumerator UploadCoroutine(string content)
    {
        WWWForm form = new WWWForm();
        form.AddField("data", content);

        using (UnityWebRequest www = UnityWebRequest.Post(webAppUrl, form))
        {
            yield return www.SendWebRequest();

            if (www.result != UnityWebRequest.Result.Success)
                Debug.LogError("Erro: " + www.error);
            else
                Debug.Log("Resposta: " + www.downloadHandler.text);
        }
    }

    public void DownloadLeaderboard()
    {
        StartCoroutine(DownloadCoroutine());
    }

    public string LoadedData()
    {
        if (data != null)
            return data;
        else return null;
    }

    IEnumerator DownloadCoroutine()
    {
        using (UnityWebRequest www = UnityWebRequest.Get(webAppUrl))
        {
            yield return www.SendWebRequest();

            if (www.result != UnityWebRequest.Result.Success)
                Debug.LogError("Erro: " + www.error);
            else
                Debug.Log("Conteúdo do placar:\n" + www.downloadHandler.text);

            data = www.downloadHandler.text;
            GetComponent<DataController>().GenerateGoogleData(data);
        }
    }

    void ResetData()
    {
        StartCoroutine(UploadCoroutine(""));
    }
}
