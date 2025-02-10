using com.armatur.common.flags;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;
using UnityEngine.UI;

public class SessionsManipulator : Singleton<SessionsManipulator>
{
    [SerializeField]
    private TMPro.TMP_InputField SessionNameInput, TrackNameInput;

    [SerializeField]
    private TMPro.TMP_Dropdown TracksDropdown, SessionsDropdown;

    [SerializeField]
    private TMPro.TMP_InputField SessionName;

    [SerializeField]
    private GameObject SessionSelection;

    [SerializeField]
    private Transform SessionsHub;

    [SerializeField]
    private GameObject SessionBtn;


    private List<KineticSession> Sessions = new List<KineticSession>();

    public List<TrackLib> TrackLibs = new List<TrackLib>();

    private void Start()
    {
        List<string> SessionNames = Directory.GetFiles(Application.persistentDataPath, "*.kfs").ToList();

        List<string> TrackLibsNames = Directory.GetFiles(Application.persistentDataPath, "*.kft").ToList();

        for (int i = 0; i < SessionNames.Count; i++)
        {
            SessionNames[i] = SessionNames[i].Replace(Application.persistentDataPath, "");
            SessionNames[i] = SessionNames[i].Replace(".kfs", "");
            SessionNames[i] = SessionNames[i].Remove(0, 1);
        }

        for (int i = 0; i < TrackLibsNames.Count; i++)
        {
            TrackLibsNames[i] = TrackLibsNames[i].Replace(Application.persistentDataPath, "");
            TrackLibsNames[i] = TrackLibsNames[i].Replace(".kft", "");
            TrackLibsNames[i] = TrackLibsNames[i].Remove(0, 1);
        }

        foreach (string s in TrackLibsNames)
        {
            BinaryFormatter bf = new BinaryFormatter();
            FileStream file = File.Open(Application.persistentDataPath + "/" + s+ ".kft", FileMode.Open);

            TrackLibs.Add((TrackLib)bf.Deserialize(file));

            file.Close();
        }

        foreach (string s in SessionNames)
        {
            BinaryFormatter bf = new BinaryFormatter();
            FileStream file = File.Open(Application.persistentDataPath + "/" + s + ".kfs", FileMode.Open);
            Sessions.Add((KineticSession)bf.Deserialize(file));
            file.Close();
        }

        KineticFieldController.Instance.ActivePoint.AddListener(ActivePointChanged);

        UpdateTrackLibsDropdown();
        UpdateSessionsDropdown();

        SessionsDropdown.onValueChanged.AddListener(SessionDropdownChanged);
        TracksDropdown.onValueChanged.AddListener(TrackLibDropdownChanged);

        TrackNameInput.onEndEdit.AddListener(TrackNameChanged);
        SessionNameInput.onEndEdit.AddListener(SessionNameChanged);

        KineticFieldController.Instance.Session.AddListener(SessionChanged);
        TracksManager.Instance.CurrentLib.AddListener(TrackLibChanged);

        if (PlayerPrefs.HasKey("last_session"))
        {
            KineticSession session = Sessions.FirstOrDefault(s => s.SessionName == PlayerPrefs.GetString("last_session"));
            if (session!=null) 
            {
                SessionsDropdown.SetValueWithoutNotify(Sessions.IndexOf(session)+1);
                Load(session);
            } 
        }

        if (PlayerPrefs.HasKey("last_track_lib"))
        {
            Debug.Log("load track lib");

            Debug.Log(PlayerPrefs.GetString("last_track_lib"));
            TrackLib trackLib = TrackLibs.FirstOrDefault(s => s.Name == PlayerPrefs.GetString("last_track_lib"));
            if (trackLib != null)
            {
                Debug.Log(trackLib.Name);
                Debug.Log(TrackLibs.IndexOf(trackLib) + 1);
                TracksManager.Instance.CurrentLib.SetState(trackLib);
                TracksDropdown.SetValueWithoutNotify(TrackLibs.IndexOf(trackLib)+1);
            }
        }


    }

    private void TrackLibChanged(TrackLib lib)
    {
        TrackNameInput.gameObject.SetActive(lib!=null);
        if (lib!=null)
        {
            TrackNameInput.SetTextWithoutNotify(lib.Name);
        }
        UpdateTrackLibsDropdown();
        Autosave();
    }

    private void SessionChanged(KineticSession session)
    {
        SessionNameInput.gameObject.SetActive(session != null);

        if (session != null)
        {
            SessionNameInput.SetTextWithoutNotify(session.SessionName);
        }
    }

    private void SessionNameChanged(string name)
    {
        string delitingSessionName = KineticFieldController.Instance.Session.Value.SessionName;
        if (name == delitingSessionName)
        {
            return;
        }
        KineticFieldController.Instance.Session.Value.SessionName = name;
        Autosave();
        UpdateSessionsDropdown();
        SessionsDropdown.SetValueWithoutNotify(Sessions.IndexOf(KineticFieldController.Instance.Session.Value)+1);
        SessionsDropdown.RefreshShownValue();
 
        File.Delete(Application.persistentDataPath + "/" + delitingSessionName + ".kfs");
    }

    private void TrackNameChanged(string name)
    {
        string delitingTrackName = TracksManager.Instance.CurrentLib.Value.Name;
        if (name == delitingTrackName)
        {
            return;
        }
        TracksManager.Instance.CurrentLib.Value.Name = name;
        Autosave();
        UpdateTrackLibsDropdown();
        TracksDropdown.SetValueWithoutNotify(TrackLibs.IndexOf(TracksManager.Instance.CurrentLib.Value) + 1);
        TracksDropdown.RefreshShownValue();

        File.Delete(Application.persistentDataPath + "/" + delitingTrackName + ".kft");
       
    }

