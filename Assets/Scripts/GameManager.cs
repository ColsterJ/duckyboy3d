using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour {
    public GameObject fader;
    public bool hasTuna = false;
    private Animator faderAnimator;
    public bool awaitingUIFeedback = false;

    // Use this for initialization
    void Start () {
        Application.targetFrameRate = 60;
        QualitySettings.vSyncCount = 1;
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;

        faderAnimator = fader.GetComponent<Animator>();
	}
	
	// Update is called once per frame
	void Update () {
        //if (Input.GetKeyDown(KeyCode.Escape))
        // StartCoroutine(DoSmoothQuit());
        if (Input.GetKeyDown(KeyCode.R))
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    IEnumerator DoSmoothQuit()
    {
        faderAnimator.SetTrigger("DoFadeOut");
        while (!faderAnimator.GetCurrentAnimatorStateInfo(0).IsName("IdleFadedOut") && enabled)    // Wait until faded out before teleporting
        {
            yield return new WaitForFixedUpdate();
        }
        Application.Quit();
        yield return null;
    }
}
