using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class PuffLoading : MonoBehaviour
{
	public PuffLoading_Element _mainIcon;

    public Text _loadingText;

	public float _minScale = 0;
	public float _maxScale = 1;

    public float _appearDuration = 1;
	public float _disappearDuration = 0.3f;

	public Color[] _colors;
	int _currentColorIndex = 0;

    int _priorColorIndex = 0;
    float _startTime;

    void RestartEffect()
    {
        GameObject newItem = Instantiate(_mainIcon.gameObject);
        newItem.transform.SetParent(_mainIcon.transform.parent);
        newItem.transform.position = _mainIcon.transform.position;
        newItem.SetActive(true);
        PuffLoading_Element newItemComponent = newItem.GetComponent<PuffLoading_Element>();
		newItem.GetComponent<PuffLoading_Element>().onFinishDisappear = RestartEffect;

		newItemComponent.StartAnimation(_appearDuration, _disappearDuration, _minScale, _maxScale);

        if (_colors.Length > 0)
        {
            _priorColorIndex = _currentColorIndex;
            _currentColorIndex++;

            if (_currentColorIndex >= _colors.Length)
            {
                _currentColorIndex = 0;
            }

            newItemComponent.SetColor(_colors[_currentColorIndex]);
        }

        _startTime = Time.time;
    }

    void Start()
	{
        if (_colors.Length > 0)
        {
            if (_loadingText != null)
            {
                _loadingText.color = _colors[0];
            }
        }

        _mainIcon.gameObject.SetActive (false);
		_mainIcon.onFinishDisappear = RestartEffect;
		RestartEffect ();
	}

    void Update()
    {
        if (_loadingText != null)
        {
            if (_colors.Length > 0 && _currentColorIndex >= 0)
            {
                float currentTime = Time.time - _startTime;

                if (currentTime <= _appearDuration)
                {
                    Color toColor = SimpleTween.Linear(currentTime, _colors[_priorColorIndex], _colors[_currentColorIndex], _appearDuration);
                    _loadingText.color = toColor;
                }
            }
        }
    }
}