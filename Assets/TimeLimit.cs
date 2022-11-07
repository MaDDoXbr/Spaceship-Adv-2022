using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class TimeLimit : MonoBehaviour
{
    public float Lifetime = 2.5f;

    // Start is called before the first frame update
    IEnumerator Start()
    {
        yield return new WaitForSeconds(Lifetime);
        Destroy(gameObject);
    }

    // Update is called once per frame
    void OnDestroy()
    {
        Done_GameController.Instance.Player.CanFire = true;
        var playershipGO = Done_GameController.Instance.Player.gameObject;
        CustomEvent.Trigger(playershipGO, "togglecanfire", true);
    }
}
