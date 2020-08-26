using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DotRunningLoading_z2_Element : MonoBehaviour
{
	Image _mainIcon;
	float _startTime;

	float _duration;

	float _minScale;
	float _maxScale;

	float _minAlpha;
	float _maxAlpha;

    Image[] _graphicList;

	public void StartAnimation(float duration, float minScale, float maxScale, float minAlpha, float maxAlpha, float delay){
        _mainIcon = transform.GetChild(0).GetComponent<Image>();
        _graphicList = GetComponentsInChildren<Image>(true);

		_mainIcon.GetComponent<RectTransform> ().localScale = new Vector3 (minScale, minScale, minScale);

		Color mainIconColor = _mainIcon.color;
		mainIconColor.a = 0;
		_mainIcon.color = mainIconColor;

		_duration = duration;

		_minScale = minScale;
		_maxScale = maxScale;

		_minAlpha = minAlpha;
		_maxAlpha = maxAlpha;

		_startTime = Time.time + delay;
	}

	public void UpdateAnimation(){		
		float currentTime = Time.time - _startTime;
		if(currentTime < 0){
			return;
		}

		if (currentTime <= _duration) {
			float mainIconScale = SimpleTween.EaseOutQuat (currentTime, _minScale, _maxScale, _duration);
			_mainIcon.GetComponent<RectTransform> ().localScale = new Vector3 (mainIconScale, mainIconScale, mainIconScale);

			Color mainIconColor = _mainIcon.color;
			if (currentTime <= _duration) {
				mainIconColor.a = SimpleTween.EaseOutQuat (currentTime, _maxAlpha, _minAlpha, _duration);
			}

			_mainIcon.color = mainIconColor;
		} else {
			_startTime = Time.time;
		}
	}

	public void SetColor(Color toColor){
        for(int i = 0; i < _graphicList.Length; i++)
        {
            _graphicList[i].color = toColor;
        }
	}
}