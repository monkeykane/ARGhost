using UnityEngine;
using System.Collections;

public class DragonGyro : MonoBehaviour
{

    #region [Private fields]

    private const float lowPassFilterFactor = 0.2f;



    private readonly Quaternion baseIdentity = Quaternion.Euler(90, 0, 0);

    private readonly Quaternion landscapeRight = Quaternion.Euler(0, 0, 90);

    private readonly Quaternion landscapeLeft = Quaternion.Euler(0, 0, -90);

    private readonly Quaternion upsideDown = Quaternion.Euler(0, 0, 180);



    private Quaternion cameraBase = Quaternion.identity;

    private Quaternion calibration = Quaternion.identity;

    private Quaternion baseOrientation = Quaternion.Euler(90, 0, 0);

    private Quaternion baseOrientationRotationFix = Quaternion.identity;



    private Quaternion referanceRotation = Quaternion.identity;

    private bool debug = true;



    #endregion



    #region [Unity events]



    protected void Start()
    {

        AttachGyro();

    }



    protected void Update()
    {
        if (Input.gyro.enabled == false) return;

        transform.rotation = Quaternion.Slerp(transform.rotation,

            cameraBase * (ConvertRotation(referanceRotation * Input.gyro.attitude) * GetRotFix()), lowPassFilterFactor);

    }

    protected void OnGUI()
    {

        //if (!debug)

        //    return;

        //GUILayout.Label("Orientation: " + Screen.orientation);

        //GUILayout.Label("Calibration: " + calibration);

        //GUILayout.Label("Camera base: " + cameraBase);

        //GUILayout.Label("input.gyro.attitude: " + Input.gyro.attitude);

        //GUILayout.Label("transform.rotation: " + transform.rotation);



        //if (GUILayout.Button("On/off gyro: " + Input.gyro.enabled, GUILayout.Height(100)))
        //{

        //    Input.gyro.enabled = !Input.gyro.enabled;

        //    if (Input.gyro.enabled)
        //        cameraBase = new Quaternion(0, 0, 0, 0);
        //    StartCoroutine(InitCameraBaseOnGyro());


        //}

    }

    public void EnableGyro( bool value )
    {
        Input.gyro.enabled = value;
        //cameraBase = new Quaternion(0, 0, 0, 0);
        //StartCoroutine(InitCameraBaseOnGyro());
    }

    IEnumerator InitCameraBaseOnGyro()
    {
        yield return new WaitForFixedUpdate();
        cameraBase = Quaternion.Inverse(ConvertRotation(referanceRotation * Input.gyro.attitude));
    }


    #endregion



    #region [Public methods]



    /// <summary>

    /// Attaches gyro controller to the transform.

    /// </summary>

    private void AttachGyro()
    {

        ResetBaseOrientation();

        UpdateCalibration(true);

        UpdateCameraBaseRotation(true);

        RecalculateReferenceRotation();

    }

    #endregion



    #region [Private methods]



    /// <summary>

    /// Update the gyro calibration.

    /// </summary>

    private void UpdateCalibration(bool onlyHorizontal)
    {

        if (onlyHorizontal)
        {

            var fw = (Input.gyro.attitude) * (-Vector3.forward);

            fw.z = 0;

            if (fw == Vector3.zero)
            {

                calibration = Quaternion.identity;

            }

            else
            {

                calibration = (Quaternion.FromToRotation(baseOrientationRotationFix * Vector3.up, fw));

            }

        }

        else
        {

            calibration = Input.gyro.attitude;

        }

    }



    /// <summary>

    /// Update the camera base rotation.

    /// </summary>

    /// <param name='onlyHorizontal'>

    /// Only y rotation.

    /// </param>

    private void UpdateCameraBaseRotation(bool onlyHorizontal)
    {

        if (onlyHorizontal)
        {

            var fw = transform.forward;

            fw.y = 0;

            if (fw == Vector3.zero)
            {

                cameraBase = Quaternion.identity;

            }

            else
            {

                cameraBase = Quaternion.FromToRotation(Vector3.forward, fw);

            }

        }

        else
        {

            cameraBase = transform.rotation;

        }

    }



    /// <summary>

    /// Converts the rotation from right handed to left handed.

    /// </summary>

    /// <returns>

    /// The result rotation.

    /// </returns>

    /// <param name='q'>

    /// The rotation to convert.

    /// </param>

    private static Quaternion ConvertRotation(Quaternion q)
    {

        return new Quaternion(q.x, q.y, -q.z, -q.w);

    }



    /// <summary>

    /// Gets the rot fix for different orientations.

    /// </summary>

    /// <returns>

    /// The rot fix.

    /// </returns>

    private Quaternion GetRotFix()
    {
        return Quaternion.identity;
    }



    /// <summary>

    /// Recalculates reference system.

    /// </summary>

    private void ResetBaseOrientation()
    {

        baseOrientationRotationFix = GetRotFix();

        baseOrientation = baseOrientationRotationFix * baseIdentity;

    }



    /// <summary>

    /// Recalculates reference rotation.

    /// </summary>

    private void RecalculateReferenceRotation()
    {

        referanceRotation = Quaternion.Inverse(baseOrientation) * Quaternion.Inverse(calibration);

    }



    #endregion
}
