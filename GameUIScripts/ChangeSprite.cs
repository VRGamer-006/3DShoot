using UnityEngine;
using System.Collections;

public class ChangeSprite : MonoBehaviour {
	public string firstSprite;
	public string secondSprite;
	public UISprite sprite;
	
	public bool canChangeSprite = true;
	
	void OnClick () {
		if(!canChangeSprite) {
			return;
		}
		
		if(sprite.spriteName == firstSprite) {
			sprite.spriteName = secondSprite;
		}
		else {
			sprite.spriteName = firstSprite;
		}
	}
}
