using System.Collections;
using System.Collections.Generic;
using Unity.Collections;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UIElements;

public class CartrigeScript : MonoBehaviour
{


   

    public Animator animator;

    public SpriteRenderer spriteRenderer;

    //public GameObject GuiderObj;

    [HideInInspector]
    public Transform CartrigeStartPos;

    [HideInInspector]
    public AudioManagerScript audioManagerScript;

    [HideInInspector]
    public GameManagerScript gameManagerScript;

    [HideInInspector]
    public int Id;

    [HideInInspector]
    public Vector2 CompatibleSlotPos;


    public float MouseFollowSpeed;
    public float StartPosFollowSpeed;
    public float SlotFollowSpeed;
    public float MovmentSpeed;
    public int RockingSpeed;
    public int MaxRockingAngle;


    Transform DragTransform = null;

    Transform CartrigeTransform = null;


    Vector3 offset;

    Vector2 StartPos;

    Vector2 ToyOutlinePos;

    float Angle;

    int initialOrderInLayer;

    GameObject PreviusObj;


    bool Returning;
    bool OnStartingPos;
    bool OcupiedStatusSeted;
    bool OnWrongSlot;
    bool OnFinalPos;
    bool Rocking;
    bool LeftFinished;
    bool RightFinished;
    bool MoveToCompatibleSlot;

    bool MouseDown;
    bool TouchingCompatibleOutline;




    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.GetComponent<FittingShapesSlotIdScript>().ID == Id)
        {
            // adding compatible slots position as target destination

            //CartrigeTransform = collision.transform;

            TouchingCompatibleOutline = true;
            ToyOutlinePos = collision.transform.position;

            //print("add");

            print("touch comp");

            //CartrigeStartPos.gameObject.GetComponent<CartrigePositionScript>().IsOcupied = false;
        }
        else if (collision.gameObject.GetComponent<Rigidbody2D>() && !Returning)
        {
            // executing actions when wrong slot is touched

            //TouchingCompatibleOutline = false;

            OnWrongSlot = true;
            PreviusObj = collision.gameObject;
        }
    }


    private void OnTriggerExit2D(Collider2D collision)
    {

        if (collision.gameObject.GetComponent<FittingShapesSlotIdScript>().ID == Id && !OcupiedStatusSeted)
        {
            TouchingCompatibleOutline = false;
            print("rst");
        }

        if (collision.gameObject == PreviusObj && !Rocking)
        {
            OnWrongSlot = false;
        }
    }



    void HandleRocking()
    {
        if (Rocking)
        {
            if (Angle < MaxRockingAngle && !LeftFinished)
            {
                Angle = Mathf.Lerp(Angle, Angle + 5, RockingSpeed * Time.deltaTime);
                transform.rotation = Quaternion.Euler(0, 0, Angle);
            }
            else if (Angle >= MaxRockingAngle)
            {
                LeftFinished = true;
            }


            if (Angle > -MaxRockingAngle && !RightFinished && LeftFinished)
            {
                Angle = Mathf.Lerp(Angle, Angle - 5, RockingSpeed * Time.deltaTime);
                transform.rotation = Quaternion.Euler(0, 0, Angle);
            }
            else if (Angle <= -MaxRockingAngle)
            {
                RightFinished = true;
            }


            if (Angle < 0 && LeftFinished && RightFinished)
            {
                Angle = Mathf.Lerp(Angle, Angle + 5, RockingSpeed * Time.deltaTime);
                transform.rotation = Quaternion.Euler(0, 0, Angle);
            }
            else if (LeftFinished && RightFinished)
            {
                transform.rotation = Quaternion.Euler(0, 0, 0);
                LeftFinished = false;
                RightFinished = false;
                Rocking = false;
                OnWrongSlot = false;
            }
        }
    }




    void OnMouseButtonDown()
    {
        MouseDown = true;
        spriteRenderer.sortingOrder = initialOrderInLayer + 1;

        gameManagerScript.Interacting = true;
        gameManagerScript.OnToyInteraction();

        //print("grab");

        audioManagerScript.PlayTapSound();
    }


    void OnMouseButtonUp()
    {
        MouseDown = false;
        gameManagerScript.Interacting = false;
        gameManagerScript.OnToyInteraction();
    }




    void Start()
    {
        initialOrderInLayer = spriteRenderer.sortingOrder;

    }


    void Update()
    {


        if (OnStartingPos)
        {

            if (Input.GetMouseButtonDown(0) && !OcupiedStatusSeted)
            {
                RaycastHit2D hit = Physics2D.Raycast(Camera.main.ScreenToWorldPoint(Input.mousePosition), Vector2.zero);

                if (hit.transform && hit.transform.gameObject == transform.gameObject)
                {
                    DragTransform = hit.transform;

                    OnMouseButtonDown();

                    offset = DragTransform.position - Camera.main.ScreenToWorldPoint(Input.mousePosition);
                }
            }
            else if (Input.GetMouseButtonUp(0))
            {
                DragTransform = null;

                OnMouseButtonUp();
            }



            if (DragTransform != null)
            {
                // move towards mouse pos

                DragTransform.position = Vector3.Lerp(DragTransform.position,
                    new Vector2(Camera.main.ScreenToWorldPoint(Input.mousePosition).x, Camera.main.ScreenToWorldPoint(Input.mousePosition).y)
                    + new Vector2(offset.x, offset.y), MouseFollowSpeed * Time.deltaTime);

                Returning = false;
            }




            if (!MouseDown && TouchingCompatibleOutline && !Returning)
            {
                // move towards compatible slot pos after touching it
                transform.position = Vector2.MoveTowards(transform.position, ToyOutlinePos, SlotFollowSpeed * Time.deltaTime);

                if (!OcupiedStatusSeted)
                {
                    CartrigeStartPos.gameObject.GetComponent<CartrigePositionScript>().IsOcupied = false;
                    spriteRenderer.sortingOrder = initialOrderInLayer;
                    gameManagerScript.ActiveToys.Remove(gameObject);
                    OcupiedStatusSeted = true;
                    GameManagerScript.CartrigeInSlotCount++;

                    audioManagerScript.PlayTouchSound();

                    //print("compatible");
                }

                if (Vector2.Distance(transform.position, ToyOutlinePos) <= 0.05f && !OnFinalPos)
                {
                    //print(" play");

                    animator.SetTrigger("Snap");
                    OnFinalPos = true;
                }
            }
            else if (!MouseDown && !OcupiedStatusSeted)
            {
                // move towards starting pos when slot isnt compatible or isnt touched

                if (OnWrongSlot)
                {
                    if (!Rocking)
                    {
                        audioManagerScript.PlayWrongTouchSound();
                        Rocking = true;
                        print("wrong");
                    }
                }
                else
                {
                    transform.position = Vector2.MoveTowards(transform.position, StartPos, MovmentSpeed * Time.deltaTime);

                    Returning = true;

                    if (Vector2.Distance(transform.position, StartPos) < 0.05f && spriteRenderer.sortingOrder != initialOrderInLayer)
                    {
                        spriteRenderer.sortingOrder = initialOrderInLayer;
                        //print("rst");
                    }
                }
            }

        }
        else
        {
            // move towards initial pos at spawning

            transform.position = Vector2.MoveTowards(transform.position, CartrigeStartPos.position, StartPosFollowSpeed * Time.deltaTime);

            if (Vector2.Distance(transform.position, CartrigeStartPos.position) < 0.03f)
            {
                OnStartingPos = true;
                StartPos = new Vector2(transform.position.x, transform.position.y);
                //print("on start");
            }
        }


        HandleRocking();

    }
}
