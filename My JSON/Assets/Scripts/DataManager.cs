using System.Collections.Generic;
using System.Collections;
using System.IO;
using Newtonsoft.Json;
using UnityEngine;
using UnityEngine.UI;
using TMPro;


public class DataManager : MonoBehaviour
{
    public TextMeshProUGUI titleText;
    public TextMeshProUGUI descriptionText;
    public Button buttonPrefab;
    public Transform buttonContainer;
    public GameObject ballPrefab;
    public Transform spawnPoint;
    public Button pauseButton;
    private IEnumerator spawnCoroutine;
    private bool isSpawning = false;
    private bool isPaused = false;
    private Root root;    


    void Start()
    {
        
        LoadData();
        pauseButton.onClick.AddListener(TogglePlay);
        pauseButton.gameObject.SetActive(false);



        // Load JSON file
    
        
    }

        void LoadData()
        {
            TextAsset jsonFile = Resources.Load<TextAsset>("WorkoutInfoJSONAssignment"); 
            root = JsonConvert.DeserializeObject<Root>(jsonFile.text);
           // string jsonString = File.ReadAllText(path);
            //root = JsonUtility.FromJson<Root>(jsonString);
            titleText.text = root.projectName;
            CreateWorkoutButtons();
        }

        [System.Serializable]
            public class Root
            {
                public int numberOfWorkoutBalls;
                public string projectName;
                public List<WorkoutInfo> workoutInfo;
            }

        [System.Serializable]
        public class WorkoutDetail
        {
            public int ballId;
            public float speed;
            public float ballDirection;
        }

        [System.Serializable]
        public class WorkoutInfo
        {
            public int workoutID;
            public string workoutName;
            public string description;
            public string ballType;
            public List<WorkoutDetail> workoutDetails;
        }


    void CreateWorkoutButtons()
    {
        foreach(var button in root.workoutInfo)
        {
            Button newButton = Instantiate(buttonPrefab, buttonContainer);
            newButton.GetComponentInChildren<TextMeshProUGUI>().text = button.workoutName;
            newButton.onClick.AddListener(() => OnWorkOutClicked(button));
        }
    }

    void OnWorkOutClicked(WorkoutInfo button)
    {
        if (isSpawning)
        {
            StopCoroutine(spawnCoroutine);
            isSpawning = false;
        }

        pauseButton.GetComponentInChildren<TextMeshProUGUI>().text = "Play";
        isPaused = false;
        descriptionText.text = button.description;

        spawnCoroutine = SpawnWithDelay(button.workoutDetails);
        pauseButton.gameObject.SetActive(true);
    }


    IEnumerator SpawnWithDelay(List<WorkoutDetail> workoutDetails)
    {
        isSpawning = true;
        foreach (var detail in workoutDetails)
        {
            yield return new WaitWhile(() => isPaused);
            GameObject newBall = Instantiate(ballPrefab);
            newBall.transform.position = spawnPoint.position;
            Rigidbody rigidbody = newBall.GetComponent<Rigidbody>();
            //rigidbody.velocity = new Vector3(0, 0, detail.speed);
            rigidbody.AddForce(transform.forward * detail.speed, ForceMode.Acceleration);

            yield return new WaitForSeconds(2f);
        }
        isSpawning = false;
        pauseButton.GetComponentInChildren<TextMeshProUGUI>().text = "Play";
    }

    void TogglePlay()
    {
        if (isSpawning && !isPaused)
        {
            isPaused = true;
            pauseButton.GetComponentInChildren<TextMeshProUGUI>().text = "Play";
        }
        else if (isSpawning && isPaused)
        {
            isPaused = false;
            pauseButton.GetComponentInChildren<TextMeshProUGUI>().text = "Pause";
        }
        else if (!isSpawning)
        {
            StartCoroutine(spawnCoroutine);
            isSpawning = true;
            pauseButton.GetComponentInChildren<TextMeshProUGUI>().text = "Pause";
        }
    }
    

    }