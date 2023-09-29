public enum PlayerActions
{
    //movement
    RiseGear,
    DecreaseGear,
    Sit,
    StandUp,
    Stop,
    SlowWalking,
    Walking,
    Runing,
    Sprint,
    Stomp,

    //camera
    NoDamage,
    LowDamage,
    MediumDamage,
    HighDamage,

    //machine gun
    Shoot,
    Aim,
    StopAim,
    ChangeShootingMode,

    //stealth mode
    ThrowDistraction,
    RecuperateDistraction,

    //lights
    TurnOnLights,
    TurnOffLights,

    //LED MATRIX
    NormalFace,
    ScaredFace,
    BlinkFace,
    Asteroids,
    ColorsMinigame,
    MemorizeMinigame,
    DontTouchTheWallsMinigame,
    JustWaitMinigame,
    PuzzleMinigame,
    RollMinigame,
    PushFastMinigame,
    AdjustValuesMinigame,
    MinigameFinished,
    Die,

    //SCENES
    LoadFoundry,
    LoadFreezer,
    LoadWareHouse,
    LoadGarden,
    LoadResidentialZone,

    //ARCADE
    OpenArmorWheel,
    CloseArmorWheel,
    ShowArcadeStats,
    HideArcadeStats,
    ChangeUpgradeLocation,
    TurnOnPower,
    
    //change input mode
    ChangeInputMode
}