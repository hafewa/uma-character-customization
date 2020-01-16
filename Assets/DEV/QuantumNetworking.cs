using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class QuantumNetworking : MonoBehaviourPun
{
    public PhotonView pv;
    public QuantumPlayer qp;
    public QuantumCharacterCustomization qcc;
    public GameObject[] toDestroy;

    private void Start()
    {
        if (!pv.IsMine)
        {
            foreach (var go in toDestroy) Destroy(go);
        }
    }

    public void UpdateNetwork()
    {

    }
}
