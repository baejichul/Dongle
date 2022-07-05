using System.Collections;
using System.Collections.Generic;
using UnityEngine;




public class Dongle : MonoBehaviour
{
    public GameManager _manager;
    public ParticleSystem _effect;
    public int _level;
    public bool _isDrag;
    public bool _isMerge;


    public Rigidbody2D _rigid;
    CircleCollider2D _circleCol;
    Animator _ani;
    SpriteRenderer _spr;

    float _deadTime;

    void Awake()
    {
        _spr   = GetComponent<SpriteRenderer>();
        _rigid = GetComponent<Rigidbody2D>();
        _ani   = GetComponent<Animator>();
        _circleCol = GetComponent<CircleCollider2D>();
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

    public void Hide(Vector3 targetPos)
    {
        _isMerge = true;
        _rigid.simulated = false;
        _circleCol.enabled = false;

        if (targetPos == Vector3.up * 100)
        {
            EffectPlay();
        }

        StartCoroutine(HideRoutine(targetPos));
    }

    IEnumerator HideRoutine(Vector3 targetPos)
    {
        int frameCount = 0;
        while ( frameCount < 20)
        {
            frameCount++;

            if (targetPos != Vector3.up * 100)
            {
                transform.position = Vector3.Lerp(transform.position, targetPos, 1.0f);
            }
            else if (targetPos == Vector3.up * 100)
            {
                transform.localScale = Vector3.Lerp(transform.localScale, Vector3.zero, 0.2f);
            }
            
            yield return null;
        }

        _manager.score += (int) Mathf.Pow(2, _level);
        _isMerge = false;
        gameObject.SetActive(false);
    }

    void LevelUp()
    {
        _isMerge = true;
        _rigid.velocity = Vector2.zero;
        _rigid.angularVelocity = 0;

        StartCoroutine(LevelUpRoutine());
    }

    IEnumerator LevelUpRoutine()
    {
        yield return new WaitForSeconds(0.2f);

        _ani.SetInteger("Level", _level + 1);
        EffectPlay();

        yield return new WaitForSeconds(0.3f);
        _level++;

        _manager._maxLevel = Mathf.Max(_level, _manager._maxLevel);

        _isMerge = false;

    }
    void EffectPlay()
    {
        _effect.transform.position = transform.position;
        _effect.transform.localScale = transform.localScale;
        _effect.Play();
    }

    void OnTriggerStay2D(Collider2D collision)
    {
        if(collision.tag == "Finish")
        {
            _deadTime += Time.deltaTime;

            if (_deadTime > 2.0f)
            {
                _spr.color = new Color(0.9f, 0.2f, 0.2f);
            }
            
            if (_deadTime > 5.0f)
            {
                _manager.GameOver();
            }
        }    
    }

    void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.tag == "Finish")
        {
            _deadTime = 0;
            _spr.color = Color.white;
        }
    }

    void OnCollisionStay2D(Collision2D collision)
    {
        if (collision.gameObject.tag == "Dongle")
        {
            Dongle collisionDongle = collision.gameObject.GetComponent<Dongle>();

            if ( (_level == collisionDongle._level) && (!_isMerge && !collisionDongle._isMerge) && _level < 7 )
            {
                // 나와 충돌체 위치 가져오기
                float tformX = transform.position.x;
                float tformY = transform.position.y;
                float collX  = collision.transform.position.x;
                float collY  = collision.transform.position.y;

                // 1.내가 충돌체보다 아래에 있을때
                // 2.충돌체와 동일한 높이일때, 내가 오른쪽에 있을때
                if ( tformY < collY || (tformY == collY && tformX > collX) )
                {
                    // 충돌체 숨기기
                    collisionDongle.Hide(transform.position);
                    // 나는 레벨업
                    LevelUp();
                }
            }

        }
    }
}
