using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WallScript : MonoBehaviour {

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {

    }

    public void setWidth(float width)
    {
        float currentSize = this.GetComponent<MeshRenderer>().bounds.size.x;
        Vector3 newScale = this.transform.localScale;
        newScale.x = width * newScale.x / currentSize;
        this.transform.localScale = newScale;
    }

    public void setHeight(float height)
    {
        float currentSize = this.GetComponent<MeshRenderer>().bounds.size.y;
        Vector3 newScale = this.transform.localScale;
        newScale.y = height * newScale.y / currentSize;
        this.transform.localScale = newScale;
    }

    public void setSize(float width, float height)
    {
        setWidth(width);
        setHeight(height);
    }

    public void setSize(Vector2 size)
    {
        setSize(size.x, size.y);
    }

    public float getWidth()
    {
        return this.GetComponent<MeshRenderer>().bounds.size.x;
    }

    public float getHeight()
    {
        return this.GetComponent<MeshRenderer>().bounds.size.y;
    }

    public Vector2 getSize()
    {
        return this.GetComponent<MeshRenderer>().bounds.size;
    }

    public void setPosition(float posX, float posY)
    {
        Vector3 imagePosition = this.transform.position;
        imagePosition.x = posX;
        imagePosition.y = posY;
        this.transform.position = imagePosition;
    }

    public void setPosition(Vector3 position)
    {
        setPosition(position.x, position.y);
    }
}
