using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class CameraController : MonoBehaviour
{
    public static CameraController sharedInstance;

    public Camera cam;

    public Vector3 offset = new Vector3(0f, 0f, -100f);


    public float minSize = 2f;
    public float maxSize = 5f;


    public float zoomSpeed = 5f;
    public float touchZoomSpeed = 1f;
    float touchesPrevPosDifference, touchesCurPosDifference, zoomModifier;
    private Vector2 firstTouchPrevPos, secondTouchPrevPos;



    public float dragSpeed = 0.3f;
    public float touchDragSpeed = 1f;
    private bool canDragCamera = false;

    Vector2 targetPosition;

    private GameObject target;

    private MouseClicksManager mouseClicksManager;

    private void Awake()
    {
        sharedInstance = this;
        
    }

    // Start is called before the first frame update
    void Start()
    {
        if (GameObject.FindGameObjectWithTag("MainCamera"))
        {
            cam = GameObject.FindGameObjectWithTag("MainCamera").GetComponent<Camera>();
            if (GameObject.FindGameObjectWithTag("Player"))
            {
                this.target = GameObject.FindGameObjectWithTag("Player");

                targetPosition = target.transform.localPosition;
                cam.transform.localPosition = new Vector3(targetPosition.x, targetPosition.y, this.offset.z);
            }
            else
            {
                Debug.Log("No se ha encontrado un objetivo");
            }
        }
        else
        {
            Debug.Log("No se ha encontrado una cámara principal");
        }

        mouseClicksManager = GameManager.sharedInstance.mouseClicksManager;


    }


    private void Update()
    {
        CameraZoom();
        CameraMovement();
    }


    private void CameraZoom()
    {
        if (Application.isMobilePlatform)
        {
            if (Input.touchCount == 2)
            {
                Touch firstTouch = Input.GetTouch(0);
                Touch secondTouch = Input.GetTouch(1);

                this.firstTouchPrevPos = firstTouch.position - firstTouch.deltaPosition;
                this.secondTouchPrevPos = secondTouch.position - secondTouch.deltaPosition;

                touchesPrevPosDifference = (this.firstTouchPrevPos - this.secondTouchPrevPos).magnitude;
                touchesCurPosDifference = (firstTouch.position - secondTouch.position).magnitude;

                this.zoomModifier = (firstTouch.deltaPosition - secondTouch.deltaPosition).magnitude * this.touchZoomSpeed * Time.deltaTime;

                if (this.touchesPrevPosDifference > touchesCurPosDifference)
                {
                    cam.orthographicSize += this.zoomModifier;
                }
                if (this.touchesPrevPosDifference < touchesCurPosDifference)
                {
                   cam.orthographicSize -= this.zoomModifier;
                }

                cam.orthographicSize = Mathf.Clamp(cam.orthographicSize, this.minSize, this.maxSize);

            }
        }
        else
        {
            cam.orthographicSize += -Input.GetAxis("Mouse ScrollWheel") * zoomSpeed * Time.deltaTime;
            cam.orthographicSize = Mathf.Clamp(cam.orthographicSize, this.minSize, this.maxSize);



        }

    }


    private void CameraMovement()
    {


        if (GameManager.sharedInstance.gameState == GameState.Normal || GameManager.sharedInstance.gameState == GameState.SettingFight)
        {
            targetPosition = target.transform.localPosition;
            cam.transform.localPosition = new Vector3(targetPosition.x, targetPosition.y, this.offset.z);
        }
        else if (GameManager.sharedInstance.gameState == GameState.Fighting)
        {
            if (Application.isMobilePlatform)
            {
                if (Input.touchCount == 1 && /*!MouseClicksManager.sharedInstance.IsPointerOverUIObject()*/ !mouseClicksManager.IsPointerOverUIObject())
                {
                    if (Input.GetTouch(0).phase == TouchPhase.Moved)
                    {
                        Vector2 touchDeltaPosition = Input.GetTouch(0).deltaPosition;

                        cam.transform.Translate(new Vector2(-touchDeltaPosition.x * touchDragSpeed * Time.deltaTime , -touchDeltaPosition.y * touchDragSpeed * Time.deltaTime));
                    }
                }
            }
            else
            {
                if (Input.GetMouseButtonDown(0))
                {
                    this.canDragCamera = true;
                }

                if (this.canDragCamera)
                {
                    if (!EventSystem.current.IsPointerOverGameObject())
                    {
                        cam.transform.localPosition += new Vector3(-Input.GetAxis("Mouse X") * dragSpeed, -Input.GetAxis("Mouse Y") * dragSpeed, 0f);
                    }
                    else
                    {
                        this.canDragCamera = false;
                    }
                }

                if (Input.GetMouseButtonUp(0))
                {
                    this.canDragCamera = false;
                }

            }
        }

    }
}
