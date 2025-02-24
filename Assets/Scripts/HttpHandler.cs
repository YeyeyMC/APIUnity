using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.Networking;
using UnityEngine.UI;
using TMPro;

public class HttpHandler : MonoBehaviour
{
    [SerializeField] private RawImage[] picture;
    [SerializeField] private string rickAPI = "https://rickandmortyapi.com/api/character";
    [SerializeField] private string myAPI = "https://my-json-server.typicode.com/YeyeyMC/APIUnity/users";
    [SerializeField] private TMP_Text[] deck;
    [SerializeField] private TMP_Dropdown dropdown;
    [SerializeField] private TMP_Text user;

    void Start()
    {
    }

    IEnumerator GetUser(int dropValue)
    {
        UnityWebRequest www = UnityWebRequest.Get(myAPI);
        yield return www.SendWebRequest();

        if (www.result == UnityWebRequest.Result.ConnectionError)
        {
            Debug.Log(www.error);
        }
        else
        {
            string originalJson = www.downloadHandler.text;

            string wrappedJson = "{\"users\":" + originalJson + "}";

            Debug.Log(www.responseCode);
            FakeUsersList fakeUsers = JsonUtility.FromJson<FakeUsersList>(wrappedJson);

            if (dropValue >= 0 && dropValue < fakeUsers.users.Length)
            {
                FakeUser selectedUser = fakeUsers.users[dropValue];

                user.text = selectedUser.username;
                Debug.Log("Usuario seleccionado: " + selectedUser.username);

                for (int i = 0; i < selectedUser.deck.Length; i++)
                {
                    int cardId = selectedUser.deck[i];
                    StartCoroutine(GetCharacter(cardId, i));
                }
            }
            else
            {
                Debug.Log("Valor del dropdown fuera de rango.");
            }
        }
    }


    /*IEnumerator GetText()
    {
        UnityWebRequest www = UnityWebRequest.Get(rickAPI);
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
                    Debug.Log($" {personaje.id}: {personaje.name}");
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
    }*/

    IEnumerator GetCharacter(int id, int index)
    {
        UnityWebRequest www = UnityWebRequest.Get(rickAPI+"/"+id);
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

                StartCoroutine(GetImage(personaje.image, index));
                StartCoroutine(SetName(personaje.name, index));
            }
            else
            {
                string mensaje = "status: " + www.responseCode;
                mensaje += "\nError: " + www.error;
                Debug.Log(www.error);
            }
        }
    }



    IEnumerator GetImage(string imageUrl, int index)
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
            picture[index].texture = texture;
        }
    }

    IEnumerator SetName(string name, int index)
    {
        deck[index].text = name;
        yield return null;
    }

    public void SendRequest()
    {
        int dropValue = dropdown.value;
        Debug.Log(dropValue);

        StartCoroutine (GetUser(dropValue));
    }
}

[System.Serializable]
public class Character
{
    public int id;
    public string name;
    public string image;
}

[System.Serializable]
public class CharactersList
{
    public Character[] results;
}

[System.Serializable]
public class FakeUser
{
    public int id;
    public string username;
    public int[] deck;
}

[System.Serializable]
public class FakeUsersList
{
    public FakeUser[] users;
}