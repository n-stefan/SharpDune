/* Sound file tables */

namespace SharpDune.Table;

static class TableSound
{
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
    internal static readonly VoiceData[] g_table_voices = [ //[NUM_VOICES]
		new() { str = "+VSCREAM1.VOC",  priority = 11}, /*   0 */
		new() { str = "+EXSAND.VOC",    priority = 10}, /*   1 */
		new() { str = "+ROCKET.VOC",    priority = 11}, /*   2 */
		new() { str = "+BUTTON.VOC",    priority = 10}, /*   3 */
		new() { str = "+VSCREAM5.VOC",  priority = 11}, /*   4 */
		new() { str = "+CRUMBLE.VOC",   priority = 15}, /*   5 */
		new() { str = "+EXSMALL.VOC",   priority = 9}, /*   6 */
		new() { str = "+EXMED.VOC",     priority = 10}, /*   7 */
		new() { str = "+EXLARGE.VOC",   priority = 14}, /*   8 */
		new() { str = "+EXCANNON.VOC",  priority = 11}, /*   9 */
		new() { str = "+GUNMULTI.VOC",  priority = 9}, /*  10 */
		new() { str = "+GUN.VOC",       priority = 10}, /*  11 */
		new() { str = "+EXGAS.VOC",     priority = 10}, /*  12 */
		new() { str = "+EXDUD.VOC",     priority = 10}, /*  13 */
		new() { str = "+VSCREAM2.VOC",  priority = 11}, /*  14 */
		new() { str = "+VSCREAM3.VOC",  priority = 11}, /*  15 */
		new() { str = "+VSCREAM4.VOC",  priority = 11}, /*  16 */
		new() { str = "+%cAFFIRM.VOC",  priority = 15}, /*  17 */
		new() { str = "+%cREPORT1.VOC", priority = 15}, /*  18 */
		new() { str = "+%cREPORT2.VOC", priority = 15}, /*  19 */
		new() { str = "+%cREPORT3.VOC", priority = 15}, /*  20 */
		new() { str = "+%cOVEROUT.VOC", priority = 15}, /*  21 */
		new() { str = "+%cMOVEOUT.VOC", priority = 15}, /*  22 */
		new() { str = "?POPPA.VOC",     priority = 15}, /*  23 */
		new() { str = "?SANDBUG.VOC",   priority = 15}, /*  24 */
		new() { str = "+STATICP.VOC",   priority = 10}, /*  25 */
		new() { str = "+WORMET3P.VOC",  priority = 16}, /*  26 */
		new() { str = "+MISLTINP.VOC",  priority = 10}, /*  27 */
		new() { str = "+SQUISH2.VOC",   priority = 12}, /*  28 */
		new() { str = "%cENEMY.VOC",    priority = 20}, /*  29 */
		new() { str = "%cHARK.VOC",     priority = 20}, /*  30 */
		new() { str = "%cATRE.VOC",     priority = 20}, /*  31 */
		new() { str = "%cORDOS.VOC",    priority = 20}, /*  32 */
		new() { str = "%cFREMEN.VOC",   priority = 20}, /*  33 */
		new() { str = "%cSARD.VOC",     priority = 20}, /*  34 */
		new() { str = "FILLER.VOC",     priority = 20}, /*  35 */
		new() { str = "%cUNIT.VOC",     priority = 20}, /*  36 */
		new() { str = "%cSTRUCT.VOC",   priority = 20}, /*  37 */
		new() { str = "%cONE.VOC",      priority = 19}, /*  38 */
		new() { str = "%cTWO.VOC",      priority = 19}, /*  39 */
		new() { str = "%cTHREE.VOC",    priority = 19}, /*  40 */
		new() { str = "%cFOUR.VOC",     priority = 19}, /*  41 */
		new() { str = "%cFIVE.VOC",     priority = 19}, /*  42 */
		new() { str = "%cCONST.VOC",    priority = 20}, /*  43 */
		new() { str = "%cRADAR.VOC",    priority = 20}, /*  44 */
		new() { str = "%cOFF.VOC",      priority = 20}, /*  45 */
		new() { str = "%cON.VOC",       priority = 20}, /*  46 */
		new() { str = "%cFRIGATE.VOC",  priority = 20}, /*  47 */
		new() { str = "?%cARRIVE.VOC",  priority = 20}, /*  48 */
		new() { str = "%cWARNING.VOC",  priority = 20}, /*  49 */
		new() { str = "%cSABOT.VOC",    priority = 20}, /*  50 */
		new() { str = "%cMISSILE.VOC",  priority = 20}, /*  51 */
		new() { str = "%cBLOOM.VOC",    priority = 20}, /*  52 */
		new() { str = "%cDESTROY.VOC",  priority = 20}, /*  53 */
		new() { str = "%cDEPLOY.VOC",   priority = 20}, /*  54 */
		new() { str = "%cAPPRCH.VOC",   priority = 20}, /*  55 */
		new() { str = "%cLOCATED.VOC",  priority = 20}, /*  56 */
		new() { str = "%cNORTH.VOC",    priority = 20}, /*  57 */
		new() { str = "%cEAST.VOC",     priority = 20}, /*  58 */
		new() { str = "%cSOUTH.VOC",    priority = 20}, /*  59 */
		new() { str = "%cWEST.VOC",     priority = 20}, /*  60 */
		new() { str = "?%cWIN.VOC",     priority = 20}, /*  61 */
		new() { str = "?%cLOSE.VOC",    priority = 20}, /*  62 */
		new() { str = "%cLAUNCH.VOC",   priority = 20}, /*  63 */
		new() { str = "%cATTACK.VOC",   priority = 20}, /*  64 */
		new() { str = "%cVEHICLE.VOC",  priority = 20}, /*  65 */
		new() { str = "%cREPAIR.VOC",   priority = 20}, /*  66 */
		new() { str = "%cHARVEST.VOC",  priority = 20}, /*  67 */
		new() { str = "%cWORMY.VOC",    priority = 20}, /*  68 */
		new() { str = "%cCAPTURE.VOC",  priority = 20}, /*  69 */
		new() { str = "%cNEXT.VOC",     priority = 20}, /*  70 */
		new() { str = "%cNEXT2.VOC",    priority = 20}, /*  71 */
		new() { str = "/BLASTER.VOC",   priority = 10}, /*  72 */
		new() { str = "/GLASS6.VOC",    priority = 10}, /*  73 */
		new() { str = "/LIZARD1.VOC",   priority = 10}, /*  74 */
		new() { str = "/FLESH.VOC",     priority = 10}, /*  75 */
		new() { str = "/CLICK.VOC",     priority = 10}, /*  76 */
		new() { str = "-3HOUSES.VOC",   priority = 12}, /*  77 */
		new() { str = "-ANDNOW.VOC",    priority = 12}, /*  78 */
		new() { str = "-ARRIVED.VOC",   priority = 12}, /*  79 */
		new() { str = "-BATTLE.VOC",    priority = 12}, /*  80 */
		new() { str = "-BEGINS.VOC",    priority = 12}, /*  81 */
		new() { str = "-BLDING.VOC",    priority = 12}, /*  82 */
		new() { str = "-CONTROL2.VOC",  priority = 12}, /*  83 */
		new() { str = "-CONTROL3.VOC",  priority = 12}, /*  84 */
		new() { str = "-CONTROL4.VOC",  priority = 12}, /*  85 */
		new() { str = "-CONTROLS.VOC",  priority = 12}, /*  86 */
		new() { str = "-DUNE.VOC",      priority = 12}, /*  87 */
		new() { str = "-DYNASTY.VOC",   priority = 12}, /*  88 */
		new() { str = "-EACHHOME.VOC",  priority = 12}, /*  89 */
		new() { str = "-EANDNO.VOC",    priority = 12}, /*  90 */
		new() { str = "-ECONTROL.VOC",  priority = 12}, /*  91 */
		new() { str = "-EHOUSE.VOC",    priority = 12}, /*  92 */
		new() { str = "-EMPIRE.VOC",    priority = 12}, /*  93 */
		new() { str = "-EPRODUCE.VOC",  priority = 12}, /*  94 */
		new() { str = "-ERULES.VOC",    priority = 12}, /*  95 */
		new() { str = "-ETERRIT.VOC",   priority = 12}, /*  96 */
		new() { str = "-EMOST.VOC",     priority = 12}, /*  97 */
		new() { str = "-ENOSET.VOC",    priority = 12}, /*  98 */
		new() { str = "-EVIL.VOC",      priority = 12}, /*  99 */
		new() { str = "-HARK.VOC",      priority = 12}, /* 100 */
		new() { str = "-HOME.VOC",      priority = 12}, /* 101 */
		new() { str = "-HOUSE2.VOC",    priority = 12}, /* 102 */
		new() { str = "-INSID.VOC",     priority = 12}, /* 103 */
		new() { str = "-KING.VOC",      priority = 12}, /* 104 */
		new() { str = "-KNOWN.VOC",     priority = 12}, /* 105 */
		new() { str = "-MELANGE.VOC",   priority = 12}, /* 106 */
		new() { str = "-NOBLE.VOC",     priority = 12}, /* 107 */
		new() { str = "?NOW.VOC",       priority = 12}, /* 108 */
		new() { str = "-OFDUNE.VOC",    priority = 12}, /* 109 */
		new() { str = "-ORD.VOC",       priority = 12}, /* 110 */
		new() { str = "-PLANET.VOC",    priority = 12}, /* 111 */
		new() { str = "-PREVAIL.VOC",   priority = 12}, /* 112 */
		new() { str = "-PROPOSED.VOC",  priority = 12}, /* 113 */
		new() { str = "-SANDLAND.VOC",  priority = 12}, /* 114 */
		new() { str = "-SPICE.VOC",     priority = 12}, /* 115 */
		new() { str = "-SPICE2.VOC",    priority = 12}, /* 116 */
		new() { str = "-VAST.VOC",      priority = 12}, /* 117 */
		new() { str = "-WHOEVER.VOC",   priority = 12}, /* 118 */
		new() { str = "?YOUR.VOC",      priority = 12}, /* 119 */
		new() { str = "?FILLER.VOC",    priority = 12}, /* 120 */
		new() { str = "-DROPEQ2P.VOC",  priority = 10}, /* 121 */
		new() { str = "/EXTINY.VOC",    priority = 10}, /* 122 */
		new() { str = "-WIND2BP.VOC",   priority = 10}, /* 123 */
		new() { str = "-BRAKES2P.VOC",  priority = 11}, /* 124 */
		new() { str = "-GUNSHOT.VOC",   priority = 10}, /* 125 */
		new() { str = "-GLASS.VOC",     priority = 11}, /* 126 */
		new() { str = "-MISSLE8.VOC",   priority = 10}, /* 127 */
		new() { str = "-CLANK.VOC",     priority = 10}, /* 128 */
		new() { str = "-BLOWUP1.VOC",   priority = 10}, /* 129 */
		new() { str = "-BLOWUP2.VOC",   priority = 11}  /* 130 */
	];

