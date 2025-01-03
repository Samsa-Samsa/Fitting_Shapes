//Create Parent object of game objects which must to scaled on different resolutions and set this code to that parent obj
//Set the main camera to the mainCamera in inspector

//Set the default resolution in Vector2 defaultResolution variable X 1920 : Y 1080

//In the inspector Set Canvas Scaler > Ui Scale Mode to the Scale With Screen Size and adjust Reference Resolution also x1920 y1080 
//Set Screen Match Mode to Match Width Or Height also in Canvas Scaler and set the Value of the Match 0.5

//After that create Ui Panel in the Canvas and set in to cameraRect variable.

using UnityEngine;

public class ResponsivePosition : MonoBehaviour
{
    [SerializeField] private Camera mainCamera;
    [SerializeField] private Vector2 defaultResolution; //1920x1080
    [SerializeField] private RectTransform cameraRect;
    private Rect screenArea;

    public GameObject TreeObject;

    public GameObject[] Vfxes;

    public float maxTreeScale;

    private void Awake() => Test();

    private void Update()
    {
        //if (Input.GetMouseButtonDown(0))
        //{
        //    Test();
        //}

    }

    public static float ScreenRatioForTree;

    private void Test()
    {

        //print(cameraRect.rect.width);
        //print(defaultResolution.x);
        //with scren resolution only
        transform.localScale = Vector3.one;

        transform.position = new Vector3(mainCamera.transform.position.x, mainCamera.transform.position.y, 0);

        float screenRatio = cameraRect.rect.width / defaultResolution.x;


        for (int i = 0; i < Vfxes.Length; i++)
        {
            Vfxes[i].transform.localScale = new Vector3(Vfxes[i].transform.localScale.x * screenRatio,
                Vfxes[i].transform.localScale.y * screenRatio, Vfxes[i].transform.localScale.z * screenRatio);
        }


        ScreenRatioForTree = 1;

        if (TreeObject.transform.localScale.x * screenRatio < maxTreeScale)
        {
            TreeObject.transform.localScale = new Vector2(TreeObject.transform.localScale.x * screenRatio,
                TreeObject.transform.localScale.y * screenRatio);

            ScreenRatioForTree = screenRatio;
        }


        transform.localScale = new Vector3(transform.localScale.x * screenRatio, transform.localScale.y * screenRatio);

        //only with safe area
        // transform.localScale = Vector3.one;
        // transform.position = new Vector3(mainCamera.transform.position.x, mainCamera.transform.position.y, 0);
        // float screenRatio = cameraRect.rect.width / defaultResolution.x;
        // transform.localScale = new Vector3(transform.localScale.x*screenRatio,transform.localScale.y*screenRatio);


        // Debug.Log(screenRatio + " Screen ratio");
        // Debug.Log(cameraRect.rect.width + " Screen size");
        // Debug.Log(Screen.safeArea.width + " Safe area");
    }
}