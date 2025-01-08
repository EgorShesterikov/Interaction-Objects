using UnityEngine;

[RequireComponent(typeof(Rigidbody), typeof(Outline))]
public class InteractionObject : MonoBehaviour
{
    private Outline _outline;
    private Rigidbody _rigidbody;

    public Rigidbody Rigidbody => _rigidbody;

    private void Awake()
    {
        _outline = GetComponent<Outline>();
        _rigidbody = GetComponent<Rigidbody>();
    }

    private void Start()
    {
        InitializeOutline();
    }

    public void SetOutline(bool value)
    {
        _outline.enabled = value;
    }

    private void InitializeOutline()
    {
        _outline.enabled = false;
        _outline.OutlineColor = Color.cyan;
        _outline.OutlineMode = Outline.Mode.OutlineVisible;
        _outline.OutlineWidth = 10;
    }
}