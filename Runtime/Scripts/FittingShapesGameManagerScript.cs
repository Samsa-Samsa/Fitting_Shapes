using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using  UnityEngine.SceneManagement;
using UnityEngine.UI;
using Random = UnityEngine.Random;

//using static UnityEngine.InputManagerEntry;

public class GameManagerScript : MonoBehaviour
{
    // Start is called before the first frame update

    public AudioManagerScript audioManagerScript;
    [SerializeField] private Button homeButton;
    

    [HideInInspector]
    public List<GameObject> ActiveToys = new List<GameObject>();

    [HideInInspector]
    public bool Interacting;

    List<int> ToyIds = new List<int>();

    public Sprite[] ToySprites;

    public GameObject[] ToySlots;
    public GameObject GuiderObj;

    public int GuiderMovmentSpeed;
    public float GuiderSpawnTime;
    public float GuiderWaitingTime;


    public GameObject CartrigeSample;
    public GameObject text;

    public GameObject[] CartrigeStartingPositions;

    public Transform SpawnPosition;


    public GameObject KonfetiVfx;
    public GameObject[] FireWorkVfxes;

    public float FireworkExplosionInterval;

    public static int CartrigeInSlotCount;


    public int CartrigeId;

    bool CoroutineRunning;

    bool Win;

    Coroutine ToyInteractionCo;
    Coroutine GuiderCo;


    Vector2 CompatibleSlotPos;

    bool startguiding;
    private FittingShapesEntryPoint _entryPont;


    void ShufleList<T>(List<T> list)
    {
        int n = list.Count;
        for (int i = n - 1; i > 0; i--)
        {
            int j = Random.Range(0, i + 1);
            T temp = list[i];
            list[i] = list[j];
            list[j] = temp;
        }
    }


    private void Awake()
    {
        homeButton.onClick.AddListener(FinishOnButton);
    }

    void SetCompatibleSlotPos(GameObject ToyCartrige)
    {
        for (int i = 0; i < ToySlots.Length; i++)
        {
            if (ToySlots[i].GetComponent<FittingShapesSlotIdScript>().ID == ToyCartrige.GetComponent<CartrigeScript>().Id)
            {
                //print(" comp " + ToySlots[i] + " to " + ToyCartrige);
                CompatibleSlotPos = ToySlots[i].transform.position;
            }
        }
    }


    public void OnToyInteraction()
    {
        GuiderObj.SetActive(false);
        startguiding = false;

        if(ToyInteractionCo != null)
        {
            StopCoroutine(ToyInteractionCo);
        }

        if(GuiderCo != null)
        {
            StopCoroutine(GuiderCo);
        }

        if (!Interacting)
        {
            ToyInteractionCo = StartCoroutine(ToyInteractionTimer());
        }
    }



    IEnumerator GuiderCorutine()
    {
        if (!startguiding)
        {
            GuiderObj.SetActive(true);
            yield return new WaitForSeconds(GuiderWaitingTime);
            startguiding = true;
            //print("move ");
        }
        else
        {
            yield return new WaitForSeconds(GuiderWaitingTime);
            ToyInteractionCo = StartCoroutine(ToyInteractionTimer());
            GuiderObj.SetActive(false);
            //print("disapear ");

        }
    }



    IEnumerator ToyInteractionTimer()
    {
        yield return new WaitForSeconds(GuiderSpawnTime);

        if (ActiveToys.Count > 0)
        {

            int randomNum = Random.Range(0, ActiveToys.Count);

            //print("cnt " + ActiveToys.Count);
            //print("rndm " + randomNum);

            SetCompatibleSlotPos(ActiveToys[randomNum]);
            GuiderObj.transform.position = ActiveToys[randomNum].transform.position;


            //print("ehee start " + randomNum);

            GuiderCo = StartCoroutine(GuiderCorutine());
        }
    }



