using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Item3DView : MonoBehaviour
{

    [SerializeField] private List<Image> _images;
    public void SetSprites(List<Sprite> sprites)
	{
        for(int i = 0; i < sprites.Count; i++)
		{
			_images[i].sprite = sprites[i];
			_images[i].gameObject.SetActive(true);
		}
	}
}
