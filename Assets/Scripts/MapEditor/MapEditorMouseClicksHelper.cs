#if (UNITY_EDITOR)
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class MapEditorMouseClicksHelper : MonoBehaviour
{
    public Camera cam;
    public MapEditor mapEditor;

    Vector3Int? startingCoordinate;
    Vector3Int? endingCoordinate;




    private void Start()
    {
        if (this.cam == null)
        {
            if (GameObject.FindGameObjectWithTag("MainCamera").GetComponent<Camera>())
            {
                this.cam = GameObject.FindGameObjectWithTag("MainCamera").GetComponent<Camera>();
                this.mapEditor = MapEditor.sharedInstance;
            }
            else
            {
                Debug.Log("No se ha encontrado mapa");
            }
        }

        if (this.mapEditor == null)
        {
            this.mapEditor = MapEditor.sharedInstance;
        }

    }


    private void Update()
    {
        switch (MapEditor.sharedInstance.brushStyle)
        {
            case TileBrushStyle.SingleTile:
                if (Input.GetMouseButton(0))
                {
                    if (Cast2DRayOnMousePosition().collider == null)
                    {
                        this.mapEditor.DrawTile(GetMouseCoordinate());
                    }
                }
                else if (Input.GetMouseButton(1))
                {
                    RaycastHit2D hit = Cast2DRayOnMousePosition();

                    if (Cast2DRayOnMousePosition().collider == null)
                    {
                        this.mapEditor.currentMap.mapMatrix.DestroyObjectAt((Vector2Int)GetMouseCoordinate());
                    }
                    /*
                    if (hit.collider != null)
                    {
                        switch (hit.collider.tag)
                        {
                            case "Tile":
                                Vector2 mouseWorldPosition = this.cam.ScreenToWorldPoint(Input.mousePosition);
                                Vector3Int coordinate = MapEditor.sharedInstance.selectedTilemap.WorldToCell(mouseWorldPosition);
                                MapEditor.sharedInstance.currentMap.tileMatrix.RemoveObjectAt(coordinate);

                                break;
                        }
                    }*/
                }
                break;
            case TileBrushStyle.Section:
                if (Input.GetMouseButtonDown(0))
                {
                    startingCoordinate = GetMouseCoordinate();
                }
                else if (Input.GetMouseButtonUp(0))
                {
                    if (startingCoordinate != null)
                    {
                        endingCoordinate = GetMouseCoordinate();

                        int x = endingCoordinate.Value.x - startingCoordinate.Value.x;
                        int y = endingCoordinate.Value.y - startingCoordinate.Value.y;

                        float signX = Mathf.Sign(x);
                        float signY = Mathf.Sign(y);


                        for (int i = 0; i <= Mathf.Abs(y); i++)
                        {
                            for (int j = 0; j <= Mathf.Abs(x); j++)
                            {
                                this.mapEditor.DrawTile(new Vector3Int(startingCoordinate.Value.x + ((int)signX * j), startingCoordinate.Value.y + ((int)signY * i), startingCoordinate.Value.z));
                            }
                        }
                        this.startingCoordinate = null;
                        this.endingCoordinate = null;
                    }
                    
                }
                if (Input.GetMouseButtonDown(1))
                {
                    this.startingCoordinate = null;
                    this.endingCoordinate = null;
                }
                break;
        }
    }





    private RaycastHit2D Cast2DRayOnMousePosition(float maxLength = 1000f)
    {
        Vector2 mouseWorldPosition = this.cam.ScreenToWorldPoint(Input.mousePosition);
        Ray ray = new Ray(mouseWorldPosition, Vector2.zero);

        return Physics2D.Raycast(ray.origin, ray.direction, maxLength);
    }



    private Vector3Int GetMouseCoordinate()
    {
        Vector2 mouseWorldPosition = this.cam.ScreenToWorldPoint(Input.mousePosition);
        return this.mapEditor.selectedTilemap.WorldToCell(mouseWorldPosition);
    }

}
#endif
