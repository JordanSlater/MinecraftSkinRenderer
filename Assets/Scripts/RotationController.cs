using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class RotationController : MonoBehaviour
{
    public float sensitivity = 1;

    public bool ableToRotate = true;

    void Update()
    {
        if (Input.GetMouseButton(0) && ableToRotate)
        {
            float rotateHorizontal = Input.GetAxis("Mouse X");
            float rotateVertical = -Input.GetAxis("Mouse Y");

            // yay angle jank
            var newX = transform.rotation.eulerAngles.x + rotateVertical * 2 * sensitivity;
            if (newX < 270 && newX >= 180)
            {
                newX = 270;
            }
            if (newX > 90 && newX < 180)
            {
                newX = 90;
            }

            Quaternion target = Quaternion.Euler(
                newX,
                transform.rotation.eulerAngles.y + rotateHorizontal * 2 * sensitivity,
                0
            );
            transform.rotation = target;
        }
    }
}
