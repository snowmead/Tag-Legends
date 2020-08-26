using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SandClockLoading : MonoBehaviour
{
    public RectTransform _body;
    public RectTransform _upperSand;
    public RectTransform _lowerSand;

    public Text _loadingText;

    public float _rotateDuration = 0.5f;
    public float _timerDuration = 1;

    public Color[] _transitiveColors;//Length > 1
    Image[] _graphicList;
    int _fromColorIndex;
    int _toColorIndex = 0;

    float _startTime;

    float _fromAngle;

	float _toAngle;

	bool _upToDown = false;

    void Start()
	{
        _graphicList = GetComponentsInChildren<Image>(true);
        
        if(_transitiveColors.Length > 1){
            SetColor(_transitiveColors[0]);            
        }

		SwapSand ();
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

    //Swap lower and upper sand
    void SwapSand(){
        RectTransform temp = _upperSand;
        _upperSand = _lowerSand;
        _lowerSand = temp;
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

        _upToDown = !_upToDown; 
        if (_upToDown) {
            _fromAngle = 0;
            _toAngle = -180;
        } else {
            _fromAngle = -180;
            _toAngle = -360;
        }

        SwapSand ();
    }

    void Update(){
        float currentTime = Time.time - _startTime;
        if(currentTime <= _timerDuration){
			float toPos = _lowerSand.rect.height;
			float sandZPos = SimpleTween.Linear(currentTime, 0, toPos, _timerDuration);
			_upperSand.localPosition = new Vector3 (0, sandZPos, 0);

			sandZPos = SimpleTween.Linear(currentTime, -toPos, 0, _timerDuration);
			_lowerSand.localPosition = new Vector3 (0, sandZPos, 0);

            if(_transitiveColors.Length > 1){
                Color toColor = SimpleTween.Linear(currentTime, _transitiveColors[_fromColorIndex], _transitiveColors[_toColorIndex], _timerDuration);
                SetColor(toColor);
            }
        }
		else if (currentTime <= _rotateDuration + _timerDuration) {
			float currentAngle = SimpleTween.EaseOutQuat(currentTime - _timerDuration, _fromAngle, _toAngle, _rotateDuration);
			_body.localEulerAngles = new Vector3 (0, 0, currentAngle);
		}
		else {			
			//Ends 1 clock cycle
			ResetClock ();
        }
    }
}