  /*
    private void CurrentLibChanged(TrackLib lib)
    {
        if (!TrackLibs.Contains(lib) && lib!=null)
        {
            TrackLibs.Add(lib);
            UpdateTrackLibsDropdown();
            TracksDropdown.SetValueWithoutNotify(TrackLibs.IndexOf(lib));
            PlayerPrefs.SetString("last_track_lib", lib.Name);
        }
        else
        {
            PlayerPrefs.SetString("last_track_lib", string.Empty);
        }
       
    }*/

    private void UpdateSessionsDropdown()
    {
        SessionsDropdown.options.Clear();
        SessionsDropdown.options.Add(new TMPro.TMP_Dropdown.OptionData("-"));
        foreach (KineticSession s in Sessions)
        {
            SessionsDropdown.options.Add(new TMPro.TMP_Dropdown.OptionData(s.SessionName));
        }
  
    }

    private void SessionDropdownChanged(int v)
    {
        Autosave();
        if (v<=0)
        {
            KineticFieldController.Instance.LoadSession(null);
        }
        else
        {
            Load(Sessions[v-1]);
        }
    }

    private void TrackLibDropdownChanged(int v)
    {
        Autosave();
        SaveTrackLib(TracksManager.Instance.CurrentLib.Value);
        if (v<=0)
        {
            TracksManager.Instance.CurrentLib.SetState(null);
        }
        else
        {
            TracksManager.Instance.CurrentLib.SetState(TrackLibs[v - 1]);
        }
        
    }

    private void UpdateTrackLibsDropdown()
    {
        TracksDropdown.options.Clear();

        TracksDropdown.options.Add(new TMPro.TMP_Dropdown.OptionData("-"));

        foreach (TrackLib tl in TrackLibs)
        {
            TracksDropdown.options.Add(new TMPro.TMP_Dropdown.OptionData(tl.Name));
        }
    }

    public void ActivePointChanged(KineticPoint obj)
    {
        SaveToFile(KineticFieldController.Instance.Session.Value);
    }


    private void SaveToFile(KineticSession session)
    {
        if (session!=null)
        {
            if (session.SessionName == string.Empty)
            {
                return;
            }
            BinaryFormatter bf = new BinaryFormatter();
            FileStream file = File.Create(Application.persistentDataPath + "/" + session.SessionName + ".kfs");
            bf.Serialize(file, session);
            file.Close(); 
        }
    }

    public void Load(KineticSession session)
    {
        KineticFieldController.Instance.ActivePoint.SetState(null);
        KineticFieldController.Instance.LoadSession(session);

        SessionSelection.SetActive(false);

        PresetsLerper.Instance.SetState(false);
        PresetsLerper.Instance.SetState(true);

        UpdateSessionsDropdown();
    }

    public void Open()
    {
        SessionSelection.SetActive(true);
        foreach (Transform t in SessionsHub)
        {
            Destroy(t.gameObject);
        }

        for (int i = 0; i < Sessions.Count; i++)
        {
            GameObject newBtn = Instantiate(SessionBtn);
            newBtn.transform.SetParent(SessionsHub);
            int sId = i;
            newBtn.GetComponent<Button>().onClick.AddListener(()=> { Load(Sessions[sId]);});
            newBtn.GetComponentInChildren<TMPro.TextMeshProUGUI>().text = Sessions[i].SessionName;
            if (Sessions[i] == KineticFieldController.Instance.Session.Value)
            {
                newBtn.GetComponent<Button>().interactable = false;
            }
        }
    }

    public void SaveTrackLib(TrackLib lib)
    {
        if (lib == null)
        {
            return;
        }
        if (lib.Name == string.Empty)
        {
            return;
        }

        BinaryFormatter bf = new BinaryFormatter();
        FileStream file = File.Create(Application.persistentDataPath + "/" + lib.Name + ".kft");
        bf.Serialize(file, lib);
        file.Close();

        if (!TrackLibs.Contains(lib))
        {
            TrackLibs.Add(lib);
            UpdateTrackLibsDropdown();
        }
    }


    //new iplement

    public void NewSession()
    {
        Autosave();
        KineticSession newSession = new KineticSession("NewSession_" + Sessions.Count);
        Sessions.Add(newSession);
        SaveToFile(newSession);
        Load(newSession);
        UpdateSessionsDropdown();

        SessionsDropdown.SetValueWithoutNotify(Sessions.IndexOf(newSession) + 1);
    }

    public void NewTrackLib()
    {
        Autosave();
        TrackLib newLib = new TrackLib("NewTrackLib_" + (TrackLibs.Count + 1));
        TrackLibs.Add(newLib);
        TracksManager.Instance.CurrentLib.SetState(newLib);
        UpdateTrackLibsDropdown();

        TracksDropdown.SetValueWithoutNotify(TrackLibs.IndexOf(newLib)+1);
    }

    public void Autosave()
    {
        if (TracksManager.Instance.CurrentLib.Value!=null)
        {
            SaveTrackLib(TracksManager.Instance.CurrentLib.Value);
            Debug.Log(TracksManager.Instance.CurrentLib.Value.Name);
            PlayerPrefs.SetString("last_track_lib", TracksManager.Instance.CurrentLib.Value.Name);
        }

        if (KineticFieldController.Instance.Session.Value!=null)
        {
            SaveToFile(KineticFieldController.Instance.Session.Value);
            PlayerPrefs.SetString("last_session", KineticFieldController.Instance.Session.Value.SessionName);
        }

        PlayerPrefs.Save();
    }

    private void OnApplicationQuit()
    {
        Autosave();
    }

    public void Exit()
    {
        Application.Quit();
    }
}
