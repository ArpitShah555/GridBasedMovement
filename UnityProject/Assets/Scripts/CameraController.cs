/*
    Control the Rotation of the camera on mouse click
 */

using UnityEngine;

public class CameraController : MonoBehaviour {

    Vector3 prevPos;
    float speed = 10f;

    // Update is called once per frame
    void Update() {
#if UNITY_EDITOR
        if (Input.GetMouseButtonDown(1)) {
            prevPos = Input.mousePosition;
        }
        if (Input.GetMouseButton(1)) {
            this.transform.localEulerAngles += new Vector3(0, (Input.mousePosition.x - prevPos.x) * Time.deltaTime * speed, 0);
            prevPos = Input.mousePosition;
        }
#elif UNITY_ANDROID
        // rotate on 2 finger touch
        if (Input.touchCount == 2) {
            if (Input.GetTouch(0).phase == TouchPhase.Moved) {
                this.transform.localEulerAngles += new Vector3(0, Input.GetTouch(0).deltaPosition.x, 0);
            }
        }
#endif
    }
}
