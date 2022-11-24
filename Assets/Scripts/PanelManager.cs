using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PanelManager : MonoBehaviour
{
    [Header("References")]
    public Animator animator;
    public Transform impulsePoint;
    public PunchManager pm;
    public GameObject headTransform;

    [Header("Panel")]
    public Panel[] panels;
    public int panelIndex = 0;
    private GameObject panelModel;
    public int currentHealth;
    public int hitsToBreakOff;
    private int numHits;
    public List<GameObject> corePiecesList = new List<GameObject>();
    public List<GameObject> breakOffPiecesList = new List<GameObject>();
    private GameObject activePanel;

    [Header("Sounds")]
    private AudioSource punchSource;
    public AudioSource matSource;
    public SoundEffects punchSounds;
    [SerializeField] private SoundEffects materialSounds;


    private void Start()
    {
        CreatePanel();
        punchSource = GetComponent<AudioSource>();
        //transform.position = new Vector3(transform.position.x, headTransform.transform.position.y, transform.position.z);
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            Punch();
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "RHand")
        {
            Debug.Log("Right Punch");
            StartCoroutine(Vibrate(OVRInput.Controller.RTouch));
            Punch();
        }
        else if (other.tag == "LHand")
        {
            Debug.Log("Left Punch");
            StartCoroutine(Vibrate(OVRInput.Controller.LTouch));
            Punch();
        }
    }

    public void Punch()
    {
        // Give animator random clip to play
        int rn = Random.Range(1, 4);
        animator.Play("Punched" + rn);
        // Play Sound Effect
        punchSource.clip = punchSounds.soundEffects[Random.Range(0, punchSounds.soundEffects.Length)];
        punchSource.Play();
        // Decrease object health
        currentHealth--;
        numHits++;

        if (currentHealth <= 0)
        {
            matSource.clip = panels[panelIndex].breakEffect;
            matSource.Play();
            Break();
        }
        else if (numHits >= hitsToBreakOff && breakOffPiecesList.Count > 0)
        {
            Debug.Log("Break");
            Chip();
        }
        else
        {
            matSource.clip = materialSounds.soundEffects[Random.Range(0, materialSounds.soundEffects.Length)];
            matSource.Play();
        }
    }

    public void Chip()
    {
        Rigidbody chipRB = breakOffPiecesList[0].GetComponent<Rigidbody>();
        chipRB.isKinematic = false;
        chipRB.useGravity = true;
        chipRB.AddExplosionForce(100, impulsePoint.position, 0.2f);
        breakOffPiecesList.Remove(breakOffPiecesList[0]);
        numHits = 0;
    }

    public void Break()
    {
        // Enable object gravity
        foreach (GameObject child in corePiecesList)
        {
            Rigidbody childRB = child.GetComponent<Rigidbody>();
            childRB.isKinematic = false;
            childRB.useGravity = true;

            // Create impulse force in front of the object
            childRB.AddExplosionForce(100, impulsePoint.position, 0.2f);
            //StartCoroutine(Fade(child));
        }
        // Call CreatePanel()
        panelIndex++;
        if (panelIndex >= panels.Length)
        {
            panelIndex = 0;
        }
        CreatePanel();
        // Unparent the shrapnel
    }

    // Creates and instantiates the current pannel
    public void CreatePanel()
    {
        // Clear up parts of the previous panel
        if (activePanel != null)
        {
            //Destroy(activePanel);
        }
        // Checks if the panel at that index point exists
        if (panelIndex < panels.Length)
        {
            // Set up current panel
            panelModel = panels[panelIndex].model;
            // Get Object Health
            Debug.Log($"Panel Health: {panels[panelIndex].health}");
            currentHealth = panels[panelIndex].health;
            // Instantiate panel
            activePanel = Instantiate(panelModel, transform.position, transform.rotation);
            activePanel.transform.parent = gameObject.transform;
            // Clear list of components
            corePiecesList.Clear();
            // Get a list of all children of the panel model
            int childIndex = 0;
            for (int i = childIndex; i < panels[panelIndex].breakOffPieces; i++)
            {
                breakOffPiecesList.Add(activePanel.transform.GetChild(i).gameObject);
                childIndex++;
            }
            for (int i = childIndex; i < panelModel.transform.childCount; i++)
            {
                corePiecesList.Add(activePanel.transform.GetChild(i).gameObject);
            }
            // Get numbers of hits to break off a piece
            hitsToBreakOff = currentHealth / panels[panelIndex].breakOffPieces;
            Debug.Log(hitsToBreakOff);
            // Add Sound Effects
            materialSounds = panels[panelIndex].soundList;
        }
    }

    /*
    public IEnumerator Fade(GameObject child)
    {
        var mat = child.GetComponent<MeshRenderer>().material;
        // Fade out panel
        yield return new WaitForSecondsRealtime(3);
        float t = 0;
        while (mat.color.a > 0)
        {
            //float t = Mathf.Lerp(0, 1, );
            t += Time.deltaTime;
            mat.color = Color.Lerp(Color.white, new Color(1, 1, 1, 0), t);
        }
    }
    */

    public IEnumerator Vibrate(OVRInput.Controller controller)
    {
        OVRInput.SetControllerVibration(1, 1, controller);
        yield return new WaitForSecondsRealtime(0.1f);
        OVRInput.SetControllerVibration(0, 0, controller);
    }
}
