using System.Drawing;
using UnityEngine;

public class HandJointController
{
    private const string NAME_JOINT = "Hand Joint";

    private const float SPRING_VALUE = 750;
    private const float MAXIMUM_FORCE_VALUE = 1000;
    private const float LINEAR_LIMIT_VALUE = 0.001f;

    private ConfigurableJoint _joint;

    public void Init(Transform targetTransform)
    {
        if (_joint != null)
        {
            InitializeJointPosition(targetTransform);
            return;
        }

        _joint = new GameObject(NAME_JOINT).AddComponent<ConfigurableJoint>();

        InitializeJointMotions();
        InitializeJointPosition(targetTransform);
    }

    public void SetConfigureJoint(Rigidbody rigidBody, float armLength)  
    {
        if (_joint == null) return;

        _joint.connectedBody = rigidBody;

        var distance = Vector3.Distance(_joint.transform.position, rigidBody.transform.position);
        var anchor = distance > armLength ? armLength : distance;
        _joint.anchor = Vector3.forward * anchor;

        var massMultiplier = Mathf.Clamp(rigidBody.mass / 2, 1, float.MaxValue);

        _joint.linearLimitSpring = new SoftJointLimitSpring { spring = SPRING_VALUE * massMultiplier };
        _joint.linearLimit = new SoftJointLimit { limit = LINEAR_LIMIT_VALUE };
        _joint.xDrive = new JointDrive { positionSpring = SPRING_VALUE * massMultiplier, maximumForce = MAXIMUM_FORCE_VALUE };
        _joint.yDrive = new JointDrive { positionSpring = SPRING_VALUE * massMultiplier, maximumForce = MAXIMUM_FORCE_VALUE };
        _joint.zDrive = new JointDrive { positionSpring = SPRING_VALUE * massMultiplier, maximumForce = MAXIMUM_FORCE_VALUE };

        _joint.connectedMassScale = Mathf.Clamp(rigidBody.mass, 1, float.MaxValue);
    }

    public void ResetConfigureJoint()
    {
        _joint.connectedBody = null;
    }

    private void InitializeJointMotions()
    {
        _joint.autoConfigureConnectedAnchor = false;

        _joint.xMotion = ConfigurableJointMotion.Limited;
        _joint.yMotion = ConfigurableJointMotion.Limited;
        _joint.zMotion = ConfigurableJointMotion.Limited;

        _joint.angularXMotion = ConfigurableJointMotion.Locked;
        _joint.angularYMotion = ConfigurableJointMotion.Locked;
        _joint.angularZMotion = ConfigurableJointMotion.Locked;
    }

    private void InitializeJointPosition(Transform targetTransform)
    {
        _joint.GetComponent<Rigidbody>().isKinematic = true;

        _joint.transform.parent = targetTransform;
        _joint.transform.position = targetTransform.position;
        _joint.transform.rotation = targetTransform.rotation;
    }
}