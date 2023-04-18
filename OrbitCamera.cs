using System;
using System.Collections.Generic;
using Godot;

public partial class OrbitCamera : Camera3D
{
    [Export] public float ScrollSpeed = 50.0f;
    [Export] public float ZoomSpeed = 50.0f;
    [Export] public float DefaultDistance = 5.0f;
    [Export] public float RotateSpeed = 1.0f;

    private Node3D Anchor;
    private Vector2 _moveSpeed = new();
    private float _scrollSpeed = 0.0f;
    private bool isZoomIn = false;
    private bool isZoomOut = false;

    private Vector3 _rotation;
    private float _distance;
    private const float halfPi = (float)Math.PI / 2;

    public override void _Ready()
    {
        if (Anchor == null)
        {
            Anchor = GetParent<Node3D>();
        }
        _distance = DefaultDistance;
        _rotation = Anchor.Transform.Basis.GetRotationQuaternion().GetEuler();
    }

    public override void _Process(double delta)
    {
        if (isZoomIn)
        {
            _scrollSpeed = -1 * ZoomSpeed;
        }
        if (isZoomOut)
        {
            _scrollSpeed = 1 * ZoomSpeed;
        }
        ProcessTransformation(delta);
    }
    
    private void ProcessTransformation(double delta)
    {
        _rotation.X += -_moveSpeed.Y * (float)delta * RotateSpeed;
        _rotation.Y += _moveSpeed.X * (float)delta * RotateSpeed;

        if (_rotation.X < -halfPi)
        {
            _rotation.X = -halfPi;
        }
        if (_rotation.X > halfPi)
        {
            _rotation.X = halfPi;
        }
        _moveSpeed = new();

        _distance += _scrollSpeed * (float)delta;
        if (_distance < 0.0f)
        {
            _distance = 0.0f;
        }
        _scrollSpeed = 0.0f;

        SetIdentity();
        TranslateObjectLocal(new Vector3(0.0f, 0.0f, _distance));
        Anchor.SetIdentity();

        var tempTransform = Anchor.Transform;
        tempTransform.Basis = new Basis(Quaternion.FromEuler(_rotation));
        Anchor.Transform = tempTransform;
    }

    public override void _Input(InputEvent @event)
    {
        base._Input(@event);
        if (@event is InputEventMouseMotion)
        {
            ProcessMouseRotation(@event as InputEventMouseMotion);
        }
        if (@event is InputEventMouseButton)
        {
            ProcessMouseScroll(@event as InputEventMouseButton);
        }
    }
    private void ProcessMouseRotation(InputEventMouseMotion @event)
    {
        if (Input.IsMouseButtonPressed(MouseButton.Left))
            _moveSpeed = @event.Relative;
    }

    private void ProcessMouseScroll(InputEventMouseButton @event)
    {
        if (@event.ButtonIndex == MouseButton.WheelUp)
        {
            _scrollSpeed = -1 * ScrollSpeed;
        }
        else if (@event.ButtonIndex == MouseButton.WheelDown)
        {
            _scrollSpeed = 1 * ScrollSpeed;
        }
    }
}