    IEnumerator CartrigeControll()
    {
        CoroutineRunning = true;

        for (int i = 0; i < CartrigeStartingPositions.Length; i++)
        {
            if (!CartrigeStartingPositions[i].GetComponent<CartrigePositionScript>().IsOcupied)
            {

                yield return new WaitForSeconds(1);


                if (ToyIds.Count > 0)
                {
                    //print("new toy");
                    GameObject Toy = Instantiate(CartrigeSample);

                    Toy.transform.position = SpawnPosition.position;

                    //print("ratree " + ResponsivePosition.ScreenRatioForTree);


                    Toy.transform.localScale = new Vector2(Toy.transform.localScale.x * ResponsivePosition.ScreenRatioForTree,
                       Toy.transform.localScale.y * ResponsivePosition.ScreenRatioForTree);


                    //Toy.transform.localScale = new Vector2(2.12f, 2.13f);


                    Toy.GetComponent<CartrigeScript>().CartrigeStartPos = CartrigeStartingPositions[i].transform;

                    Toy.GetComponent<CartrigeScript>().spriteRenderer.sprite = ToySprites[ToyIds[0]];
                    Toy.name = "Toy ID " + (ToyIds[0] + 1);

                    Toy.GetComponent<CartrigeScript>().audioManagerScript = audioManagerScript;
                    Toy.GetComponent<CartrigeScript>().gameManagerScript = GetComponent<GameManagerScript>();
                    Toy.GetComponent<CartrigeScript>().Id = ToyIds[0] + 1;

                    ToyIds.RemoveAt(0);
                    CartrigeStartingPositions[i].GetComponent<CartrigePositionScript>().IsOcupied = true;

                    //SetCompatibleSlotPos(Toy);

                    ActiveToys.Add(Toy);

                    if(ActiveToys.Count >= CartrigeStartingPositions.Length)
                    {
                        if(ToyInteractionCo != null)
                        {
                            StopCoroutine(ToyInteractionCo);
                        }
                        ToyInteractionCo = StartCoroutine(ToyInteractionTimer());
                    }

                }
            }
        }

        CoroutineRunning = false;

    }



    IEnumerator HandleFireworks()
    {
        yield return new WaitForSeconds(1);

        for (int i = 0; i < FireWorkVfxes.Length; i++)
        {
            FireWorkVfxes[i].SetActive(true);
            audioManagerScript.PlayFireworksSound();
            yield return new WaitForSeconds(FireworkExplosionInterval);
        }
    }


    void PlayWiningAction()
    {
        KonfetiVfx.SetActive(true);
        StartCoroutine(HandleFireworks());

    }



    void Start()
    {

        for (int i = 0; i < ToySprites.Length; i++)
        {
            ToyIds.Add(i);
        }

        Application.targetFrameRate = 120;

        ShufleList(ToyIds);

    }

    // Update is called once per frame
    void Update()
    {


        if (startguiding)
        {
            GuiderObj.transform.position = 
                Vector2.MoveTowards(GuiderObj.transform.position, CompatibleSlotPos, GuiderMovmentSpeed * Time.deltaTime);

            if (Vector2.Distance(GuiderObj.transform.position, CompatibleSlotPos) < 0.05f)
            {
                StartCoroutine(GuiderCorutine());
                startguiding = false;
            }
        }




        if (!CoroutineRunning)
        {
            StartCoroutine(CartrigeControll());
        }

        if (CartrigeInSlotCount >= ToySprites.Length && !Win)
        {
           // print("win");
            SetFinishForPackage();
            PlayWiningAction();

            Win = true;
            

        }

    }

    public void SetEntryPoint(FittingShapesEntryPoint EntryPoint)
    {
        _entryPont = EntryPoint;
    }

    private void SetFinishForPackage()
    {
        StartCoroutine(FinishAfterFireworks());
    }

    private IEnumerator FinishAfterFireworks()
    {
        yield return new WaitForSecondsRealtime(5f);
        
    }

    private void FinishOnButton()
    {
        _entryPont.InvokeGameFinished();
    }
}
