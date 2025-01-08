using System.Collections;
using UnityEngine;

public class InteractionObjectTaker : MonoBehaviour
{
    private const float WAIT_TIME_CHECK_OBJECT_IN_RAY = 0.1f;
    private const float VELOCITY_DIVISOR_ON_DISCONNECT = 10f;

    [Range(0, 10), SerializeField] private float interactionDistance = 2;
    [Range(0, 5), SerializeField] private float offsetStartRaycast = 1;
    [Range(0, 10), SerializeField] private float armLength = 1;
    [Range(0, 1000), SerializeField] private float throwPower = 400;

    private InteractionObject _tackedInteractionObject;
    private InteractionObject _raycastInteractionObject;

    private readonly HandJointController _handJointController = new();
    private readonly ObjectRaycaster<InteractionObject> _objectRaycaster = new();

    private void Start()
    {
        _handJointController.Init(transform);
    }

    private void OnEnable()
    {
        StartCoroutine(CheckInteractionObjectInRay());

        _objectRaycaster.OnSelected += ObjectSelect;
        _objectRaycaster.OnDeselected += ObjectDeselect;
    }

    private void OnDisable()
    {
        _objectRaycaster.OnSelected -= ObjectSelect;
        _objectRaycaster.OnDeselected -= ObjectDeselect;
    }

    private IEnumerator CheckInteractionObjectInRay()
    {
        while (true)
        {
            _objectRaycaster.CheckObjectInRay(CalculateStartRaycastPosition(), transform.forward, interactionDistance);
            yield return new WaitForSeconds(WAIT_TIME_CHECK_OBJECT_IN_RAY);
        }
    }

    public void TakeObject()
    {
        if (_tackedInteractionObject == null)
        {
            if (_raycastInteractionObject != null)
            {
                ConnectObjectInJoint(_raycastInteractionObject);
            }
        }
        else
        {
            DisconnectObjectFromJoint();
        }
    }

    public void ThrowObject()
    {
        if (_tackedInteractionObject != null)
        {
            _tackedInteractionObject.Rigidbody.AddForce(transform.forward * throwPower);
            DisconnectObjectFromJoint();
        }
    }

    private void ConnectObjectInJoint(InteractionObject obj)
    {
        _tackedInteractionObject = obj;
        _tackedInteractionObject.SetOutline(false);
        _handJointController.SetConfigureJoint(_tackedInteractionObject.Rigidbody, armLength);
    }

    private void DisconnectObjectFromJoint()
    {
        Vector3 velocity = _tackedInteractionObject.Rigidbody.velocity;
        _tackedInteractionObject.Rigidbody.velocity = velocity / VELOCITY_DIVISOR_ON_DISCONNECT;
        _tackedInteractionObject = null;

        _handJointController.ResetConfigureJoint();
    }

    private void ObjectSelect(InteractionObject obj)
    {
        if (_tackedInteractionObject) return;

        _raycastInteractionObject = obj;

        _raycastInteractionObject.SetOutline(true);
    }

    private void ObjectDeselect(InteractionObject obj)
    {
        obj.SetOutline(false);

        _raycastInteractionObject = null;
    }

    private Vector3 CalculateStartRaycastPosition()
    {
        return transform.position + transform.forward * offsetStartRaycast;
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawRay(CalculateStartRaycastPosition(), transform.forward * interactionDistance);
    }
}