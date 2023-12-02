using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HeadBobController : MonoBehaviour
{
    [SerializeField] private bool enable = true;
    [SerializeField, Range(0, 0.1f)] private float amplitude = 0.015f;
    [SerializeField, Range(0, 30)] private float frequency = 10.0f;

    [SerializeField] private Transform camera = null;
    [SerializeField] private Transform CameraHolder = null;

    private float toggleSpeed = 3.0f;
    private Vector3 _startPos;
    private CharacterController _controller;

    void Update()
    {
        if (!enable) return;
        CheckMotion();
        camera.LookAt(FocusTarget());
    }
    private void ResetPosition()
    {
        if (camera.localPosition == _startPos) return;
        camera.localPosition = Vector3.Lerp(camera.localPosition, _startPos, 1 * Time.deltaTime);
    }

    private void Awake()
    {
        _controller = GetComponent<CharacterController>();
        _startPos = camera.localPosition;

    }
    private void PlayMotion(Vector3 motion)
    {
        camera.localPosition += motion;
    }
    private void CheckMotion()
    {
        float speed = new Vector3(_controller.velocity.x, 0, _controller.velocity.z).magnitude;
        ResetPosition();
        if (speed < toggleSpeed) return;
        if (!_controller.isGrounded) return;

        PlayMotion(FootStepMotion());
    }
    private Vector3 FootStepMotion()
    {
        Vector3 pos = Vector3.zero;
        pos.y += Mathf.Sin(Time.time * frequency) * amplitude;
        pos.x += Mathf.Cos(Time.time * frequency / 2) * amplitude * 2;
        return pos;
    }
    private Vector3 FocusTarget()
    {
        Vector3 pos = new Vector3(transform.position.x, transform.position.y + CameraHolder.localPosition.y, transform.position.z);
        pos += CameraHolder.forward * 15.0f;
        return pos;
    }
}