    /*
     * Available music.
     * @note The code compares pointers rather than the text itself, thus strings must be unique.
     */
    internal static readonly MusicData[] g_table_musics = [
        new() { name = null, index = 0 }, /*  0 */
		new() { name = "dune1", index = 2 }, /*  1 */
		new() { name = "dune1", index = 3 }, /*  2 */
		new() { name = "dune1", index = 4 }, /*  3 */
		new() { name = "dune1", index = 5 }, /*  4 */
		new() { name = "dune17", index = 4 }, /*  5 */
		new() { name = "dune8", index = 3 }, /*  6 */
		new() { name = "dune8", index = 2 }, /*  7 */
		new() { name = "dune1", index = 6 }, /*  8 */
		new() { name = "dune2", index = 6 }, /*  9 */
		new() { name = "dune3", index = 6 }, /* 10 */
		new() { name = "dune4", index = 6 }, /* 11 */
		new() { name = "dune5", index = 6 }, /* 12 */
		new() { name = "dune6", index = 6 }, /* 13 */
		new() { name = "dune9", index = 4 }, /* 14 */
		new() { name = "dune9", index = 5 }, /* 15 */
		new() { name = "dune18", index = 6 }, /* 16 */
		new() { name = "dune10", index = 7 }, /* 17 */
		new() { name = "dune11", index = 7 }, /* 18 */
		new() { name = "dune12", index = 7 }, /* 19 */
		new() { name = "dune13", index = 7 }, /* 20 */
		new() { name = "dune14", index = 7 }, /* 21 */
		new() { name = "dune15", index = 7 }, /* 22 */
		new() { name = "dune1", index = 8 }, /* 23 */
		new() { name = "dune7", index = 2 }, /* 24 */
		new() { name = "dune7", index = 3 }, /* 25 */
		new() { name = "dune7", index = 4 }, /* 26 */
		new() { name = "dune0", index = 2 }, /* 27 */
		new() { name = "dune7", index = 6 }, /* 28 */
		new() { name = "dune16", index = 7 }, /* 29 */
		new() { name = "dune19", index = 4 }, /* 30 */
		new() { name = "dune19", index = 2 }, /* 31 */
		new() { name = "dune19", index = 3 }, /* 32 */
		new() { name = "dune20", index = 2 }, /* 33 */
		new() { name = "dune16", index = 8 }, /* 34 */
		new() { name = "dune0", index = 3 }, /* 35 */
		new() { name = "dune0", index = 4 }, /* 36 */
		new() { name = "dune0", index = 5 }, /* 37 */
	];

