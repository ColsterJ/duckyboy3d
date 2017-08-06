using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Teleporter : MonoBehaviour {
    public Transform player, playerCamTarget;
    private Vector3 playerCamTarget_offset;
    private Breathe pctBreatheCS;

    void Start()
    {
        pctBreatheCS = playerCamTarget.GetComponent<Breathe>();
    }

    public void TeleportPlayer(Transform where)
    {
        //playerCamTarget_offset = playerCamTarget.position - pctBreatheCS.target.position;
        playerCamTarget_offset = playerCamTarget.position - player.position;

        Vector3 nearestGround;
        NearestGround.GetNearestGround(where.position, out nearestGround);

        //playerCamTarget.gameObject.SetActive(false);
        player.position = nearestGround;
        playerCamTarget.position = nearestGround + playerCamTarget_offset;
        //playerCamTarget.gameObject.SetActive(true);
    }
}
