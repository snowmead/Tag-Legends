using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SimpleRotation : MonoBehaviour
{
	public RectTransform _mainIcon;
    public Text _loadingText;

    public float _timeStep = 0.02f;
    public float _stepAngle = -30;

	public Color[] _transitiveColors;//Length > 1
	Image[] _graphicList;
    int _fromColorIndex;
    int _toColorIndex = 0;

    float _totalAngle = 0;

    float _startTime;

    void Start()
	{
		_graphicList = GetComponentsInChildren<Image>(true);

        if (_transitiveColors.Length > 1)
        {
            SetColor(_transitiveColors[0]);
        }

        Reset();
    }

    void SetColor(Color toColor)
    {
        if (_loadingText != null)
        {
            _loadingText.color = toColor;
		}

		for(int i = 0; i < _graphicList.Length; i++)
		{
			_graphicList[i].color = toColor;
		}
    }

    void Reset()
    {
        //Get transitive color index
        if (_transitiveColors.Length > 1)
        {            
            _totalAngle += Mathf.Abs(_stepAngle);

            if (_totalAngle > 360)
            {
                _totalAngle %= 360;

                _fromColorIndex = _toColorIndex;
                _toColorIndex++;

                if (_toColorIndex >= _transitiveColors.Length)
                {
                    _toColorIndex = 0;
                }
            }
        }

        _startTime = Time.time;
    }

    // Update is called once per frame
    void Update()
    {
        float currentTime = Time.time - _startTime;

        if (currentTime >= _timeStep)
        {            
            Vector3 angle = _mainIcon.localEulerAngles;
            angle.z += _stepAngle;

            _mainIcon.localEulerAngles = angle;

            if (_transitiveColors.Length > 1)
            {
                Color toColor = Color.Lerp(_transitiveColors[_fromColorIndex], _transitiveColors[_toColorIndex], _totalAngle / 360);
                SetColor(toColor);
            }

            Reset();
        }
    }
}
