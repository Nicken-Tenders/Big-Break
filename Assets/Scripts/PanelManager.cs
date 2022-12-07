using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Liminal.SDK.Core;
using Liminal.Core.Fader;

public class PanelManager : MonoBehaviour
{
    [Header("References")]
    public Animator animator;
    public Transform impulsePoint;
    public GameObject headTransform;
    public ParticleSystem leftFistParticle;
    public ParticleSystem rightFistParticle;
    private int winNum;
    private int breakNum;

    [Header("Panel")]
    public Panel[] panels;
    public int panelIndex = 0;
    private GameObject panelModel;
    public int currentHealth;
    public int hitsToBreakOff;
    public GameObject panelAnimator;
    private int numHits;
    public List<GameObject> corePiecesList = new List<GameObject>();
    public List<GameObject> breakOffPiecesList = new List<GameObject>();
    private GameObject panelInstance;
    private GameObject panelParticalSystem;
    private GameObject particalInstance;

    [Header("Sounds")]
    private AudioSource punchSource;
    public AudioSource matSource;
    public SoundEffects punchSounds;
    [SerializeField] private SoundEffects materialSounds;


    private void Start()
    {
        winNum = panels.Length;
        CreatePanel();
        punchSource = GetComponent<AudioSource>();
        rightFistParticle.Stop();
        leftFistParticle.Stop();
        //transform.position = new Vector3(transform.position.x, headTransform.transform.position.y, transform.position.z);
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space) && gameObject.GetComponent<BoxCollider>().enabled == true)
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
            rightFistParticle.Play();
            Punch();
        }
        else if (other.tag == "LHand")
        {
            Debug.Log("Left Punch");
            StartCoroutine(Vibrate(OVRInput.Controller.LTouch));
            leftFistParticle.Play();
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
            Debug.Log("Chip");
            Chip();
            matSource.clip = materialSounds.soundEffects[Random.Range(0, materialSounds.soundEffects.Length)];
            matSource.Play();
        }
        else
        {
            matSource.clip = materialSounds.soundEffects[Random.Range(0, materialSounds.soundEffects.Length)];
            matSource.Play();
        }
        particalInstance.GetComponent<ParticleSystem>().Play();
    }

    public void Chip()
    {
        Rigidbody chipRB = breakOffPiecesList[0].GetComponent<Rigidbody>();
        chipRB.isKinematic = false;
        chipRB.useGravity = true;
        chipRB.AddExplosionForce(1000, impulsePoint.position, 4f);
        breakOffPiecesList.Remove(breakOffPiecesList[0]);
        numHits = 0;
    }

    public void Break()
    {
        breakNum++;
        // Enable object gravity
        foreach (GameObject child in corePiecesList)
        {
            Rigidbody childRB = child.GetComponent<Rigidbody>();
            childRB.isKinematic = false;
            childRB.useGravity = true;

            // Create impulse force in front of the object
            childRB.AddExplosionForce(1000, impulsePoint.position, 4f);
            //StartCoroutine(Fade(child));
        }
        // Call CreatePanel()
        panelIndex++;
        if (panelIndex >= panels.Length)
        {
            panelIndex = 0;
        }
        if (breakNum >= winNum)
        {
            StartCoroutine(Win());
        }
        else
        {
            StartCoroutine(Wait(1));
        }
        // Unparent the shrapnel
    }

    // Creates and instantiates the current pannel
    public void CreatePanel()
    {
        // Clear up parts of the previous panel
        if (panelInstance != null)
        {
            //Destroy(activePanel);
        }
        if (particalInstance != null)
        {
            Destroy(particalInstance);
        }
        // Checks if the panel at that index point exists
        if (panelIndex < panels.Length)
        {
            // Set up current panel
            panelModel = panels[panelIndex].model;
            panelParticalSystem = panels[panelIndex].particles;
            // Get Object Health
            Debug.Log($"Panel Health: {panels[panelIndex].health}");
            currentHealth = panels[panelIndex].health;
            // Instantiate panel
            panelInstance = Instantiate(panelModel, transform.position, transform.rotation);
            panelInstance.transform.parent = panelAnimator.transform;
            particalInstance = Instantiate(panelParticalSystem, new Vector3 (transform.position.x, (transform.position.y + 1.3f), transform.position.z), transform.rotation);
            particalInstance.GetComponent<ParticleSystem>().Stop();
            particalInstance.transform.parent = this.transform;
            // Clear list of components
            corePiecesList.Clear();
            // Get a list of all children of the panel model
            int childIndex = 0;
            for (int i = childIndex; i < panels[panelIndex].breakOffPieces; i++)
            {
                breakOffPiecesList.Add(panelInstance.transform.GetChild(i).gameObject);
                childIndex++;
            }
            for (int i = childIndex; i < panelModel.transform.childCount; i++)
            {
                corePiecesList.Add(panelInstance.transform.GetChild(i).gameObject);
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

    public IEnumerator Wait(int waitTime)
    {
        BoxCollider col = gameObject.GetComponent<BoxCollider>();
        col.enabled = false;
        yield return new WaitForSeconds(waitTime);
        CreatePanel();
        col.enabled = true;
    }

    public IEnumerator Win()
    {
        // Fanfare
        // Confetti
        ///Time.timeScale = 0.5f;
        ///yield return new WaitForSecondsRealtime(2);
        ///Time.timeScale = 1;
        yield return new WaitForSecondsRealtime(2);

        gameObject.GetComponent<Collider>().enabled = false;
        var fader = ScreenFader.Instance;
        fader.FadeTo(Color.black, 2);
        yield return new WaitForSecondsRealtime(2);
        ExperienceApp.End();
    }
}