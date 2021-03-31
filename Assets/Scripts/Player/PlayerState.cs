namespace Player
{
    public enum PlayerState
    {
        ShouldSpawnAtSpawn,
        ShouldSpawnAtLastPosition,
        ShouldSpawnAtLastStandablePosition,
        ShouldSpawnCanNotMove,
        ShouldMakeTurn,
        ActiveAiming,
        ActivePowerMode,
        ActiveInMotion
    }
}