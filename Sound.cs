/* Sound */

using System;

namespace SharpDune
{
	/* Information about sound files. */
	class VoiceData
	{
		internal string str;         /*!< Pointer to a string. */
		internal ushort priority;    /*!< priority */
	}

	/* Information about sound files. */
	class MusicData
	{
		internal string name;       /*!< Pointer to a string. */
		internal ushort index;      /*!< index */
	}

	/* Audio and visual feedback about events and commands.
	 * in Intro, messageId is used as a flag (force drawing
	 * of message if messageId == 1) */
	class Feedback
	{
		internal ushort[] voiceId = new ushort[Sound.NUM_SPEECH_PARTS]; /*!< English spoken text. */
		internal ushort messageId;                                      /*!< Message to display in the viewport when audio is disabled. */
		internal ushort soundId;                                        /*!< Sound. */
	}

	class Sound
	{
		/* Number of voices in the game. */
		const byte NUM_VOICES = 131;
		/* Maximal number of spoken audio fragments in one message. */
		internal const byte NUM_SPEECH_PARTS = 5;

		static byte[][] g_voiceData = new byte[NUM_VOICES][];         /*!< Preloaded Voices sound data */
		static uint[] g_voiceDataSize = new uint[NUM_VOICES];         /*!< Preloaded Voices sound data size in byte */
		static ushort[] s_spokenWords = new ushort[NUM_SPEECH_PARTS]; /*!< Buffer with speech to play. */
		static short s_currentVoicePriority;                          /*!< Priority of the currently playing Speech */

		/*
		 * Available music.
		 * @note The code compares pointers rather than the text itself, thus strings must be unique.
		 */
		static MusicData[] g_table_musics = {
			new MusicData { name = null, index = 0 }, /*  0 */
			new MusicData { name = "dune1", index = 2 }, /*  1 */
			new MusicData { name = "dune1", index = 3 }, /*  2 */
			new MusicData { name = "dune1", index = 4 }, /*  3 */
			new MusicData { name = "dune1", index = 5 }, /*  4 */
			new MusicData { name = "dune17", index = 4 }, /*  5 */
			new MusicData { name = "dune8", index = 3 }, /*  6 */
			new MusicData { name = "dune8", index = 2 }, /*  7 */
			new MusicData { name = "dune1", index = 6 }, /*  8 */
			new MusicData { name = "dune2", index = 6 }, /*  9 */
			new MusicData { name = "dune3", index = 6 }, /* 10 */
			new MusicData { name = "dune4", index = 6 }, /* 11 */
			new MusicData { name = "dune5", index = 6 }, /* 12 */
			new MusicData { name = "dune6", index = 6 }, /* 13 */
			new MusicData { name = "dune9", index = 4 }, /* 14 */
			new MusicData { name = "dune9", index = 5 }, /* 15 */
			new MusicData { name = "dune18", index = 6 }, /* 16 */
			new MusicData { name = "dune10", index = 7 }, /* 17 */
			new MusicData { name = "dune11", index = 7 }, /* 18 */
			new MusicData { name = "dune12", index = 7 }, /* 19 */
			new MusicData { name = "dune13", index = 7 }, /* 20 */
			new MusicData { name = "dune14", index = 7 }, /* 21 */
			new MusicData { name = "dune15", index = 7 }, /* 22 */
			new MusicData { name = "dune1", index = 8 }, /* 23 */
			new MusicData { name = "dune7", index = 2 }, /* 24 */
			new MusicData { name = "dune7", index = 3 }, /* 25 */
			new MusicData { name = "dune7", index = 4 }, /* 26 */
			new MusicData { name = "dune0", index = 2 }, /* 27 */
			new MusicData { name = "dune7", index = 6 }, /* 28 */
			new MusicData { name = "dune16", index = 7 }, /* 29 */
			new MusicData { name = "dune19", index = 4 }, /* 30 */
			new MusicData { name = "dune19", index = 2 }, /* 31 */
			new MusicData { name = "dune19", index = 3 }, /* 32 */
			new MusicData { name = "dune20", index = 2 }, /* 33 */
			new MusicData { name = "dune16", index = 8 }, /* 34 */
			new MusicData { name = "dune0", index = 3 }, /* 35 */
			new MusicData { name = "dune0", index = 4 }, /* 36 */
			new MusicData { name = "dune0", index = 5 }, /* 37 */
		};

