using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public Dongle _lastDongle;
    public GameObject _donglePrefab;
    public Transform _dongleGroup;
    public GameObject _effectPrefab;
    public Transform _effectGroup;

    public int score;
    public int _maxLevel;
    public bool _isGameOver;

    const string DONGLE_NAME = "Dongle";
    const string DONGLE_EFFECT_NAME = "Effect";

    void Awake()
    {
        Application.targetFrameRate = 60;
            
    }
    // Start is called before the first frame update
    void Start()
    {
        _maxLevel = 2;
        NextDongle();
    }

    Dongle GetDongle()
    {
        // ����Ʈ ����
        GameObject instantEff = Instantiate(_effectPrefab, _effectGroup);
        instantEff.name = DONGLE_EFFECT_NAME;
        ParticleSystem instantEffect = instantEff.GetComponent<ParticleSystem>();

        // ���� ����
        GameObject instant = Instantiate(_donglePrefab, _dongleGroup);
        instant.name = DONGLE_NAME;

        Dongle instantDongle = instant.GetComponent<Dongle>();
        instantDongle._effect = instantEffect;

        return instantDongle;
    }

    void NextDongle()
    {
        if (_isGameOver)
            return;

        Dongle newDongle = GetDongle();
        _lastDongle = newDongle;
        _lastDongle._manager = this;
        _lastDongle._level = Random.Range(0, _maxLevel);
        _lastDongle.gameObject.SetActive(true);

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
    }
}
