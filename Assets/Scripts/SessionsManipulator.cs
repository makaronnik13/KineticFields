using com.armatur.common.flags;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;
using UnityEngine.UI;

public class SessionsManipulator : MonoBehaviour
{
    [SerializeField]
    private TMPro.TMP_InputField SessionName;

    [SerializeField]
    private GameObject SessionSelection;

    [SerializeField]
    private Transform SessionsHub;

    [SerializeField]
    private GameObject SessionBtn;

  

 
    private List<string> FileNames = new List<string>();

    private void Start()
    {
        FileNames = Directory.GetFiles(Application.persistentDataPath, "*.kfs").ToList();

        for (int i = 0; i < FileNames.Count; i++)
        {
            FileNames[i] = FileNames[i].Replace(Application.persistentDataPath, "");
            FileNames[i] = FileNames[i].Replace(".kfs", "");
            FileNames[i] = FileNames[i].Remove(0, 1);
        }

        if (FileNames.Count == 0)
        {
            CreateSession();
        }
        else
        {
            Load(FileNames[0]);
        }

        KineticFieldController.Instance.Session.AddListener(SessionChanged);
    }

    private void Update()
    {
        if (Input.GetKey(KeyCode.LeftAlt))
        {
            if (Input.GetKeyDown(KeyCode.S))
            {
                Save(KineticFieldController.Instance.Session.Value.SessionName);
            }
        }

        
    }

   

    private void SessionChanged(KineticSession session)
    {
        SessionName.text = session.SessionName;
    }

    public void SaveClicked()
    {
       
        Save(SessionName.text);
    }



    public void Save(string name)
    {
        Debug.Log("save " + name);

        
        if (KineticFieldController.Instance.Session.Value!=null)
        {
            KineticFieldController.Instance.Session.Value.SessionName = name;
            BinaryFormatter bf = new BinaryFormatter();
            FileStream file = File.Create(Application.persistentDataPath + "/" + KineticFieldController.Instance.Session.Value.SessionName + ".kfs");
            bf.Serialize(file, KineticFieldController.Instance.Session.Value);
            file.Close();

            if (!FileNames.Contains(KineticFieldController.Instance.Session.Value.SessionName))
            {
                FileNames.Add(KineticFieldController.Instance.Session.Value.SessionName);
            }
        }
    }

    public void Load(string sessionName)
    {
        Debug.Log("Load "+sessionName);

       // Save(sessionName);

            BinaryFormatter bf = new BinaryFormatter();
            FileStream file = File.Open(Application.persistentDataPath + "/" + sessionName+".kfs", FileMode.Open);

            KineticFieldController.Instance.Session.SetState((KineticSession)bf.Deserialize(file));

        KineticFieldController.Instance.Session.Value.Init();


            file.Close();
        KineticFieldController.Instance.LoadSession(KineticFieldController.Instance.Session.Value);

        Debug.Log(KineticFieldController.Instance.Session.Value.SessionName+" Loaded");
        SessionSelection.SetActive(false);
    }

    public void Open()
    {
        SessionSelection.SetActive(true);
        foreach (Transform t in SessionsHub)
        {
            Destroy(t.gameObject);
        }

        for (int i = 0; i < FileNames.Count; i++)
        {
            GameObject newBtn = Instantiate(SessionBtn);
            newBtn.transform.SetParent(SessionsHub);
            string fn = FileNames[i];
            newBtn.GetComponent<Button>().onClick.AddListener(()=> { Load(fn);});
            newBtn.GetComponentInChildren<TMPro.TextMeshProUGUI>().text = FileNames[i];
            if (FileNames[i] == KineticFieldController.Instance.Session.Value.SessionName)
            {
                newBtn.GetComponent<Button>().interactable = false;
            }
        }
    }

    public void CreateSession()
    {
        //Save(KineticFieldController.Instance.Session.Name);

        KineticSession newSession = new KineticSession("NewSession_" + FileNames.Count);
        KineticFieldController.Instance.LoadSession(newSession);
        Save(newSession.SessionName);
        Load(newSession.SessionName);
    }
}
