using System;
using TMPro;
using UnityEditor;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.EnhancedTouch;
using UnityEngine.UI;
using Touch = UnityEngine.InputSystem.EnhancedTouch.Touch;
using TouchPhase = UnityEngine.InputSystem.TouchPhase;

public class TouchReporter : MonoBehaviour
{
    [SerializeField] private bool enhancedTouch;
    [SerializeField] private float _z;

    [SerializeField] private TextMeshProUGUI tmPro;
    [SerializeField] private Button toggle;
    private TextMeshProUGUI _toggleText;
    [SerializeField] private GameObject obj;
    private GameObject[] _objs;
    private const int ObjectPoolCount = 20;

    private void Start()
    {
        EnhancedTouchSupport.Enable();

        _toggleText = toggle.GetComponentInChildren<TextMeshProUGUI>();
        
        _objs = new GameObject[ObjectPoolCount];
        for (int i = 0; i < ObjectPoolCount; i++)
        {
            _objs[i] = Instantiate(obj, transform);
        }
    }

    public void ToggleEnhanced()
    {
        if (enhancedTouch)
        {
            enhancedTouch = false;
            _toggleText.text = "EnhancedTouch";
        }
        else
        {
            enhancedTouch = true;
            _toggleText.text = "NormalTouch";
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
                    switch (phase)
                    {
                        case TouchPhase.Began:
                        case TouchPhase.Moved:
                        case TouchPhase.Stationary:
                            var pos = Camera.main.ScreenToWorldPoint(new Vector3(touch.position.x.ReadValue(),
                                touch.position.y.ReadValue(), _z));
                            _objs[i].transform.position = pos;
                            _objs[i].SetActive(true);
                            continue;

                        case TouchPhase.Ended:
                        case TouchPhase.None:
                        case TouchPhase.Canceled:
                            _objs[i].SetActive(false);
                            break;
                        
                        default:
                            throw new Exception($"Unknown touch phase: {phase.ToString()}");
                    }
                }

                _objs[i].SetActive(false);
            }
        }
    }
}