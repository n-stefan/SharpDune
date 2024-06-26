﻿namespace SharpDune.SaveLoad;

static class SaveLoadScenario
{
    static readonly SaveLoadDesc[] s_saveReinforcement = [
        SLD_ENTRY(SLDT_UINT16, nameof(Reinforcement.unitID)),
        SLD_ENTRY(SLDT_UINT16, nameof(Reinforcement.locationID)),
        SLD_ENTRY(SLDT_UINT16, nameof(Reinforcement.timeLeft)),
        SLD_ENTRY(SLDT_UINT16, nameof(Reinforcement.timeBetween)),
        SLD_ENTRY(SLDT_UINT16, nameof(Reinforcement.repeat)),
        SLD_END()
    ];

    internal static SaveLoadDesc[] g_saveScenario = [
        SLD_ENTRY(SLDT_UINT16, nameof(CScenario.score)),
        SLD_ENTRY(SLDT_UINT16, nameof(CScenario.winFlags)),
        SLD_ENTRY(SLDT_UINT16, nameof(CScenario.loseFlags)),
        SLD_ENTRY(SLDT_UINT32, nameof(CScenario.mapSeed)),
        SLD_ENTRY(SLDT_UINT16, nameof(CScenario.mapScale)),
        SLD_ENTRY(SLDT_UINT16, nameof(CScenario.timeOut)),
        SLD_ARRAY(SLDT_UINT8, nameof(CScenario.pictureBriefing), 14),
        SLD_ARRAY(SLDT_UINT8, nameof(CScenario.pictureWin), 14),
        SLD_ARRAY(SLDT_UINT8, nameof(CScenario.pictureLose), 14),
        SLD_ENTRY(SLDT_UINT16, nameof(CScenario.killedAllied)),
        SLD_ENTRY(SLDT_UINT16, nameof(CScenario.killedEnemy)),
        SLD_ENTRY(SLDT_UINT16, nameof(CScenario.destroyedAllied)),
        SLD_ENTRY(SLDT_UINT16, nameof(CScenario.destroyedEnemy)),
        SLD_ENTRY(SLDT_UINT16, nameof(CScenario.harvestedAllied)),
        SLD_ENTRY(SLDT_UINT16, nameof(CScenario.harvestedEnemy)),
        SLD_SLD2(nameof(CScenario.reinforcement), s_saveReinforcement, 16),
        SLD_END()
    ];
}
