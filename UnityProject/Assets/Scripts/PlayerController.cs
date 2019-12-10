/*
    This script contains the logic for
    players movement and rotation within the grid.
 */

using System.Collections;
using UnityEngine;

public class PlayerController : MonoBehaviour {

    [HideInInspector]
    public int currX = 0, currY = 0;
    [HideInInspector]
    public int maxX = 9, maxY = 16; // the size of the grid
    [HideInInspector]
    public bool is3dEnabled = true;
    [HideInInspector]
    public GameObject camera3D;

    private bool isStarted = false;
    private bool isTouchValid = false;
    private float minTouchTravel = 10f;
    private float rotateTime = 0.4f, moveTime = 0.5f;
    private enum Direction { Up, Down, Left, Right };
    private Direction currentDirection;

    private void OnEnable() {
        // default direction
        currentDirection = Direction.Up;
        // toggle 3D cam if needed
        if (camera3D != null) {
            camera3D.SetActive(is3dEnabled);
        }
    }

    // Update is called once per frame
    void Update() {
        camera3D.transform.position = transform.position;
        if (Input.GetKeyDown(KeyCode.DownArrow)) {
            currentDirection = Direction.Down;
        }
        if (Input.GetKeyDown(KeyCode.UpArrow)) {
            currentDirection = Direction.Up;
        }
        if (Input.GetKeyDown(KeyCode.RightArrow)) {
            currentDirection = Direction.Right;
        }
        if (Input.GetKeyDown(KeyCode.LeftArrow)) {
            currentDirection = Direction.Left;
        }

        // detect and process swipes
        if (Input.touchCount == 1) {
            Touch touch = Input.GetTouch(0);
            switch (touch.phase) {
                case TouchPhase.Began:
                    isTouchValid = true;
                    break;
                case TouchPhase.Moved:
                    // touch is invalid once the required action is performed
                    if (isTouchValid) {
                        Vector3 currentMousePos = Input.mousePosition;
                        if (Mathf.Abs(touch.deltaPosition.x) > minTouchTravel || Mathf.Abs(touch.deltaPosition.y) > minTouchTravel) {
                            // detect swipe direction
                            if (Mathf.Abs(touch.deltaPosition.x) > Mathf.Abs(touch.deltaPosition.y)) {
                                if (touch.deltaPosition.x > 0) {
                                    isTouchValid = false;
                                    if (is3dEnabled) {
                                        UpdateDirection((int)(camera3D.transform.eulerAngles.y + 90));
                                    } else {
                                        currentDirection = Direction.Right;
                                    }
                                } else {
                                    isTouchValid = false;
                                    if (is3dEnabled) {
                                        UpdateDirection((int)(camera3D.transform.eulerAngles.y - 90));
                                    } else {
                                        currentDirection = Direction.Left;
                                    }
                                }
                            } else {
                                if (touch.deltaPosition.y < 0) {
                                    isTouchValid = false;
                                    if (is3dEnabled) {
                                        UpdateDirection((int)(camera3D.transform.eulerAngles.y + 180));
                                    } else {
                                        currentDirection = Direction.Down;
                                    }
                                } else {
                                    if (is3dEnabled) {
                                        UpdateDirection((int)(camera3D.transform.eulerAngles.y + 0));
                                    } else {
                                        currentDirection = Direction.Up;
                                    }
                                }
                            }
                            // start the game if not started
                            if (!isStarted) {
                                isStarted = true;
                                StartCoroutine(MoveNext());
                            }
                        }
                    }
                    break;
            }
        }
    }

    // only in the case where 3D camera is enabled
    void UpdateDirection(int angle) {
        // bring angle between 0 to 360 for example -90=270
        if (angle < 0) { angle += 360; }
        if (angle >= 360) { angle -= 360; }

        // detect direction
        if (angle > 45 && angle < 135) {
            currentDirection = Direction.Right;
        } else if (angle > 135 && angle < 225) {
            currentDirection = Direction.Down;
        } else if (angle > 225 && angle < 315) {
            currentDirection = Direction.Left;
        } else {
            currentDirection = Direction.Up;
        }
    }

    // the most important part of the script.
    // player movement...
    IEnumerator MoveNext() {
        // rotate player only if needed
        switch (currentDirection) {
            case Direction.Up:
                if (this.transform.eulerAngles.y != 0f) {
                    iTween.RotateTo(this.gameObject, iTween.Hash("y", 0f, "time", rotateTime));
                }
                currY++;
                break;
            case Direction.Right:
                if (this.transform.eulerAngles.y != 90f) {
                    iTween.RotateTo(this.gameObject, iTween.Hash("y", 90f, "time", rotateTime));
                }
                currX++;
                break;
            case Direction.Down:
                if (this.transform.eulerAngles.y != 180f) {
                    iTween.RotateTo(this.gameObject, iTween.Hash("y", 180f, "time", rotateTime));
                }
                currY--;
                break;
            case Direction.Left:
                if (this.transform.eulerAngles.y != 270f) {
                    iTween.RotateTo(this.gameObject, iTween.Hash("y", 270f, "time", rotateTime));
                }
                currX--;
                break;
        }

        // screen edge detection
        if (currX < 0) {
            currX = maxX - 1;
            transform.position = FindObjectOfType<GridGenerator>().transform.Find(currY + "" + currX).position;
            currX--;
        }
        if (currX > maxX - 1) {
            currX = 0;
            transform.position = FindObjectOfType<GridGenerator>().transform.Find(currY + "" + currX).position;
            currX++;
        }
        if (currY < 0) {
            currY = maxY - 1;
            transform.position = FindObjectOfType<GridGenerator>().transform.Find(currY + "" + currX).position;
            currY--;
        }
        if (currY > maxY - 1) {
            currY = 0;
            transform.position = FindObjectOfType<GridGenerator>().transform.Find(currY + "" + currX).position;
            currY++;
        }

        // continuous movement in the specified direction
        Vector3 newpos = FindObjectOfType<GridGenerator>().transform.Find(currY + "" + currX).position;
        iTween.MoveTo(gameObject, iTween.Hash("position", newpos, "easeType", "linear", "time", moveTime));
        yield return new WaitForSeconds(moveTime);
        StartCoroutine(MoveNext());
    }
}
