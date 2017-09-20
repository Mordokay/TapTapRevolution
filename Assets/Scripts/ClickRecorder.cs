using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System;
using UnityEngine.Networking;
using GooglePlayGames;

public class ClickRecorder : MonoBehaviour {

    public List<float> myTime = new List<float>();
    StreamWriter writer;
    string tableUID;

    int count = 0;

    public string addTapTime = "http://web.ist.utl.pt/ist165821/addTapTime.php";
    public string getTable = "http://web.ist.utl.pt/ist165821/displayTable.php";
    public string createTable = "http://web.ist.utl.pt/ist165821/createTable.php";
    public string getAllTables = "http://web.ist.utl.pt/ist165821/GetAllTables.php";

    IEnumerator PostScores(string tableName, float time)
    {
        string post_url = addTapTime + "?time=" + WWW.EscapeURL(time.ToString()) + "&name=" + WWW.EscapeURL(tableName);

        // Post the URL to the site and create a download object to get the result.
        WWW hs_post = new WWW(post_url);
        yield return hs_post; // Wait until the download is done

        Debug.Log(hs_post.text);

        /*
        if (hs_post.error != null)
        {
            print("There was an error posting the high score: " + hs_post.error);
        }
        */
    }

    IEnumerator GetAllTables()
    {
        string post_url = getAllTables;

        // Post the URL to the site and create a download object to get the result.
        WWW hs_post = new WWW(post_url);
        yield return hs_post; // Wait until the download is done

        //Debug.Log(hs_post.text);

        string auxResult = hs_post.text.Substring( 0, hs_post.text.Length - 1);
        string[] strArr = auxResult.Split(' ');
        for(int i = 0; i < strArr.Length; i++)
        {
            Debug.Log(i + " : " + strArr[i]);
        }

        /*
        if (hs_post.error != null)
        {
            print("There was an error posting the high score: " + hs_post.error);
        }
        */
    }

    IEnumerator CreateTable(string name)
    {
        string post_url = createTable + "?name=" + WWW.EscapeURL(name.ToString());

        // Post the URL to the site and create a download object to get the result.
        WWW hs_post = new WWW(post_url);
        yield return hs_post; // Wait until the download is done

        Debug.Log(hs_post.text);
        /*
        if (hs_post.error != null)
        {
            print("There was an error posting the high score: " + hs_post.error);
        }
        */
    }

    // Get the scores from the MySQL DB to display in a GUIText.
    // remember to use StartCoroutine when calling this function!
    IEnumerator GetTimes()
    {
        WWW hs_get = new WWW(getTable);
        yield return hs_get;

        Debug.Log(hs_get.text);

        /*
        if (hs_get.error != null)
        {
            print("There was an error getting the high score: " + hs_get.error);
        }
        else
        {
            Debug.Log(hs_get.text); // this is a GUIText that will display the scores in game.
        }
        */
    }

    private void Start()
    {
        //Guid uid = Guid.NewGuid();
        //tableUID = uid.ToString();
        //StartStream(uid.ToString());
        //StartCoroutine(CreateTable(tableUID));
        AddNewTable();
    }

    public void AddNewTable()
    {
        //Only creates tables if user is logged in
        if (PlayGamesPlatform.Instance.localUser.authenticated)
        {
            Guid uid = Guid.NewGuid();
            tableUID = uid.ToString();
            tableUID = tableUID.Replace("-", "");
            tableUID = PlayGamesPlatform.Instance.localUser.userName + "_" + tableUID;
            Debug.Log("Unique ID: " + tableUID);
            StartCoroutine(CreateTable(tableUID));
            StartCoroutine(GetAllTables());
        }

#if UNITY_EDITOR
        Guid uid2 = Guid.NewGuid();
        tableUID = uid2.ToString();
        tableUID = tableUID.Replace("-", "");
        tableUID = PlayGamesPlatform.Instance.localUser.userName + "_" + tableUID;
        Debug.Log("Unique ID: " + tableUID);
        StartCoroutine(CreateTable(tableUID));
        StartCoroutine(GetAllTables());
#endif
    }

    void StartStream(string name)
    {
        writer = new StreamWriter("Assets/Resources/" + name + ".txt", true);
    }

    public void AddLine(float time)
    {
        if (count < 200)
        {
            count++;
            //myTime.Add(time);
            //string myText = time.ToString();
            //writer.WriteLine(myText);

            StartCoroutine(PostScores(tableUID, time));
        }
        else
        {
            //CloseStream();
        }
    }

    public void CloseStream()
    {
        if (writer.BaseStream != null)
        {
            writer.Close();
        }
    }

    void ReadString(string name)
    {
        string path = "Assets/Resources/" + name + ".txt";

        StreamReader reader = new StreamReader(path);

        string line;
        while ((line = reader.ReadLine()) != null)
        {
            Debug.Log(line);
        }
        CloseStream();
    }
}