		/* Available voices.
		 * Prefix :
		 *  '+' : Don't include in voiceSet 0xFFFF
		 *  '?' : don't preload voice in RAM
		 *  '/' : Only include in voiceSet 0xFFFE
		 *  '-' : Only include in voiceSet 0xFFFF
		 *  '%' : Don't include in voiceSet 0xFFFF and 0xFFFE
		 * %c => replaced by language 'F'(french) 'G'(german) 'Z'
		 *       or house prefix char ('A'treides, 'O'rdos or Fremen,
		 *           'H'arkonnen or Sardokar, 'M'ercenary)
		 */
		static VoiceData[] g_table_voices = { //[NUM_VOICES]
			new VoiceData { str = "+VSCREAM1.VOC",  priority = 11}, /*   0 */
			new VoiceData { str = "+EXSAND.VOC",    priority = 10}, /*   1 */
			new VoiceData { str = "+ROCKET.VOC",    priority = 11}, /*   2 */
			new VoiceData { str = "+BUTTON.VOC",    priority = 10}, /*   3 */
			new VoiceData { str = "+VSCREAM5.VOC",  priority = 11}, /*   4 */
			new VoiceData { str = "+CRUMBLE.VOC",   priority = 15}, /*   5 */
			new VoiceData { str = "+EXSMALL.VOC",   priority = 9}, /*   6 */
			new VoiceData { str = "+EXMED.VOC",     priority = 10}, /*   7 */
			new VoiceData { str = "+EXLARGE.VOC",   priority = 14}, /*   8 */
			new VoiceData { str = "+EXCANNON.VOC",  priority = 11}, /*   9 */
			new VoiceData { str = "+GUNMULTI.VOC",  priority = 9}, /*  10 */
			new VoiceData { str = "+GUN.VOC",       priority = 10}, /*  11 */
			new VoiceData { str = "+EXGAS.VOC",     priority = 10}, /*  12 */
			new VoiceData { str = "+EXDUD.VOC",     priority = 10}, /*  13 */
			new VoiceData { str = "+VSCREAM2.VOC",  priority = 11}, /*  14 */
			new VoiceData { str = "+VSCREAM3.VOC",  priority = 11}, /*  15 */
			new VoiceData { str = "+VSCREAM4.VOC",  priority = 11}, /*  16 */
			new VoiceData { str = "+%cAFFIRM.VOC",  priority = 15}, /*  17 */
			new VoiceData { str = "+%cREPORT1.VOC", priority = 15}, /*  18 */
			new VoiceData { str = "+%cREPORT2.VOC", priority = 15}, /*  19 */
			new VoiceData { str = "+%cREPORT3.VOC", priority = 15}, /*  20 */
			new VoiceData { str = "+%cOVEROUT.VOC", priority = 15}, /*  21 */
			new VoiceData { str = "+%cMOVEOUT.VOC", priority = 15}, /*  22 */
			new VoiceData { str = "?POPPA.VOC",     priority = 15}, /*  23 */
			new VoiceData { str = "?SANDBUG.VOC",   priority = 15}, /*  24 */
			new VoiceData { str = "+STATICP.VOC",   priority = 10}, /*  25 */
			new VoiceData { str = "+WORMET3P.VOC",  priority = 16}, /*  26 */
			new VoiceData { str = "+MISLTINP.VOC",  priority = 10}, /*  27 */
			new VoiceData { str = "+SQUISH2.VOC",   priority = 12}, /*  28 */
			new VoiceData { str = "%cENEMY.VOC",    priority = 20}, /*  29 */
			new VoiceData { str = "%cHARK.VOC",     priority = 20}, /*  30 */
			new VoiceData { str = "%cATRE.VOC",     priority = 20}, /*  31 */
			new VoiceData { str = "%cORDOS.VOC",    priority = 20}, /*  32 */
			new VoiceData { str = "%cFREMEN.VOC",   priority = 20}, /*  33 */
			new VoiceData { str = "%cSARD.VOC",     priority = 20}, /*  34 */
			new VoiceData { str = "FILLER.VOC",     priority = 20}, /*  35 */
			new VoiceData { str = "%cUNIT.VOC",     priority = 20}, /*  36 */
			new VoiceData { str = "%cSTRUCT.VOC",   priority = 20}, /*  37 */
			new VoiceData { str = "%cONE.VOC",      priority = 19}, /*  38 */
			new VoiceData { str = "%cTWO.VOC",      priority = 19}, /*  39 */
			new VoiceData { str = "%cTHREE.VOC",    priority = 19}, /*  40 */
			new VoiceData { str = "%cFOUR.VOC",     priority = 19}, /*  41 */
			new VoiceData { str = "%cFIVE.VOC",     priority = 19}, /*  42 */
			new VoiceData { str = "%cCONST.VOC",    priority = 20}, /*  43 */
			new VoiceData { str = "%cRADAR.VOC",    priority = 20}, /*  44 */
			new VoiceData { str = "%cOFF.VOC",      priority = 20}, /*  45 */
			new VoiceData { str = "%cON.VOC",       priority = 20}, /*  46 */
			new VoiceData { str = "%cFRIGATE.VOC",  priority = 20}, /*  47 */
			new VoiceData { str = "?%cARRIVE.VOC",  priority = 20}, /*  48 */
			new VoiceData { str = "%cWARNING.VOC",  priority = 20}, /*  49 */
			new VoiceData { str = "%cSABOT.VOC",    priority = 20}, /*  50 */
			new VoiceData { str = "%cMISSILE.VOC",  priority = 20}, /*  51 */
			new VoiceData { str = "%cBLOOM.VOC",    priority = 20}, /*  52 */
			new VoiceData { str = "%cDESTROY.VOC",  priority = 20}, /*  53 */
			new VoiceData { str = "%cDEPLOY.VOC",   priority = 20}, /*  54 */
			new VoiceData { str = "%cAPPRCH.VOC",   priority = 20}, /*  55 */
			new VoiceData { str = "%cLOCATED.VOC",  priority = 20}, /*  56 */
			new VoiceData { str = "%cNORTH.VOC",    priority = 20}, /*  57 */
			new VoiceData { str = "%cEAST.VOC",     priority = 20}, /*  58 */
			new VoiceData { str = "%cSOUTH.VOC",    priority = 20}, /*  59 */
			new VoiceData { str = "%cWEST.VOC",     priority = 20}, /*  60 */
			new VoiceData { str = "?%cWIN.VOC",     priority = 20}, /*  61 */
			new VoiceData { str = "?%cLOSE.VOC",    priority = 20}, /*  62 */
			new VoiceData { str = "%cLAUNCH.VOC",   priority = 20}, /*  63 */
			new VoiceData { str = "%cATTACK.VOC",   priority = 20}, /*  64 */
			new VoiceData { str = "%cVEHICLE.VOC",  priority = 20}, /*  65 */
			new VoiceData { str = "%cREPAIR.VOC",   priority = 20}, /*  66 */
			new VoiceData { str = "%cHARVEST.VOC",  priority = 20}, /*  67 */
			new VoiceData { str = "%cWORMY.VOC",    priority = 20}, /*  68 */
			new VoiceData { str = "%cCAPTURE.VOC",  priority = 20}, /*  69 */
			new VoiceData { str = "%cNEXT.VOC",     priority = 20}, /*  70 */
			new VoiceData { str = "%cNEXT2.VOC",    priority = 20}, /*  71 */
			new VoiceData { str = "/BLASTER.VOC",   priority = 10}, /*  72 */
			new VoiceData { str = "/GLASS6.VOC",    priority = 10}, /*  73 */
			new VoiceData { str = "/LIZARD1.VOC",   priority = 10}, /*  74 */
			new VoiceData { str = "/FLESH.VOC",     priority = 10}, /*  75 */
			new VoiceData { str = "/CLICK.VOC",     priority = 10}, /*  76 */
			new VoiceData { str = "-3HOUSES.VOC",   priority = 12}, /*  77 */
			new VoiceData { str = "-ANDNOW.VOC",    priority = 12}, /*  78 */
			new VoiceData { str = "-ARRIVED.VOC",   priority = 12}, /*  79 */
			new VoiceData { str = "-BATTLE.VOC",    priority = 12}, /*  80 */
			new VoiceData { str = "-BEGINS.VOC",    priority = 12}, /*  81 */
			new VoiceData { str = "-BLDING.VOC",    priority = 12}, /*  82 */
			new VoiceData { str = "-CONTROL2.VOC",  priority = 12}, /*  83 */
			new VoiceData { str = "-CONTROL3.VOC",  priority = 12}, /*  84 */
			new VoiceData { str = "-CONTROL4.VOC",  priority = 12}, /*  85 */
			new VoiceData { str = "-CONTROLS.VOC",  priority = 12}, /*  86 */
			new VoiceData { str = "-DUNE.VOC",      priority = 12}, /*  87 */
			new VoiceData { str = "-DYNASTY.VOC",   priority = 12}, /*  88 */
			new VoiceData { str = "-EACHHOME.VOC",  priority = 12}, /*  89 */
			new VoiceData { str = "-EANDNO.VOC",    priority = 12}, /*  90 */
			new VoiceData { str = "-ECONTROL.VOC",  priority = 12}, /*  91 */
			new VoiceData { str = "-EHOUSE.VOC",    priority = 12}, /*  92 */
			new VoiceData { str = "-EMPIRE.VOC",    priority = 12}, /*  93 */
			new VoiceData { str = "-EPRODUCE.VOC",  priority = 12}, /*  94 */
			new VoiceData { str = "-ERULES.VOC",    priority = 12}, /*  95 */
			new VoiceData { str = "-ETERRIT.VOC",   priority = 12}, /*  96 */
			new VoiceData { str = "-EMOST.VOC",     priority = 12}, /*  97 */
			new VoiceData { str = "-ENOSET.VOC",    priority = 12}, /*  98 */
			new VoiceData { str = "-EVIL.VOC",      priority = 12}, /*  99 */
			new VoiceData { str = "-HARK.VOC",      priority = 12}, /* 100 */
			new VoiceData { str = "-HOME.VOC",      priority = 12}, /* 101 */
			new VoiceData { str = "-HOUSE2.VOC",    priority = 12}, /* 102 */
			new VoiceData { str = "-INSID.VOC",     priority = 12}, /* 103 */
			new VoiceData { str = "-KING.VOC",      priority = 12}, /* 104 */
			new VoiceData { str = "-KNOWN.VOC",     priority = 12}, /* 105 */
			new VoiceData { str = "-MELANGE.VOC",   priority = 12}, /* 106 */
			new VoiceData { str = "-NOBLE.VOC",     priority = 12}, /* 107 */
			new VoiceData { str = "?NOW.VOC",       priority = 12}, /* 108 */
			new VoiceData { str = "-OFDUNE.VOC",    priority = 12}, /* 109 */
			new VoiceData { str = "-ORD.VOC",       priority = 12}, /* 110 */
			new VoiceData { str = "-PLANET.VOC",    priority = 12}, /* 111 */
			new VoiceData { str = "-PREVAIL.VOC",   priority = 12}, /* 112 */
			new VoiceData { str = "-PROPOSED.VOC",  priority = 12}, /* 113 */
			new VoiceData { str = "-SANDLAND.VOC",  priority = 12}, /* 114 */
			new VoiceData { str = "-SPICE.VOC",     priority = 12}, /* 115 */
			new VoiceData { str = "-SPICE2.VOC",    priority = 12}, /* 116 */
			new VoiceData { str = "-VAST.VOC",      priority = 12}, /* 117 */
			new VoiceData { str = "-WHOEVER.VOC",   priority = 12}, /* 118 */
			new VoiceData { str = "?YOUR.VOC",      priority = 12}, /* 119 */
			new VoiceData { str = "?FILLER.VOC",    priority = 12}, /* 120 */
			new VoiceData { str = "-DROPEQ2P.VOC",  priority = 10}, /* 121 */
			new VoiceData { str = "/EXTINY.VOC",    priority = 10}, /* 122 */
			new VoiceData { str = "-WIND2BP.VOC",   priority = 10}, /* 123 */
			new VoiceData { str = "-BRAKES2P.VOC",  priority = 11}, /* 124 */
			new VoiceData { str = "-GUNSHOT.VOC",   priority = 10}, /* 125 */
			new VoiceData { str = "-GLASS.VOC",     priority = 11}, /* 126 */
			new VoiceData { str = "-MISSLE8.VOC",   priority = 10}, /* 127 */
			new VoiceData { str = "-CLANK.VOC",     priority = 10}, /* 128 */
			new VoiceData { str = "-BLOWUP1.VOC",   priority = 10}, /* 129 */
			new VoiceData { str = "-BLOWUP2.VOC",   priority = 11}  /* 130 */
		};

