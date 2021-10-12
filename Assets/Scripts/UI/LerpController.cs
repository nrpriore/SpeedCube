using UnityEngine;
using UnityEngine.UI;

// Class to normalize common Lerpable components and animations
public class LerpController : MonoBehaviour {
	private const float LERP_THRESHOLD = 0.005f;	// Precision before Lerping stops
	private const float DEFAULT_SPEED = 10f;		// Default Lerp speed if _speed isn't set

	// Public getters
	public float Speed {get{ return _speed;}}
	public Vector3 TargetPos {get{ return _targetPos;}}
	public Vector2 TargetSize {get{ return _targetSize;}}
	public Vector3 TargetScale {get{ return _targetScale;}}
	public float TargetAlpha {get{ return _targetAplha;}}
	public RectTransform rectTransform {get{ return _rt;}}
	public Image image {get{ return _image;}}
	
	private float _speed = 0;		// Current speed of the LerpedObject
	private Vector3 _targetPos;		// Target position of the LerpedObject
	private Vector2 _targetSize;	// Target size of the LerpedObject
	private Vector3 _targetScale;	// Target scale of the LerpedObject
	private float _targetAplha;		// Target alpha of the LerpedObject Image

	private RectTransform _rt;		// Quick reference to the RectTransform of the GameObject
	private Image _image;			// Quick reference to the Image of the GameObject


	// Monobehaviour --------------------------------------------------------------------------- //
	// Runs when this component is added to the Lerpable GameObject
	void Awake() {
		// Ensure we have a RectTransform to reference
		_rt = gameObject.GetComponent<RectTransform>();
		if(_rt == null) {
			throw new System.Exception(gameObject.name + " has no RectTransform so a LerpController cannot be added!");
		}
		_image = gameObject.GetComponent<Image>();

		// Default values
		if(_speed == 0) {
			SetSpeed(DEFAULT_SPEED);
		}
		
		_targetPos = _rt.anchoredPosition;
		_targetSize = _rt.sizeDelta;
		_targetScale = _rt.localScale;
		_targetAplha = _image.color.a;
	}

	public LerpController Init(float speed = DEFAULT_SPEED) {
		SetSpeed(speed);
		return this;
	}
	
	// Runs every frame
	void Update() {
		// Position
		if(Vector3.Distance(_rt.anchoredPosition, _targetPos) > LERP_THRESHOLD) {
			_rt.anchoredPosition = Vector3.Lerp(_rt.anchoredPosition, _targetPos, Time.deltaTime * _speed);
		}

		// Size
		if(Vector2.Distance(_rt.sizeDelta, _targetSize) > LERP_THRESHOLD) {
			_rt.sizeDelta = Vector2.Lerp(_rt.sizeDelta, _targetSize, Time.deltaTime * _speed);
		}

		// Scale
		if(Vector3.Distance(_rt.localScale, _targetScale) > LERP_THRESHOLD) {
			_rt.localScale = Vector3.Lerp(_rt.localScale, _targetScale, Time.deltaTime * _speed);
		}

		// Alpha
		if(_image != null) {
			if(Mathf.Abs(_image.color.a - _targetAplha) > LERP_THRESHOLD) {
				float alpha = Mathf.Lerp(_image.color.a, _targetAplha, Time.deltaTime * _speed);
				Functions.SetImageAlpha(_image, alpha);
			}
			else if(_image.color.a != _targetAplha) {
				Functions.SetImageAlpha(_image, _targetAplha);
			}
		}
	}


	// Public Methods -------------------------------------------------------------------------- //
	// Override the DEFAULT_SPEED
	public void SetSpeed(float speed) {
		_speed = speed;
	}

	// Set the target position
	public void SetTargetPosition(Vector3 targetPos) {
		_targetPos = targetPos;
	}
	// Instantly set the position
	public void SetPosition(Vector3 pos) {
		_rt.anchoredPosition = _targetPos = pos;
	}

	// Set the target size
	public void SetTargetSize(Vector3 targetSize) {
		_targetSize = targetSize;
	}
	// Instantly set the size
	public void SetSize(Vector3 size) {
		_rt.sizeDelta = _targetSize = size;
	}

	// Set the target scale
	public void SetTargetScale(Vector3 targetScale) {
		_targetScale = targetScale;
	}
	// Instantly set the scale
	public void SetScale(Vector3 scale) {
		_rt.localScale = _targetScale = scale;
	}
	
	// Set the target alpha
	public void SetTargetAlpha(float targetAlpha) {
		if(_image == null) {
			Debug.Log(gameObject.name + " has no Image so the target alpha will not be changed");
		}
		_targetAplha = targetAlpha;
	}
	// Instantly set the alpha
	public void SetAlpha(float alpha) {
		Functions.SetImageAlpha(_image, alpha);
		_targetAplha = alpha;
	}
}
