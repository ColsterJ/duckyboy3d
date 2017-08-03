using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FovPopper : MonoBehaviour {
    public float defaultFovPopAmount = 15.0f, defaultFovPopDurationSeconds = 1.0f;
    private float fovPopAmount, fovPopDurationSeconds;
    private Camera myCam;
    private float orig_cam_fov;

    // Use this for initialization
    void Start () {
        myCam = GetComponent<Camera>();
        orig_cam_fov = myCam.fieldOfView;

        ResetFovPopValues();
    }
	
    public void SetFovPopAmount(float popAmt)
    {
        fovPopAmount = popAmt;
    }
    public void SetFovPopDuration(float popDur)
    {
        fovPopDurationSeconds = popDur;
    }
    public void ResetFovPopValues()
    {
        fovPopAmount = defaultFovPopAmount;
        fovPopDurationSeconds = defaultFovPopDurationSeconds;
    }
    public void DoFovPopIn()
    {
        StartCoroutine(FovPopIn());
    }
    public void DoFovPopOut()
    {
        StartCoroutine(FovPopOut());
    }

    IEnumerator FovPopIn()
    {
        float elapsedTime = 0;
        while (elapsedTime < fovPopDurationSeconds)
        {
            myCam.fieldOfView -= (fovPopAmount/fovPopDurationSeconds) * Time.deltaTime;
            elapsedTime += Time.deltaTime;
            yield return new WaitForEndOfFrame();
        }
    }
    IEnumerator FovPopOut()
    {
        float elapsedTime = 0;
        while (elapsedTime < fovPopDurationSeconds)
        {
            myCam.fieldOfView += (fovPopAmount / fovPopDurationSeconds) * Time.deltaTime;
            elapsedTime += Time.deltaTime;
            yield return new WaitForEndOfFrame();
        }
        ResetFovPopValues();
    }
}