		/*
		 * Mapping soundID -> voice.
		 */
		static ushort[] g_table_voiceMapping = {
			0xFFFF, /*   0 */
			0xFFFF, /*   1 */
			0xFFFF, /*   2 */
			0xFFFF, /*   3 */
			0xFFFF, /*   4 */
			0xFFFF, /*   5 */
			0xFFFF, /*   6 */
			0xFFFF, /*   7 */
			0xFFFF, /*   8 */
			0xFFFF, /*   9 */
			0xFFFF, /*  10 */
			0xFFFF, /*  11 */
			0xFFFF, /*  12 */
			0xFFFF, /*  13 */
			0xFFFF, /*  14 */
			0xFFFF, /*  15 */
			0xFFFF, /*  16 */
			0xFFFF, /*  17 */
			0xFFFF, /*  18 */
			0xFFFF, /*  19 */
			13,     /*  20 */
			0xFFFF, /*  21 */
			0xFFFF, /*  22 */
			0xFFFF, /*  23 */
			121,    /*  24 */
			0xFFFF, /*  25 */
			0xFFFF, /*  26 */
			0xFFFF, /*  27 */
			0xFFFF, /*  28 */
			0xFFFF, /*  29 */
			0,      /*  30 */
			4,      /*  31 */
			14,     /*  32 */
			15,     /*  33 */
			16,     /*  34 */
			28,     /*  35 */
			0xFFFF, /*  36 */
			0xFFFF, /*  37 */
			3,      /*  38 */
			12,     /*  39 */
			1,      /*  40 */
			7,      /*  41 */
			2,      /*  42 */
			0xFFFF, /*  43 */
			5,      /*  44 */
			0xFFFF, /*  45 */
			0xFFFF, /*  46 */
			0xFFFF, /*  47 */
			0xFFFF, /*  48 */
			7,      /*  49 */
			6,      /*  50 */
			8,      /*  51 */
			0xFFFF, /*  52 */
			0xFFFF, /*  53 */
			122,    /*  54 */
			0xFFFF, /*  55 */
			9,      /*  56 */
			9,      /*  57 */
			11,     /*  58 */
			10,     /*  59 */
			43,     /*  60 */
			0xFFFF, /*  61 */
			25,     /*  62 */
			26,     /*  63 */
			27,     /*  64 */
			72,     /*  65 */
			73,     /*  66 */
			74,     /*  67 */
			75,     /*  68 */
			76,     /*  69 */
			0xFFFF, /*  70 */
			0xFFFF, /*  71 */
			0xFFFF, /*  72 */
			0xFFFF, /*  73 */
			0xFFFF, /*  74 */
			0xFFFF, /*  75 */
			0xFFFF, /*  76 */
			0xFFFF, /*  77 */
			0xFFFF, /*  78 */
			0xFFFF, /*  79 */
			0xFFFF, /*  80 */
			0xFFFF, /*  81 */
			0xFFFF, /*  82 */
			0xFFFF, /*  83 */
			0xFFFF, /*  84 */
			0xFFFF, /*  85 */
			0xFFFF, /*  86 */
			0xFFFF, /*  87 */
			0xFFFF, /*  88 */
			0xFFFF, /*  89 */
			0xFFFF, /*  90 */
			0xFFFF, /*  91 */
			0xFFFF, /*  92 */
			0xFFFF, /*  93 */
			0xFFFF, /*  94 */
			0xFFFF, /*  95 */
			0xFFFF, /*  96 */
			0xFFFF, /*  97 */
			0xFFFF, /*  98 */
			0xFFFF, /*  99 */
			0xFFFF, /* 100 */
			0xFFFF, /* 101 */
			0xFFFF, /* 102 */
			0xFFFF, /* 103 */
			0xFFFF, /* 104 */
			0xFFFF, /* 105 */
			0xFFFF, /* 106 */
			0xFFFF, /* 107 */
			123,    /* 108 */
			0xFFFF, /* 109 */
			124,    /* 110 */
			0xFFFF, /* 111 */
			125,    /* 112 */
			126,    /* 113 */
			127,    /* 114 */
			0xFFFF, /* 115 */
			0xFFFF, /* 116 */
			128,    /* 117 */
			129,    /* 118 */
			130     /* 119 */
		};

