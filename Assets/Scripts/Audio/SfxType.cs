namespace Audio
{
    public enum SfxType
    {
        LemmingHitGround = 1,
        LemmingRotate = 2,
        LemmingGroundRoll = 3,
        LemmingLaunch = 4,
        LemmingLaunchVoice = 11,
        LemmingExplosion = 13,
        IceBlockActivate = 5,
        IceBlockDeactivate = 6,
        ExpandActivate = 7,
        ExpandDeactivate = 8,
        RandomActivate = 15,
        MushroomHit = 9,
        PointReclaimed = 12,
        HoleAppeared = 10,
        PowerMeterMax = 14,
        FireDashActivate = 16,
        HitStopEngaged = 17,

        //Used in the inspector
        // ReSharper disable once UnusedMember.Global
        AcidSizzle = 18,
        SpikeTrap = 19,
        SpikeTrapPrepare = 20,

        //Used in the inspector
        // ReSharper disable once UnusedMember.Global
        UiSelect = 21,
    }
}