using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{

    [Header("=== [ Core ] ===")]
    public int _score;
    public int _maxLevel;
    public bool _isGameOver;

    [Header("=== [ Object Pooling ] ===")]
    public Dongle _lastDongle;
    public GameObject _donglePrefab;
    public Transform _dongleGroup;
    public GameObject _effectPrefab;
    public Transform _effectGroup;

    public List<Dongle> _donglePool;
    public List<ParticleSystem> _effectPool;

    [Range(1, 30)]
    public int _poolSize;
    public int _poolCursor;

    [Header("=== [ Audio ] ===")]
    public AudioSource _bgmPlayer;
    public AudioSource[] _sfxPlayer;
    public AudioClip[] _sfxClip;


    public enum SFX
    {
        LEVELUP,
        NEXT,
        ATTACH,
        BUTTON,
        OVER
    }
    int _sfxCursor;

    [Header("=== [ Constant ] ===")]
    const string DONGLE_NAME = "Dongle";
    const string DONGLE_EFFECT_NAME = "Effect";
    const string DONGLE_MAX_SCORE = "Max_Score";

    [Header("=== [ UI ] ===")]
    public GameObject _startUI;
    public GameObject _endUI;
    public Text _txtScore;
    public Text _txtMaxScore;
    public Text _txtLastScore;

    [Header("=== [ ETC ] ===")]
    public GameObject _gObjLine;
    public GameObject _gObjBtm;

    void Awake()
    {
        Application.targetFrameRate = 60;

        // Ǯ �ʱ�ȭ
        _donglePool = new List<Dongle>();
        _effectPool = new List<ParticleSystem>();
        for (int i = 0; i < _poolSize; i++)
        {
            MakeDongle();
        }

        InitAudioSrc();

        Transform _trfCanvas = GameObject.FindGameObjectWithTag("Canvas").transform;
        _startUI      = _trfCanvas.Find("StartUI").gameObject;
        _endUI        = _trfCanvas.Find("EndUI").gameObject;
        _txtLastScore = _endUI.transform.Find("BtnReStart/TxtLastScore").GetComponent<Text>();

        Transform _trfPlayground = GameObject.FindGameObjectWithTag("Playground").transform;
        _gObjLine = _trfPlayground.Find("Line").gameObject;
        _gObjBtm  = _trfPlayground.Find("Bottom").gameObject;

        _txtScore     = _trfCanvas.Find("TxtScore").GetComponent<Text>();
        _txtMaxScore  = _trfCanvas.Find("TxtMaxScore").GetComponent<Text>();

        if (!PlayerPrefs.HasKey(DONGLE_MAX_SCORE))
            PlayerPrefs.SetInt(DONGLE_MAX_SCORE, 0); 
        
        _txtMaxScore.text = PlayerPrefs.GetInt(DONGLE_MAX_SCORE).ToString();
    }

    void Update()
    {
        if ( Input.GetButtonDown("Cancel"))
        {
            Application.Quit();
        }
    }

    void LateUpdate()
    {
        _txtScore.text = _score.ToString();
    }

    void InitAudioSrc()
    {
        // AudioSource �ʱ�ȭ
        _bgmPlayer = transform.Find("BGMPlayer").GetComponent<AudioSource>();

        // AudioSource �迭�ʱ�ȭ
        AudioSource sfxPlayer1 = transform.Find("SFXPlayer1").GetComponent<AudioSource>();
        AudioSource sfxPlayer2 = transform.Find("SFXPlayer2").GetComponent<AudioSource>();
        AudioSource sfxPlayer3 = transform.Find("SFXPlayer3").GetComponent<AudioSource>();

        _sfxPlayer = new AudioSource[] { sfxPlayer1, sfxPlayer2, sfxPlayer3 };

        // AudionClip �迭�ʱ�ȭ
        AudioClip levelUpA = Resources.Load<AudioClip>("Audio/LevelUp A");
        AudioClip levelUpB = Resources.Load<AudioClip>("Audio/LevelUp B");
        AudioClip levelUpC = Resources.Load<AudioClip>("Audio/LevelUp C");
        AudioClip next = Resources.Load<AudioClip>("Audio/Next");
        AudioClip attach = Resources.Load<AudioClip>("Audio/Attach");
        AudioClip button = Resources.Load<AudioClip>("Audio/Button");
        AudioClip gameOver = Resources.Load<AudioClip>("Audio/GameOver");

        _sfxClip = new AudioClip[] { levelUpA, levelUpB, levelUpC, next, attach, button, gameOver };
    }
    public void GameStart()
    {
        // ������Ʈ Ȱ��ȭ
        _gObjLine.SetActive(true);
        _gObjBtm.SetActive(true);
        _txtScore.gameObject.SetActive(true);
        _txtMaxScore.gameObject.SetActive(true);
        _startUI.SetActive(false);

        _bgmPlayer.Play();
        SfxPlay(SFX.BUTTON);
        _maxLevel = 2;

        Invoke("NextDongle", 1.5f);
    }

    Dongle MakeDongle()
    {
        // ����Ʈ ����
        GameObject instantEff = Instantiate(_effectPrefab, _effectGroup);
        instantEff.name = DONGLE_EFFECT_NAME + _effectPool.Count;
        ParticleSystem instantEffect = instantEff.GetComponent<ParticleSystem>();
        _effectPool.Add(instantEffect);

        // ���� ����
        GameObject instant = Instantiate(_donglePrefab, _dongleGroup);
        instant.name = DONGLE_NAME + _donglePool.Count;

        Dongle instantDongle = instant.GetComponent<Dongle>();
        instantDongle._manager = this;
        instantDongle._effect = instantEffect;
        _donglePool.Add(instantDongle);

        return instantDongle;
    }

    Dongle GetDongle()
    {
        /* Object Pooling �������� �Ʒ������� MakeDongle�� ��ü
        // ����Ʈ ����
        GameObject instantEff = Instantiate(_effectPrefab, _effectGroup);
        instantEff.name = DONGLE_EFFECT_NAME;
        ParticleSystem instantEffect = instantEff.GetComponent<ParticleSystem>();

        // ���� ����
        GameObject instant = Instantiate(_donglePrefab, _dongleGroup);
        instant.name = DONGLE_NAME;

        Dongle instantDongle = instant.GetComponent<Dongle>();
        instantDongle._effect = instantEffect;
        _donglePool.Add(instantDongle);

        return instantDongle;
        */

        for (int i = 0; i < _donglePool.Count; i++)
        {
            _poolCursor = (_poolCursor + 1) % _donglePool.Count;
            if ( !_donglePool[_poolCursor].gameObject.activeSelf)
            {
                return _donglePool[_poolCursor];
            }
        }

        return MakeDongle();
    }

    void NextDongle()
    {
        if (_isGameOver)
            return;

        // Dongle newDongle = GetDongle();
        // _lastDongle = newDongle;
        // _lastDongle._manager = this;

        _lastDongle = GetDongle();
        _lastDongle._level = Random.Range(0, _maxLevel);
        _lastDongle.gameObject.SetActive(true);

        SfxPlay(SFX.NEXT);
        StartCoroutine("WaitNext");
    }

    IEnumerator WaitNext()
    {
        while(_lastDongle != null)
        {
            yield return null;      //yield return null�� ���� ������ �����Ӵ� ���ѹݺ��ȴ�.
        }
        yield return new WaitForSeconds(2.5f);
        NextDongle();
    }

    public void TouchDown()
    {
        if (_lastDongle == null)
            return;

        _lastDongle.Drag();
    }

    public void TouchUp()
    {
        if (_lastDongle == null)
            return;

        _lastDongle.Drop();
        _lastDongle = null;
    }

    public void GameOver()
    {
        if (_isGameOver)
        {
            return;
        }
        _isGameOver = true;
        // Debug.Log("Game Over");

        StartCoroutine("GameOverRoutine");
    }

    IEnumerator GameOverRoutine()
    {
        // 1. ��� �ȿ� Ȱ��ȭ �Ǿ��ִ� ��� ���� ��������
        Dongle[] dongleArr = FindObjectsOfType<Dongle>();

        // 2. ����� ���� ��� ������ ����ȿ�� ��Ȱ��ȭ
        // 3. 1�� ����� �ϳ��� �����ؼ� �����
        for (int i = 0; i < dongleArr.Length; i++)
        {
            dongleArr[i]._rigid.simulated = false;
            dongleArr[i].Hide(Vector3.up * 100);
            yield return new WaitForSeconds(0.1f);
        }

        yield return new WaitForSeconds(1.0f);

        // �ְ� ���� ����
        int maxScore = Mathf.Max(_score, PlayerPrefs.GetInt(DONGLE_MAX_SCORE));
        PlayerPrefs.SetInt(DONGLE_MAX_SCORE, maxScore);

        // ���ӿ��� UIǥ��
        _endUI.SetActive(true);
        _txtLastScore.text = "���� : " + _txtScore.text;

        _bgmPlayer.Stop();
        SfxPlay(SFX.OVER);
    }

    public void Reset()
    {
        SfxPlay(SFX.BUTTON);
        StartCoroutine(ResetCoroutine());
    }

    IEnumerator ResetCoroutine()
    {
        yield return new WaitForSeconds(1.0f);
        SceneManager.LoadScene("MainScene");
    }

    public void SfxPlay(SFX type)
    {
        switch (type)
        {
            case SFX.LEVELUP:
                _sfxPlayer[_sfxCursor].clip = _sfxClip[Random.Range(0, 3)];
                break;
            case SFX.NEXT:
                _sfxPlayer[_sfxCursor].clip = _sfxClip[3];
                break;
            case SFX.ATTACH:
                _sfxPlayer[_sfxCursor].clip = _sfxClip[4];
                break;
            case SFX.BUTTON:
                _sfxPlayer[_sfxCursor].clip = _sfxClip[5];
                break;
            case SFX.OVER:
                _sfxPlayer[_sfxCursor].clip = _sfxClip[6];
                break;
        }

        _sfxPlayer[_sfxCursor].Play();
        _sfxCursor = (_sfxCursor + 1) % _sfxPlayer.Length;
    }
}
