using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public Dongle _lastDongle;
    public GameObject _donglePrefab;
    public Transform _dongleGroup;


    // Start is called before the first frame update
    void Start()
    {
        NextDongle();
    }

    Dongle GetDongle()
    {
        GameObject instant = Instantiate(_donglePrefab, _dongleGroup);
        Dongle instantDongle = instant.GetComponent<Dongle>();
        return instantDongle;
    }

    void NextDongle()
    {
        Dongle newDongle = GetDongle();
        // newDongle
    }

    public void TouchDown()
    {
        _lastDongle.Drag();
    }

    public void TouchUp()
    {
        _lastDongle.Drop();
    }

}