		/*
		 * Feedback on events and user commands (English audio, viewport message, and sound).
		 * @see g_translatedVoice
		 */
		internal static Feedback[] g_feedback = {
			new Feedback { voiceId = new ushort[] { 0x002B, 0xFFFF, 0xFFFF, 0xFFFF, 0xFFFF }, messageId = 0x33, soundId = 0x003C }, /*  0 */
			new Feedback { voiceId = new ushort[] { 0x0031, 0x001D, 0x0024, 0x0037, 0xFFFF }, messageId = 0x34, soundId = 0xFFFF }, /*  1 */
			new Feedback { voiceId = new ushort[] { 0x0031, 0x001D, 0x0024, 0x0037, 0x0039 }, messageId = 0x34, soundId = 0xFFFF }, /*  2 */
			new Feedback { voiceId = new ushort[] { 0x0031, 0x001D, 0x0024, 0x0037, 0x003A }, messageId = 0x34, soundId = 0xFFFF }, /*  3 */
			new Feedback { voiceId = new ushort[] { 0x0031, 0x001D, 0x0024, 0x0037, 0x003B }, messageId = 0x34, soundId = 0xFFFF }, /*  4 */
			new Feedback { voiceId = new ushort[] { 0x0031, 0x001D, 0x0024, 0x0037, 0x003C }, messageId = 0x34, soundId = 0xFFFF }, /*  5 */
			new Feedback { voiceId = new ushort[] { 0x0031, 0x001E, 0x0024, 0x0037, 0xFFFF }, messageId = 0x35, soundId = 0xFFFF }, /*  6 */
			new Feedback { voiceId = new ushort[] { 0x0031, 0x001F, 0x0024, 0x0037, 0xFFFF }, messageId = 0x36, soundId = 0xFFFF }, /*  7 */
			new Feedback { voiceId = new ushort[] { 0x0031, 0x0020, 0x0024, 0x0037, 0xFFFF }, messageId = 0x37, soundId = 0xFFFF }, /*  8 */
			new Feedback { voiceId = new ushort[] { 0x0031, 0x0021, 0x0024, 0x0037, 0xFFFF }, messageId = 0x38, soundId = 0xFFFF }, /*  9 */
			new Feedback { voiceId = new ushort[] { 0x0031, 0x0022, 0x0037, 0xFFFF, 0xFFFF }, messageId = 0x39, soundId = 0xFFFF }, /* 10 */
			new Feedback { voiceId = new ushort[] { 0x0031, 0x0023, 0x0024, 0x0037, 0xFFFF }, messageId = 0x3A, soundId = 0xFFFF }, /* 11 */
			new Feedback { voiceId = new ushort[] { 0x0031, 0x0032, 0x0037, 0xFFFF, 0xFFFF }, messageId = 0x3B, soundId = 0xFFFF }, /* 12 */
			new Feedback { voiceId = new ushort[] { 0x001D, 0x0024, 0x0035, 0xFFFF, 0xFFFF }, messageId = 0x00, soundId = 0xFFFF }, /* 13 */
			new Feedback { voiceId = new ushort[] { 0x001E, 0x0024, 0x0035, 0xFFFF, 0xFFFF }, messageId = 0x3C, soundId = 0xFFFF }, /* 14 */
			new Feedback { voiceId = new ushort[] { 0x001F, 0x0024, 0x0035, 0xFFFF, 0xFFFF }, messageId = 0x3D, soundId = 0xFFFF }, /* 15 */
			new Feedback { voiceId = new ushort[] { 0x0020, 0x0024, 0x0035, 0xFFFF, 0xFFFF }, messageId = 0x3E, soundId = 0xFFFF }, /* 16 */
			new Feedback { voiceId = new ushort[] { 0x0021, 0x0024, 0x0035, 0xFFFF, 0xFFFF }, messageId = 0x3F, soundId = 0xFFFF }, /* 17 */
			new Feedback { voiceId = new ushort[] { 0x0022, 0x0035, 0xFFFF, 0xFFFF, 0xFFFF }, messageId = 0x40, soundId = 0xFFFF }, /* 18 */
			new Feedback { voiceId = new ushort[] { 0x0023, 0x0024, 0x0035, 0xFFFF, 0xFFFF }, messageId = 0x41, soundId = 0xFFFF }, /* 19 */
			new Feedback { voiceId = new ushort[] { 0x0032, 0x0035, 0xFFFF, 0xFFFF, 0xFFFF }, messageId = 0x00, soundId = 0xFFFF }, /* 20 */
			new Feedback { voiceId = new ushort[] { 0x001D, 0x0025, 0x0035, 0xFFFF, 0xFFFF }, messageId = 0x00, soundId = 0xFFFF }, /* 21 */
			new Feedback { voiceId = new ushort[] { 0x001E, 0x0025, 0x0035, 0xFFFF, 0xFFFF }, messageId = 0x42, soundId = 0xFFFF }, /* 22 */
			new Feedback { voiceId = new ushort[] { 0x001F, 0x0025, 0x0035, 0xFFFF, 0xFFFF }, messageId = 0x43, soundId = 0xFFFF }, /* 23 */
			new Feedback { voiceId = new ushort[] { 0x0020, 0x0025, 0x0035, 0xFFFF, 0xFFFF }, messageId = 0x44, soundId = 0xFFFF }, /* 24 */
			new Feedback { voiceId = new ushort[] { 0x0021, 0x0025, 0x0035, 0xFFFF, 0xFFFF }, messageId = 0x45, soundId = 0xFFFF }, /* 25 */
			new Feedback { voiceId = new ushort[] { 0x0022, 0x0035, 0xFFFF, 0xFFFF, 0xFFFF }, messageId = 0x46, soundId = 0xFFFF }, /* 26 */
			new Feedback { voiceId = new ushort[] { 0x0023, 0x0025, 0x0035, 0xFFFF, 0xFFFF }, messageId = 0x47, soundId = 0xFFFF }, /* 27 */
			new Feedback { voiceId = new ushort[] { 0x002C, 0x002E, 0xFFFF, 0xFFFF, 0xFFFF }, messageId = 0x00, soundId = 0xFFFF }, /* 28 */
			new Feedback { voiceId = new ushort[] { 0x002C, 0x002D, 0xFFFF, 0xFFFF, 0xFFFF }, messageId = 0x00, soundId = 0xFFFF }, /* 29 */
			new Feedback { voiceId = new ushort[] { 0x001E, 0x0024, 0x0036, 0xFFFF, 0xFFFF }, messageId = 0x48, soundId = 0xFFFF }, /* 30 */
			new Feedback { voiceId = new ushort[] { 0x001F, 0x0024, 0x0036, 0xFFFF, 0xFFFF }, messageId = 0x49, soundId = 0xFFFF }, /* 31 */
			new Feedback { voiceId = new ushort[] { 0x0020, 0x0024, 0x0036, 0xFFFF, 0xFFFF }, messageId = 0x4A, soundId = 0xFFFF }, /* 32 */
			new Feedback { voiceId = new ushort[] { 0x0021, 0x0024, 0x0036, 0xFFFF, 0xFFFF }, messageId = 0x4B, soundId = 0xFFFF }, /* 33 */
			new Feedback { voiceId = new ushort[] { 0x0022, 0x0036, 0xFFFF, 0xFFFF, 0xFFFF }, messageId = 0x4C, soundId = 0xFFFF }, /* 34 */
			new Feedback { voiceId = new ushort[] { 0x0023, 0x0024, 0x0036, 0xFFFF, 0xFFFF }, messageId = 0x4D, soundId = 0xFFFF }, /* 35 */
			new Feedback { voiceId = new ushort[] { 0x0034, 0x0038, 0xFFFF, 0xFFFF, 0xFFFF }, messageId = 0x00, soundId = 0xFFFF }, /* 36 */
			new Feedback { voiceId = new ushort[] { 0x0031, 0x0044, 0xFFFF, 0xFFFF, 0xFFFF }, messageId = 0x4E, soundId = 0x0017 }, /* 37 */
			new Feedback { voiceId = new ushort[] { 0x002F, 0x0030, 0xFFFF, 0xFFFF, 0xFFFF }, messageId = 0x50, soundId = 0xFFFF }, /* 38 */
			new Feedback { voiceId = new ushort[] { 0x0031, 0x0033, 0x0037, 0xFFFF, 0xFFFF }, messageId = 0x51, soundId = 0xFFFF }, /* 39 */
			new Feedback { voiceId = new ushort[] { 0x003D, 0xFFFF, 0xFFFF, 0xFFFF, 0xFFFF }, messageId = 0x00, soundId = 0xFFFF }, /* 40 */
			new Feedback { voiceId = new ushort[] { 0x003E, 0xFFFF, 0xFFFF, 0xFFFF, 0xFFFF }, messageId = 0x00, soundId = 0xFFFF }, /* 41 */
			new Feedback { voiceId = new ushort[] { 0x0033, 0x003F, 0xFFFF, 0xFFFF, 0xFFFF }, messageId = 0x00, soundId = 0xFFFF }, /* 42 */
			new Feedback { voiceId = new ushort[] { 0x0026, 0xFFFF, 0xFFFF, 0xFFFF, 0xFFFF }, messageId = 0x00, soundId = 0x002E }, /* 43 */
			new Feedback { voiceId = new ushort[] { 0x0027, 0xFFFF, 0xFFFF, 0xFFFF, 0xFFFF }, messageId = 0x00, soundId = 0x002E }, /* 44 */
			new Feedback { voiceId = new ushort[] { 0x0028, 0xFFFF, 0xFFFF, 0xFFFF, 0xFFFF }, messageId = 0x00, soundId = 0x002E }, /* 45 */
			new Feedback { voiceId = new ushort[] { 0x0029, 0xFFFF, 0xFFFF, 0xFFFF, 0xFFFF }, messageId = 0x00, soundId = 0x002E }, /* 46 */
			new Feedback { voiceId = new ushort[] { 0x002A, 0xFFFF, 0xFFFF, 0xFFFF, 0xFFFF }, messageId = 0x00, soundId = 0x002E }, /* 47 */
			new Feedback { voiceId = new ushort[] { 0x0040, 0xFFFF, 0xFFFF, 0xFFFF, 0xFFFF }, messageId = 0x5A, soundId = 0x0017 }, /* 48 */
			new Feedback { voiceId = new ushort[] { 0x001E, 0x0024, 0x003F, 0xFFFF, 0xFFFF }, messageId = 0x9A, soundId = 0xFFFF }, /* 49 */
			new Feedback { voiceId = new ushort[] { 0x001F, 0x0024, 0x003F, 0xFFFF, 0xFFFF }, messageId = 0x9B, soundId = 0xFFFF }, /* 50 */
			new Feedback { voiceId = new ushort[] { 0x0020, 0x0024, 0x003F, 0xFFFF, 0xFFFF }, messageId = 0x9C, soundId = 0xFFFF }, /* 51 */
			new Feedback { voiceId = new ushort[] { 0x0021, 0x0024, 0x003F, 0xFFFF, 0xFFFF }, messageId = 0x9D, soundId = 0xFFFF }, /* 52 */
			new Feedback { voiceId = new ushort[] { 0x0022, 0x0024, 0x003F, 0xFFFF, 0xFFFF }, messageId = 0x9E, soundId = 0xFFFF }, /* 53 */
			new Feedback { voiceId = new ushort[] { 0x0023, 0x0024, 0x003F, 0xFFFF, 0xFFFF }, messageId = 0x9F, soundId = 0xFFFF }, /* 54 */
			new Feedback { voiceId = new ushort[] { 0x001E, 0x0041, 0x0042, 0xFFFF, 0xFFFF }, messageId = 0xA2, soundId = 0xFFFF }, /* 55 */
			new Feedback { voiceId = new ushort[] { 0x001F, 0x0041, 0x0042, 0xFFFF, 0xFFFF }, messageId = 0xA3, soundId = 0xFFFF }, /* 56 */
			new Feedback { voiceId = new ushort[] { 0x0020, 0x0041, 0x0042, 0xFFFF, 0xFFFF }, messageId = 0xA4, soundId = 0xFFFF }, /* 57 */
			new Feedback { voiceId = new ushort[] { 0x0021, 0x0041, 0x0042, 0xFFFF, 0xFFFF }, messageId = 0xA5, soundId = 0xFFFF }, /* 58 */
			new Feedback { voiceId = new ushort[] { 0x0022, 0x0041, 0x0042, 0xFFFF, 0xFFFF }, messageId = 0xA6, soundId = 0xFFFF }, /* 59 */
			new Feedback { voiceId = new ushort[] { 0x0023, 0x0041, 0x0042, 0xFFFF, 0xFFFF }, messageId = 0xA7, soundId = 0xFFFF }, /* 60 */
			new Feedback { voiceId = new ushort[] { 0x0046, 0x0047, 0xFFFF, 0xFFFF, 0xFFFF }, messageId = 0x00, soundId = 0xFFFF }, /* 61 */
			new Feedback { voiceId = new ushort[] { 0x001E, 0xFFFF, 0xFFFF, 0xFFFF, 0xFFFF }, messageId = 0x00, soundId = 0xFFFF }, /* 62 */
			new Feedback { voiceId = new ushort[] { 0x001F, 0xFFFF, 0xFFFF, 0xFFFF, 0xFFFF }, messageId = 0x00, soundId = 0xFFFF }, /* 63 */
			new Feedback { voiceId = new ushort[] { 0x0020, 0xFFFF, 0xFFFF, 0xFFFF, 0xFFFF }, messageId = 0x00, soundId = 0xFFFF }, /* 64 */
			new Feedback { voiceId = new ushort[] { 0x0021, 0xFFFF, 0xFFFF, 0xFFFF, 0xFFFF }, messageId = 0x00, soundId = 0xFFFF }, /* 65 */
			new Feedback { voiceId = new ushort[] { 0x0022, 0xFFFF, 0xFFFF, 0xFFFF, 0xFFFF }, messageId = 0x00, soundId = 0xFFFF }, /* 66 */
			new Feedback { voiceId = new ushort[] { 0x0023, 0xFFFF, 0xFFFF, 0xFFFF, 0xFFFF }, messageId = 0x00, soundId = 0xFFFF }, /* 67 */
			new Feedback { voiceId = new ushort[] { 0x001E, 0x0043, 0x0036, 0xFFFF, 0xFFFF }, messageId = 0x93, soundId = 0xFFFF }, /* 68 */
			new Feedback { voiceId = new ushort[] { 0x001F, 0x0043, 0x0036, 0xFFFF, 0xFFFF }, messageId = 0x94, soundId = 0xFFFF }, /* 69 */
			new Feedback { voiceId = new ushort[] { 0x0020, 0x0043, 0x0036, 0xFFFF, 0xFFFF }, messageId = 0x95, soundId = 0xFFFF }, /* 70 */
			new Feedback { voiceId = new ushort[] { 0x0021, 0x0043, 0x0036, 0xFFFF, 0xFFFF }, messageId = 0x96, soundId = 0xFFFF }, /* 71 */
			new Feedback { voiceId = new ushort[] { 0x0022, 0x0043, 0x0036, 0xFFFF, 0xFFFF }, messageId = 0x97, soundId = 0xFFFF }, /* 72 */
			new Feedback { voiceId = new ushort[] { 0x0023, 0x0043, 0x0036, 0xFFFF, 0xFFFF }, messageId = 0x98, soundId = 0xFFFF }, /* 73 */
			new Feedback { voiceId = new ushort[] { 0xFFFF, 0xFFFF, 0xFFFF, 0xFFFF, 0xFFFF }, messageId = 0x01, soundId = 0xFFFF }, /* 74 */
			new Feedback { voiceId = new ushort[] { 0xFFFF, 0xFFFF, 0xFFFF, 0xFFFF, 0xFFFF }, messageId = 0x00, soundId = 0xFFFF }, /* 75 */
			new Feedback { voiceId = new ushort[] { 0xFFFF, 0xFFFF, 0xFFFF, 0xFFFF, 0xFFFF }, messageId = 0x01, soundId = 0xFFFF }, /* 76 */
			new Feedback { voiceId = new ushort[] { 0x006F, 0x0069, 0xFFFF, 0xFFFF, 0xFFFF }, messageId = 0x00, soundId = 0xFFFF }, /* 77 */
			new Feedback { voiceId = new ushort[] { 0x0072, 0xFFFF, 0xFFFF, 0xFFFF, 0xFFFF }, messageId = 0x00, soundId = 0xFFFF }, /* 78 */
			new Feedback { voiceId = new ushort[] { 0x0065, 0x0073, 0x006A, 0xFFFF, 0xFFFF }, messageId = 0x00, soundId = 0xFFFF }, /* 79 */
			new Feedback { voiceId = new ushort[] { 0x0074, 0x0056, 0x005D, 0xFFFF, 0xFFFF }, messageId = 0x00, soundId = 0xFFFF }, /* 80 */
			new Feedback { voiceId = new ushort[] { 0x0076, 0x0053, 0x0054, 0xFFFF, 0xFFFF }, messageId = 0x00, soundId = 0xFFFF }, /* 81 */
			new Feedback { voiceId = new ushort[] { 0x0068, 0x0071, 0x0059, 0xFFFF, 0xFFFF }, messageId = 0x00, soundId = 0xFFFF }, /* 82 */
			new Feedback { voiceId = new ushort[] { 0x005C, 0x005E, 0x0061, 0x005B, 0xFFFF }, messageId = 0x00, soundId = 0xFFFF }, /* 83 */
			new Feedback { voiceId = new ushort[] { 0x0062, 0x0060, 0xFFFF, 0xFFFF, 0xFFFF }, messageId = 0x00, soundId = 0xFFFF }, /* 84 */
			new Feedback { voiceId = new ushort[] { 0x005A, 0x005F, 0xFFFF, 0xFFFF, 0xFFFF }, messageId = 0x00, soundId = 0xFFFF }, /* 85 */
			new Feedback { voiceId = new ushort[] { 0x0075, 0x004F, 0xFFFF, 0xFFFF, 0xFFFF }, messageId = 0x00, soundId = 0xFFFF }, /* 86 */
			new Feedback { voiceId = new ushort[] { 0x004E, 0x004D, 0x0055, 0x006D, 0xFFFF }, messageId = 0x01, soundId = 0xFFFF }, /* 87 */
			new Feedback { voiceId = new ushort[] { 0x006B, 0xFFFF, 0xFFFF, 0xFFFF, 0xFFFF }, messageId = 0x00, soundId = 0xFFFF }, /* 88 */
			new Feedback { voiceId = new ushort[] { 0x0067, 0x006E, 0xFFFF, 0xFFFF, 0xFFFF }, messageId = 0x00, soundId = 0xFFFF }, /* 89 */
			new Feedback { voiceId = new ushort[] { 0x0063, 0x0064, 0xFFFF, 0xFFFF, 0xFFFF }, messageId = 0x00, soundId = 0xFFFF }, /* 90 */
			new Feedback { voiceId = new ushort[] { 0x0066, 0x0070, 0xFFFF, 0xFFFF, 0xFFFF }, messageId = 0x00, soundId = 0xFFFF }, /* 91 */
			new Feedback { voiceId = new ushort[] { 0x0077, 0x0050, 0x0051, 0xFFFF, 0xFFFF }, messageId = 0x01, soundId = 0xFFFF }, /* 92 */
			new Feedback { voiceId = new ushort[] { 0x006C, 0xFFFF, 0xFFFF, 0xFFFF, 0xFFFF }, messageId = 0x01, soundId = 0xFFFF }  /* 93 */
		};