    /*
     * Mapping soundID -> voice.
     */
    internal static readonly ushort[] g_table_voiceMapping = [
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
	];

    /*
     * Feedback on events and user commands (English audio, viewport message, and sound).
     * @see g_translatedVoice
     */
    internal static Feedback[] g_feedback = [
        new() { voiceId = [0x002B, 0xFFFF, 0xFFFF, 0xFFFF, 0xFFFF], messageId = 0x33, soundId = 0x003C }, /*  0 */
		new() { voiceId = [0x0031, 0x001D, 0x0024, 0x0037, 0xFFFF], messageId = 0x34, soundId = 0xFFFF }, /*  1 */
		new() { voiceId = [0x0031, 0x001D, 0x0024, 0x0037, 0x0039], messageId = 0x34, soundId = 0xFFFF }, /*  2 */
		new() { voiceId = [0x0031, 0x001D, 0x0024, 0x0037, 0x003A], messageId = 0x34, soundId = 0xFFFF }, /*  3 */
		new() { voiceId = [0x0031, 0x001D, 0x0024, 0x0037, 0x003B], messageId = 0x34, soundId = 0xFFFF }, /*  4 */
		new() { voiceId = [0x0031, 0x001D, 0x0024, 0x0037, 0x003C], messageId = 0x34, soundId = 0xFFFF }, /*  5 */
		new() { voiceId = [0x0031, 0x001E, 0x0024, 0x0037, 0xFFFF], messageId = 0x35, soundId = 0xFFFF }, /*  6 */
		new() { voiceId = [0x0031, 0x001F, 0x0024, 0x0037, 0xFFFF], messageId = 0x36, soundId = 0xFFFF }, /*  7 */
		new() { voiceId = [0x0031, 0x0020, 0x0024, 0x0037, 0xFFFF], messageId = 0x37, soundId = 0xFFFF }, /*  8 */
		new() { voiceId = [0x0031, 0x0021, 0x0024, 0x0037, 0xFFFF], messageId = 0x38, soundId = 0xFFFF }, /*  9 */
		new() { voiceId = [0x0031, 0x0022, 0x0037, 0xFFFF, 0xFFFF], messageId = 0x39, soundId = 0xFFFF }, /* 10 */
		new() { voiceId = [0x0031, 0x0023, 0x0024, 0x0037, 0xFFFF], messageId = 0x3A, soundId = 0xFFFF }, /* 11 */
		new() { voiceId = [0x0031, 0x0032, 0x0037, 0xFFFF, 0xFFFF], messageId = 0x3B, soundId = 0xFFFF }, /* 12 */
		new() { voiceId = [0x001D, 0x0024, 0x0035, 0xFFFF, 0xFFFF], messageId = 0x00, soundId = 0xFFFF }, /* 13 */
		new() { voiceId = [0x001E, 0x0024, 0x0035, 0xFFFF, 0xFFFF], messageId = 0x3C, soundId = 0xFFFF }, /* 14 */
		new() { voiceId = [0x001F, 0x0024, 0x0035, 0xFFFF, 0xFFFF], messageId = 0x3D, soundId = 0xFFFF }, /* 15 */
		new() { voiceId = [0x0020, 0x0024, 0x0035, 0xFFFF, 0xFFFF], messageId = 0x3E, soundId = 0xFFFF }, /* 16 */
		new() { voiceId = [0x0021, 0x0024, 0x0035, 0xFFFF, 0xFFFF], messageId = 0x3F, soundId = 0xFFFF }, /* 17 */
		new() { voiceId = [0x0022, 0x0035, 0xFFFF, 0xFFFF, 0xFFFF], messageId = 0x40, soundId = 0xFFFF }, /* 18 */
		new() { voiceId = [0x0023, 0x0024, 0x0035, 0xFFFF, 0xFFFF], messageId = 0x41, soundId = 0xFFFF }, /* 19 */
		new() { voiceId = [0x0032, 0x0035, 0xFFFF, 0xFFFF, 0xFFFF], messageId = 0x00, soundId = 0xFFFF }, /* 20 */
		new() { voiceId = [0x001D, 0x0025, 0x0035, 0xFFFF, 0xFFFF], messageId = 0x00, soundId = 0xFFFF }, /* 21 */
		new() { voiceId = [0x001E, 0x0025, 0x0035, 0xFFFF, 0xFFFF], messageId = 0x42, soundId = 0xFFFF }, /* 22 */
		new() { voiceId = [0x001F, 0x0025, 0x0035, 0xFFFF, 0xFFFF], messageId = 0x43, soundId = 0xFFFF }, /* 23 */
		new() { voiceId = [0x0020, 0x0025, 0x0035, 0xFFFF, 0xFFFF], messageId = 0x44, soundId = 0xFFFF }, /* 24 */
		new() { voiceId = [0x0021, 0x0025, 0x0035, 0xFFFF, 0xFFFF], messageId = 0x45, soundId = 0xFFFF }, /* 25 */
		new() { voiceId = [0x0022, 0x0035, 0xFFFF, 0xFFFF, 0xFFFF], messageId = 0x46, soundId = 0xFFFF }, /* 26 */
		new() { voiceId = [0x0023, 0x0025, 0x0035, 0xFFFF, 0xFFFF], messageId = 0x47, soundId = 0xFFFF }, /* 27 */
		new() { voiceId = [0x002C, 0x002E, 0xFFFF, 0xFFFF, 0xFFFF], messageId = 0x00, soundId = 0xFFFF }, /* 28 */
		new() { voiceId = [0x002C, 0x002D, 0xFFFF, 0xFFFF, 0xFFFF], messageId = 0x00, soundId = 0xFFFF }, /* 29 */
		new() { voiceId = [0x001E, 0x0024, 0x0036, 0xFFFF, 0xFFFF], messageId = 0x48, soundId = 0xFFFF }, /* 30 */
		new() { voiceId = [0x001F, 0x0024, 0x0036, 0xFFFF, 0xFFFF], messageId = 0x49, soundId = 0xFFFF }, /* 31 */
		new() { voiceId = [0x0020, 0x0024, 0x0036, 0xFFFF, 0xFFFF], messageId = 0x4A, soundId = 0xFFFF }, /* 32 */
		new() { voiceId = [0x0021, 0x0024, 0x0036, 0xFFFF, 0xFFFF], messageId = 0x4B, soundId = 0xFFFF }, /* 33 */
		new() { voiceId = [0x0022, 0x0036, 0xFFFF, 0xFFFF, 0xFFFF], messageId = 0x4C, soundId = 0xFFFF }, /* 34 */
		new() { voiceId = [0x0023, 0x0024, 0x0036, 0xFFFF, 0xFFFF], messageId = 0x4D, soundId = 0xFFFF }, /* 35 */
		new() { voiceId = [0x0034, 0x0038, 0xFFFF, 0xFFFF, 0xFFFF], messageId = 0x00, soundId = 0xFFFF }, /* 36 */
		new() { voiceId = [0x0031, 0x0044, 0xFFFF, 0xFFFF, 0xFFFF], messageId = 0x4E, soundId = 0x0017 }, /* 37 */
		new() { voiceId = [0x002F, 0x0030, 0xFFFF, 0xFFFF, 0xFFFF], messageId = 0x50, soundId = 0xFFFF }, /* 38 */
		new() { voiceId = [0x0031, 0x0033, 0x0037, 0xFFFF, 0xFFFF], messageId = 0x51, soundId = 0xFFFF }, /* 39 */
		new() { voiceId = [0x003D, 0xFFFF, 0xFFFF, 0xFFFF, 0xFFFF], messageId = 0x00, soundId = 0xFFFF }, /* 40 */
		new() { voiceId = [0x003E, 0xFFFF, 0xFFFF, 0xFFFF, 0xFFFF], messageId = 0x00, soundId = 0xFFFF }, /* 41 */
		new() { voiceId = [0x0033, 0x003F, 0xFFFF, 0xFFFF, 0xFFFF], messageId = 0x00, soundId = 0xFFFF }, /* 42 */
		new() { voiceId = [0x0026, 0xFFFF, 0xFFFF, 0xFFFF, 0xFFFF], messageId = 0x00, soundId = 0x002E }, /* 43 */
		new() { voiceId = [0x0027, 0xFFFF, 0xFFFF, 0xFFFF, 0xFFFF], messageId = 0x00, soundId = 0x002E }, /* 44 */
		new() { voiceId = [0x0028, 0xFFFF, 0xFFFF, 0xFFFF, 0xFFFF], messageId = 0x00, soundId = 0x002E }, /* 45 */
		new() { voiceId = [0x0029, 0xFFFF, 0xFFFF, 0xFFFF, 0xFFFF], messageId = 0x00, soundId = 0x002E }, /* 46 */
		new() { voiceId = [0x002A, 0xFFFF, 0xFFFF, 0xFFFF, 0xFFFF], messageId = 0x00, soundId = 0x002E }, /* 47 */
		new() { voiceId = [0x0040, 0xFFFF, 0xFFFF, 0xFFFF, 0xFFFF], messageId = 0x5A, soundId = 0x0017 }, /* 48 */
		new() { voiceId = [0x001E, 0x0024, 0x003F, 0xFFFF, 0xFFFF], messageId = 0x9A, soundId = 0xFFFF }, /* 49 */
		new() { voiceId = [0x001F, 0x0024, 0x003F, 0xFFFF, 0xFFFF], messageId = 0x9B, soundId = 0xFFFF }, /* 50 */
		new() { voiceId = [0x0020, 0x0024, 0x003F, 0xFFFF, 0xFFFF], messageId = 0x9C, soundId = 0xFFFF }, /* 51 */
		new() { voiceId = [0x0021, 0x0024, 0x003F, 0xFFFF, 0xFFFF], messageId = 0x9D, soundId = 0xFFFF }, /* 52 */
		new() { voiceId = [0x0022, 0x0024, 0x003F, 0xFFFF, 0xFFFF], messageId = 0x9E, soundId = 0xFFFF }, /* 53 */
		new() { voiceId = [0x0023, 0x0024, 0x003F, 0xFFFF, 0xFFFF], messageId = 0x9F, soundId = 0xFFFF }, /* 54 */
		new() { voiceId = [0x001E, 0x0041, 0x0042, 0xFFFF, 0xFFFF], messageId = 0xA2, soundId = 0xFFFF }, /* 55 */
		new() { voiceId = [0x001F, 0x0041, 0x0042, 0xFFFF, 0xFFFF], messageId = 0xA3, soundId = 0xFFFF }, /* 56 */
		new() { voiceId = [0x0020, 0x0041, 0x0042, 0xFFFF, 0xFFFF], messageId = 0xA4, soundId = 0xFFFF }, /* 57 */
		new() { voiceId = [0x0021, 0x0041, 0x0042, 0xFFFF, 0xFFFF], messageId = 0xA5, soundId = 0xFFFF }, /* 58 */
		new() { voiceId = [0x0022, 0x0041, 0x0042, 0xFFFF, 0xFFFF], messageId = 0xA6, soundId = 0xFFFF }, /* 59 */
		new() { voiceId = [0x0023, 0x0041, 0x0042, 0xFFFF, 0xFFFF], messageId = 0xA7, soundId = 0xFFFF }, /* 60 */
		new() { voiceId = [0x0046, 0x0047, 0xFFFF, 0xFFFF, 0xFFFF], messageId = 0x00, soundId = 0xFFFF }, /* 61 */
		new() { voiceId = [0x001E, 0xFFFF, 0xFFFF, 0xFFFF, 0xFFFF], messageId = 0x00, soundId = 0xFFFF }, /* 62 */
		new() { voiceId = [0x001F, 0xFFFF, 0xFFFF, 0xFFFF, 0xFFFF], messageId = 0x00, soundId = 0xFFFF }, /* 63 */
		new() { voiceId = [0x0020, 0xFFFF, 0xFFFF, 0xFFFF, 0xFFFF], messageId = 0x00, soundId = 0xFFFF }, /* 64 */
		new() { voiceId = [0x0021, 0xFFFF, 0xFFFF, 0xFFFF, 0xFFFF], messageId = 0x00, soundId = 0xFFFF }, /* 65 */
		new() { voiceId = [0x0022, 0xFFFF, 0xFFFF, 0xFFFF, 0xFFFF], messageId = 0x00, soundId = 0xFFFF }, /* 66 */
		new() { voiceId = [0x0023, 0xFFFF, 0xFFFF, 0xFFFF, 0xFFFF], messageId = 0x00, soundId = 0xFFFF }, /* 67 */
		new() { voiceId = [0x001E, 0x0043, 0x0036, 0xFFFF, 0xFFFF], messageId = 0x93, soundId = 0xFFFF }, /* 68 */
		new() { voiceId = [0x001F, 0x0043, 0x0036, 0xFFFF, 0xFFFF], messageId = 0x94, soundId = 0xFFFF }, /* 69 */
		new() { voiceId = [0x0020, 0x0043, 0x0036, 0xFFFF, 0xFFFF], messageId = 0x95, soundId = 0xFFFF }, /* 70 */
		new() { voiceId = [0x0021, 0x0043, 0x0036, 0xFFFF, 0xFFFF], messageId = 0x96, soundId = 0xFFFF }, /* 71 */
		new() { voiceId = [0x0022, 0x0043, 0x0036, 0xFFFF, 0xFFFF], messageId = 0x97, soundId = 0xFFFF }, /* 72 */
		new() { voiceId = [0x0023, 0x0043, 0x0036, 0xFFFF, 0xFFFF], messageId = 0x98, soundId = 0xFFFF }, /* 73 */
		new() { voiceId = [0xFFFF, 0xFFFF, 0xFFFF, 0xFFFF, 0xFFFF], messageId = 0x01, soundId = 0xFFFF }, /* 74 */
		new() { voiceId = [0xFFFF, 0xFFFF, 0xFFFF, 0xFFFF, 0xFFFF], messageId = 0x00, soundId = 0xFFFF }, /* 75 */
		new() { voiceId = [0xFFFF, 0xFFFF, 0xFFFF, 0xFFFF, 0xFFFF], messageId = 0x01, soundId = 0xFFFF }, /* 76 */
		new() { voiceId = [0x006F, 0x0069, 0xFFFF, 0xFFFF, 0xFFFF], messageId = 0x00, soundId = 0xFFFF }, /* 77 */
		new() { voiceId = [0x0072, 0xFFFF, 0xFFFF, 0xFFFF, 0xFFFF], messageId = 0x00, soundId = 0xFFFF }, /* 78 */
		new() { voiceId = [0x0065, 0x0073, 0x006A, 0xFFFF, 0xFFFF], messageId = 0x00, soundId = 0xFFFF }, /* 79 */
		new() { voiceId = [0x0074, 0x0056, 0x005D, 0xFFFF, 0xFFFF], messageId = 0x00, soundId = 0xFFFF }, /* 80 */
		new() { voiceId = [0x0076, 0x0053, 0x0054, 0xFFFF, 0xFFFF], messageId = 0x00, soundId = 0xFFFF }, /* 81 */
		new() { voiceId = [0x0068, 0x0071, 0x0059, 0xFFFF, 0xFFFF], messageId = 0x00, soundId = 0xFFFF }, /* 82 */
		new() { voiceId = [0x005C, 0x005E, 0x0061, 0x005B, 0xFFFF], messageId = 0x00, soundId = 0xFFFF }, /* 83 */
		new() { voiceId = [0x0062, 0x0060, 0xFFFF, 0xFFFF, 0xFFFF], messageId = 0x00, soundId = 0xFFFF }, /* 84 */
		new() { voiceId = [0x005A, 0x005F, 0xFFFF, 0xFFFF, 0xFFFF], messageId = 0x00, soundId = 0xFFFF }, /* 85 */
		new() { voiceId = [0x0075, 0x004F, 0xFFFF, 0xFFFF, 0xFFFF], messageId = 0x00, soundId = 0xFFFF }, /* 86 */
		new() { voiceId = [0x004E, 0x004D, 0x0055, 0x006D, 0xFFFF], messageId = 0x01, soundId = 0xFFFF }, /* 87 */
		new() { voiceId = [0x006B, 0xFFFF, 0xFFFF, 0xFFFF, 0xFFFF], messageId = 0x00, soundId = 0xFFFF }, /* 88 */
		new() { voiceId = [0x0067, 0x006E, 0xFFFF, 0xFFFF, 0xFFFF], messageId = 0x00, soundId = 0xFFFF }, /* 89 */
		new() { voiceId = [0x0063, 0x0064, 0xFFFF, 0xFFFF, 0xFFFF], messageId = 0x00, soundId = 0xFFFF }, /* 90 */
		new() { voiceId = [0x0066, 0x0070, 0xFFFF, 0xFFFF, 0xFFFF], messageId = 0x00, soundId = 0xFFFF }, /* 91 */
		new() { voiceId = [0x0077, 0x0050, 0x0051, 0xFFFF, 0xFFFF], messageId = 0x01, soundId = 0xFFFF }, /* 92 */
		new() { voiceId = [0x006C, 0xFFFF, 0xFFFF, 0xFFFF, 0xFFFF], messageId = 0x01, soundId = 0xFFFF }  /* 93 */
	];

