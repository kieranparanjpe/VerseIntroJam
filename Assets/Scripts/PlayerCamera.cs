using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCamera : MonoBehaviour
{
    [SerializeField] private Transform player;

    public Queue<float> height = new Queue<float>();

    void Start()
    {
        height.Enqueue(3);
    }

    void Update()
    {
        transform.position = new Vector3(player.position.x, Mathf.Lerp(transform.position.y, height.Peek(), 0.01f), -10);
    }
}