		/* Translated audio feedback of events and user commands. */
		static ushort[][] g_translatedVoice = { //[][NUM_SPEECH_PARTS]
			new ushort[] {0x002B, 0xFFFF, 0xFFFF, 0xFFFF, 0xFFFF}, /*  0 */
			new ushort[] {0x0031, 0x001D, 0xFFFF, 0xFFFF, 0xFFFF}, /*  1 */
			new ushort[] {0x0031, 0x001D, 0xFFFF, 0xFFFF, 0xFFFF}, /*  2 */
			new ushort[] {0x0031, 0x001D, 0xFFFF, 0xFFFF, 0xFFFF}, /*  3 */
			new ushort[] {0x0031, 0x001D, 0xFFFF, 0xFFFF, 0xFFFF}, /*  4 */
			new ushort[] {0x0031, 0x001D, 0xFFFF, 0xFFFF, 0xFFFF}, /*  5 */
			new ushort[] {0x0031, 0x001D, 0xFFFF, 0xFFFF, 0xFFFF}, /*  6 */
			new ushort[] {0x0031, 0x001D, 0xFFFF, 0xFFFF, 0xFFFF}, /*  7 */
			new ushort[] {0x0031, 0x001D, 0xFFFF, 0xFFFF, 0xFFFF}, /*  8 */
			new ushort[] {0x0031, 0x001D, 0xFFFF, 0xFFFF, 0xFFFF}, /*  9 */
			new ushort[] {0x0031, 0x001D, 0xFFFF, 0xFFFF, 0xFFFF}, /* 10 */
			new ushort[] {0x0031, 0x001D, 0xFFFF, 0xFFFF, 0xFFFF}, /* 11 */
			new ushort[] {0x0031, 0x0032, 0xFFFF, 0xFFFF, 0xFFFF}, /* 12 */
			new ushort[] {0x0024, 0xFFFF, 0xFFFF, 0xFFFF, 0xFFFF}, /* 13 */
			new ushort[] {0x0037, 0xFFFF, 0xFFFF, 0xFFFF, 0xFFFF}, /* 14 */
			new ushort[] {0x0037, 0xFFFF, 0xFFFF, 0xFFFF, 0xFFFF}, /* 15 */
			new ushort[] {0x0037, 0xFFFF, 0xFFFF, 0xFFFF, 0xFFFF}, /* 16 */
			new ushort[] {0x0037, 0xFFFF, 0xFFFF, 0xFFFF, 0xFFFF}, /* 17 */
			new ushort[] {0x0037, 0xFFFF, 0xFFFF, 0xFFFF, 0xFFFF}, /* 18 */
			new ushort[] {0x0037, 0xFFFF, 0xFFFF, 0xFFFF, 0xFFFF}, /* 19 */
			new ushort[] {0x0035, 0xFFFF, 0xFFFF, 0xFFFF, 0xFFFF}, /* 20 */
			new ushort[] {0x0025, 0xFFFF, 0xFFFF, 0xFFFF, 0xFFFF}, /* 21 */
			new ushort[] {0x0025, 0xFFFF, 0xFFFF, 0xFFFF, 0xFFFF}, /* 22 */
			new ushort[] {0x0025, 0xFFFF, 0xFFFF, 0xFFFF, 0xFFFF}, /* 23 */
			new ushort[] {0x0025, 0xFFFF, 0xFFFF, 0xFFFF, 0xFFFF}, /* 24 */
			new ushort[] {0x0025, 0xFFFF, 0xFFFF, 0xFFFF, 0xFFFF}, /* 25 */
			new ushort[] {0x0025, 0xFFFF, 0xFFFF, 0xFFFF, 0xFFFF}, /* 26 */
			new ushort[] {0x0025, 0xFFFF, 0xFFFF, 0xFFFF, 0xFFFF}, /* 27 */
			new ushort[] {0x002E, 0xFFFF, 0xFFFF, 0xFFFF, 0xFFFF}, /* 28 */
			new ushort[] {0x002D, 0xFFFF, 0xFFFF, 0xFFFF, 0xFFFF}, /* 29 */
			new ushort[] {0x0036, 0xFFFF, 0xFFFF, 0xFFFF, 0xFFFF}, /* 30 */
			new ushort[] {0x0036, 0xFFFF, 0xFFFF, 0xFFFF, 0xFFFF}, /* 31 */
			new ushort[] {0x0036, 0xFFFF, 0xFFFF, 0xFFFF, 0xFFFF}, /* 32 */
			new ushort[] {0x0036, 0xFFFF, 0xFFFF, 0xFFFF, 0xFFFF}, /* 33 */
			new ushort[] {0x0036, 0xFFFF, 0xFFFF, 0xFFFF, 0xFFFF}, /* 34 */
			new ushort[] {0x0036, 0xFFFF, 0xFFFF, 0xFFFF, 0xFFFF}, /* 35 */
			new ushort[] {0x0034, 0xFFFF, 0xFFFF, 0xFFFF, 0xFFFF}, /* 36 */
			new ushort[] {0x0031, 0x0044, 0xFFFF, 0xFFFF, 0xFFFF}, /* 37 */
			new ushort[] {0x002F, 0xFFFF, 0xFFFF, 0xFFFF, 0xFFFF}, /* 38 */
			new ushort[] {0x0031, 0x0033, 0xFFFF, 0xFFFF, 0xFFFF}, /* 39 */
			new ushort[] {0xFFFF, 0xFFFF, 0xFFFF, 0xFFFF, 0xFFFF}, /* 40 */
			new ushort[] {0xFFFF, 0xFFFF, 0xFFFF, 0xFFFF, 0xFFFF}, /* 41 */
			new ushort[] {0x003F, 0xFFFF, 0xFFFF, 0xFFFF, 0xFFFF}, /* 42 */
			new ushort[] {0x0026, 0xFFFF, 0xFFFF, 0xFFFF, 0xFFFF}, /* 43 */
			new ushort[] {0x0027, 0xFFFF, 0xFFFF, 0xFFFF, 0xFFFF}, /* 44 */
			new ushort[] {0x0028, 0xFFFF, 0xFFFF, 0xFFFF, 0xFFFF}, /* 45 */
			new ushort[] {0x0029, 0xFFFF, 0xFFFF, 0xFFFF, 0xFFFF}, /* 46 */
			new ushort[] {0x002A, 0xFFFF, 0xFFFF, 0xFFFF, 0xFFFF}, /* 47 */
			new ushort[] {0x0040, 0xFFFF, 0xFFFF, 0xFFFF, 0xFFFF}, /* 48 */
			new ushort[] {0x0041, 0xFFFF, 0xFFFF, 0xFFFF, 0xFFFF}, /* 49 */
			new ushort[] {0x0041, 0xFFFF, 0xFFFF, 0xFFFF, 0xFFFF}, /* 50 */
			new ushort[] {0x0041, 0xFFFF, 0xFFFF, 0xFFFF, 0xFFFF}, /* 51 */
			new ushort[] {0x0041, 0xFFFF, 0xFFFF, 0xFFFF, 0xFFFF}, /* 52 */
			new ushort[] {0x0041, 0xFFFF, 0xFFFF, 0xFFFF, 0xFFFF}, /* 53 */
			new ushort[] {0x0041, 0xFFFF, 0xFFFF, 0xFFFF, 0xFFFF}, /* 54 */
			new ushort[] {0x0042, 0xFFFF, 0xFFFF, 0xFFFF, 0xFFFF}, /* 55 */
			new ushort[] {0x0042, 0xFFFF, 0xFFFF, 0xFFFF, 0xFFFF}, /* 56 */
			new ushort[] {0x0042, 0xFFFF, 0xFFFF, 0xFFFF, 0xFFFF}, /* 57 */
			new ushort[] {0x0042, 0xFFFF, 0xFFFF, 0xFFFF, 0xFFFF}, /* 58 */
			new ushort[] {0x0042, 0xFFFF, 0xFFFF, 0xFFFF, 0xFFFF}, /* 59 */
			new ushort[] {0x0042, 0xFFFF, 0xFFFF, 0xFFFF, 0xFFFF}, /* 60 */
			new ushort[] {0x0046, 0xFFFF, 0xFFFF, 0xFFFF, 0xFFFF}, /* 61 */
			new ushort[] {0x001E, 0xFFFF, 0xFFFF, 0xFFFF, 0xFFFF}, /* 62 */
			new ushort[] {0x001F, 0xFFFF, 0xFFFF, 0xFFFF, 0xFFFF}, /* 63 */
			new ushort[] {0x0020, 0xFFFF, 0xFFFF, 0xFFFF, 0xFFFF}, /* 64 */
			new ushort[] {0xFFFF, 0xFFFF, 0xFFFF, 0xFFFF, 0xFFFF}, /* 65 */
			new ushort[] {0xFFFF, 0xFFFF, 0xFFFF, 0xFFFF, 0xFFFF}, /* 66 */
			new ushort[] {0xFFFF, 0xFFFF, 0xFFFF, 0xFFFF, 0xFFFF}, /* 67 */
			new ushort[] {0x0043, 0xFFFF, 0xFFFF, 0xFFFF, 0xFFFF}, /* 68 */
			new ushort[] {0x0043, 0xFFFF, 0xFFFF, 0xFFFF, 0xFFFF}, /* 69 */
			new ushort[] {0x0043, 0xFFFF, 0xFFFF, 0xFFFF, 0xFFFF}, /* 70 */
			new ushort[] {0x0043, 0xFFFF, 0xFFFF, 0xFFFF, 0xFFFF}, /* 71 */
			new ushort[] {0x0043, 0xFFFF, 0xFFFF, 0xFFFF, 0xFFFF}, /* 72 */
			new ushort[] {0x0043, 0xFFFF, 0xFFFF, 0xFFFF, 0xFFFF}, /* 73 */
			new ushort[] {0xFFFF, 0xFFFF, 0xFFFF, 0xFFFF, 0xFFFF}, /* 74 */
			new ushort[] {0x0057, 0xFFFF, 0xFFFF, 0xFFFF, 0xFFFF}, /* 75 */
			new ushort[] {0x0052, 0x0058, 0xFFFF, 0xFFFF, 0xFFFF}, /* 76 */
			new ushort[] {0x006F, 0x0069, 0xFFFF, 0xFFFF, 0xFFFF}, /* 77 */
			new ushort[] {0x0072, 0xFFFF, 0xFFFF, 0xFFFF, 0xFFFF}, /* 78 */
			new ushort[] {0x0065, 0x0073, 0x006A, 0xFFFF, 0xFFFF}, /* 79 */
			new ushort[] {0x0074, 0x0056, 0x005D, 0xFFFF, 0xFFFF}, /* 80 */
			new ushort[] {0x0076, 0x0053, 0x0054, 0xFFFF, 0xFFFF}, /* 81 */
			new ushort[] {0x0068, 0x0071, 0x0059, 0xFFFF, 0xFFFF}, /* 82 */
			new ushort[] {0x005C, 0x005E, 0x0061, 0x005B, 0xFFFF}, /* 83 */
			new ushort[] {0x0062, 0x0060, 0xFFFF, 0xFFFF, 0xFFFF}, /* 84 */
			new ushort[] {0x005A, 0x005F, 0xFFFF, 0xFFFF, 0xFFFF}, /* 85 */
			new ushort[] {0x0075, 0x004F, 0xFFFF, 0xFFFF, 0xFFFF}, /* 86 */
			new ushort[] {0x004E, 0x004D, 0x0055, 0x006D, 0xFFFF}, /* 87 */
			new ushort[] {0x006B, 0xFFFF, 0xFFFF, 0xFFFF, 0xFFFF}, /* 88 */
			new ushort[] {0x0067, 0x006E, 0xFFFF, 0xFFFF, 0xFFFF}, /* 89 */
			new ushort[] {0x0063, 0x0064, 0xFFFF, 0xFFFF, 0xFFFF}, /* 90 */
			new ushort[] {0x0066, 0x0070, 0xFFFF, 0xFFFF, 0xFFFF}, /* 91 */
			new ushort[] {0x0077, 0x0050, 0x0051, 0xFFFF, 0xFFFF}, /* 92 */
			new ushort[] {0x006C, 0xFFFF, 0xFFFF, 0xFFFF, 0xFFFF}  /* 93 */
		};

