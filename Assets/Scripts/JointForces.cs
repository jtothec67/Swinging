using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JointForces : MonoBehaviour
{
    public CharacterJoint chestJoint;
    public CharacterJoint leftLegJoint;
    public CharacterJoint rightLegJoint;
    public float chestBendAngle = 30f;
    public float chestBendSpeed = 5f;
    public float legBendAngle = 30f;
    public float legBendSpeed = 5f;

    private Quaternion initialChestRotation;
    private Quaternion initialLeftLegRotation;
    private Quaternion initialRightLegRotation;

    void Start()
    {
        initialChestRotation = chestJoint.transform.localRotation;
        initialLeftLegRotation = leftLegJoint.transform.localRotation;
        initialRightLegRotation = rightLegJoint.transform.localRotation;
    }

    void Update()
    {
        if (Input.GetKey(KeyCode.Q))
        {
            BendForward();
        }
        else if (Input.GetKey(KeyCode.E))
        {
            BendBackward();
        }
        else
        {
            StopBending();
        }
    }

    void BendForward()
    {
        Quaternion chestTargetRotation = initialChestRotation * Quaternion.Euler(0, chestBendAngle, 0);
        chestJoint.transform.localRotation = Quaternion.Slerp(chestJoint.transform.localRotation, chestTargetRotation, Time.deltaTime * chestBendSpeed);

        Quaternion legTargetRotation = Quaternion.Euler(0, -legBendAngle, 0);
        leftLegJoint.transform.localRotation = Quaternion.Slerp(leftLegJoint.transform.localRotation, legTargetRotation, Time.deltaTime * legBendSpeed);
        rightLegJoint.transform.localRotation = Quaternion.Slerp(rightLegJoint.transform.localRotation, legTargetRotation, Time.deltaTime * legBendSpeed);
    }

    void BendBackward()
    {
        Quaternion chestTargetRotation = initialChestRotation * Quaternion.Euler(0, -chestBendAngle, 0);
        chestJoint.transform.localRotation = Quaternion.Slerp(chestJoint.transform.localRotation, chestTargetRotation, Time.deltaTime * chestBendSpeed);

        Quaternion legTargetRotation = Quaternion.Euler(0, legBendAngle, 0);
        leftLegJoint.transform.localRotation = Quaternion.Slerp(leftLegJoint.transform.localRotation, legTargetRotation, Time.deltaTime * legBendSpeed);
        rightLegJoint.transform.localRotation = Quaternion.Slerp(rightLegJoint.transform.localRotation, legTargetRotation, Time.deltaTime * legBendSpeed);
    }

    void StopBending()
    {
        chestJoint.transform.localRotation = Quaternion.Slerp(chestJoint.transform.localRotation, initialChestRotation, Time.deltaTime * chestBendSpeed);
        leftLegJoint.transform.localRotation = Quaternion.Slerp(leftLegJoint.transform.localRotation, initialLeftLegRotation, Time.deltaTime * legBendSpeed);
        rightLegJoint.transform.localRotation = Quaternion.Slerp(rightLegJoint.transform.localRotation, initialRightLegRotation, Time.deltaTime * legBendSpeed);
    }
}
