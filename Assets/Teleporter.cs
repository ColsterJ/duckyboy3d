using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Teleporter : MonoBehaviour {
    public Transform player, playerCamTarget;
    public GameObject fader;
    private Vector3 playerCamTarget_offset, teleportLandDirection = Vector3.right;
    private Breathe pctBreatheCS;
    private Animator faderAnimator; 

    void Start()
    {
        pctBreatheCS = playerCamTarget.GetComponent<Breathe>();
        faderAnimator = fader.GetComponent<Animator>();
    }

    public void TeleportPlayer(Transform where)
    {
        playerCamTarget_offset = playerCamTarget.position - player.position;

        Vector3 nearestGround;
        NearestGround.GetNearestGround(where.position, out nearestGround);

        player.position = nearestGround;
        playerCamTarget.position = nearestGround + playerCamTarget_offset;
    }

    public void SetTeleportDirection(string direction)
    {
        switch (direction)
        {
            case "right":
                teleportLandDirection = Vector3.right;
                break;
            case "left":
                teleportLandDirection = Vector3.left;
                break;
            case "forward":
                teleportLandDirection = Vector3.forward;
                break;
            case "back":
                teleportLandDirection = Vector3.back;
                break;
            default:
                teleportLandDirection = Vector3.right;
                break;
        }
    }
    public void FancyTeleportPlayer(Transform where)
    {
        StartCoroutine(DoFancyTeleport(where));
    }
    IEnumerator DoFancyTeleport(Transform where)
    {
        faderAnimator.SetTrigger("DoFadeOut");
        while ( !faderAnimator.GetCurrentAnimatorStateInfo(0).IsName("IdleFadedOut") && enabled)    // Wait until faded out before teleporting
        {
            yield return new WaitForFixedUpdate();
        }
        TeleportPlayer(where);
        player.GetComponent<PlayerMovement>().FaceDirection(teleportLandDirection);
        yield return new WaitForSeconds(0.5f); // Delay fading back in for 1/2 second, so it doesn't seem too instantaneous (also hides the jank of teleporting)

        faderAnimator.SetTrigger("DoFadeIn");
        yield return null;
    }
}
