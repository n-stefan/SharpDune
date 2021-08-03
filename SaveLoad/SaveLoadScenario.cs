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
			SLD_ENTRY(/*scenario,*/ SLDT_UINT16, nameof(Scenario.score)),
			SLD_ENTRY(/*scenario,*/ SLDT_UINT16, nameof(Scenario.winFlags)),
			SLD_ENTRY(/*scenario,*/ SLDT_UINT16, nameof(Scenario.loseFlags)),
			SLD_ENTRY(/*scenario,*/ SLDT_UINT32, nameof(Scenario.mapSeed)),
			SLD_ENTRY(/*scenario,*/ SLDT_UINT16, nameof(Scenario.mapScale)),
			SLD_ENTRY(/*scenario,*/ SLDT_UINT16, nameof(Scenario.timeOut)),
			SLD_ARRAY(/*scenario,*/ SLDT_UINT8, nameof(Scenario.pictureBriefing), 14),
			SLD_ARRAY(/*scenario,*/ SLDT_UINT8, nameof(Scenario.pictureWin), 14),
			SLD_ARRAY(/*scenario,*/ SLDT_UINT8, nameof(Scenario.pictureLose), 14),
			SLD_ENTRY(/*scenario,*/ SLDT_UINT16, nameof(Scenario.killedAllied)),
			SLD_ENTRY(/*scenario,*/ SLDT_UINT16, nameof(Scenario.killedEnemy)),
			SLD_ENTRY(/*scenario,*/ SLDT_UINT16, nameof(Scenario.destroyedAllied)),
			SLD_ENTRY(/*scenario,*/ SLDT_UINT16, nameof(Scenario.destroyedEnemy)),
			SLD_ENTRY(/*scenario,*/ SLDT_UINT16, nameof(Scenario.harvestedAllied)),
			SLD_ENTRY(/*scenario,*/ SLDT_UINT16, nameof(Scenario.harvestedEnemy)),
			SLD_SLD2(/*scenario,*/ nameof(Scenario.reinforcement), s_saveReinforcement, 16),
			SLD_END()
		};
	}
}
