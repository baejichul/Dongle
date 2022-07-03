using System.Collections;
using System.Collections.Generic;
using UnityEngine;




public class Dongle : MonoBehaviour
{

    public int _level;
    public bool _isDrag;
    Rigidbody2D _rigid;
    Animator _ani;

    void Awake()
    {
        _rigid = GetComponent<Rigidbody2D>();
        _ani   = GetComponent<Animator>();
    }


    void OnEnable()
    {
        _ani.SetInteger("Level", _level);
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (_isDrag)
            MoveMouse();
    }

    void MoveMouse()
    {
        Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            
        // x축 이동제한
        float leftBorder   = -4.2f + transform.localScale.x / 2f;
        float righttBorder = 4.2f - transform.localScale.x / 2f;

        if (mousePos.x < leftBorder)
            mousePos.x = leftBorder;
        else if (mousePos.x > righttBorder)
            mousePos.x = righttBorder;

        mousePos.y = 8.0f;      // y축 고정
        mousePos.z = 0.0f;      // z축 고정
        transform.position = Vector3.Lerp(transform.position, mousePos, 0.2f);
    }

    public void Drag()
    {
        _isDrag = true;
    }

    public void Drop()
    {
        _isDrag = false;
        _rigid.simulated = true;
    }
}
