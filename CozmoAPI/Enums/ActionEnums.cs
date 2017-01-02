using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CozmoAPI.Enums
{
    public enum ActionType
    {
        alignWithObject = 0,
        calibrateMotors = 1,
        displayFaceImage = 2,
        displayProceduralFace = 3,
        driveOffChargerContacts = 4,
        driveStraight = 5,
        enrollNamedFace = 6,
        facePlant = 7,
        flipBlock = 8,
        gotoObject = 9,
        gotoPose = 10,
        mountCharger = 11,
        panAndTilt = 12,
        pickupObject = 13,
        placeObjectOnGround = 14,
        placeObjectOnGroundHere = 15,
        placeOnObject = 16,
        placeRelObject = 17,
        playAnimation = 18,
        playAnimationTrigger = 19,
        popAWheelie = 20,
        readToolCode = 21,
        realignWithObject = 22,
        rollObject = 23,
        sayText = 24,
        sayTextWithIntent = 25,
        searchForNearbyObject = 26,
        setHeadAngle = 27,
        setLiftHeight = 28,
        trackFace = 29,
        trackObject = 30,
        trackPet = 31,
        traverseObject = 32,
        turnInPlace = 33,
        turnTowardsFace = 34,
        turnTowardsImagePoint = 35,
        turnTowardsLastFacePose = 36,
        turnTowardsObject = 37,
        turnTowardsPose = 38,
        visuallyVerifyFace = 39,
        visuallyVerifyNoObjectAtPose = 40,
        visuallyVerifyObject = 41,
        wait = 42,
        waitForImages = 43,
        count = 44
    }
}
