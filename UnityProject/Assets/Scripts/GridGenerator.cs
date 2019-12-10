/*
    Generate a grid of the given size in 3D space.
 */

using UnityEngine;
using UnityEngine.UI;

public class GridGenerator : MonoBehaviour {

    public Toggle cam3dToggle;
    public GameObject camera3D;
    public int xBlocks = 9, yBlocks = 16;
    [HideInInspector]
    public GameObject playerPrefab;

    private GameObject instantiatedPlayer;

    public void GenerateGrid() {
        // continue only if the asset bundle is loaded
        if (playerPrefab == null) {
            return;
        }
        // reset/clear everything
        while (this.transform.childCount > 0) {
            DestroyImmediate(this.transform.GetChild(0).gameObject);
        }
        if (instantiatedPlayer != null) {
            DestroyImmediate(instantiatedPlayer);
        }
        camera3D.transform.eulerAngles = Vector3.zero;
        // Get Bounds
        Vector3 min = Camera.main.ScreenToWorldPoint(new Vector3(0f, 0f, 100f));
        Vector3 max = Camera.main.ScreenToWorldPoint(new Vector3(Screen.width, Screen.height, 100f));
        // calc lengths
        float xLen = (max.x - min.x) / (xBlocks - 1);
        float zLen = (max.z - min.z) / (yBlocks - 1);
        // and set the position...........
        for (int i = 0; i < yBlocks; i++) {
            for (int j = 0; j < xBlocks; j++) {
                GameObject g = GameObject.CreatePrimitive(PrimitiveType.Sphere);
                g.transform.position = min + new Vector3(xLen * j, 0f, zLen * i);
                g.transform.localScale = Vector3.one;
                g.transform.SetParent(this.transform);
                g.name = i + "" + j;
                g.tag = "Grid";
            }
        }

        // all set. Instantiate the player...
        instantiatedPlayer = Instantiate(playerPrefab);
        instantiatedPlayer.GetComponent<PlayerController>().camera3D = camera3D;
        instantiatedPlayer.GetComponent<PlayerController>().is3dEnabled = cam3dToggle.isOn;
        instantiatedPlayer.GetComponent<PlayerController>().maxX = xBlocks;
        instantiatedPlayer.GetComponent<PlayerController>().maxY = yBlocks;
        // set the current position of the player
        instantiatedPlayer.GetComponent<PlayerController>().currX = (int)(xBlocks / 2);
        instantiatedPlayer.GetComponent<PlayerController>().currY = (int)(yBlocks / 2);
        instantiatedPlayer.transform.position = min + new Vector3(xLen * (int)(xBlocks / 2), 0f, zLen * (int)(yBlocks / 2));
        instantiatedPlayer.SetActive(true);
    }
}