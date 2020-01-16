using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UMA;
using UMA.Examples;
using UMA.CharacterSystem;

public class QuantumPlayer : MonoBehaviour
{
    [Header("Quantum Controllers")]
    public QuantumAnimationManager qam;
    public QuantumCharacterCustomization qcc;
    public QuantumNetworking qnet;
    [Header("Player Controller")]
    public Joystick joystick;
    public Rigidbody rb;
    public float speed = 5f, speedRotation = 25f;
    public bool busy;
    public GameObject[] go, ui;

    private IEnumerator Start()
    {
        while(rb == null)
        {
            rb = GetComponent<Rigidbody>();
            yield return new WaitForSeconds(0.25f);
        }
        qcc.Initialize();
        qam.Initialize();
    }

    private void Update()
    {
        if (!qnet.pv.IsMine || !joystick || rb == null || busy) return;
        rb.MovePosition(transform.position + (transform.forward * joystick.Vertical) * Time.deltaTime * speed);
        transform.Rotate(new Vector3(0, joystick.Horizontal, 0) * Time.deltaTime * speedRotation);
    }

    public void Active(GameObject o)
    {
        foreach (var obj in ui) obj.SetActive(false);
        o.SetActive(true);
    }

    public void Customize()
    {
        busy = !busy;
        foreach (var o in go) o.SetActive(!o.activeSelf);
    }
}
