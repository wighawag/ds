using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TravelMarkerController : MonoBehaviour
{
    [SerializeField]
    Transform destinationMarker;

    [SerializeField]
    ParabolicLineController line;

    private void Start()
    {
        destinationMarker.gameObject.SetActive(false);
    }

    //private void Update()
    //{
    //    if (MapInteractionManager.CurrentSelectedCell != null && MapManager.isMakingMove)
    //    {
    //        if (MapInteractionManager.CurrentSelectedCell != MapInteractionManager.CurrentMouseCell)
    //        {
    //            ShowTravelMarkers(
    //                MapManager.instance.grid.CellToWorld(MapInteractionManager.CurrentSelectedCell),
    //                MapManager.instance.grid.CellToWorld(MapInteractionManager.CurrentMouseCell)
    //            );
    //            line.enabled = true;
    //        }
    //        else
    //        {
    //            //HideLine();
    //        }
    //    }
    //}

    public void ShowTravelMarkers(Vector3Int startPos, Vector3Int endPos, bool isCube = true)
    {
        Vector3 startPosWorld = MapManager.instance.grid.CellToWorld(
            GridExtensions.CubeToGrid(startPos)
        );
        Vector3 endPosWorld = MapManager.instance.grid.CellToWorld(
            GridExtensions.CubeToGrid(endPos)
        );
        if (isCube)
            ShowTravelMarkers(startPosWorld, endPosWorld);
        else
            ShowTravelMarkers(
                MapManager.instance.grid.CellToWorld(startPos),
                MapManager.instance.grid.CellToWorld(endPos)
            );
    }

    public void ShowTravelMarkers(Vector3 startPos, Vector3 endPos)
    {
        Vector3 startOffset = new Vector3(startPos.x, startPos.y, MapHeightManager.instance.GetHeightAtPosition(startPos));
        Vector3 endOffset = new Vector3(endPos.x, endPos.y, MapHeightManager.instance.GetHeightAtPosition(endPos));
        Vector3 worldEndPos = endPos;
        line.DrawLine(startOffset, endOffset);
        destinationMarker.position = worldEndPos;
        destinationMarker.gameObject.SetActive(true);
    }

    public void HideLine()
    {
        line.HideLine();
        Destroy(gameObject);
    }
}
