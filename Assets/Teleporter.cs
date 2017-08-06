using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Teleporter : MonoBehaviour {
    public Transform player, playerCamTarget;
    private Vector3 playerCamTarget_offset;

    void Start()
    {
        playerCamTarget_offset = playerCamTarget.position - player.position;
    }

    public void TeleportPlayer(Transform where)
    {
        
        playerCamTarget.gameObject.SetActive(false);
        player.position = where.position;
        playerCamTarget.position = where.position + playerCamTarget_offset;
        playerCamTarget.gameObject.SetActive(true);
    }
}
