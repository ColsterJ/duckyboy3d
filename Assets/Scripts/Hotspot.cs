using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Hotspot : MonoBehaviour {
    public UnityEvent interact;
    public bool fovKick = false;
    public bool useCustomPopAmount = false;
    public bool useCustomPopDuration = false;
    public float customPopAmount = 15.0f;
    public float customPopDuration = 1.0f;
    public Fungus.Flowchart flowchart;
    [HideInInspector]
    public bool interacting = false;
    private FovPopper fp;
    private HotspotManager mgr;
    private GameManager gameMgr;

    void Start()
    {
        if(fovKick)
            fp = GameObject.Find("Player Camera").GetComponent<FovPopper>();
        mgr = GameObject.Find("Hotspot Manager").GetComponent<HotspotManager>();
        gameMgr = GameObject.Find("Game Manager").GetComponent<GameManager>();

        if (flowchart == null)
            flowchart = GameObject.Find("Flowchart").GetComponent<Fungus.Flowchart>();
    }

    public void Interact()
    {
        interacting = true;
        mgr.HideInteractPrompt();
        mgr.SetPlayerMovement(false);
        interact.Invoke();
        StartCoroutine(WaitForFungusOrUI());

        if (useCustomPopAmount)
            fp.SetFovPopAmount(customPopAmount);
        if (useCustomPopDuration)
            fp.SetFovPopDuration(customPopDuration);

        if(fp) fp.DoFovPopIn();
    }
    public void EndInteraction() // Still public because it's still used in UnityEvents for teleporting to blue door apartment for now
    {
        interacting = false;
        if(fp) fp.DoFovPopOut();
        mgr.SetPlayerMovement(true);
    }

    IEnumerator WaitForFungusOrUI()
    {
        while (flowchart.HasExecutingBlocks() || gameMgr.awaitingUIFeedback)
        {
            yield return new WaitForSeconds(0.125f); // There could be a gap between a block executing and awaitingUIFeedback being set, ergo
                                                    // the semi-slow delay between checking both so EndInteraction() doesn't get called prematurely,
                                                    // allowing player to move during cutscene events
        }
        EndInteraction();
        yield return null;
    }
}
