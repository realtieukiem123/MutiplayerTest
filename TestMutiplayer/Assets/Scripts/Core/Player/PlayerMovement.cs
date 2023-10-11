using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class PlayerMovement : NetworkBehaviour
{
    [Header("Refer")]
    [SerializeField] private InputReader inputReader;
    [SerializeField] private Transform bodyTranform;
    [SerializeField] private Rigidbody2D rb;
    [Header("Setting")]
    [SerializeField] private float moveSpeed = 4f;
    [SerializeField] private float turningRate = 30f;
    private Vector2 preMoveInput;

    void Update()
    {
        if (!IsOwner) { return; }
        float zRotation = preMoveInput.x * -turningRate * Time.deltaTime;
        bodyTranform.Rotate(0f, 0f, zRotation);
    }
    private void FixedUpdate()
    {
        if (!IsOwner) { return; }
        rb.velocity = (Vector2)bodyTranform.up * preMoveInput.y * moveSpeed;
    }
    public override void OnNetworkSpawn()
    {
        if (!IsOwner) { return; }
        inputReader.MoveEvent += HandleMove;
    }
    public override void OnNetworkDespawn()
    {
        if (!IsOwner) { return; }
        inputReader.MoveEvent -= HandleMove;
    }


    private void HandleMove(Vector2 moveInput)
    {
        preMoveInput = moveInput;
    }
}
