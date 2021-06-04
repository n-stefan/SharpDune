
using System.Collections.Generic;

namespace SharpDune
{
	enum FourCC
	{
		BLDG,
		CAT,
		DATA,
		EVNT,
		FORM,
		INFO,
		MAP,
		NAME,
		ORDR,
		PLYR,
		RPAL,
		RTBL,
		SCEN,
		SINF,
		SSET,
		TEAM,
		TEXT,
		UNIT,
		XMID,
		ODUN
	}

	class MultiChar
	{
		private static readonly Dictionary<FourCC, int> _dict = new()
        {
			{ FourCC.BLDG, Calc('B', 'L', 'D', 'G') },
			{ FourCC.CAT,  Calc('C', 'A', 'T', ' ') },
			{ FourCC.DATA, Calc('D', 'A', 'T', 'A') },
			{ FourCC.EVNT, Calc('E', 'V', 'N', 'T') },
			{ FourCC.FORM, Calc('F', 'O', 'R', 'M') },
			{ FourCC.INFO, Calc('I', 'N', 'F', 'O') },
			{ FourCC.MAP,  Calc('M', 'A', 'P', ' ') },
			{ FourCC.NAME, Calc('N', 'A', 'M', 'E') },
			{ FourCC.ORDR, Calc('O', 'R', 'D', 'R') },
			{ FourCC.PLYR, Calc('P', 'L', 'Y', 'R') },
			{ FourCC.RPAL, Calc('R', 'P', 'A', 'L') },
			{ FourCC.RTBL, Calc('R', 'T', 'B', 'L') },
			{ FourCC.SCEN, Calc('S', 'C', 'E', 'N') },
			{ FourCC.SINF, Calc('S', 'I', 'N', 'F') },
			{ FourCC.SSET, Calc('S', 'S', 'E', 'T') },
			{ FourCC.TEAM, Calc('T', 'E', 'A', 'M') },
			{ FourCC.TEXT, Calc('T', 'E', 'X', 'T') },
			{ FourCC.UNIT, Calc('U', 'N', 'I', 'T') },
			{ FourCC.XMID, Calc('X', 'M', 'I', 'D') },

			/* OpenDUNE extensions. */
			{ FourCC.ODUN, Calc('O', 'D', 'U', 'N') }  /* OpenDUNE Unit New. */
		};

		private static int Calc(char a, char b, char c, char d) =>
			(a << 24) | (b << 16) | (c << 8) | d;

		internal int this[FourCC code] =>
			_dict[code];
	}
}
