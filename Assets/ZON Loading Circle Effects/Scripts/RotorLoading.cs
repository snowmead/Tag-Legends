using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class RotorLoading : MonoBehaviour
{
	public RectTransform _outsideIcon;
    public RectTransform _insideIcon;

    public Text _loadingText;

	public float _duration = 1;
    public float _delayTime = 0.3f;

	public int _insideSpeedMultiplier = 2;
	public bool _sameDirection = true;
    
    public Color[] _transitiveColors;//Length > 1
    Image[] _graphicList;
    int _fromColorIndex;
    int _toColorIndex = 0;

    float _startTime;

    void Start(){
        _graphicList = GetComponentsInChildren<Image>(true);
        
    	if (_transitiveColors.Length > 1)
        {
        	SetColor(_transitiveColors[0]);
        }

    	Reset();
    }

    void Reset(){
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
			float mainIconRotation = SimpleTween.EaseOutQuat(currentTime, 0, -360, _duration);
			_outsideIcon.localEulerAngles = new Vector3 (0, 0, mainIconRotation);

			if (!_sameDirection) {
				mainIconRotation = -mainIconRotation;
			}

			_insideIcon.localEulerAngles = new Vector3 (0, 0, mainIconRotation * _insideSpeedMultiplier);

		    if (_transitiveColors.Length > 1)
		    {
		        Color toColor = SimpleTween.Linear(currentTime, _transitiveColors[_fromColorIndex], _transitiveColors[_toColorIndex], _duration);
		       	SetColor(toColor);
		    }
		} else {
			if (currentTime > _delayTime + _duration) {
				Reset();
			}
		}
	}
}