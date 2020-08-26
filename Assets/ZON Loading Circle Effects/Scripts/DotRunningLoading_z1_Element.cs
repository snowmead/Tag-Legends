using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DotRunningLoading_z1_Element : MonoBehaviour
{
	Image _mainIcon;

    Text _loadingText;

    float _startTime;

	float _duration;

	float _minScale;
	float _maxScale;

	Color[] _transitiveColors;//Length > 1
    Image[] _graphicList;    
    int _fromColorIndex;
    int _toColorIndex = 0;

    public void StartAnimation(float duration, float minScale, float maxScale, float delay, Color[] transitiveColors, Text loadingText = null)
	{
		_loadingText = loadingText;
        _transitiveColors = transitiveColors;

        _mainIcon = transform.GetChild(0).GetComponent<Image>();
        _graphicList = GetComponentsInChildren<Image>(true);

        if (_transitiveColors.Length > 1)
        {
            SetColor(_transitiveColors[0]);
        }

        _mainIcon.GetComponent<RectTransform> ().localScale = new Vector3 (minScale, minScale, minScale);
		_mainIcon.enabled = false;

		_duration = duration;

		_minScale = minScale;
		_maxScale = maxScale;

        Reset();
        _startTime = Time.time + delay;
	}

	public void UpdateAnimation(){		
		float currentTime = Time.time - _startTime;
		if(currentTime < 0){
			return;
		}
		else{
			_mainIcon.enabled = true;
		}

		if (currentTime <= _duration) {			
			float mainIconScale;
			
			if(currentTime <= _duration/2){
				mainIconScale = SimpleTween.EaseOutQuat (currentTime, _minScale, _maxScale, _duration/2);
			}
			else{
				mainIconScale = SimpleTween.Linear (_duration - currentTime, _minScale, _maxScale, _duration/2);
			}

			_mainIcon.GetComponent<RectTransform> ().localScale = new Vector3 (mainIconScale, mainIconScale, mainIconScale);
            
            if (_transitiveColors.Length > 1)
            {
                Color toColor = SimpleTween.Linear(currentTime, _transitiveColors[_fromColorIndex], _transitiveColors[_toColorIndex], _duration);
                SetColor(toColor);
            }
        } else {
            Reset();
        }
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

        _startTime = Time.time;
    }

	public void SetColor(Color toColor){
        if(_loadingText != null)
        {
			_loadingText.color = toColor;
		}

        for(int i = 0; i < _graphicList.Length; i++)
        {
            _graphicList[i].color = toColor;
        }
	}
}