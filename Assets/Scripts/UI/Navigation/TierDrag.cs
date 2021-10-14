using UnityEngine;
using UnityEngine.EventSystems;

// Governs the vertical drag behavior for tier navigation
public class TierDrag : MonoBehaviour, IDragHandler, IEndDragHandler {

	private const float QUICKDRAG_TIME = 0.2f;	// Drags less than this time will count as quick drags
	private const float QUICKDRAG_DIST = 0.2f;	// Quick drags have to move at least this % of screen to count
	private const float LONGDRAG_MIN_Y = 0.3f;	// Long drags must start within this % of their side of the screen
	private const float LONGDRAG_DIST = 0.60f;	// Long drags must move at least this % of screen to count

	private NavController _controller;
	private bool _dragging;
	private float _timer;
	private Vector2 _direction;
	private float _height;

	void Update() {
		if(_dragging) {
			_timer += Time.deltaTime;
		}
	}

	public void Init(NavController navController) {
		_controller = navController;
		_dragging = false;
		_timer = 0;
		_direction = Vector2.zero;
		_height = gameObject.GetComponent<RectTransform>().ScaledSize(0).y;
	}

	public void OnDrag(PointerEventData e) {
		if(!_dragging) {
			_direction = (Mathf.Abs(e.delta.x) >= Mathf.Abs(e.delta.y))? new Vector2(1, 0) : new Vector2(0, 1);
			_dragging = true;
		}
		
		if(_direction.y == 1) {
			_controller.DragTier(e.delta * _direction);
		}
	}

	public void OnEndDrag(PointerEventData e) {
		if(_direction.y == 1) {
			float deltaY = e.position.y - e.pressPosition.y;
			if(_timer <= QUICKDRAG_TIME && Mathf.Abs(deltaY) > _height * QUICKDRAG_DIST) {
				if(deltaY > 0) {
					_controller.UpTier();
				}
				else {
					_controller.DownTier();
				}
			}
			else if(e.position.y < _height * LONGDRAG_MIN_Y && Mathf.Abs(deltaY) > _height * LONGDRAG_DIST) {
				_controller.UpTier();
			}
			else if(e.position.y > _height * (1f - LONGDRAG_MIN_Y) && Mathf.Abs(deltaY) > _height * LONGDRAG_DIST) {
				_controller.DownTier();
			}
			else {
				_controller.ResetTier();
			}
		}
		
		_dragging = false;
		_direction = Vector2.zero;
		_timer = 0;
	}
}