    /* Translated audio feedback of events and user commands. */
    internal static readonly ushort[][] g_translatedVoice = [ //[][NUM_SPEECH_PARTS]
		[0x002B, 0xFFFF, 0xFFFF, 0xFFFF, 0xFFFF], /*  0 */
		[0x0031, 0x001D, 0xFFFF, 0xFFFF, 0xFFFF], /*  1 */
		[0x0031, 0x001D, 0xFFFF, 0xFFFF, 0xFFFF], /*  2 */
		[0x0031, 0x001D, 0xFFFF, 0xFFFF, 0xFFFF], /*  3 */
		[0x0031, 0x001D, 0xFFFF, 0xFFFF, 0xFFFF], /*  4 */
		[0x0031, 0x001D, 0xFFFF, 0xFFFF, 0xFFFF], /*  5 */
		[0x0031, 0x001D, 0xFFFF, 0xFFFF, 0xFFFF], /*  6 */
		[0x0031, 0x001D, 0xFFFF, 0xFFFF, 0xFFFF], /*  7 */
		[0x0031, 0x001D, 0xFFFF, 0xFFFF, 0xFFFF], /*  8 */
		[0x0031, 0x001D, 0xFFFF, 0xFFFF, 0xFFFF], /*  9 */
		[0x0031, 0x001D, 0xFFFF, 0xFFFF, 0xFFFF], /* 10 */
		[0x0031, 0x001D, 0xFFFF, 0xFFFF, 0xFFFF], /* 11 */
		[0x0031, 0x0032, 0xFFFF, 0xFFFF, 0xFFFF], /* 12 */
		[0x0024, 0xFFFF, 0xFFFF, 0xFFFF, 0xFFFF], /* 13 */
		[0x0037, 0xFFFF, 0xFFFF, 0xFFFF, 0xFFFF], /* 14 */
		[0x0037, 0xFFFF, 0xFFFF, 0xFFFF, 0xFFFF], /* 15 */
		[0x0037, 0xFFFF, 0xFFFF, 0xFFFF, 0xFFFF], /* 16 */
		[0x0037, 0xFFFF, 0xFFFF, 0xFFFF, 0xFFFF], /* 17 */
		[0x0037, 0xFFFF, 0xFFFF, 0xFFFF, 0xFFFF], /* 18 */
		[0x0037, 0xFFFF, 0xFFFF, 0xFFFF, 0xFFFF], /* 19 */
		[0x0035, 0xFFFF, 0xFFFF, 0xFFFF, 0xFFFF], /* 20 */
		[0x0025, 0xFFFF, 0xFFFF, 0xFFFF, 0xFFFF], /* 21 */
		[0x0025, 0xFFFF, 0xFFFF, 0xFFFF, 0xFFFF], /* 22 */
		[0x0025, 0xFFFF, 0xFFFF, 0xFFFF, 0xFFFF], /* 23 */
		[0x0025, 0xFFFF, 0xFFFF, 0xFFFF, 0xFFFF], /* 24 */
		[0x0025, 0xFFFF, 0xFFFF, 0xFFFF, 0xFFFF], /* 25 */
		[0x0025, 0xFFFF, 0xFFFF, 0xFFFF, 0xFFFF], /* 26 */
		[0x0025, 0xFFFF, 0xFFFF, 0xFFFF, 0xFFFF], /* 27 */
		[0x002E, 0xFFFF, 0xFFFF, 0xFFFF, 0xFFFF], /* 28 */
		[0x002D, 0xFFFF, 0xFFFF, 0xFFFF, 0xFFFF], /* 29 */
		[0x0036, 0xFFFF, 0xFFFF, 0xFFFF, 0xFFFF], /* 30 */
		[0x0036, 0xFFFF, 0xFFFF, 0xFFFF, 0xFFFF], /* 31 */
		[0x0036, 0xFFFF, 0xFFFF, 0xFFFF, 0xFFFF], /* 32 */
		[0x0036, 0xFFFF, 0xFFFF, 0xFFFF, 0xFFFF], /* 33 */
		[0x0036, 0xFFFF, 0xFFFF, 0xFFFF, 0xFFFF], /* 34 */
		[0x0036, 0xFFFF, 0xFFFF, 0xFFFF, 0xFFFF], /* 35 */
		[0x0034, 0xFFFF, 0xFFFF, 0xFFFF, 0xFFFF], /* 36 */
		[0x0031, 0x0044, 0xFFFF, 0xFFFF, 0xFFFF], /* 37 */
		[0x002F, 0xFFFF, 0xFFFF, 0xFFFF, 0xFFFF], /* 38 */
		[0x0031, 0x0033, 0xFFFF, 0xFFFF, 0xFFFF], /* 39 */
		[0xFFFF, 0xFFFF, 0xFFFF, 0xFFFF, 0xFFFF], /* 40 */
		[0xFFFF, 0xFFFF, 0xFFFF, 0xFFFF, 0xFFFF], /* 41 */
		[0x003F, 0xFFFF, 0xFFFF, 0xFFFF, 0xFFFF], /* 42 */
		[0x0026, 0xFFFF, 0xFFFF, 0xFFFF, 0xFFFF], /* 43 */
		[0x0027, 0xFFFF, 0xFFFF, 0xFFFF, 0xFFFF], /* 44 */
		[0x0028, 0xFFFF, 0xFFFF, 0xFFFF, 0xFFFF], /* 45 */
		[0x0029, 0xFFFF, 0xFFFF, 0xFFFF, 0xFFFF], /* 46 */
		[0x002A, 0xFFFF, 0xFFFF, 0xFFFF, 0xFFFF], /* 47 */
		[0x0040, 0xFFFF, 0xFFFF, 0xFFFF, 0xFFFF], /* 48 */
		[0x0041, 0xFFFF, 0xFFFF, 0xFFFF, 0xFFFF], /* 49 */
		[0x0041, 0xFFFF, 0xFFFF, 0xFFFF, 0xFFFF], /* 50 */
		[0x0041, 0xFFFF, 0xFFFF, 0xFFFF, 0xFFFF], /* 51 */
		[0x0041, 0xFFFF, 0xFFFF, 0xFFFF, 0xFFFF], /* 52 */
		[0x0041, 0xFFFF, 0xFFFF, 0xFFFF, 0xFFFF], /* 53 */
		[0x0041, 0xFFFF, 0xFFFF, 0xFFFF, 0xFFFF], /* 54 */
		[0x0042, 0xFFFF, 0xFFFF, 0xFFFF, 0xFFFF], /* 55 */
		[0x0042, 0xFFFF, 0xFFFF, 0xFFFF, 0xFFFF], /* 56 */
		[0x0042, 0xFFFF, 0xFFFF, 0xFFFF, 0xFFFF], /* 57 */
		[0x0042, 0xFFFF, 0xFFFF, 0xFFFF, 0xFFFF], /* 58 */
		[0x0042, 0xFFFF, 0xFFFF, 0xFFFF, 0xFFFF], /* 59 */
		[0x0042, 0xFFFF, 0xFFFF, 0xFFFF, 0xFFFF], /* 60 */
		[0x0046, 0xFFFF, 0xFFFF, 0xFFFF, 0xFFFF], /* 61 */
		[0x001E, 0xFFFF, 0xFFFF, 0xFFFF, 0xFFFF], /* 62 */
		[0x001F, 0xFFFF, 0xFFFF, 0xFFFF, 0xFFFF], /* 63 */
		[0x0020, 0xFFFF, 0xFFFF, 0xFFFF, 0xFFFF], /* 64 */
		[0xFFFF, 0xFFFF, 0xFFFF, 0xFFFF, 0xFFFF], /* 65 */
		[0xFFFF, 0xFFFF, 0xFFFF, 0xFFFF, 0xFFFF], /* 66 */
		[0xFFFF, 0xFFFF, 0xFFFF, 0xFFFF, 0xFFFF], /* 67 */
		[0x0043, 0xFFFF, 0xFFFF, 0xFFFF, 0xFFFF], /* 68 */
		[0x0043, 0xFFFF, 0xFFFF, 0xFFFF, 0xFFFF], /* 69 */
		[0x0043, 0xFFFF, 0xFFFF, 0xFFFF, 0xFFFF], /* 70 */
		[0x0043, 0xFFFF, 0xFFFF, 0xFFFF, 0xFFFF], /* 71 */
		[0x0043, 0xFFFF, 0xFFFF, 0xFFFF, 0xFFFF], /* 72 */
		[0x0043, 0xFFFF, 0xFFFF, 0xFFFF, 0xFFFF], /* 73 */
		[0xFFFF, 0xFFFF, 0xFFFF, 0xFFFF, 0xFFFF], /* 74 */
		[0x0057, 0xFFFF, 0xFFFF, 0xFFFF, 0xFFFF], /* 75 */
		[0x0052, 0x0058, 0xFFFF, 0xFFFF, 0xFFFF], /* 76 */
		[0x006F, 0x0069, 0xFFFF, 0xFFFF, 0xFFFF], /* 77 */
		[0x0072, 0xFFFF, 0xFFFF, 0xFFFF, 0xFFFF], /* 78 */
		[0x0065, 0x0073, 0x006A, 0xFFFF, 0xFFFF], /* 79 */
		[0x0074, 0x0056, 0x005D, 0xFFFF, 0xFFFF], /* 80 */
		[0x0076, 0x0053, 0x0054, 0xFFFF, 0xFFFF], /* 81 */
		[0x0068, 0x0071, 0x0059, 0xFFFF, 0xFFFF], /* 82 */
		[0x005C, 0x005E, 0x0061, 0x005B, 0xFFFF], /* 83 */
		[0x0062, 0x0060, 0xFFFF, 0xFFFF, 0xFFFF], /* 84 */
		[0x005A, 0x005F, 0xFFFF, 0xFFFF, 0xFFFF], /* 85 */
		[0x0075, 0x004F, 0xFFFF, 0xFFFF, 0xFFFF], /* 86 */
		[0x004E, 0x004D, 0x0055, 0x006D, 0xFFFF], /* 87 */
		[0x006B, 0xFFFF, 0xFFFF, 0xFFFF, 0xFFFF], /* 88 */
		[0x0067, 0x006E, 0xFFFF, 0xFFFF, 0xFFFF], /* 89 */
		[0x0063, 0x0064, 0xFFFF, 0xFFFF, 0xFFFF], /* 90 */
		[0x0066, 0x0070, 0xFFFF, 0xFFFF, 0xFFFF], /* 91 */
		[0x0077, 0x0050, 0x0051, 0xFFFF, 0xFFFF], /* 92 */
		[0x006C, 0xFFFF, 0xFFFF, 0xFFFF, 0xFFFF]  /* 93 */
	];
}