		static string s_currentMusic = null;        /*!< Currently loaded music file. */

		static void Driver_Music_Play(short index, ushort volume)
		{
			var music = CDriver.g_driverMusic;
			var musicBuffer = CDriver.g_bufferMusic;

			if (index < 0 || index > 120 || Config.g_gameConfig.music == 0) return;

			if (music.index == 0xFFFF) return;

			if (musicBuffer.index != 0xFFFF)
			{
				Mt32Mpu.MPU_Stop(musicBuffer.index);
				Mt32Mpu.MPU_ClearData(musicBuffer.index);
				musicBuffer.index = 0xFFFF;
			}

			musicBuffer.index = Mt32Mpu.MPU_SetData(music.content, (ushort)index, musicBuffer.buffer[0]);

			Mt32Mpu.MPU_Play(musicBuffer.index);
			Mt32Mpu.MPU_SetVolume(musicBuffer.index, (ushort)(((volume & 0xFF) * 90) / 256), 0);
		}

		static void Driver_Music_LoadFile(string musicName)
		{
			var music = CDriver.g_driverMusic;
			var sound = CDriver.g_driverSound;

			CDriver.Driver_Music_Stop();

			if (music.index == 0xFFFF) return;

			if (music.content == sound.content)
			{
				music.content = null;
				music.filename = null; //"\0"
				music.contentMalloced = false;
			}
			else
			{
				CDriver.Driver_UnloadFile(music);
			}

			if (sound.filename != null /*"\0"*/ && musicName != null && string.Equals(CDriver.Drivers_GenerateFilename(musicName, music), sound.filename, StringComparison.OrdinalIgnoreCase))
			{ //strcasecmp
				CDriver.g_driverMusic.content = CDriver.g_driverSound.content;
				CDriver.g_driverMusic.filename = CDriver.g_driverSound.filename; //memcpy(g_driverMusic->filename, g_driverSound->filename, sizeof(g_driverMusic->filename));
				CDriver.g_driverMusic.contentMalloced = CDriver.g_driverSound.contentMalloced;

				return;
			}

			CDriver.Driver_LoadFile(musicName, music);
		}

