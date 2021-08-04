namespace SharpDune.SaveLoad
{
    class SaveLoadScenario
    {
		static readonly SaveLoadDesc[] s_saveReinforcement = {
			SLD_ENTRY(/*reinforcement,*/ SLDT_UINT16, nameof(Reinforcement.unitID)),
			SLD_ENTRY(/*reinforcement,*/ SLDT_UINT16, nameof(Reinforcement.locationID)),
			SLD_ENTRY(/*reinforcement,*/ SLDT_UINT16, nameof(Reinforcement.timeLeft)),
			SLD_ENTRY(/*reinforcement,*/ SLDT_UINT16, nameof(Reinforcement.timeBetween)),
			SLD_ENTRY(/*reinforcement,*/ SLDT_UINT16, nameof(Reinforcement.repeat)),
			SLD_END()
		};

		internal static SaveLoadDesc[] g_saveScenario = {
			SLD_ENTRY(/*scenario,*/ SLDT_UINT16, nameof(CScenario.score)),
			SLD_ENTRY(/*scenario,*/ SLDT_UINT16, nameof(CScenario.winFlags)),
			SLD_ENTRY(/*scenario,*/ SLDT_UINT16, nameof(CScenario.loseFlags)),
			SLD_ENTRY(/*scenario,*/ SLDT_UINT32, nameof(CScenario.mapSeed)),
			SLD_ENTRY(/*scenario,*/ SLDT_UINT16, nameof(CScenario.mapScale)),
			SLD_ENTRY(/*scenario,*/ SLDT_UINT16, nameof(CScenario.timeOut)),
			SLD_ARRAY(/*scenario,*/ SLDT_UINT8, nameof(CScenario.pictureBriefing), 14),
			SLD_ARRAY(/*scenario,*/ SLDT_UINT8, nameof(CScenario.pictureWin), 14),
			SLD_ARRAY(/*scenario,*/ SLDT_UINT8, nameof(CScenario.pictureLose), 14),
			SLD_ENTRY(/*scenario,*/ SLDT_UINT16, nameof(CScenario.killedAllied)),
			SLD_ENTRY(/*scenario,*/ SLDT_UINT16, nameof(CScenario.killedEnemy)),
			SLD_ENTRY(/*scenario,*/ SLDT_UINT16, nameof(CScenario.destroyedAllied)),
			SLD_ENTRY(/*scenario,*/ SLDT_UINT16, nameof(CScenario.destroyedEnemy)),
			SLD_ENTRY(/*scenario,*/ SLDT_UINT16, nameof(CScenario.harvestedAllied)),
			SLD_ENTRY(/*scenario,*/ SLDT_UINT16, nameof(CScenario.harvestedEnemy)),
			SLD_SLD2(/*scenario,*/ nameof(CScenario.reinforcement), s_saveReinforcement, 16),
			SLD_END()
		};
	}
}
