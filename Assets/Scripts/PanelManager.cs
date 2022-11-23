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
    private GameObject currentPanel;
    public int currentHealth;
    private List<GameObject> panelComponents = new List<GameObject>();
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
        // Give animator random amount of z angle to twist from -45 to 45
        int rn = Random.Range(1, 4);
        // Animator plays punched clip with given value
        /// animator.SetTrigger("Punched" + rn);
        animator.Play("Punched" + rn);
        // Play Sound Effect
        punchSource.clip = punchSounds.soundEffects[Random.Range(0, punchSounds.soundEffects.Length)];
        punchSource.Play();
        // Decrease object health
        currentHealth--;

        if (currentHealth <= 0)
        {
            matSource.clip = panels[panelIndex].breakEffect;
            matSource.Play();
            Break();
        }
        else
        {
            matSource.clip = materialSounds.soundEffects[Random.Range(0, materialSounds.soundEffects.Length)];
            matSource.Play();
        }
    }

    public void Break()
    {
        // Enable object 
        foreach (GameObject child in panelComponents)
        {
            child.GetComponent<Rigidbody>().useGravity = true;
            child.GetComponent<Rigidbody>().isKinematic = false;
        }
        // Allow object to split
        // Create impulse force in front of the object
        foreach (GameObject child in panelComponents)
        {
            child.GetComponent<Rigidbody>().AddExplosionForce(100, impulsePoint.position, 0.1f);
            child.GetComponent<Rigidbody>().isKinematic = false;
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

    public IEnumerator Vibrate(OVRInput.Controller controller)
    {
        OVRInput.SetControllerVibration(1, 1, controller);
        yield return new WaitForSecondsRealtime(0.1f);
        OVRInput.SetControllerVibration(0, 0, controller);
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
            currentPanel = panels[panelIndex].model;
            // Get Object Health
            Debug.Log($"Panel Health: {panels[panelIndex].health}");
            currentHealth = panels[panelIndex].health;
            // Instantiate panel
            activePanel = Instantiate(currentPanel, transform.position, transform.rotation);
            activePanel.transform.parent = gameObject.transform;
            // Clear list of components
            panelComponents.Clear();
            // Get a list of all children of the panel model
            for (int i = 0; i < currentPanel.transform.childCount; i++)
            {
                panelComponents.Add(activePanel.transform.GetChild(i).gameObject);
            }
            materialSounds = panels[panelIndex].soundList;
        }
    }
}
