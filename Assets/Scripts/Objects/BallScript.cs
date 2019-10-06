using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BallScript : MonoBehaviour {

    static Vector3[] positions = new Vector3[6];
    static float rayonBalle, distancePosEnd;
    private bool collided = false;
    public bool isolated = false, destroy = false, start = false, move = false;
    private Vector3 posEnd = new Vector2(0, 0);
    public Vector2 posStart = new Vector2(0, 0), nouvellePosition = new Vector2(0, 0);
    private string tagString;
    public AudioSource sonTir, sonCollision, sonDisparait;
    [HideInInspector] public bool modeEdition = false;

    void Start () {
        //setSize(.1f*getCameraWidth());
        setSize((1/(float) MyData.nbColonne)*getCameraWidth());
        rayonBalle = 0.5f * getSize();
    }

    private void OnMouseDown()
    {
        if (modeEdition)
        {
            switch (GameObject.Find("EditeurManager").GetComponent<EditeurManagerScript>().touchePressee)
            {
                case "1":
                    GetComponent<SpriteRenderer>().sprite = GameObject.Find("EditeurManager").GetComponent<EditeurManagerScript>().balleRouge;
                    tag = "BalleRouge";
                    break;
                case "2":
                    GetComponent<SpriteRenderer>().sprite = GameObject.Find("EditeurManager").GetComponent<EditeurManagerScript>().balleBleue;
                    tag = "BalleBleue";
                    break;
                case "3":
                    GetComponent<SpriteRenderer>().sprite = GameObject.Find("EditeurManager").GetComponent<EditeurManagerScript>().balleVerte;
                    tag = "BalleVerte";
                    break;
                case "4":
                    GetComponent<SpriteRenderer>().sprite = GameObject.Find("EditeurManager").GetComponent<EditeurManagerScript>().balleMauve;
                    tag = "BalleMauve";
                    break;
                case "5":
                    GetComponent<SpriteRenderer>().sprite = GameObject.Find("EditeurManager").GetComponent<EditeurManagerScript>().balleViolette;
                    tag = "BalleViolette";
                    break;
                case "6":
                    GetComponent<SpriteRenderer>().sprite = GameObject.Find("EditeurManager").GetComponent<EditeurManagerScript>().balleJaune;
                    tag = "BalleJaune";
                    break;
                case "7":
                    GetComponent<SpriteRenderer>().sprite = GameObject.Find("EditeurManager").GetComponent<EditeurManagerScript>().balleOrange;
                    tag = "BalleOrange";
                    break;
                case "8":
                    GetComponent<SpriteRenderer>().sprite = GameObject.Find("EditeurManager").GetComponent<EditeurManagerScript>().block;
                    tag = "Block";
                    break;
                case "9":
                    GetComponent<SpriteRenderer>().sprite = GameObject.Find("EditeurManager").GetComponent<EditeurManagerScript>().balleVide;
                    tag = "Untagged";
                    break;
            }

            if(tag.Equals("Untagged"))
                transform.Find("Ombre").GetComponent<SpriteRenderer>().enabled = false;
            else
                transform.Find("Ombre").GetComponent<SpriteRenderer>().enabled = true;
        }
    }


    void Update () {
        if (start)
            if (MoveTo(posStart))
                start = false;

        if (collided)
           if(MoveTo(posEnd))
                AnalysePostCollision();

        if (destroy)
        {
            setSize(0.9f * getSize());

            if (getSize() < 0.01*getCameraWidth())
                Destroy(gameObject);
        }

        if (move)
        {
            /*
            if (isolated)
            {
                print("Stop moving");
                move = false;
            }
            */
            if (MoveTo(nouvellePosition))
                move = false;
        }
    }

    public float getCameraWidth()
    {
        return Vector3.Distance(Camera.main.ViewportToWorldPoint(new Vector3(0f, 0.0f, 0f)), Camera.main.ViewportToWorldPoint(new Vector3(1f, 0f, 0f)));
    }

    public float getCameraHeight()
    {
        return Vector3.Distance(Camera.main.ViewportToWorldPoint(new Vector3(0f, 0.0f, 0f)), Camera.main.ViewportToWorldPoint(new Vector3(0f, 1f, 0f)));
    }

    public void setSize(float width)
    {
        float currentSize = this.GetComponent<SpriteRenderer>().bounds.size.x;
        Vector3 newScale = this.transform.localScale;
        newScale.x = width * newScale.x / currentSize;
        newScale.y = newScale.x;
        this.transform.localScale = newScale;
    }

    public float getSize()
    {
        return this.GetComponent<SpriteRenderer>().bounds.size.x;
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

    public Vector2 getPosition()
    {
        return new Vector2(this.transform.position.x, this.transform.position.y);
    }

    public void applyForce(Vector2 direction)
    {
        this.GetComponent<Rigidbody2D>().bodyType = RigidbodyType2D.Dynamic;
        this.GetComponent<Rigidbody2D>().gravityScale = 0;
        this.GetComponent<Rigidbody2D>().velocity = 40*rayonBalle*direction.normalized;
        tagString = this.tag;
        this.tag = "Projectile";
    }

    public void playShotSound()
    {
        this.sonTir.Play();
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        sonCollision.Play();
        if (!this.isolated && (collision.gameObject.tag.Contains("Balle") || collision.gameObject.tag.Contains("Block")))
        {
            this.GetComponent<Rigidbody2D>().bodyType = RigidbodyType2D.Kinematic;
            this.GetComponent<Rigidbody2D>().velocity = new Vector2(0, 0);
            this.collided = true;

            //Ajout de la balle à la liste
            GameObject.Find("GameManager").GetComponent<GameManagerScript>().balles.Add(this.gameObject);

            findFinalPosition(collision.transform);
        }
        else if(collision.gameObject.tag == "Plafond")
        {
            this.GetComponent<Rigidbody2D>().bodyType = RigidbodyType2D.Kinematic;
            this.GetComponent<Rigidbody2D>().velocity = new Vector2(0, 0);
            this.collided = true;

            //Ajout de la balle à la liste
            GameObject.Find("GameManager").GetComponent<GameManagerScript>().balles.Add(this.gameObject);

            findFinalPosition();
        }
        else if (collision.gameObject.tag == "Destructor")
        {
            Destroy(gameObject);
        }
    }

    private void findFinalPosition()
    {
        positions[0] = new Vector2( -getCameraWidth() / 2 + rayonBalle * (1 - (MyData.nouvelleLigne % 2) + 2 * (int)((getCameraWidth() / 2 + this.transform.position.x) / (2 * rayonBalle))),
                                    getCameraHeight() / 2 - rayonBalle);
        positions[1] = new Vector2( -getCameraWidth() / 2 + rayonBalle * (3 - (MyData.nouvelleLigne % 2) + 2 * (int)((getCameraWidth() / 2 + this.transform.position.x) / (2 * rayonBalle))),
                                    getCameraHeight() / 2 - rayonBalle);
        
        if(positions[0].x < -getCameraWidth() / 2)
            posEnd = positions[1];
        else if (positions[1].x > getCameraWidth() / 2)
            posEnd = positions[0];
        else if ((this.transform.position - positions[0]).sqrMagnitude < (this.transform.position - positions[1]).sqrMagnitude)
            posEnd = positions[0];
        else
            posEnd = positions[1];
    }

    private void findFinalPosition(Transform transform)
    {
        positions[0] = new Vector2(transform.position.x - 2 * rayonBalle, transform.position.y);
        positions[1] = new Vector2(transform.position.x - rayonBalle, transform.position.y + rayonBalle * Mathf.Sqrt(3));
        positions[2] = new Vector2(transform.position.x + rayonBalle, transform.position.y + rayonBalle * Mathf.Sqrt(3));
        positions[3] = new Vector2(transform.position.x + 2 * rayonBalle, transform.position.y);
        positions[4] = new Vector2(transform.position.x + rayonBalle, transform.position.y - rayonBalle * Mathf.Sqrt(3));
        positions[5] = new Vector2(transform.position.x - rayonBalle, transform.position.y - rayonBalle * Mathf.Sqrt(3));

        distancePosEnd = (this.transform.position - positions[0]).sqrMagnitude;
        posEnd = positions[0];
        foreach (Vector3 pos in positions)
        {
            if (pos.x > (-getCameraWidth() / 2) && pos.x < (getCameraWidth() / 2))
            {
                if ((this.transform.position - pos).sqrMagnitude < distancePosEnd)
                {
                    distancePosEnd = (this.transform.position - pos).sqrMagnitude;
                    posEnd = pos;
                }
            }
        }
    }

    public void AnalysePostCollision()
    {
        if(this.tag != null)            //Test
            this.tag = tagString;
        detecteBallesIdentiques(GameObject.Find("GameManager").GetComponent<GameManagerScript>().balles,
                                GameObject.Find("GameManager").GetComponent<GameManagerScript>().ballesIdentiques);


        if (GameObject.Find("GameManager").GetComponent<GameManagerScript>().ballesIdentiques.Count > 2)
        {
            GameObject.Find("GameManager").GetComponent<GameManagerScript>().destruction = true;
            SortList(GameObject.Find("GameManager").GetComponent<GameManagerScript>().ballesIdentiques);
        }
        else
        {
            GameObject.Find("GameManager").GetComponent<GameManagerScript>().ballesIdentiques.Clear();
            GameObject.Find("Arrow").GetComponent<ArrowScript>().recharge = true;
        }

        collided = false;
    }
    
    public bool MoveTo(Vector3 pos)
    {
        float sqrRemainingDistance = (this.transform.position - pos).sqrMagnitude;

        if (sqrRemainingDistance > float.Epsilon)
        {
            Vector3 newPosition = Vector3.MoveTowards(this.transform.position, pos, 10 * Time.deltaTime);
            this.GetComponent<Rigidbody2D>().MovePosition(newPosition);
        }

        return sqrRemainingDistance < float.Epsilon;
    }
    
    public void detecteBallesIdentiques(List<GameObject> balles, List<GameObject> ballesIdentiques)
    {
        foreach (GameObject obj in balles)
        {
            if (obj != null)
            {
                if (Vector3.Distance(this.transform.position, obj.transform.position) < 2.1f * rayonBalle)
                {
                    if (!ballesIdentiques.Contains(obj) && obj.tag.Equals(this.tag))
                    {
                        ballesIdentiques.Add(obj);
                        obj.GetComponent<BallScript>().detecteBallesIdentiques( GameObject.Find("GameManager").GetComponent<GameManagerScript>().balles, 
                                                                                GameObject.Find("GameManager").GetComponent<GameManagerScript>().ballesIdentiques);
                    }
                }
            }
        }
    }
    /*
    public void detruitBalleIdentique()
    {
        for(int i = GameObject.Find("GameManager").GetComponent<GameManagerScript>().ballesIdentiques.Count - 1; i >= 0; i--)
        {
            GameObject.Find("GameManager").GetComponent<GameManagerScript>().balles.RemoveAt(GameObject.Find("GameManager").GetComponent<GameManagerScript>().balles.IndexOf(GameObject.Find("GameManager").GetComponent<GameManagerScript>().ballesIdentiques[i]));
            Destroy(GameObject.Find("GameManager").GetComponent<GameManagerScript>().ballesIdentiques[i]);
        }

        GameObject.Find("GameManager").GetComponent<GameManagerScript>().ballesIdentiques.Clear();
    }
    */
    public List<GameObject> SortList(List<GameObject> list)
    {
        int lenD = list.Count;
        int j = 0;
        GameObject tmp;
        for (int i = 0; i < lenD; i++)
        {
            j = i;
            for (int k = i; k < lenD; k++)
            {
                if (list[j].transform.position.y < list[k].transform.position.y)
                    j = k;
                else if (list[j].transform.position.y == list[k].transform.position.y)
                    if (list[j].transform.position.x < list[k].transform.position.x)
                        j = k;
            }
            tmp = list[i];
            list[i] = list[j];
            list[j] = tmp;
        }

        return list;
    }
}
