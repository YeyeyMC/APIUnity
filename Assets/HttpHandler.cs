using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.Networking;
using UnityEngine.UI;

public class HttpHandler : MonoBehaviour
{
    [SerializeField] private RawImage picture;
    [SerializeField] private string url = "https://rickandmortyapi.com/api/character";

    void Start()
    {

    }

    IEnumerator GetText()
    {
        UnityWebRequest www = UnityWebRequest.Get(url);
        yield return www.SendWebRequest();

        if (www.result == UnityWebRequest.Result.ConnectionError)
        {
            Debug.Log(www.error);
        }
        else
        {
            if(www.responseCode == 200)
            {
                
                CharactersList personajes = JsonUtility.FromJson<CharactersList>(www.downloadHandler.text);
                foreach (var personaje in personajes.results)
                {
                    Debug.Log($" {personaje.id}: {personaje.name} es un {personaje.species}");
                }

                Debug.Log(www.downloadHandler.text);
            }
            else
            {
                string mensaje = "status: " + www.responseCode;
                mensaje += "\nError: " + www.error;
                Debug.Log(www.error);
            }
        }
    }

    IEnumerator GetCharacter(int id)
    {
        UnityWebRequest www = UnityWebRequest.Get(url+"/"+id);
        yield return www.SendWebRequest();

        if (www.result == UnityWebRequest.Result.ConnectionError)
        {
            Debug.Log(www.error);
        }
        else
        {
            if (www.responseCode == 200)
            {
                Character personaje = JsonUtility.FromJson<Character>(www.downloadHandler.text);

                Debug.Log(www.downloadHandler.text);

                StartCoroutine(GetImage(personaje.image));
            }
            else
            {
                string mensaje = "status: " + www.responseCode;
                mensaje += "\nError: " + www.error;
                Debug.Log(www.error);
            }
        }
    }

    IEnumerator GetImage(string imageUrl)
    {
        UnityWebRequest request = UnityWebRequestTexture.GetTexture(imageUrl);
        yield return request.SendWebRequest();
        if (request.result == UnityWebRequest.Result.ConnectionError)
        {
            Debug.Log(request.error);
        }
        else
        {
            var texture = ((DownloadHandlerTexture)request.downloadHandler).texture;
            picture.texture = texture;
        }
    }

    public void SendRequest()
    {
        StartCoroutine (GetCharacter(6));
    }
}

[System.Serializable]
public class Character
{
    public int id;
    public string name;
    public string species;
    public string image;
}

[System.Serializable]
public class CharactersList
{
    public Character[] results;
}
