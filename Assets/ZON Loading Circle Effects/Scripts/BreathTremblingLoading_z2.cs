using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class BreathTremblingLoading_z2 : MonoBehaviour
{
	public Image _insideIcon;
	public Image _outsideIcon;

    public TextMeshProUGUI _loadingText;

	public float _insideMinScale = 0.7f;
	public float _insideMaxScale = 1;

	public float _outsideMinScale = 1;
	public float _outsideMaxScale = 1.5f;
    
	public float _expiratoryDuration = 0.5f;
    public float _inhaleDuration = 0.8f;

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

    void SetColor(Color toColor)
    {
        if (_loadingText != null)
        {
            _loadingText.color = toColor;
		}

		for(int i = 0; i < _graphicList.Length; i++)
		{
			if(_graphicList[i] == _outsideIcon)
			{
				Color outsideIconColor = toColor;
				outsideIconColor.a = _outsideIcon.color.a;
				_graphicList[i].color = outsideIconColor;
			}
			else
			{
				_graphicList[i].color = toColor;
			}
		}
    }

    void Update(){    	
        float currentTime = Time.time - _startTime;

		if (currentTime <= _expiratoryDuration) {

			float mainIconScale = SimpleTween.Linear(currentTime, _insideMaxScale, _insideMinScale, _expiratoryDuration);
			_insideIcon.transform.localScale = new Vector3 (mainIconScale, mainIconScale, mainIconScale);

			mainIconScale = SimpleTween.Linear(currentTime, _outsideMinScale, _outsideMaxScale, _expiratoryDuration);
			_outsideIcon.transform.localScale = new Vector3 (mainIconScale, mainIconScale, mainIconScale);

			Color mainIconColor = _outsideIcon.color;
			mainIconColor.a = SimpleTween.Linear(currentTime, 1, 0, _expiratoryDuration);

			_outsideIcon.color = mainIconColor;

			if (_transitiveColors.Length > 1)
            {
            	Color toColor = SimpleTween.Linear(currentTime, _transitiveColors[_fromColorIndex], _transitiveColors[_toColorIndex], _expiratoryDuration);
                SetColor(toColor);
			}

		} else {
			currentTime -= _expiratoryDuration;
			if (currentTime <=  _inhaleDuration) {
				float mainIconScale = SimpleTween.Linear(currentTime, _insideMinScale, _insideMaxScale, _inhaleDuration);
				_insideIcon.transform.localScale = new Vector3 (mainIconScale, mainIconScale, mainIconScale);

				_outsideIcon.enabled = false;
			} else {
				_outsideIcon.enabled = true;
    			Reset();
			}
		}
	}
}