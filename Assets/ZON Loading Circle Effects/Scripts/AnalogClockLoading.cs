using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AnalogClockLoading : MonoBehaviour
{
    public RectTransform _hourHand;
    public RectTransform _minuteHand;

    public Text _loadingText;

    public float _duration = 1;

    public Color[] _transitiveColors;//Length > 1
	Image[] _graphicList;
    int _fromColorIndex;
    int _toColorIndex = 0;

    float _startTime;
    float _currentHour = 0;

    void Start(){
		_graphicList = GetComponentsInChildren<Image>(true);
        
        if(_transitiveColors.Length > 1){
            SetColor(_transitiveColors[0]);            
        }

        ResetClock();
    }

    void SetColor(Color toColor){
        if(_loadingText != null){
            _loadingText.color = toColor;
        }

		for(int i = 0; i < _graphicList.Length; i++)
		{
			_graphicList[i].color = toColor;
		}
    }

    void Update(){
        float currentTime = Time.time - _startTime;
        if (currentTime <= _duration) {
            float currentAngle = SimpleTween.Linear(currentTime, 0, -360, _duration);
            _minuteHand.localEulerAngles = new Vector3 (0, 0, currentAngle);

            currentAngle = SimpleTween.Linear(currentTime, -_currentHour * 30, -_currentHour * 30 - 30, _duration);
            _hourHand.localEulerAngles = new Vector3 (0, 0, currentAngle);

            if(_transitiveColors.Length > 1){
                Color toColor = SimpleTween.Linear(currentTime, _transitiveColors[_fromColorIndex], _transitiveColors[_toColorIndex], _duration);
                SetColor(toColor);
            }
        } else {
            //Ends 1 clock cycle
            ResetClock();

            _currentHour++;
            _currentHour = _currentHour % 12;
        }
    }

    void ResetClock(){
        //Get transitive color index
        if(_transitiveColors.Length > 1){
            _fromColorIndex = _toColorIndex;
            _toColorIndex ++;

            if(_toColorIndex >= _transitiveColors.Length){
                _toColorIndex = 0;
            }
        }

       _startTime = Time.time;
    }
}