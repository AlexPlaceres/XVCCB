﻿namespace XVCCB.Data.Binary;

public enum MTBCurveId : uint
{
    INDEX_POSITIONX = 0x0,
    INDEX_POSITIONY = 0x1,
    INDEX_POSITIONZ = 0x2,
    INDEX_POSITIONXYZ = 0x3,
    INDEX_SCALEX = 0x4,
    INDEX_SCALEY = 0x5,
    INDEX_SCALEZ = 0x6,
    INDEX_SCALINGXYZ = 0x7,
    INDEX_QUATROTATION = 0x8,
    INDEX_QUATERNIONX = 0x9,
    INDEX_QUATERNIONY = 0xA,
    INDEX_QUATERNIONZ = 0xB,
    INDEX_QUATERNIONW = 0xC,
    INDEX_ROTATEX = 0xD,
    INDEX_ROTATEY = 0xE,
    INDEX_ROTATEZ = 0xF,
    INDEX_VISIBILITY = 0x10,
    INDEX_FIXGLOBAL = 0x11,
    NUMBER_OF_INDEX = 0x12,
}