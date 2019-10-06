using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ArrowScript : MonoBehaviour {

    [SerializeField] public Animator animatorPerd;
    private float angle = 0, angleViseur, angleRebond, rayonBalle;
    private Vector2 angleSouris, angleRef, positionImpact;
    public bool recharge = false;
    private bool standby = false;
    private GameObject nouvelleBalle;
    public GameObject viseur, viseurRebond;
    private int nbTir = 0/*, nouvelleLigne = 0*/;
    private RaycastHit2D hitRebond, hit;

    private SpriteRenderer rendererViseur, rendererRebond;
    //TEST offest
    float offset = 0;

    void Start () {
        rayonBalle = 0.05f * getCameraWidth();
        float ratio = getHeight() / getWidth();
        setWidth(0.08f * getCameraWidth());
        setHeight(getWidth() * ratio);
        transform.position = new Vector2(0, -0.48f * getCameraHeight());

        angleRef = -transform.position;

        //Création de la première balle 
        //PremiereBalle();
        
        hitRebond = new RaycastHit2D();
        hit = new RaycastHit2D();

        //Test viseur
        viseur = Instantiate(gameObject.GetComponent<ArrowScript>().viseur,
                                transform.position,
                                Quaternion.identity);

        rendererViseur = viseur.GetComponentInChildren<SpriteRenderer>();
        rendererViseur.enabled = false;

        viseurRebond = Instantiate(gameObject.GetComponent<ArrowScript>().viseur,
                                transform.position,
                                Quaternion.identity);

        rendererRebond = viseurRebond.GetComponentInChildren<SpriteRenderer>();
        rendererRebond.enabled = false;
    }
	
	void Update ()
    {
        if (!MyData.pause)
        {
            if (Input.GetMouseButton(0))    //Détermine l'angle de tir
            {
                angleSouris = Camera.main.ScreenToWorldPoint(Input.mousePosition) - transform.position;
                if (angleSouris.y < 0.2f)
                    angleSouris.y = 0.2f;
                angle = -Mathf.Sign(Camera.main.ScreenToWorldPoint(Input.mousePosition).x) * Vector2.Angle(angleRef, angleSouris);

                gameObject.GetComponent<SpriteRenderer>().transform.rotation = Quaternion.Euler(0, 0, angle);

                if (!standby)
                    Vise();
            }
            else if (Input.GetMouseButtonUp(0) && !standby)     //Tire la balle
            {
                nouvelleBalle.GetComponent<BallScript>().playShotSound();
                nouvelleBalle.GetComponent<BallScript>().applyForce(GameObject.Find("Arrow").GetComponent<ArrowScript>().getDirection());
                nouvelleBalle.layer = 8;
                nbTir++;
                standby = true;

                rendererRebond.enabled = false;
                rendererViseur.enabled = false;
            }

            if (recharge) //Crée une nouvelle balle
            {
                if (!GameObject.Find("GameManager").GetComponent<GameManagerScript>().deuxiemeDetection)
                {
                    if (GameObject.Find("GameManager").GetComponent<GameManagerScript>().ballesDisponibles.Count > 0)
                    {
                        NouvelleBalle();
                        recharge = false;
                        standby = false;

                        if ((nbTir + 1) % (GameObject.Find("GameManager").GetComponent<GameManagerScript>().delaiNouvelleLigne + 1) == 0)
                        {
                            NouvelleLigne();
                            nbTir = 0;
                        }
                        // Niveau perdu
                        else
                            PerdNiveau();
                    }
                    else
                        print("Erreur !!!!!!!!!!!!!!!!");
                }
            }
        }
        
    }
    /*
    public void PremiereBalle()
    {
        nouvelleBalle = Instantiate(GameObject.Find("GameManager").GetComponent<GameManagerScript>().ballPrefabs[Random.Range(0, GameObject.Find("GameManager").GetComponent<GameManagerScript>().ballPrefabs.Length)],
                                    new Vector2(0, -getCameraHeight()),
                                    Quaternion.identity);
        nouvelleBalle.GetComponent<Rigidbody2D>().gravityScale = 0;
        nouvelleBalle.GetComponent<BallScript>().start = true;
        nouvelleBalle.GetComponent<BallScript>().posStart = GameObject.Find("Arrow").transform.position;
        nouvelleBalle.layer = 2;
    }
    */
    public void NouvelleBalle()
    {
        int i = Random.Range(0, GameObject.Find("GameManager").GetComponent<GameManagerScript>().ballesDisponibles.Count);

        nouvelleBalle = Instantiate(GameObject.Find("GameManager").GetComponent<GameManagerScript>().ballesDisponibles[i],
                                    new Vector2(0, -getCameraHeight()),
                                    Quaternion.identity);
        nouvelleBalle.GetComponent<Rigidbody2D>().gravityScale = 0;
        nouvelleBalle.GetComponent<BallScript>().start = true;
        nouvelleBalle.GetComponent<BallScript>().posStart = GameObject.Find("Arrow").transform.position;
        nouvelleBalle.layer = 2;
    }

    public void NouvelleLigne()
    {
        MyData.nouvelleLigne++;
        int nbBlock = 0;

        for (int colonne = 0; colonne < MyData.nbColonne - MyData.nouvelleLigne % 2; colonne++)
        {
            if (!GameObject.Find("GameManager").GetComponent<GameManagerScript>().utiliseBlocks)
            {
                GameObject nouvelleBalle = Instantiate(GameObject.Find("GameManager").GetComponent<GameManagerScript>().ballesDisponibles[Random.Range(0, GameObject.Find("GameManager").GetComponent<GameManagerScript>().ballesDisponibles.Count)],
                                                        new Vector2(-getCameraWidth() / 2 + ((1 + 2 * colonne) + (MyData.nouvelleLigne % 2)) * GameObject.Find("GameManager").GetComponent<GameManagerScript>().rayonBalle,
                                                                    getCameraHeight() / 2 + GameObject.Find("GameManager").GetComponent<GameManagerScript>().rayonBalle * (Mathf.Sqrt(3) - 1)),
                                                        Quaternion.identity);
                GameObject.Find("GameManager").GetComponent<GameManagerScript>().balles.Add(nouvelleBalle);
            }
            //Si les blocks sont utilisés dans le niveau
            else
            {

                if (Random.Range(0, 100) < 83 || nbBlock > 2)
                {
                    GameObject nouvelleBalle = Instantiate(GameObject.Find("GameManager").GetComponent<GameManagerScript>().ballesDisponibles[Random.Range(0, GameObject.Find("GameManager").GetComponent<GameManagerScript>().ballesDisponibles.Count)],
                                                            new Vector2(-getCameraWidth() / 2 + ((1 + 2 * colonne) + (MyData.nouvelleLigne % 2)) * GameObject.Find("GameManager").GetComponent<GameManagerScript>().rayonBalle,
                                                                        getCameraHeight() / 2 + GameObject.Find("GameManager").GetComponent<GameManagerScript>().rayonBalle * (Mathf.Sqrt(3) - 1)),
                                                            Quaternion.identity);
                    GameObject.Find("GameManager").GetComponent<GameManagerScript>().balles.Add(nouvelleBalle);
                }
                //Création des blocks
                else
                {
                    GameObject nouvelleBalle = Instantiate(GameObject.Find("GameManager").GetComponent<GameManagerScript>().block,
                                                            new Vector2(-getCameraWidth() / 2 + ((1 + 2 * colonne) + (MyData.nouvelleLigne % 2)) * GameObject.Find("GameManager").GetComponent<GameManagerScript>().rayonBalle,
                                                                        getCameraHeight() / 2 + GameObject.Find("GameManager").GetComponent<GameManagerScript>().rayonBalle * (Mathf.Sqrt(3) - 1)),
                                                            Quaternion.identity);
                    GameObject.Find("GameManager").GetComponent<GameManagerScript>().balles.Add(nouvelleBalle);
                    GameObject.Find("GameManager").GetComponent<GameManagerScript>().blocks.Add(nouvelleBalle);
                    nbBlock++;
                }
            }
        }

        foreach (GameObject balle in GameObject.Find("GameManager").GetComponent<GameManagerScript>().balles)
        {
            if (balle != null)
            {
                balle.GetComponent<BallScript>().move = true;
                balle.GetComponent<BallScript>().nouvellePosition.Set(balle.transform.position.x, balle.transform.position.y - (Mathf.Sqrt(3)) * GameObject.Find("GameManager").GetComponent<GameManagerScript>().rayonBalle);

                if (balle.transform.position.y < -getCameraWidth() / 2 - rayonBalle)
                {
                    animatorPerd.SetBool("isHidden", false);
                    MyData.pause = true;
                }
            }
        }
    }

    public void PerdNiveau()
    {
        foreach (GameObject balle in GameObject.Find("GameManager").GetComponent<GameManagerScript>().balles)
        {
            if (balle != null)
            {
                if (balle.transform.position.y < -getCameraWidth() / 2 - rayonBalle)
                {
                    animatorPerd.SetBool("isHidden", false);
                    MyData.pause = true;

                    // Admob trigger
                    MyData.gameOver = true;
                    MyData.adTrigger++;
                }
            }
        }
    }

    public void Vise()
    {
        hit = Physics2D.Raycast(transform.position, getDirection());
        rendererViseur.enabled = true;

        if (hit.collider != null)
        {
            if (hit.collider.tag == "Mur")
            {
                rendererRebond.enabled = true;

                if (hit.point.x < 0)
                {
                    /*
                     * Position du point d'impact du rayon de visée:    hit.point
                     * Position du centre de la balle lors de l'impact: new Vector2(hit.point.x + rayonBalle, hit.point.y - (rayonBalle / Mathf.Tan(Mathf.Deg2Rad * angle)))
                     * Position de l'impact de la trajectoire déssinée: new Vector2(hit.point.x, hit.point.y - (rayonBalle / Mathf.Tan(Mathf.Deg2Rad * angle)))
                     * Angle de la trajectoire déssinée:                Mathf.Atan2(hit.point.x - transform.position.x, hit.point.y - (rayonBalle / Mathf.Tan(Mathf.Deg2Rad * angle)) - transform.position.y)
                     */

                    hitRebond = Physics2D.Raycast(new Vector2(hit.point.x + rayonBalle, hit.point.y - (rayonBalle / Mathf.Tan(Mathf.Deg2Rad * angle))), new Vector2(-getDirection().x, getDirection().y));
                    //Debug.DrawLine(new Vector2(hit.point.x + rayonBalle, hit.point.y - (rayonBalle / Mathf.Tan(Mathf.Deg2Rad * angle))), hitRebond.point, Color.red);
                    //Debug.DrawLine(transform.position, new Vector2(hit.point.x + rayonBalle, hit.point.y - (rayonBalle / Mathf.Tan(Mathf.Deg2Rad * angle))), Color.blue);        
                    positionImpact = new Vector2(hit.point.x, hit.point.y - (rayonBalle / Mathf.Tan(Mathf.Deg2Rad * angle)));
                }
                else
                {
                    hitRebond = Physics2D.Raycast(new Vector2(hit.point.x - rayonBalle, hit.point.y + (rayonBalle / Mathf.Tan(Mathf.Deg2Rad * angle))), new Vector2(-getDirection().x, getDirection().y));
                    //Debug.DrawLine(new Vector2(hit.point.x - rayonBalle, hit.point.y + (rayonBalle / Mathf.Tan(Mathf.Deg2Rad * angle))), hitRebond.point, Color.red);
                    //Debug.DrawLine(transform.position, new Vector2(hit.point.x - rayonBalle, hit.point.y + (rayonBalle / Mathf.Tan(Mathf.Deg2Rad * angle))), Color.blue);
                    positionImpact = new Vector2(hit.point.x, hit.point.y + (rayonBalle / Mathf.Tan(Mathf.Deg2Rad * angle)));
                }

                /**** Section qui se trouvait dans les deux accolades au dessus avant "positionImpact" ****/
                rendererViseur.size = new Vector2(rendererViseur.size.x,
                                                        Vector3.Distance(transform.position, positionImpact) / rendererViseur.transform.localScale.y
                                                        );

                rendererRebond.transform.position = positionImpact;
                rendererRebond.size = new Vector2(rendererRebond.size.x,
                                                    Vector3.Distance(positionImpact, hitRebond.point) / rendererRebond.transform.localScale.y
                                                    );
                /******************************************************************************************/

                Vector2 angleTest = new Vector3(positionImpact.x, positionImpact.y, 0) - transform.position;
                angleViseur = -Mathf.Sign(Camera.main.ScreenToWorldPoint(Input.mousePosition).x) * Vector2.Angle(angleRef, angleTest);

                angleTest = hitRebond.point - positionImpact;
                angleRebond = Mathf.Sign(Camera.main.ScreenToWorldPoint(Input.mousePosition).x) * Vector2.Angle(angleRef, angleTest);
            }
            else
            {
                rendererViseur.size = new Vector2(  rendererViseur.size.x,
                                                    Vector3.Distance(transform.position, hit.point) / rendererViseur.transform.localScale.y);
                angleViseur = angle;
                rendererRebond.enabled = false;
            }

            //Angle de la trajectoire
            rendererViseur.transform.rotation = Quaternion.Euler(0, 0, angleViseur);
            rendererRebond.transform.rotation = Quaternion.Euler(0, 0, angleRebond);

            //Animation de la trajectoire
            offset -= 2*Time.deltaTime;
            rendererViseur.material.SetTextureOffset("_MainTex", new Vector2(0, offset));
            rendererRebond.material.SetTextureOffset("_MainTex", new Vector2(0, offset));
        }
    }

    public float getAngle()
    {
        return angle;
    }

    public Vector2 getDirection()
    {
        return angleSouris;
    }

    public void setWidth(float width)
    {
        float currentSize = this.GetComponent<SpriteRenderer>().bounds.size.x;
        Vector3 newScale = this.transform.localScale;
        newScale.x = width * newScale.x / currentSize;
        this.transform.localScale = newScale;
    }

    public void setHeight(float height)
    {
        float currentSize = this.GetComponent<SpriteRenderer>().bounds.size.y;
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
        return this.GetComponent<SpriteRenderer>().bounds.size.x;
    }

    public float getHeight()
    {
        return this.GetComponent<SpriteRenderer>().bounds.size.y;
    }

    public Vector2 getSize()
    {
        return this.GetComponent<SpriteRenderer>().bounds.size;
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

    public float getCameraWidth()
    {
        return Vector3.Distance(Camera.main.ViewportToWorldPoint(new Vector3(0f, 0.0f, 0f)), Camera.main.ViewportToWorldPoint(new Vector3(1f, 0f, 0f)));
    }

    public float getCameraHeight()
    {
        return Vector3.Distance(Camera.main.ViewportToWorldPoint(new Vector3(0f, 0.0f, 0f)), Camera.main.ViewportToWorldPoint(new Vector3(0f, 1f, 0f)));
    }
}
