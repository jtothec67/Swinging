using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JointForces : MonoBehaviour
{
    public CharacterJoint chestJoint; // Assign this in the inspector
    public float bendAngle = 45f; // Maximum angle to bend forward
    public float springForce = 1000f; // Spring force to return to default position
    public float damper = 10f; // Damper to control smoothness

    void Start()
    {
        if (chestJoint == null)
        {
            Debug.LogError("Please assign the chest CharacterJoint in the inspector.");
            return;
        }

        ConfigureJoint();
    }

    void Update()
    {
        if (Input.GetKey(KeyCode.B)) // Use 'B' key to trigger the bend, change as needed
        {
            BendCharacterForward();
        }
        else
        {
            ResetBend();
        }
    }

    void ConfigureJoint()
    {
        SoftJointLimitSpring swingSpring = new SoftJointLimitSpring
        {
            spring = springForce,
            damper = damper
        };

        chestJoint.swingLimitSpring = swingSpring;
        chestJoint.twistLimitSpring = swingSpring;
    }

    void BendCharacterForward()
    {
        // Allow forward bend
        SoftJointLimit swing1Limit = chestJoint.swing1Limit;
        swing1Limit.limit = bendAngle;
        chestJoint.swing1Limit = swing1Limit;

        // Restrict side swing
        SoftJointLimit swing2Limit = chestJoint.swing2Limit;
        swing2Limit.limit = 0;
        chestJoint.swing2Limit = swing2Limit;

        // Restrict twisting
        SoftJointLimit twistLimit = new SoftJointLimit();
        twistLimit.limit = 0;
        chestJoint.lowTwistLimit = twistLimit;
        chestJoint.highTwistLimit = twistLimit;
    }

    void ResetBend()
    {
        // Reset forward bend
        SoftJointLimit swing1Limit = chestJoint.swing1Limit;
        swing1Limit.limit = 0;
        chestJoint.swing1Limit = swing1Limit;

        // Reset side swing
        SoftJointLimit swing2Limit = chestJoint.swing2Limit;
        swing2Limit.limit = 0;
        chestJoint.swing2Limit = swing2Limit;

        // Reset twisting
        SoftJointLimit twistLimit = new SoftJointLimit();
        twistLimit.limit = 0;
        chestJoint.lowTwistLimit = twistLimit;
        chestJoint.highTwistLimit = twistLimit;
    }
}
