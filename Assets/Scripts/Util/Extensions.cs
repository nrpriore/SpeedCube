using UnityEngine;

public static class Extensions {

	// Get scaled size of RectTransform on same frame that scale was changed - normally have to wait a frame
	// NOTE - travels linearly through parents based on depth. ONLY used in specific circumstances
	public static Vector2 ScaledSize(this RectTransform rectTransform, int parentDepth) {
		float scaleX = rectTransform.localScale.x;
		float scaleY = rectTransform.localScale.y;

		RectTransform currDepth = rectTransform;
		for(int i = 1; i <= parentDepth; i++) {
			currDepth = rectTransform.parent.GetComponent<RectTransform>();
			if(currDepth != null) {
				scaleX *= currDepth.localScale.x;
				scaleY *= currDepth.localScale.y;
				continue;
			}
			break;
		}
		return new Vector2(rectTransform.rect.size.x * scaleX, rectTransform.rect.size.y * scaleY);
	}

	// Get semi-global scale of RectTransform on same frame that scale was changed
	// NOTE - travels linearly through parents based on depth. ONLY used in specific circumstances
	public static Vector2 RecursiveScale(this RectTransform rectTransform, int parentDepth) {
		float scaleX = rectTransform.localScale.x;
		float scaleY = rectTransform.localScale.y;

		RectTransform currDepth = rectTransform;
		for(int i = 1; i <= parentDepth; i++) {
			currDepth = rectTransform.parent.GetComponent<RectTransform>();
			if(currDepth != null) {
				scaleX *= currDepth.localScale.x;
				scaleY *= currDepth.localScale.y;
				continue;
			}
			break;
		}
		return new Vector2(scaleX, scaleY);
	}

}