		static ushort currentMusicID = 0;
		/*
        * Plays a music.
        * @param index The index of the music to play.
        */
		internal static void Music_Play(ushort musicID)
		{
			if (musicID == 0xFFFF || musicID >= 38 || musicID == currentMusicID) return;

			currentMusicID = musicID;

			if (g_table_musics[musicID].name != s_currentMusic)
			{
				s_currentMusic = g_table_musics[musicID].name;

				CDriver.Driver_Music_Stop();
				CDriver.Driver_Voice_Play(null, 0xFF);
				Driver_Music_LoadFile(null);
				CDriver.Driver_Sound_LoadFile(null);
				Driver_Music_LoadFile(s_currentMusic);
				CDriver.Driver_Sound_LoadFile(s_currentMusic);
			}

			Driver_Music_Play((short)g_table_musics[musicID].index, 0xFF);
		}

		/*
		 * Output feedback about events of the game.
		 * @param index Feedback to provide (\c 0xFFFF means do nothing, \c 0xFFFE means stop, otherwise a feedback code).
		 * @note If sound is disabled, the main viewport is used to display a message.
		 */
		internal static void Sound_Output_Feedback(ushort index)
		{
			if (index == 0xFFFF) return;

			if (index == 0xFFFE)
			{
				byte i;

				/* Clear spoken audio. */
				for (i = 0; i < /*Common.lengthof<ushort>(s_spokenWords)*/s_spokenWords.Length; i++)
				{
					s_spokenWords[i] = 0xFFFF;
				}

				CDriver.Driver_Voice_Stop();

				Gui.g_viewportMessageText = null;
				if ((Gui.g_viewportMessageCounter & 1) != 0)
				{
					CSharpDune.g_viewport_forceRedraw = true;
					Gui.g_viewportMessageCounter = 0;
				}
				s_currentVoicePriority = 0;

				return;
			}

			if (!Config.g_enableVoices || Config.g_gameConfig.sounds == 0)
			{
				CDriver.Driver_Sound_Play((short)g_feedback[index].soundId, 0xFF);

				Gui.g_viewportMessageText = CString.String_Get_ByIndex(g_feedback[index].messageId);

				if ((Gui.g_viewportMessageCounter & 1) != 0)
				{
					CSharpDune.g_viewport_forceRedraw = true;
				}

				Gui.g_viewportMessageCounter = 4;

				return;
			}

			/* If nothing is being said currently, load new words. */
			if (s_spokenWords[0] == 0xFFFF)
			{
				byte i;

				for (i = 0; i < /*Common.lengthof<ushort>(s_spokenWords)*/s_spokenWords.Length; i++)
				{
					s_spokenWords[i] = (Config.g_config.language == (byte)Language.LANGUAGE_ENGLISH) ? g_feedback[index].voiceId[i] : g_translatedVoice[i][index]; //[index][i];
				}
			}

			Sound_StartSpeech();
		}

		/*
		 * Start speech.
		 * Start a new speech fragment if possible.
		 * @return Sound is produced.
		 */
		internal static bool Sound_StartSpeech()
		{
			if (Config.g_gameConfig.sounds == 0) return false;

			if (CDriver.Driver_Voice_IsPlaying()) return true;

			s_currentVoicePriority = 0;

			if (s_spokenWords[0] == 0xFFFF) return false;

			Sound_StartSound(s_spokenWords[0]);
			/* Move speech parts one place. */
			Array.Copy(s_spokenWords, 1, s_spokenWords, 0, s_spokenWords.Length - 1); //(s_spokenWords.Length - s_spokenWords[0]) * 2); //memmove(&s_spokenWords[0], &s_spokenWords[1], sizeof(s_spokenWords) - sizeof(s_spokenWords[0]));
			s_spokenWords[/*Common.lengthof<ushort>(s_spokenWords)*/s_spokenWords.Length - 1] = 0xFFFF;

			return true;
		}

		/*
		 * Start playing a sound sample.
		 * @param index Sample to play.
		 */
		internal static void Sound_StartSound(ushort index)
		{
			if (index == 0xFFFF || Config.g_gameConfig.sounds == 0 || (short)g_table_voices[index].priority < s_currentVoicePriority) return;

			s_currentVoicePriority = (short)g_table_voices[index].priority;

			if (g_voiceData[index] != null)
			{
				CDriver.Driver_Voice_Play(g_voiceData[index], 0xFF);
			}
			else
			{
				string filenameBuffer, filename;

				filename = g_table_voices[index].str;
				if (filename[0] == '?')
				{
					//snprintf(filenameBuffer, sizeof(filenameBuffer), filename + 1, g_playerHouseID < HOUSE_MAX ? g_table_houseInfo[g_playerHouseID].prefixChar : ' ');
					filenameBuffer = filename[1..].Replace("%c", CHouse.g_playerHouseID < HouseType.HOUSE_MAX ? Convert.ToString((char)CHouse.g_table_houseInfo[(int)CHouse.g_playerHouseID].prefixChar) : " ");

					if (CSharpDune.g_readBuffer.Length < CSharpDune.g_readBufferSize)
						Array.Resize(ref CSharpDune.g_readBuffer, (int)CSharpDune.g_readBufferSize);

					CDriver.Driver_Voice_LoadFile(filenameBuffer, CSharpDune.g_readBuffer, CSharpDune.g_readBufferSize);

					CDriver.Driver_Voice_Play(CSharpDune.g_readBuffer, 0xFF);
				}
			}
		}

