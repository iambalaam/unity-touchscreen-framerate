using System;
using TMPro;
using UnityEditor;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.EnhancedTouch;
using Touch = UnityEngine.InputSystem.EnhancedTouch.Touch;
using TouchPhase = UnityEngine.InputSystem.TouchPhase;

public class TouchReporter : MonoBehaviour
{
    [SerializeField] private bool enhancedTouch;
    [SerializeField] private float _z;

    [SerializeField] private TextMeshProUGUI tmPro;
    [SerializeField] private GameObject obj;
    private GameObject[] _objs;
    private const int ObjectPoolCount = 20;

    private void Start()
    {
        EnhancedTouchSupport.Enable();
        _objs = new GameObject[ObjectPoolCount];
        for (int i = 0; i < ObjectPoolCount; i++)
        {
            _objs[i] = Instantiate(obj, transform);
        }
    }

    private void Update()
    {
        if (enhancedTouch)
        {
            UpdateEnhancedTouch();
        }
        else
        {
            UpdateTouchScreen();
        }

    }

    private void UpdateEnhancedTouch()
    {
        var touches = Touch.activeTouches;
        tmPro.text = $"(EnhancedTouch) Touch.activeTouches\nTouches: {touches.Count}";
        for (int i = 0; i < ObjectPoolCount; i++)
        {
            if (i < Touch.activeTouches.Count)
            {
                var touch = Touch.activeTouches[i];
                var pos = Camera.main.ScreenToWorldPoint(
                    new Vector3(touch.screenPosition.x, touch.screenPosition.y, _z));
                _objs[i].transform.position = pos;
                _objs[i].SetActive(true);
            }
            else
            {
                _objs[i].SetActive(false);
            }
        }
    }

    private void UpdateTouchScreen()
    {
        var touchscreen = Touchscreen.current;
        if (touchscreen != null)
        {
            tmPro.text = $"(Touchscreen) Touchscreen.touches\nTouches: {touchscreen.touches.Count}";
            for (int i = 0; i < ObjectPoolCount; i++)
            {
                if (i < touchscreen.touches.Count)
                {
                    var touch = touchscreen.touches[i];
                    var phase = touch.phase.ReadValue();
                    if (phase != TouchPhase.None && phase != TouchPhase.Ended)
                    {
                        var pos = Camera.main.ScreenToWorldPoint(new Vector3(touch.position.x.ReadValue(),
                            touch.position.y.ReadValue(), _z));
                        _objs[i].transform.position = pos;
                        _objs[i].SetActive(true);
                        continue;
                    }
                    else
                    {
                        _objs[i].SetActive(false);
                    }
                }

                _objs[i].SetActive(false);
            }
        }
    }
}