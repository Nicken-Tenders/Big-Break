using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Liminal.SDK.Core;
using Liminal.Core.Fader;

public class PanelManager : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Animator _animator;
    [SerializeField] private Transform _impulsePoint;
    [SerializeField] private GameObject _headTransform;
    [SerializeField] private GameObject _brokenPanels;
    [SerializeField] private GameObject _panelAnimator;
    [SerializeField] private ParticleSystem _rightFistParticle;
    [SerializeField] private ParticleSystem _leftFistParticle;
    [SerializeField] private ParticleSystem _confetti;
    [SerializeField] private List<Panel> _panels;
    private BoxCollider _panelCollider;

    [Header("Gameplay")]
    [SerializeField] private float _explosionForceMin;
    [SerializeField] private float _explosionForceMax;
    [SerializeField] private int _matIndex;
    private int _winGoal;
    private int _breakDmg;

    [Header("Sounds")]
    [SerializeField] private AudioSource _matSource;
    [SerializeField] private AudioSource _victorySource;
    [SerializeField] private AudioSource _music;
    private bool _musicFade = false;
    [SerializeField] private SoundEffects _punchSounds;
    private SoundEffects _materialSounds;
    private AudioSource _punchSource;

    [Header("Panel")]
    private int _panelIndex = 0;
    private int _currentHealth;
    private int _hitsToBreakOff;
    private int _hitsToChip;
    private GameObject _panelModel;
    private GameObject _panelInstance;
    private GameObject _panelParticalSystem;
    private GameObject _particleInstance;
    private ParticleSystem _particleInstanceSystem;
    private List<GameObject> _corePiecesList = new List<GameObject>();
    private List<GameObject> _breakOffPiecesList = new List<GameObject>();

    private void Start()
    {
        _punchSource = GetComponent<AudioSource>();
        _panelCollider = GetComponent<BoxCollider>();

        _winGoal = _panels.Count;

        CreatePanel();

        _rightFistParticle.Stop();
        _leftFistParticle.Stop();
        _confetti.Stop();


        //transform.position = new Vector3(transform.position.x, headTransform.transform.position.y, transform.position.z);
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space) && _panelCollider.enabled == true)
        {
            Punch();
        }
        
        if (_musicFade == true)
        {
            _music.volume = _music.volume - (0.25f * Time.deltaTime); 
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "RHand")
        {
            StartCoroutine(Vibrate(OVRInput.Controller.RTouch));
            _rightFistParticle.Play();
            Punch();
        }
        else if (other.tag == "LHand")
        {
            StartCoroutine(Vibrate(OVRInput.Controller.LTouch));
            _leftFistParticle.Play();
            Punch();
        }
    }

    public void Punch()
    {
        // Give animator random clip to play
        //int rn = Random.Range(1, 4);
        //_animator.Play("Punched" + rn);
        _animator.Play("Shake 1");

        // Play Sound Effect
        _punchSource.clip = _punchSounds.soundEffects[Random.Range(0, _punchSounds.soundEffects.Length)];
        _punchSource.Play();

        // Decrease object health
        _currentHealth--;
        _hitsToChip++;

        if (_currentHealth <= 0)
        {
            _matSource.clip = _panels[_panelIndex].breakEffect;
            _matSource.Play();
            Break();
        }
        else if (_hitsToChip >= _hitsToBreakOff)
        {
            Chip();
            _matSource.clip = _materialSounds.soundEffects[Random.Range(0, _materialSounds.soundEffects.Length)];
            _matSource.Play();
        }
        else
        {
            _matSource.clip = _materialSounds.soundEffects[Random.Range(0, _materialSounds.soundEffects.Length)];
            _matSource.Play();
        }

        _particleInstanceSystem.Play();
    }

    public void Chip()
    {
        if (_breakOffPiecesList.Count > 0)
        {
            Rigidbody chipRB = _breakOffPiecesList[0].GetComponent<Rigidbody>();
            chipRB.isKinematic = false;
            chipRB.useGravity = true;
            chipRB.AddExplosionForce(Random.Range(_explosionForceMin, _explosionForceMax), _impulsePoint.position, 4f);
            _breakOffPiecesList.Remove(_breakOffPiecesList[0]);
        }
        _hitsToChip = 0;
        ++_matIndex;
        Debug.Log(_matIndex);
        for (int i = 0; i < _corePiecesList.Count; i++)
        {
            if (_matIndex <= _breakOffPiecesList.Count || _panels[_panelIndex].name == "LiminalPanel")
            {
                _corePiecesList[i].gameObject.transform.GetChild(0).GetComponent<ChangeMaterialScript>().ChangeMaterial(_matIndex);
            }
        }
    }

    public void Break()
    {
        ++_breakDmg;
        // Enable object gravity
        foreach (GameObject child in _corePiecesList)
        {
            Rigidbody childRB = child.GetComponent<Rigidbody>();
            childRB.isKinematic = false;
            childRB.useGravity = true;

            // Create impulse force in front of the object
            childRB.AddExplosionForce(Random.Range(_explosionForceMin, _explosionForceMax), _impulsePoint.position, 4f);
            //StartCoroutine(Fade(child));
        }

        // Call CreatePanel()
        ++_panelIndex;
        if (_panelIndex >= _panels.Count)
        {
            _panelIndex = 0;
        }
        if (_breakDmg >= _winGoal)
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
        if (_panelInstance != null)
        {
            _panelInstance.transform.parent = _brokenPanels.transform;
            //Destroy(activePanel);
        }

        // Clear previous particles
        if (_particleInstance != null)
        {
            Destroy(_particleInstance);
            _particleInstanceSystem = null;
        }

        // Checks if the panel at that index point exists
        if (_panelIndex < _panels.Count)
        {
            // Set up current panel
            _panelModel = _panels[_panelIndex].model;
            _panelParticalSystem = _panels[_panelIndex].particles;

            // Get Object Health
            _currentHealth = _panels[_panelIndex].health;

            // Instantiate panel
            _panelInstance = Instantiate(_panelModel, transform.position, transform.rotation);
            _panelInstance.transform.parent = _panelAnimator.transform;
            _matIndex = 0;

            // Instantiate particles
            _particleInstance = Instantiate(_panelParticalSystem, new Vector3(transform.position.x, (transform.position.y + 1.3f), transform.position.z), Quaternion.Euler(transform.rotation.x - 90, transform.rotation.y, transform.rotation.z));
            _particleInstanceSystem = _particleInstance.GetComponent<ParticleSystem>();
            _particleInstanceSystem.Stop();
            _particleInstance.transform.parent = transform;

            // Clear list of components
            _corePiecesList.Clear();

            // Get a list of all break of pieces
            int childIndex = 0;
            for (int i = childIndex; i < _panels[_panelIndex].breakOffPieces; i++)
            {
                _breakOffPiecesList.Add(_panelInstance.transform.GetChild(i).gameObject);
                ++childIndex;
            }

            // Get a list of all core pieces
            for (int i = childIndex; i < _panelModel.transform.childCount; i++)
            {
                _corePiecesList.Add(_panelInstance.transform.GetChild(i).gameObject);            
            }

            for (int i = 0; i < _corePiecesList.Count; i++)
            {
                _corePiecesList[i].gameObject.transform.GetChild(0).GetComponent<ChangeMaterialScript>().SetMaterialOnInstantiate();
            }

            // Get numbers of hits to break off a piece
            if (_panels[_panelIndex].breakOffPieces > 0)
            {
                _hitsToBreakOff = _currentHealth / _panels[_panelIndex].breakOffPieces;
            }
            else if (_panels[_panelIndex].name == "LiminalPanel")
            {
                _hitsToBreakOff = _currentHealth / 5;
            }

            // Add Sound Effects
            _materialSounds = _panels[_panelIndex].soundList;
        }
    }

    // Vibrate Controller
    public IEnumerator Vibrate(OVRInput.Controller controller)
    {
        OVRInput.SetControllerVibration(1, 1, controller);

        yield return new WaitForSecondsRealtime(0.1f);

        OVRInput.SetControllerVibration(0, 0, controller);
    }

    // Wait between spawning the next panel
    public IEnumerator Wait(int waitTime)
    {
        _panelCollider.enabled = false;
        yield return new WaitForSeconds(waitTime);
        CreatePanel();
        _panelCollider.enabled = true;
    }

    // Complete the victory condition
    public IEnumerator Win()
    {
        _panelCollider.enabled = false;
        _victorySource.Play();
        _confetti.Play();
        ///Time.timeScale = 0.5f;
        ///yield return new WaitForSecondsRealtime(2);
        ///Time.timeScale = 1;
        yield return new WaitForSecondsRealtime(3);

        _musicFade = true;
        var fader = ScreenFader.Instance;
        fader.FadeTo(Color.black, 2);
        yield return new WaitForSecondsRealtime(2);
        ExperienceApp.End();
    }
}