using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class BackgroundScript : MonoBehaviour {
    
    private Vector3 imageSize;
    public float imageRatio { get; private set; }
    float timer = 2f;

	void Start () {
        imageSize = this.GetComponent<SpriteRenderer>().bounds.size;
        //imagePosition = this.transform.position;
        imageRatio = imageSize.x / imageSize.y;

        setWidth(0.9f * getCameraWidth());
        setHeight(getImageWidth() / imageRatio);

        setPosition(0, 0);
    }
	
	void Update () {
        timer -= Time.deltaTime;
        if(timer > 1.5f)
            this.GetComponent<SpriteRenderer>().color = new Color(1, 1, 1, (-2 * timer + 4));
        else if (timer < 0.5f)
            this.GetComponent<SpriteRenderer>().color = new Color(1, 1, 1, 2*timer);

        if (timer < 0)
        {
            SceneManager.LoadScene("MainMenu");
            MyData.ecran = MyData.Ecrans.Accueil;
        }

        setPosition(0, this.transform.position.y + 0.1f*Time.deltaTime);
    }

    public float getCameraWidth()
    {
        return Vector3.Distance(Camera.main.ViewportToWorldPoint(new Vector3(0f, 0.0f, 0f)), Camera.main.ViewportToWorldPoint(new Vector3(1f, 0f, 0f)));
    }

    public float getCameraHeight()
    {
        return Vector3.Distance(Camera.main.ViewportToWorldPoint(new Vector3(0f, 0.0f, 0f)), Camera.main.ViewportToWorldPoint(new Vector3(0f, 1f, 0f)));
    }

    public void setWidth(float width)
    {
        float currentSize = this.GetComponent<SpriteRenderer>().bounds.size.x;
        Vector3 newScale = this.transform.localScale;
        newScale.x = width * newScale.x / currentSize;
        //newScale.y = newScale.x;
        this.transform.localScale = newScale;
    }

    public void setHeight(float height)
    {
        float currentSize = this.GetComponent<SpriteRenderer>().bounds.size.y;
        Vector3 newScale = this.transform.localScale;
        newScale.y = height * newScale.y / currentSize;
        //newScale.x = newScale.y;
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

    public float getImageWidth()
    {
        return this.GetComponent<SpriteRenderer>().bounds.size.x;
    }

    public float getImageHeight()
    {
        return this.GetComponent<SpriteRenderer>().bounds.size.y;
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
