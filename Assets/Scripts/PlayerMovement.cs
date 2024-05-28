using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class PlayerMovement : MonoBehaviour
{
    [SerializeField] private float acceleration;
    [SerializeField] private float maxSpeed;
    [SerializeField] private float jumpForce;

    private new Rigidbody rigidbody;

    public void SetParams(float acceleration, float maxSpeed)
    {
        if (acceleration > 100)
            this.acceleration = acceleration;
        if (maxSpeed > 1)
            this.maxSpeed = maxSpeed;
    }
    
    void Start()
    {
        rigidbody = GetComponent<Rigidbody>();
        SetParams(GameManager.playerAcceleration, GameManager.playerMoveSpeed);
    }

    void Update()
    {
        if (GameManager.Dead || GameManager.Pause)
        {
            rigidbody.velocity = Vector3.zero;
            return;
        }
        

        if (rigidbody.velocity.x < maxSpeed)
        {
            rigidbody.AddForce(Vector3.right * acceleration * Time.deltaTime, ForceMode.Acceleration);
        }
        

        bool onGround = Physics.Raycast(transform.position + Vector3.up * 0.01f, Vector3.down, 0.02f);

        if (onGround && MicrophoneInput.Begin)
        {
            rigidbody.AddForce(Vector3.up * jumpForce, ForceMode.Acceleration);
        }
    }
    
    void OnCollisionEnter(Collision other)
    {
        if (other.gameObject.tag == "Spike")
        {
            GameManager.Dead = true;
        }

    }
}
