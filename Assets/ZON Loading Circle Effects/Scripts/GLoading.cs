using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class GLoading : MonoBehaviour
{
    public Image _mainIcon;

    public RectTransform _startDot;
    public RectTransform _flyDot;

    public Text _loadingText;

    public float _expandAngle = 270;

    public float _expandDuration = 0.8f;
    public float _collapseDuration = 1f;

	public Color[] _transitiveColors;//Length > 1
	Image[] _graphicList;
    int _fromColorIndex;
    int _toColorIndex = 0;

    float _finishStage1Angle = 0;
    float _currentExpandAngle = 0;

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

    void Reset()
    {
        //Get transitive color index
        if (_transitiveColors.Length > 1)
        {
            _fromColorIndex = _toColorIndex;
            _toColorIndex++;

            if (_toColorIndex >= _transitiveColors.Length)
            {
                _toColorIndex = 0;
            }
        }

		Vector3 angle = _mainIcon.transform.localEulerAngles;
		angle.z = _finishStage1Angle - _currentExpandAngle;
		_mainIcon.transform.localEulerAngles = angle;

        _startTime = Time.time;
    }

    // Update is called once per frame
    void Update()
    {
        float currentTime = Time.time - _startTime;

        if(currentTime > _expandDuration + _collapseDuration)
        {
            Reset();
            currentTime = 0;
        }

        if (currentTime <= _expandDuration)
        {
            float fillAmount = _mainIcon.fillAmount;
            fillAmount = SimpleTween.EaseOutQuat(currentTime, 0, _expandAngle/360.0f, _expandDuration);

            _mainIcon.fillAmount = fillAmount;

            _currentExpandAngle = 360 * _mainIcon.fillAmount;
            _finishStage1Angle = _mainIcon.transform.localEulerAngles.z;

            MatchDotRotation();

            if (_transitiveColors.Length > 1)
            {
                Color toColor = SimpleTween.Linear(currentTime, _transitiveColors[_fromColorIndex], _transitiveColors[_toColorIndex], _expandDuration);
                SetColor(toColor);
            }
        }        
        else if(currentTime <= _expandDuration + _collapseDuration)
        {
            currentTime -= _expandDuration;

            Vector3 angle = _mainIcon.transform.localEulerAngles;
            angle.z = _finishStage1Angle - SimpleTween.Linear(currentTime,  0, _currentExpandAngle, _collapseDuration);
            _mainIcon.transform.localEulerAngles = angle;
            
            _mainIcon.fillAmount = SimpleTween.Linear(currentTime, _currentExpandAngle / 360.0f, 0, _collapseDuration);

            MatchDotRotation();
        }
    }

    void MatchDotRotation()
    {
        Vector3 angle = _startDot.localEulerAngles;
        angle.z = 360 - _mainIcon.fillAmount * 360;
        _startDot.localEulerAngles = angle;

        angle.z = _mainIcon.fillAmount * 360;
        _flyDot.localEulerAngles = angle;
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
}