using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class SpreadOutLoading : MonoBehaviour
{
	public Image _mainIcon;

    public Text _loadingText;

    public float _minScale = 0;
    public float _maxScale = 1;

	public float _minAlpha = 0;
    public float _maxAlpha = 1;

	public float _duration = 1;
    public float _delayTime = 0.3f;

    public Color[] _switchClors;//Length > 1
    Image[] _graphicList;
    int _currentColorIndex = 0;

	Color _currentTextColor;

    float _startTime;

    void Start(){
        _graphicList = GetComponentsInChildren<Image>(true);
        Reset();

        Color mainIconColor = _mainIcon.color;
        mainIconColor.a = _maxAlpha;
        SetColor(mainIconColor);

        _mainIcon.GetComponent<RectTransform>().localScale = new Vector3(_minScale, _minScale, _minScale);
    }

    void SetColor(Color toColor){
        for(int i = 0; i < _graphicList.Length; i++)
        {
            _graphicList[i].color = toColor;
        }
    }

    void Reset(){
		if(_switchClors.Length > 0){
			if (_loadingText != null) {
				_currentTextColor = _loadingText.color;
			}
            
            _currentColorIndex++;
            if(_currentColorIndex >= _switchClors.Length){
                _currentColorIndex = 0;
            }

			Color toColor = _switchClors [_currentColorIndex];
			toColor.a = _mainIcon.color.a;
            SetColor(toColor);
        }

        _startTime = Time.time;
    }

	void Update(){
        float currentTime = Time.time - _startTime;
        if (currentTime <= _duration) {
			Color mainIconColor = _mainIcon.color;
            mainIconColor.a = SimpleTween.EaseOutQuat(currentTime, _maxAlpha, _minAlpha, _duration);

			SetColor(mainIconColor);

			float mainIconScale = SimpleTween.EaseOutQuat(currentTime, _minScale, _maxScale, _duration);
			_mainIcon.transform.localScale = new Vector3 (mainIconScale, mainIconScale, mainIconScale);

			if (_switchClors.Length > 0) {
				if (_loadingText != null) {
					Color toColor = SimpleTween.EaseOutQuat(currentTime, _currentTextColor, _switchClors [_currentColorIndex], _duration);
					_loadingText.color = toColor;
				}
			}

		} else {
			if (currentTime < _duration + _delayTime) {
				_mainIcon.enabled = false;
			} else {
                Reset();
				_mainIcon.enabled = true;
			}
		}
	}
}