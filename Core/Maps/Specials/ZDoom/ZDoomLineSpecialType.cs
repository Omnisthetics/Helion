namespace Helion.Maps.Specials.ZDoom
{
    public enum ZDoomLineSpecialType
    {
        None,
        PolyStartLine,
        PolyRotateLeft,
        PolyRotateRight,
        PolyMove,
        PolyExplicitLine,
        PolyMoveTimes8,
        PolyDoorSwing,
        PolyDoorSlide,
        LineHorizon,
        DoorClose,
        DoorOpenStay,
        DoorOpenClose,
        DoorLockedRaise,
        DoorAnimated,
        AutoSave,
        Unused2,
        Unused3,
        Unused4,
        Unused5,
        FloorLowerByValue,
        FloorLowerToLowest,
        FloorLowerToNearest,
        FloorRaiseByValue,
        FloorRaiseToHighest,
        FloorRaiseToNearest,
        BuildStairsDown,
        BuildStairsUp,
        FloorRaiseCrush,
        PillarRaiseFloorToCeiling,
        PillarRaiseFlorAndLowerCeiling,
        BuildStairsDownSync,
        BuildStairsUpSync,
        ForceFieldSet,
        ForceFieldRemove,
        FloorRaiseByValueTimes8,
        FloorLowerByValueTimes8,
        FloorMoveToValue,
        CeilingWaggle,
        TeleportZombieChanger,
        CeilingLowerByValue,
        CeilingRaiseByValue,
        CeilingCrushRaiseAndLower,
        CeilingCrushStayDown,
        CeilingCrushStop,
        CeilingCrushRaiseStay,
        FloorCrushStop,
        CeilingMoveToValue,
        SectorAttach3DMidtex,
        BreakableGlass,
        TransferLight,
        SectorSetLink,
        ScrollWall,
        Unused11,
        SectorChangeFlags,
        Unused13,
        Unused14,
        Unused15,
        Unused16,
        Unused17,
        LiftPerpetual,
        PlatStop,
        LiftDownWaitUpStay,
        LiftDownValueTimes8,
        LiftUpWaitDownStay,
        PlatUpByValue,
        FloorLowerNow,
        FloorRaiseNow,
        FloorMoveToValueTimes8,
        CeilingMoveToValueTimes8,
        Teleport,
        TeleportNoFog,
        ThingThrust,
        ThingDamage,
        TeleportNewMap,
        TeleportEndGame,
        TeleportOther,
        TeleportGroup,
        TeleportInSector,
        Unused18,
        ScriptRun,
        ScriptStop,
        ScriptKill,
        ScriptWithKey,
        ScriptRunWithResult,
        ScriptLockedExecute,
        Unused20,
        Unused21,
        Unused22,
        Unused23,
        PolyObjectOverrideRotateLeft,
        PolyObjectOverrideRotateRight,
        PolyObjectOverrideMove,
        PolyObjectOverrideMoveTimes8,
        PillarBuildCrush,
        FloorAndCeilingLowerByValue,
        FloorAndCeilingRaiseByValue,
        Unused24,
        Unused25,
        FloorRaiseAndCrushDoom,
        ScrollTextureLeft,
        ScrollTextureRight,
        ScrollTextureUp,
        ScrollTextureDown,
        Unused27,
        Unused28,
        DoorWaitClose,
        Unused30,
        Unused31,
        Lightning,
        LightRaiseByValue,
        LightLowerByValue,
        LightChangeToValue,
        LightFadeToValue,
        LightGlow,
        LightFlicker,
        LightStrobe,
        LightStop,
        Unused32,
        ThingDamageTid,
        EarthQuake,
        LineIdentify,
        Unused33,
        Unused34,
        Unused35,
        ThingMove,
        Unused36,
        ThingSetSpecial,
        ThrustThingZ,
        ScriptWithPuzzle,
        ThingActivate,
        ThingDeActivate,
        ThingIidRemove,
        ThingDestroy,
        ThingProjectile,
        ThingSpawn,
        ThingProjectileGravity,
        ThingSpawnNoFog,
        FloorWaggle,
        ThingSpawnFacing,
        ChangeSectorSound,
        Unused37,
        Unused38,
        Unused39,
        Unused40,
        Unused41,
        Unused42,
        Unused43,
        Unused44,
        Unused45,
        Unused46,
        Unused47,
        Unused48,
        Unused49,
        Unused50,
        Unused51,
        Unused52,
        Unused53,
        Unused54,
        Unused55,
        Unused56,
        Unused57,
        Unused58,
        Unused59,
        Unused60,
        Unused61,
        Unused62,
        Unused63,
        CeilingCrushAndRaiseDist,
        Unused65,
        Unused66,
        Unused67,
        PlatUpNearestWaitDownStay,
        NoiseAlert,
        CommunicatorMessage,
        ThingProjectileIntercept,
        ThingChangeTid,
        ThingHate,
        ThingProjectileAim,
        ChangeSkill,
        ThingSetTranslation,
        SetSlope,
        LineMirror,
        LineAlignCeiling,
        LineAlignFloor,
        SectorSetRotation,
        SectorSetCeilingPanning,
        SectorSetFloorPanning,
        SectorSetCeilingScale,
        SectorSetFloorScale,
        StaticInit,
        SetPlayerProperty,
        CeilingLowerToHighestFloor,
        CeilingLowerInstant,
        CeilingRaiseInstant,
        CeilingCrushStay,
        CeilingCrushRaiseAlways,
        CeilingCrushRaiseSilent,
        CeilingRaiseByValueTimes8,
        CeilingLowerByValueTimes8,
        GenericFloor,
        GenericCeiling,
        DoorGeneric,
        GenericLift,
        StairsGeneric,
        GenericCrusher,
        PlatDownWaitUpStayLip,
        PlatPerpetualRaiseLip,
        TranslucentLine,
        TransferHeights,
        TransferFloorLight,
        TransferCeilingLight,
        SectorSetColor,
        SectorSetFade,
        SectorSetDamage,
        TeleportLine,
        SectorSetGravity,
        StairsBuildUpDoom,
        SectorSetWind,
        SectorSetFriction,
        SectorSetCurrent,
        ScrollTextureBoth,
        ScrollTextureModel,
        ScrollFloor,
        ScrollCeiling,
        ScrollUsingTextureOffsets,
        ACSExecuteAlways,
        PointPushSetForce,
        PlatRaiseAndStay,
        ThingSetGoalTx,
        PlatUpValueStayTx,
        PlatToggleCeiling,
        LightStrobeDoom,
        LightMinNeighbor,
        LightMaxNeighbor,
        FloorTransferTrigger,
        FloorTransferNumeric,
        ChangeCamera,
        FloorRaiseToLowestCeiling,
        FloorRaiseByValueTxTy,
        FloorRaiseByTexture,
        FloorLowerToLowestTxTy,
        FloorLowerToHighest,
        ExitNormal,
        ExitSecret,
        ElevatorRaiseToNearest,
        ElevatorMoveToFloor,
        ElevatorLowerToNearest,
        HealThing,
        DoorCloseWaitOpen,
        FloorDonut,
        FloorCeilingLowerRaise,
        CeilingRaiseToNearest,
        CeilingLowerToLowest,
        CeilingLowerToFloor,
        CeilingCrushRaiseStaySilent,
        Unused69,
        Unused70,
        FloorRaiseToLowest,
        Unused71,
        Unused72,
        Unused73,
        StairsBuildUpDoomCrush = 273,
    }
}