		/*
		 * Play a voice. Volume is based on distance to position.
		 * @param voiceID Which voice to play.
		 * @param position Which position to play it on.
		 */
		internal static void Voice_PlayAtTile(short voiceID, tile32 position)
		{
			ushort index;
			ushort volume;

			if (voiceID < 0 || voiceID >= 120) return;
			if (Config.g_gameConfig.sounds == 0) return;

			volume = 255;
			if (position.x != 0 || position.y != 0)
			{
				volume = CTile.Tile_GetDistancePacked(Gui.g_minimapPosition, CTile.Tile_PackTile(position));
				if (volume > 64) volume = 64;

				volume = (ushort)(255 - (volume * 255 / 80));
			}

			index = g_table_voiceMapping[voiceID];

			if (Config.g_enableVoices && index != 0xFFFF && g_voiceData[index] != null && g_table_voices[index].priority >= s_currentVoicePriority)
			{
				s_currentVoicePriority = (short)g_table_voices[index].priority;

				//CSharpDune.g_readBuffer = new byte[g_voiceDataSize[index]];
				//Array.Copy(g_voiceData[index], CSharpDune.g_readBuffer, g_voiceDataSize[index]); //memmove(CSharpDune.g_readBuffer, g_voiceData[index], g_voiceDataSize[index]);

				CDriver.Driver_Voice_Play(/*CSharpDune.g_readBuffer*/g_voiceData[index], s_currentVoicePriority);
			}
			else
			{
				CDriver.Driver_Sound_Play(voiceID, (short)volume);
			}
		}

		/*
		 * Play a voice.
		 * @param voiceID The voice to play.
		 */
		internal static void Voice_Play(short voiceID)
		{
			var tile = new tile32();

			tile.x = 0;
			tile.y = 0;
			Voice_PlayAtTile(voiceID, tile);
		}

		/*
		 * Initialises the MT-32.
		 * @param index The index of the music to play.
		 */
		internal static void Music_InitMT32()
		{
			ushort left = 0;

			Driver_Music_LoadFile("DUNEINIT");

			Driver_Music_Play(0, 0xFF);

			Gui.GUI_DrawText(CString.String_Get_ByIndex(15), 0, 0, 15, 12); /* "Initializing the MT-32" */

			while (CDriver.Driver_Music_IsPlaying())
			{
				Timer.Timer_Sleep(60);

				left += 6;
				Gui.GUI_DrawText(".", (short)left, 10, 15, 12);
			}
		}

		static ushort currentVoiceSet = 0xFFFE;
		/*
		 * Load voices.
		 * voiceSet 0xFFFE is for Game Intro.
		 * voiceSet 0xFFFF is for Game End.
		 * @param voiceSet Voice set to load : either a HouseID, or special values 0xFFFE or 0xFFFF.
		 */
		internal static void Voice_LoadVoices(ushort voiceSet)
		{
			int prefixChar = ' ';
			ushort voice;

			if (!Config.g_enableVoices/* == 0*/) return;

			for (voice = 0; voice < NUM_VOICES; voice++)
			{
				switch (g_table_voices[voice].str[0])
				{
					case '%':
						if (Config.g_config.language != (byte)Language.LANGUAGE_ENGLISH || currentVoiceSet == voiceSet)
						{
							if (voiceSet != 0xFFFF && voiceSet != 0xFFFE) break;
						}

						g_voiceData[voice] = null; //free(g_voiceData[voice]);
						break;

					case '+':
						if (voiceSet != 0xFFFF && voiceSet != 0xFFFE) break;

						g_voiceData[voice] = null; //free(g_voiceData[voice]);
						break;

					case '-':
						if (voiceSet == 0xFFFF) break;

						g_voiceData[voice] = null; //free(g_voiceData[voice]);
						break;

					case '/':
						if (voiceSet != 0xFFFE) break;

						g_voiceData[voice] = null; //free(g_voiceData[voice]);
						break;

					case '?':
						if (voiceSet == 0xFFFF) break;

						/* No free() as there was never a malloc(). */
						g_voiceData[voice] = null;
						break;

					default:
						break;
				}
			}

			if (currentVoiceSet == voiceSet) return;

			for (voice = 0; voice < NUM_VOICES; voice++)
			{
				string filename; //char[16]
				var str = g_table_voices[voice].str;
				Sleep.sleepIdle();  /* let a chance to update screen, etc. */
				switch (str[0])
				{
					case '%':
						if (g_voiceData[voice] != null || currentVoiceSet == voiceSet || voiceSet == 0xFFFF || voiceSet == 0xFFFE) break;

						switch ((Language)Config.g_config.language)
						{
							case Language.LANGUAGE_FRENCH: prefixChar = 'F'; break;
							case Language.LANGUAGE_GERMAN: prefixChar = 'G'; break;
							default: prefixChar = CHouse.g_table_houseInfo[voiceSet].prefixChar; break;
						}
						filename = str.Replace("%c", ((char)prefixChar).ToString()); //snprintf(filename, sizeof(filename), str, prefixChar);

						g_voiceData[voice] = Sound_LoadVoc(filename, out g_voiceDataSize[voice]);
						break;

					case '+':
						if (voiceSet == 0xFFFF || g_voiceData[voice] != null) break;

						switch ((Language)Config.g_config.language)
						{
							case Language.LANGUAGE_FRENCH: prefixChar = 'F'; break;
							case Language.LANGUAGE_GERMAN: prefixChar = 'G'; break;
							default: prefixChar = 'Z'; break;
						}
						filename = str[1..].Replace("%c", ((char)prefixChar).ToString()); //snprintf(filename, sizeof(filename), str + 1, prefixChar);

						/* XXX - In the 1.07us datafiles, a few files are named differently:
						 *
						 *  moveout.voc
						 *  overout.voc
						 *  report1.voc
						 *  report2.voc
						 *  report3.voc
						 *
						 * They come without letter in front of them. To make things a bit
						 *  easier, just check if the file exists, then remove the first
						 *  letter and see if it works then.
						 */
						if (!CFile.File_Exists(filename))
						{
							filename = filename.Remove(0, 1); //Array.Copy(filename, 1, filename, 0, filename.Length); //memmove(filename, filename + 1, strlen(filename));
						}

						g_voiceData[voice] = Sound_LoadVoc(filename, out g_voiceDataSize[voice]);
						break;

					case '-':
						if (voiceSet != 0xFFFF || g_voiceData[voice] != null) break;

						g_voiceData[voice] = Sound_LoadVoc(str[1..], out g_voiceDataSize[voice]);
						break;

					case '/':
						if (voiceSet != 0xFFFE) break;

						g_voiceData[voice] = Sound_LoadVoc(str[1..], out g_voiceDataSize[voice]);
						break;

					case '?':
						break;

					default:
						if (g_voiceData[voice] != null) break;

						g_voiceData[voice] = Sound_LoadVoc(str, out g_voiceDataSize[voice]);
						break;
				}
			}
			currentVoiceSet = voiceSet;
		}

		/*
		 * Unload voices.
		 */
		internal static void Voice_UnloadVoices()
		{
			ushort voice;

			for (voice = 0; voice < NUM_VOICES; voice++)
			{
				//free(g_voiceData[voice]);
				g_voiceData[voice] = null;
			}
		}

		/*
		 * Load a voice file to a malloc'd buffer.
		 * @param filename The name of the file to load.
		 * @return Where the file is loaded.
		 */
		static byte[] Sound_LoadVoc(string filename, out uint retFileSize)
		{
			byte[] res;

			retFileSize = 0;

			if (filename == null) return null;
			if (!CFile.File_Exists_GetSize(filename, out var fileSize)) return null;

			fileSize += 1;
			fileSize &= 0xFFFFFFFE;

			retFileSize = fileSize;
			res = new byte[fileSize]; //malloc(fileSize);
			CDriver.Driver_Voice_LoadFile(filename, res, fileSize);

			return res;
		}
	}
}
