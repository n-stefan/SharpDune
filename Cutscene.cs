/* Introduction movie and cutscenes */

using System;
using System.Diagnostics;
using static System.Math;

namespace SharpDune
{
	/* Direction of change in the #GameLoop_PalettePart_Update function. */
	enum PalettePartDirection
	{
		PPD_STOPPED,        /*!< Not changing. */
		PPD_TO_NEW_PALETTE, /*!< Modifying towards #s_palettePartTarget */
		PPD_TO_BLACK        /*!< Modifying towards all black. */
	}

	struct CreditString
	{
		internal ushort x;
		internal short y;
		internal string text;
		internal byte separator;
		internal byte charHeight;
		internal byte type;
	}

	class CreditPosition
	{
		internal ushort x;
		internal ushort y;
	}

	class Cutscene
	{
		internal static bool g_canSkipIntro; /*!< When true, you can skip the intro by pressing a key or clicking. */

        static HouseAnimation_Subtitle[] s_houseAnimation_subtitle;       /*!< Subtitle part of animation data. */
		static HouseAnimation_SoundEffect[] s_houseAnimation_soundEffect; /*!< Soundeffect part of animation data. */
		static ushort s_feedback_base_index = 0xFFFF;                     /*!< base index in g_feedback - used in Intro animation.*/
		static ushort s_subtitleIndex = 0xFFFF;                           /*!< Unknown animation data. */
		static ushort s_subtitleWait = 0xFFFF;                            /*!< Unknown animation data. */
		static ushort s_houseAnimation_currentSubtitle;               /*!< Current subtitle (index) part of animation. */
        static ushort s_houseAnimation_currentSoundEffect;            /* Current voice (index) part of animation. */
        static bool s_subtitleActive;                             /* Unknown animation data. */

        static PalettePartDirection s_palettePartDirection; /*!< Direction of change. @see PalettePartDirection */
		static uint s_paletteAnimationTimeout;              /*!< Timeout value for the next palette change. */
		static ushort s_palettePartCount;                   /*!< Number of steps left before the target palette is reached. */
		static readonly byte[] s_palettePartTarget = new byte[18];   /*!< Target palette part (6 colours). */
		static readonly byte[] s_palettePartCurrent = new byte[18];  /*!< Current value of the palette part (6 colours, updated each call to #GameLoop_PalettePart_Update). */
		static readonly byte[] s_palettePartChange = new byte[18];   /*!< Amount of change of each RGB colour of the palette part with each step. */

		/*
		 * The Intro.
		 */
		internal static void GameLoop_GameIntroAnimation()
		{
			Gui.GUI_ChangeSelectionType((ushort)SelectionType.SELECTIONTYPE_INTRO);

			GameLoop_Logos();

			if (Input.Input_Keyboard_NextKey() == 0 || !g_canSkipIntro)
			{
				var animation = HouseAnimation.g_table_houseAnimation_animation[(int)HouseAnimationType.HOUSEANIMATION_INTRO];
				var subtitle = HouseAnimation.g_table_houseAnimation_subtitle[(int)HouseAnimationType.HOUSEANIMATION_INTRO];
				var soundEffect = HouseAnimation.g_table_houseAnimation_soundEffect[(int)HouseAnimationType.HOUSEANIMATION_INTRO];

				Sound.Music_Play(0x1B);

				/* 0x4A = 74 = Intro feedback base index */
				GameLoop_PrepareAnimation(subtitle, 0x4A, soundEffect);

				GameLoop_PlayAnimation(animation);

				CDriver.Driver_Music_FadeOut();

				GameLoop_FinishAnimation();
			}

			Gui.GUI_ChangeSelectionType((ushort)SelectionType.SELECTIONTYPE_MENTAT);
		}

		/*
		 * Shows the end game "movie"
		 */
		internal static void GameLoop_GameEndAnimation()
		{
			HouseAnimation_Animation[] animation;
			HouseAnimation_Subtitle[] subtitle;
			HouseAnimation_SoundEffect[] soundEffect;
			ushort sound;

			Sound.Voice_LoadVoices(0xFFFE);

			switch (CHouse.g_playerHouseID)
			{
				case HouseType.HOUSE_HARKONNEN:
					animation = HouseAnimation.g_table_houseAnimation_animation[(int)HouseAnimationType.HOUSEANIMATION_LEVEL9_HARKONNEN];
					subtitle = HouseAnimation.g_table_houseAnimation_subtitle[(int)HouseAnimationType.HOUSEANIMATION_LEVEL9_HARKONNEN];
					soundEffect = HouseAnimation.g_table_houseAnimation_soundEffect[(int)HouseAnimationType.HOUSEANIMATION_LEVEL9_HARKONNEN];
					sound = 0x1E;
					break;

				default:
				case HouseType.HOUSE_ATREIDES:
					animation = HouseAnimation.g_table_houseAnimation_animation[(int)HouseAnimationType.HOUSEANIMATION_LEVEL9_ARTREIDES];
					subtitle = HouseAnimation.g_table_houseAnimation_subtitle[(int)HouseAnimationType.HOUSEANIMATION_LEVEL9_ARTREIDES];
					soundEffect = HouseAnimation.g_table_houseAnimation_soundEffect[(int)HouseAnimationType.HOUSEANIMATION_LEVEL9_ARTREIDES];
					sound = 0x1F;
					break;

				case HouseType.HOUSE_ORDOS:
					animation = HouseAnimation.g_table_houseAnimation_animation[(int)HouseAnimationType.HOUSEANIMATION_LEVEL9_ORDOS];
					subtitle = HouseAnimation.g_table_houseAnimation_subtitle[(int)HouseAnimationType.HOUSEANIMATION_LEVEL9_ORDOS];
					soundEffect = HouseAnimation.g_table_houseAnimation_soundEffect[(int)HouseAnimationType.HOUSEANIMATION_LEVEL9_ORDOS];
					sound = 0x20;
					break;
			}

			GameLoop_PrepareAnimation(subtitle, 0xFFFF, soundEffect);

			Sound.Music_Play(sound);

			GameLoop_PlayAnimation(animation);

			CDriver.Driver_Music_FadeOut();

			GameLoop_FinishAnimation();

			GameLoop_GameCredits();
		}

		internal static void GameLoop_LevelEndAnimation()
		{
			HouseAnimation_Animation[] animation;
			HouseAnimation_Subtitle[] subtitle;
			HouseAnimation_SoundEffect[] soundEffect;

			Input.Input_History_Clear();

			switch (CSharpDune.g_campaignID)
			{
				case 4:
					switch (CHouse.g_playerHouseID)
					{
						case HouseType.HOUSE_HARKONNEN:
							animation = HouseAnimation.g_table_houseAnimation_animation[(int)HouseAnimationType.HOUSEANIMATION_LEVEL4_HARKONNEN];
							subtitle = HouseAnimation.g_table_houseAnimation_subtitle[(int)HouseAnimationType.HOUSEANIMATION_LEVEL4_HARKONNEN];
							soundEffect = HouseAnimation.g_table_houseAnimation_soundEffect[(int)HouseAnimationType.HOUSEANIMATION_LEVEL4_HARKONNEN];
							break;

						case HouseType.HOUSE_ATREIDES:
							animation = HouseAnimation.g_table_houseAnimation_animation[(int)HouseAnimationType.HOUSEANIMATION_LEVEL4_ARTREIDES];
							subtitle = HouseAnimation.g_table_houseAnimation_subtitle[(int)HouseAnimationType.HOUSEANIMATION_LEVEL4_ARTREIDES];
							soundEffect = HouseAnimation.g_table_houseAnimation_soundEffect[(int)HouseAnimationType.HOUSEANIMATION_LEVEL4_ARTREIDES];
							break;

						case HouseType.HOUSE_ORDOS:
							animation = HouseAnimation.g_table_houseAnimation_animation[(int)HouseAnimationType.HOUSEANIMATION_LEVEL4_ORDOS];
							subtitle = HouseAnimation.g_table_houseAnimation_subtitle[(int)HouseAnimationType.HOUSEANIMATION_LEVEL4_ORDOS];
							soundEffect = HouseAnimation.g_table_houseAnimation_soundEffect[(int)HouseAnimationType.HOUSEANIMATION_LEVEL4_ORDOS];
							break;

						default: return;
					}
					break;

				case 8:
					switch (CHouse.g_playerHouseID)
					{
						case HouseType.HOUSE_HARKONNEN:
							animation = HouseAnimation.g_table_houseAnimation_animation[(int)HouseAnimationType.HOUSEANIMATION_LEVEL8_HARKONNEN];
							subtitle = HouseAnimation.g_table_houseAnimation_subtitle[(int)HouseAnimationType.HOUSEANIMATION_LEVEL8_HARKONNEN];
							soundEffect = HouseAnimation.g_table_houseAnimation_soundEffect[(int)HouseAnimationType.HOUSEANIMATION_LEVEL8_HARKONNEN];
							break;

						case HouseType.HOUSE_ATREIDES:
							animation = HouseAnimation.g_table_houseAnimation_animation[(int)HouseAnimationType.HOUSEANIMATION_LEVEL8_ARTREIDES];
							subtitle = HouseAnimation.g_table_houseAnimation_subtitle[(int)HouseAnimationType.HOUSEANIMATION_LEVEL8_ARTREIDES];
							soundEffect = HouseAnimation.g_table_houseAnimation_soundEffect[(int)HouseAnimationType.HOUSEANIMATION_LEVEL8_ARTREIDES];
							break;

						case HouseType.HOUSE_ORDOS:
							animation = HouseAnimation.g_table_houseAnimation_animation[(int)HouseAnimationType.HOUSEANIMATION_LEVEL8_ORDOS];
							subtitle = HouseAnimation.g_table_houseAnimation_subtitle[(int)HouseAnimationType.HOUSEANIMATION_LEVEL8_ORDOS];
							soundEffect = HouseAnimation.g_table_houseAnimation_soundEffect[(int)HouseAnimationType.HOUSEANIMATION_LEVEL8_ORDOS];
							break;

						default: return;
					}
					break;

				default: return;
			}

			GameLoop_PrepareAnimation(subtitle, 0xFFFF, soundEffect);

			Sound.Music_Play(0x22);

			GameLoop_PlayAnimation(animation);

			CDriver.Driver_Music_FadeOut();

			GameLoop_FinishAnimation();
		}

		static void GameLoop_FinishAnimation()
		{
			Gui.GUI_DrawText_Wrapper(null, 0, 0, 0, 0, 0x1);
			Gui.GUI_DrawText_Wrapper(null, 0, 0, 0, 0, 0x2);

			Gui.GUI_SetPaletteAnimated(Gfx.g_palette2, 60);

			Gui.GUI_ClearScreen(Screen.SCREEN_0);

			Input.Input_History_Clear();

			Gfx.GFX_ClearBlock(Screen.SCREEN_3);
		}

		static void GameLoop_PrepareAnimation(HouseAnimation_Subtitle[] subtitle, ushort feedback_base_index, HouseAnimation_SoundEffect[] soundEffect)
		{
			byte i;
			var colors = new byte[16];

			s_houseAnimation_subtitle = subtitle;
			s_houseAnimation_soundEffect = soundEffect;

			s_houseAnimation_currentSubtitle = 0;
			s_houseAnimation_currentSoundEffect = 0;

			CFont.g_fontCharOffset = 0;

			s_feedback_base_index = feedback_base_index;
			s_subtitleIndex = 0;
			s_subtitleWait = 0xFFFF;
			s_subtitleActive = false;

			s_palettePartDirection = PalettePartDirection.PPD_STOPPED;
			s_palettePartCount = 0;
			s_paletteAnimationTimeout = 0;

			Gfx.GFX_ClearScreen(Screen.SCREEN_ACTIVE);

			CFile.File_ReadBlockFile("INTRO.PAL", Gfx.g_palette1, 256 * 3);

			Buffer.BlockCopy(Gfx.g_palette1, 0, Gui.g_palette_998A, 0, 256 * 3); //memcpy(g_palette_998A, g_palette1, 256 * 3);

			CFont.Font_Select(CFont.g_fontIntro);

			Gfx.GFX_Screen_SetActive(Screen.SCREEN_0);

			Buffer.BlockCopy(Gfx.g_palette1, (144 + s_houseAnimation_subtitle[0].colour * 16) * 3, s_palettePartTarget, 0, 6 * 3); //memcpy(s_palettePartTarget, &g_palette1[(144 + s_houseAnimation_subtitle->colour * 16) * 3], 6 * 3);

			Array.Fill<byte>(Gfx.g_palette1, 0, 215 * 3, 6 * 3); //memset(&g_palette1[215 * 3], 0, 6 * 3);

			Buffer.BlockCopy(s_palettePartTarget, 0, s_palettePartCurrent, 0, 6 * 3); //memcpy(s_palettePartCurrent, s_palettePartTarget, 6 * 3);

			Array.Fill<byte>(s_palettePartChange, 0, 0, 6 * 3); //memset(s_palettePartChange, 0, 6 * 3);

			colors[0] = 0;
			for (i = 0; i < 6; i++) colors[i + 1] = (byte)(215 + i);

			Gui.GUI_InitColors(colors, 0, 15);
		}

		static void GameLoop_PlayAnimation(HouseAnimation_Animation[] animation)
		{
			byte animationStep = 0;
			var pointer = 0;

			while (animation[pointer].duration != 0)
			{
				ushort frameCount;
				ushort posX;
				ushort posY;
				var timeout = (uint)(Timer.g_timerGUI + animation[pointer].duration * 6);
				var timeout2 = timeout + 30;   /* timeout + 0.5 s */
				uint timeLeftForFrame;
				uint timeLeft;
				var mode = (ushort)(animation[pointer].flags & 0x3);
				ushort addFrameCount;   /* additional frame count */
				ushort frame;
				/*WSAObject*/
				(WSAHeader, CArray<byte>) wsa;

				if ((animation[pointer].flags & CHouse.HOUSEANIM_FLAGS_POS0_0) != 0)
				{
					posX = 0;
					posY = 0;
				}
				else
				{
					posX = 8;
					posY = 24;
				}

				s_subtitleIndex = 0;

				Debug.WriteLine($"DEBUG: GameLoop_PlayAnimation() {animationStep} {animation[pointer].str} mode={mode} flags={animation[pointer].flags & ~3:X3}");
				if (mode == 0)
				{
					wsa = (null, null);
					frame = 0;
				}
				else
				{
					string filenameBuffer; //char[16]
					byte[] wsaData;
					uint wsaSize;
					bool wsaReservedDisplayFrame;

					if (mode == 3)
					{
						frame = animation[pointer].frameCount;
						wsaReservedDisplayFrame = true;
					}
					else
					{
						frame = 0;
						wsaReservedDisplayFrame = (animation[pointer].flags & CHouse.HOUSEANIM_FLAGS_DISPLAYFRAME) != 0;
					}

					if ((animation[pointer].flags & (CHouse.HOUSEANIM_FLAGS_FADEIN2 | CHouse.HOUSEANIM_FLAGS_FADEIN)) != 0)
					{
						Gui.GUI_ClearScreen(Screen.SCREEN_1);

						//wsa = new WSAObject();
						wsaData = Gfx.GFX_Screen_Get_ByIndex(Screen.SCREEN_2);

						wsaSize = (uint)(Gfx.GFX_Screen_GetSize_ByIndex(Screen.SCREEN_2) + Gfx.GFX_Screen_GetSize_ByIndex(Screen.SCREEN_3));
						wsaReservedDisplayFrame = false;
					}
					else
					{
						//wsa = new WSAObject();
						wsaData = Gfx.GFX_Screen_Get_ByIndex(Screen.SCREEN_1);

						wsaSize = (uint)(Gfx.GFX_Screen_GetSize_ByIndex(Screen.SCREEN_1) + Gfx.GFX_Screen_GetSize_ByIndex(Screen.SCREEN_2) + Gfx.GFX_Screen_GetSize_ByIndex(Screen.SCREEN_3));
					}

					filenameBuffer = $"{animation[pointer].str}.WSA"; //snprintf(filenameBuffer, sizeof(filenameBuffer), "%.8s.WSA", animation.string);
					wsa = Wsa.WSA_LoadFile(filenameBuffer, wsaData, wsaSize, wsaReservedDisplayFrame);
				}

				addFrameCount = 0;
				if ((animation[pointer].flags & CHouse.HOUSEANIM_FLAGS_FADEOUTTEXT) != 0)
				{
					timeout -= 45;
					addFrameCount++;
				}
				else if ((animation[pointer].flags & CHouse.HOUSEANIM_FLAGS_FADETOWHITE) != 0)
				{
					timeout -= 15;
					addFrameCount++;
				}

				if ((animation[pointer].flags & CHouse.HOUSEANIM_FLAGS_FADEINTEXT) != 0)
				{
					GameLoop_PlaySubtitle(animationStep);
					Wsa.WSA_DisplayFrame(wsa, frame++, posX, posY, Screen.SCREEN_0);
					GameLoop_PalettePart_Update(true);

					Buffer.BlockCopy(s_palettePartCurrent, 0, Gfx.g_palette1, 215 * 3, 18); //memcpy(&g_palette1[215 * 3], s_palettePartCurrent, 18);

					Gui.GUI_SetPaletteAnimated(Gfx.g_palette1, 45);

					addFrameCount++;
				}
				else if ((animation[pointer].flags & (CHouse.HOUSEANIM_FLAGS_FADEIN2 | CHouse.HOUSEANIM_FLAGS_FADEIN)) != 0)
				{
					GameLoop_PlaySubtitle(animationStep);
					Wsa.WSA_DisplayFrame(wsa, frame++, posX, posY, Screen.SCREEN_1);
					addFrameCount++;

					if ((animation[pointer].flags & (CHouse.HOUSEANIM_FLAGS_FADEIN2 | CHouse.HOUSEANIM_FLAGS_FADEIN)) == CHouse.HOUSEANIM_FLAGS_FADEIN2)
					{
						Gui.GUI_Screen_FadeIn2(8, 24, 304, 120, Screen.SCREEN_1, Screen.SCREEN_0, 1, false);
					}
					else if ((animation[pointer].flags & (CHouse.HOUSEANIM_FLAGS_FADEIN2 | CHouse.HOUSEANIM_FLAGS_FADEIN)) == CHouse.HOUSEANIM_FLAGS_FADEIN)
					{
						Gui.GUI_Screen_FadeIn(1, 24, 1, 24, 38, 120, Screen.SCREEN_1, Screen.SCREEN_0);
					}
				}

				timeLeft = timeout - Timer.g_timerGUI;
				timeLeftForFrame = 0;
				frameCount = 1;

				switch (mode)
				{
					case 0:
						frameCount = (ushort)(animation[pointer].frameCount - addFrameCount);
						timeLeftForFrame = timeLeft / frameCount;
						break;

					case 1:
						frameCount = Wsa.WSA_GetFrameCount(wsa);
						timeLeftForFrame = timeLeft / animation[pointer].frameCount;
						break;

					case 2:
						frameCount = (ushort)(Wsa.WSA_GetFrameCount(wsa) - addFrameCount);
						timeLeftForFrame = timeLeft / frameCount;
						timeout -= timeLeftForFrame;
						break;

					case 3:
						frame = animation[pointer].frameCount;
						frameCount = 1;
						timeLeftForFrame = timeLeft / 20;
						break;

					default:
						CSharpDune.PrepareEnd();
						Trace.WriteLine($"ERROR: Bad mode in animation #{animationStep}.");
						Environment.Exit(0);
						break;
				}

				while (timeout > Timer.g_timerGUI)
				{
					Timer.g_timerTimeout = timeLeftForFrame;

					GameLoop_PlaySubtitle(animationStep);
					Wsa.WSA_DisplayFrame(wsa, frame++, posX, posY, Screen.SCREEN_0);

					if (mode == 1 && frame == frameCount)
					{
						frame = 0;
					}
					else if (mode == 3)
					{
						frame--;
					}

					if (Input.Input_Keyboard_NextKey() != 0 && g_canSkipIntro)
					{
						Wsa.WSA_Unload(wsa);
						return;
					}

					do
					{
						GameLoop_PalettePart_Update(false);
						Sleep.sleepIdle();
					} while (Timer.g_timerTimeout != 0 && timeout > Timer.g_timerGUI);
				}

				if (mode == 2)
				{
					bool displayed;
					do
					{
						GameLoop_PlaySubtitle(animationStep);
						displayed = Wsa.WSA_DisplayFrame(wsa, frame++, posX, posY, Screen.SCREEN_0);
						Sleep.sleepIdle();
					} while (displayed);
				}

				if ((animation[pointer].flags & CHouse.HOUSEANIM_FLAGS_FADETOWHITE) != 0)
				{
					Array.Fill<byte>(Gui.g_palette_998A, 63, 3 * 1, 255 * 3); //memset(&g_palette_998A[3 * 1], 63, 255 * 3);

					Buffer.BlockCopy(s_palettePartCurrent, 0, Gui.g_palette_998A, 215 * 3, 18); //memcpy(&g_palette_998A[215 * 3], s_palettePartCurrent, 18);

					Gui.GUI_SetPaletteAnimated(Gui.g_palette_998A, 15);

					Buffer.BlockCopy(Gfx.g_palette1, 0, Gui.g_palette_998A, 0, 256 * 3); //memcpy(g_palette_998A, g_palette1, 256 * 3);
				}

				if ((animation[pointer].flags & CHouse.HOUSEANIM_FLAGS_FADEOUTTEXT) != 0)
				{
					GameLoop_PalettePart_Update(true);

					Buffer.BlockCopy(s_palettePartCurrent, 0, Gui.g_palette_998A, 215 * 3, 18); //memcpy(&g_palette_998A[215 * 3], s_palettePartCurrent, 18);

					Gui.GUI_SetPaletteAnimated(Gui.g_palette_998A, 45);
				}

				Wsa.WSA_Unload(wsa);

				animationStep++;
				pointer++;

				while (timeout2 > Timer.g_timerGUI) Sleep.sleepIdle();
			}
		}

		/*
		 * Update part of the palette one step.
		 * @param finishNow Finish all steps now.
		 * @return Direction of change for the next call.
		 * @note If \a finishNow, the new palette is not written to the screen.
		 * @see PalettePartDirection
		 */
		static ushort GameLoop_PalettePart_Update(bool finishNow)
		{
			Sound.Sound_StartSpeech();

			if (s_palettePartDirection == PalettePartDirection.PPD_STOPPED) return 0;

			if (s_paletteAnimationTimeout >= Timer.g_timerGUI && !finishNow) return (ushort)s_palettePartDirection;

			s_paletteAnimationTimeout = Timer.g_timerGUI + 7;
			if (--s_palettePartCount == 0 || finishNow)
			{
				if (s_palettePartDirection == PalettePartDirection.PPD_TO_NEW_PALETTE)
				{
					Buffer.BlockCopy(s_palettePartTarget, 0, s_palettePartCurrent, 0, 18); //memcpy(s_palettePartCurrent, s_palettePartTarget, 18);
				}
				else
				{
					Array.Fill<byte>(s_palettePartCurrent, 0, 0, 18); //memset(s_palettePartCurrent, 0, 18);
				}

				s_palettePartDirection = PalettePartDirection.PPD_STOPPED;
			}
			else
			{
				byte i;

				for (i = 0; i < 18; i++)
				{
					if (s_palettePartDirection == PalettePartDirection.PPD_TO_NEW_PALETTE)
					{
						s_palettePartCurrent[i] = (byte)Min(s_palettePartCurrent[i] + s_palettePartChange[i], s_palettePartTarget[i]);
					}
					else
					{
						s_palettePartCurrent[i] = (byte)Max(s_palettePartCurrent[i] - s_palettePartChange[i], 0);
					}
				}
			}

			if (finishNow) return (ushort)s_palettePartDirection;

			Buffer.BlockCopy(s_palettePartCurrent, 0, Gui.g_palette_998A, 215 * 3, 18); //memcpy(&g_palette_998A[215 * 3], s_palettePartCurrent, 18);

			Gfx.GFX_SetPalette(Gui.g_palette_998A);

			return (ushort)s_palettePartDirection;
		}

		static void GameLoop_PlaySubtitle(byte animation)
		{
			HouseAnimation_Subtitle subtitle;
			byte i;
			var colors = new byte[16];

			s_subtitleIndex++;

			GameLoop_PlaySoundEffect(animation);

			subtitle = s_houseAnimation_subtitle[s_houseAnimation_currentSubtitle];

			if (subtitle.stringID == 0xFFFF || subtitle.animationID > animation) return;

			if (s_subtitleActive)
			{
				if (s_subtitleWait == 0xFFFF) s_subtitleWait = subtitle.waitFadeout;
				if (s_subtitleWait-- != 0) return;

				s_subtitleActive = false;
				s_houseAnimation_currentSubtitle++;
				s_palettePartDirection = PalettePartDirection.PPD_TO_BLACK;

				if (subtitle.paletteFadeout != 0)
				{
					s_palettePartCount = subtitle.paletteFadeout;

					for (i = 0; i < 18; i++)
					{
						s_palettePartChange[i] = (byte)(s_palettePartTarget[i] / s_palettePartCount);
						if (s_palettePartChange[i] == 0) s_palettePartChange[i] = 1;
					}

					return;
				}

				Buffer.BlockCopy(s_palettePartTarget, 0, s_palettePartChange, 0, 18); //memcpy(s_palettePartChange, s_palettePartTarget, 18);
				s_palettePartCount = 1;
				return;
			}

			if (s_subtitleWait == 0xFFFF) s_subtitleWait = subtitle.waitFadein;
			if (s_subtitleWait-- != 0) return;

			Buffer.BlockCopy(Gfx.g_palette1, (144 + (subtitle.colour * 16)) * 3, s_palettePartTarget, 0, 18); //memcpy(s_palettePartTarget, &g_palette1[(144 + (subtitle.colour * 16)) * 3], 18);

			s_subtitleActive = true;

			Gui.GUI_DrawFilledRectangle(0, (short)(subtitle.top == 85 ? 0 : subtitle.top), Gfx.SCREEN_WIDTH - 1, Gfx.SCREEN_HEIGHT - 1, 0);

			if (Config.g_enableVoices && s_feedback_base_index != 0xFFFF && s_houseAnimation_currentSubtitle != 0 && Config.g_config.language == (byte)Language.ENGLISH)
			{
				/* specific code for Intro
				 * @see GameLoop_GameIntroAnimation() */
				var feedback_index = (ushort)(s_feedback_base_index + s_houseAnimation_currentSubtitle);

				Sound.Sound_Output_Feedback(feedback_index);

				if (Sound.g_feedback[feedback_index].messageId != 0)
				{
					/* force drawing of subtitle */
					GameLoop_DrawText(CString.String_Get_ByIndex(subtitle.stringID), subtitle.top);
				}
			}
			else
			{
				if (subtitle.stringID != (ushort)Text.STR_NULL)
				{
					GameLoop_DrawText(CString.String_Get_ByIndex(subtitle.stringID), subtitle.top);
				}
			}

			s_palettePartDirection = PalettePartDirection.PPD_TO_NEW_PALETTE;

			if (subtitle.paletteFadein != 0)
			{
				s_palettePartCount = subtitle.paletteFadein;

				for (i = 0; i < 18; i++)
				{
					s_palettePartChange[i] = (byte)(s_palettePartTarget[i] / s_palettePartCount);
					if (s_palettePartChange[i] == 0) s_palettePartChange[i] = 1;
				}
			}
			else
			{
				Buffer.BlockCopy(s_palettePartTarget, 0, s_palettePartChange, 0, 18); //memcpy(s_palettePartChange, s_palettePartTarget, 18);
				s_palettePartCount = 1;
			}

			if (CHouse.g_playerHouseID != HouseType.HOUSE_INVALID || s_houseAnimation_currentSubtitle != 2) return;

			Gui.GUI_DrawText_Wrapper(null, 0, 0, 0, 0, 0x21);

			Gui.GUI_DrawText_Wrapper("Copyright (c) 1992 Westwood Studios, Inc.", 160, 189, 215, 0, 0x112);

			CFont.g_fontCharOffset = 0;

			colors[0] = 0;
			for (i = 0; i < 6; i++) colors[i + 1] = (byte)(215 + i);

			Gui.GUI_InitColors(colors, 0, 15);

			CFont.Font_Select(CFont.g_fontIntro);
		}

		static void GameLoop_DrawText(string str, ushort top)
		{
			var lines = str.Split('\r');

			Gui.GUI_DrawText_Wrapper(lines[0], 160, (short)top, 215, 0, 0x100);

			if (lines.Length == 1) return;

			Gui.GUI_DrawText_Wrapper(lines[1], 160, (short)(top + 18), 215, 0, 0x100);
		}

		static void GameLoop_PlaySoundEffect(byte animation)
		{
			var soundEffect = s_houseAnimation_soundEffect[s_houseAnimation_currentSoundEffect];

			if (soundEffect.animationID > animation || soundEffect.wait > s_subtitleIndex) return;

			Sound.Voice_Play(soundEffect.voiceID);

			s_houseAnimation_currentSoundEffect++;
		}

		/*
		 * Logos at begin of intro.
		 */
		static void GameLoop_Logos()
		{
			Screen oldScreenID;
			/* void* *//*WSAObject*/
			(WSAHeader, CArray<byte>) wsa;
			ushort frame;

			oldScreenID = Gfx.GFX_Screen_SetActive(Screen.SCREEN_0);

			Gfx.GFX_SetPalette(Gfx.g_palette2);
			Gfx.GFX_ClearScreen(Screen.SCREEN_0);

			CFile.File_ReadBlockFile("WESTWOOD.PAL", Gui.g_palette_998A, 256 * 3);

			frame = 0;
			wsa = Wsa.WSA_LoadFile("WESTWOOD.WSA", Gfx.GFX_Screen_Get_ByIndex(Screen.SCREEN_1), (uint)(Gfx.GFX_Screen_GetSize_ByIndex(Screen.SCREEN_1) + Gfx.GFX_Screen_GetSize_ByIndex(Screen.SCREEN_2) + Gfx.GFX_Screen_GetSize_ByIndex(Screen.SCREEN_3)), true);
			Wsa.WSA_DisplayFrame(wsa, frame++, 0, 0, Screen.SCREEN_0);

			Gui.GUI_SetPaletteAnimated(Gui.g_palette_998A, 60);

			Sound.Music_Play(0x24);

			Timer.g_timerTimeout = 360;

			while (Wsa.WSA_DisplayFrame(wsa, frame++, 0, 0, Screen.SCREEN_0)) Timer.Timer_Sleep(6);

			Wsa.WSA_Unload(wsa);

			if (Input.Input_Keyboard_NextKey() != 0 && g_canSkipIntro) goto logos_exit;

			Sound.Voice_LoadVoices(0xFFFF);

			for (; Timer.g_timerTimeout != 0; Sleep.sleepIdle())
			{
				if (Input.Input_Keyboard_NextKey() != 0 && g_canSkipIntro) goto logos_exit;
			}

			Gui.GUI_SetPaletteAnimated(Gfx.g_palette2, 60);

			while (CDriver.Driver_Music_IsPlaying()) Sleep.sleepIdle();

			Gui.GUI_SetPaletteAnimated(Gfx.g_palette2, 60);

			Gfx.GFX_ClearScreen(Screen.SCREEN_ACTIVE);

			Sprites.Sprites_LoadImage(CString.String_GenerateFilename("AND"), Screen.SCREEN_1, Gui.g_palette_998A);

			Gui.GUI_Screen_Copy(0, 0, 0, 0, Gfx.SCREEN_WIDTH / 8, (short)Gfx.SCREEN_HEIGHT, Screen.SCREEN_1, Screen.SCREEN_0);

			Gui.GUI_SetPaletteAnimated(Gui.g_palette_998A, 30);

			for (Timer.g_timerTimeout = 60; Timer.g_timerTimeout != 0; Sleep.sleepIdle())
			{
				if (Input.Input_Keyboard_NextKey() != 0 && g_canSkipIntro) goto logos_exit;
			}

			Gui.GUI_SetPaletteAnimated(Gfx.g_palette2, 30);

			Gui.GUI_ClearScreen(Screen.SCREEN_0);

			Sprites.Sprites_LoadImage("VIRGIN.CPS", Screen.SCREEN_1, Gui.g_palette_998A);

			Gui.GUI_Screen_Copy(0, 0, 0, 0, Gfx.SCREEN_WIDTH / 8, (short)Gfx.SCREEN_HEIGHT, Screen.SCREEN_1, Screen.SCREEN_0);

			Gui.GUI_SetPaletteAnimated(Gui.g_palette_998A, 30);

			for (Timer.g_timerTimeout = 180; Timer.g_timerTimeout != 0; Sleep.sleepIdle())
			{
				if (Input.Input_Keyboard_NextKey() != 0 && g_canSkipIntro) goto logos_exit;
			}

		//TODO: Use a local function instead?
		logos_exit:
			Gui.GUI_SetPaletteAnimated(Gfx.g_palette2, 30);

			Gui.GUI_ClearScreen(Screen.SCREEN_0);

			Gfx.GFX_Screen_SetActive(oldScreenID);
		}

		static readonly byte[] colours = { 0, 0, 12, 0, 0, 0, 0, 0, 0, 0, 0, 0 };
		/*
		 * Shows the game credits.
		 */
		static void GameLoop_GameCredits()
		{
			ushort i;
			var remap = new byte[256];
			byte[] credits_buffer;
			int credits_bufferPointer;

			Gui.GUI_Mouse_Hide_Safe();

			CWidget.Widget_SetCurrentWidget(20);

			Sprites.Sprites_LoadImage("BIGPLAN.CPS", Screen.SCREEN_1, Gui.g_palette_998A);

			Gui.GUI_ClearScreen(Screen.SCREEN_0);

			Gui.GUI_Screen_Copy((short)CWidget.g_curWidgetXBase, (short)CWidget.g_curWidgetYBase, (short)CWidget.g_curWidgetXBase, (short)CWidget.g_curWidgetYBase, (short)CWidget.g_curWidgetWidth, (short)CWidget.g_curWidgetHeight, Screen.SCREEN_1, Screen.SCREEN_0);

			Gui.GUI_SetPaletteAnimated(Gui.g_palette_998A, 60);

			Sound.Music_Play(0);

			CSharpDune.GameLoop_Uninit();

			Sound.Music_Play(33);

			/*memory = GFX_Screen_Get_ByIndex(SCREEN_2);*/

			for (i = 0; i < 256; i++)
			{
				byte high, low;    /* high / low nibble */

				remap[i] = (byte)i;

				high = (byte)(i >> 4);
				low = (byte)(i & 15);

				/* map colors 144-150 to the one of the player House */
				if (high == 9 && low <= 6)
				{
					remap[i] = (byte)((((byte)CHouse.g_playerHouseID + 9) << 4) + low);
					Debug.WriteLine($"DEBUG: GameLoop_GameCredits() remap color {i} to {remap[i]}");
				}
			}

			Sprites.Sprites_LoadImage("MAPPLAN.CPS", Screen.SCREEN_1, Gui.g_palette_998A);

			Gui.GUI_Palette_RemapScreen((ushort)(CWidget.g_curWidgetXBase << 3), CWidget.g_curWidgetYBase, (ushort)(CWidget.g_curWidgetWidth << 3), CWidget.g_curWidgetHeight, Screen.SCREEN_1, remap);

			Gui.GUI_Screen_FadeIn2((short)(CWidget.g_curWidgetXBase << 3), (short)CWidget.g_curWidgetYBase, (short)(CWidget.g_curWidgetWidth << 3), (short)CWidget.g_curWidgetHeight, Screen.SCREEN_1, Screen.SCREEN_0, 1, false);

			GameCredits_LoadPalette();

			credits_buffer = Gfx.GFX_Screen_Get_ByIndex(Screen.SCREEN_3);
			credits_bufferPointer = Gfx.SCREEN_WIDTH * CWidget.g_curWidgetHeight;
			Debug.WriteLine($"DEBUG: GameLoop_GameCredits() credit buffer is {CWidget.g_curWidgetHeight} lines in SCREEN_3 buffer");

			Gui.GUI_Mouse_Hide_Safe();

			Gui.GUI_InitColors(colours, 0, (byte)(colours.Length - 1));

			CFont.g_fontCharOffset = -1;

			Gfx.GFX_SetPalette(Gfx.g_palette1);

			for (; ; Sleep.sleepIdle())
			{
				CFile.File_ReadBlockFile(CString.String_GenerateFilename("CREDITS"), credits_buffer, Gfx.GFX_Screen_GetSize_ByIndex(Screen.SCREEN_3), credits_bufferPointer);

				GameCredits_Play(credits_buffer, credits_bufferPointer, 20, Screen.SCREEN_1, Screen.SCREEN_2, 6);

				if (Input.Input_Keyboard_NextKey() != 0) break;

				Sound.Music_Play(33);
			}

			Gui.GUI_SetPaletteAnimated(Gfx.g_palette2, 60);

			CDriver.Driver_Music_FadeOut();

			Gfx.GFX_ClearScreen(Screen.SCREEN_ACTIVE);
		}

		static void GameCredits_LoadPalette()
		{
			ushort i;
			int p;

			if (Gfx.g_palette1 != null) Trace.WriteLine($"WARNING: g_palette1 already allocated");
			else Gfx.g_palette1 = new byte[256 * 3 * 10]; //malloc(256 * 3 * 10);
			if (Gfx.g_palette2 != null) Trace.WriteLine($"WARNING: g_palette2 already allocated");
			else Gfx.g_palette2 = new byte[256 * 3]; //calloc(1, 256 * 3);

			CFile.File_ReadBlockFile("IBM.PAL", Gfx.g_palette1, 256 * 3);

			/* Create 10 fadein/fadeout palettes */
			p = 0;
			for (i = 0; i < 10; i++)
			{
				ushort j;

				for (j = 0; j < 255 * 3; j++) Gfx.g_palette1[p++] = (byte)(Gfx.g_palette1[j] * (9 - i) / 9); //*p++ = *pr++ * (9 - i) / 9;

				Gfx.g_palette1[p++] = 0x3F;
				Gfx.g_palette1[p++] = 0x3F;
				Gfx.g_palette1[p++] = 0x3F;
			}
		}

		static void GameCredits_Play(byte[] data, int dataPointer, ushort windowID, Screen spriteScreenID, Screen backScreenID, ushort delay)
		{
			ushort i;
			ushort stringCount = 0;
			uint timetoWait;
			ushort spriteID = 514;
			var textEnd = false;
			ushort spriteX;
			ushort spriteY;
			ushort spritePos = 0;
			var strings = new CreditString[33];
			var positions = new CreditPosition[6];
			ushort stage = 4;
			ushort counter = 60;

			for (i = 0; i < strings.Length; i++) strings[i] = new CreditString();
			for (i = 0; i < positions.Length; i++) positions[i] = new CreditPosition();

			CWidget.Widget_SetCurrentWidget(windowID);

			spriteX = (ushort)((CWidget.g_curWidgetWidth << 3) - Sprites.Sprite_GetWidth(Sprites.g_sprites[spriteID]));
			spriteY = (ushort)(CWidget.g_curWidgetHeight - Sprites.Sprite_GetHeight(Sprites.g_sprites[spriteID]));

			positions[0].x = spriteX;
			positions[0].y = 0;
			positions[1].x = 0;
			positions[1].y = (ushort)(spriteY / 2);
			positions[2].x = spriteX;
			positions[2].y = spriteY;
			positions[3].x = 0;
			positions[3].y = 0;
			positions[4].x = spriteX;
			positions[4].y = (ushort)(spriteY / 2);
			positions[5].x = 0;
			positions[5].y = spriteY;

			/* initialize */
			Gui.GUI_Screen_Copy(0, 0, 0, 0, Gfx.SCREEN_WIDTH / 8, (short)Gfx.SCREEN_HEIGHT, Screen.SCREEN_0, spriteScreenID);
			Gui.GUI_Screen_Copy(0, 0, 0, 0, Gfx.SCREEN_WIDTH / 8, (short)Gfx.SCREEN_HEIGHT, spriteScreenID, backScreenID);

			GameCredits_SwapScreen((ushort)(CWidget.g_curWidgetYBase + 24), CWidget.g_curWidgetHeight, spriteScreenID, Screen.SCREEN_3);

			Gfx.GFX_Screen_SetActive(Screen.SCREEN_0);
			timetoWait = Timer.g_timerSleep;

			Input.Input_History_Clear();

			while ((!textEnd || stage != 0) && (Input.Input_Keyboard_NextKey() == 0))
			{
				while (timetoWait > Timer.g_timerSleep) Sleep.sleepIdle();

				timetoWait = Timer.g_timerSleep + delay;

				while ((CWidget.g_curWidgetHeight / 6) + 2 > stringCount && data[dataPointer] != 0)
				{
					string text; //char *
					ushort y;

					if (stringCount != 0)
					{
						/* below or next to the previous string */
						y = (ushort)strings[stringCount - 1].y;
						if (strings[stringCount - 1].separator != 5)
						{
							y += (ushort)(strings[stringCount - 1].charHeight + strings[stringCount - 1].charHeight / 8);
						}
					}
					else
					{
						y = CWidget.g_curWidgetHeight;
					}

					text = CSharpDune.Encoding.GetString(data[dataPointer..(dataPointer + 50)]);

					var index = text.IndexOfAny(new[] { '\x05', '\r' });
					if (index != -1)
                    {
						dataPointer += index; //strpbrk(data, "\x05\r");
						text = text[..index];
                    }
					else
                    {
						data = new[] { (byte)0 }; //strchr(text, '\0');
					}

					strings[stringCount].separator = data[dataPointer];
					data[dataPointer] = 0; //*data = '\0';
					if (strings[stringCount].separator != 0) dataPointer++;
					strings[stringCount].type = 0;

					if (text.Length > 0)
                    {
						switch ((byte)text[0])
						{
							case 1:
								text = text[1..];
								CFont.Font_Select(CFont.g_fontNew6p);
								break;
							case 2:
								text = text[1..];
								CFont.Font_Select(CFont.g_fontNew8p);
								break;
							case 3:
							case 4:
								strings[stringCount].type = (byte)text[0];
								text = text[1..];
								break;
						}
                    }

					strings[stringCount].charHeight = CFont.g_fontCurrent.height;

					switch (strings[stringCount].type)
					{
						case 3:     /* "xxx by:" text : on the left */
							strings[stringCount].x = (ushort)(157 - CFont.Font_GetStringWidth(text));
							break;

						case 4:     /* names on the right */
							strings[stringCount].x = 161;
							break;

						default:    /* centered strings */
							strings[stringCount].x = (ushort)(1 + (Gfx.SCREEN_WIDTH - CFont.Font_GetStringWidth(text)) / 2);
							break;
					}

					strings[stringCount].y = (short)y;
					strings[stringCount].text = text;

					stringCount++;
				}

				switch (stage)
				{
					case 0: /* 0 : clear */
						Gui.GUI_ClearScreen(spriteScreenID);

						if (spriteID == 514) Gui.GUI_ClearScreen(backScreenID);

						stage++;
						counter = 2;
						break;

					case 1:
					case 4: /* 1, 4 : Wait */
						if (counter == 0)
						{
							counter = 0;
							stage++;
						}
						else
						{
							counter--;
						}
						break;

					case 2: /* 2 : display new picture */
						if (spriteID == 525) spriteID = 514;

						Gui.GUI_DrawSprite(spriteScreenID, Sprites.g_sprites[spriteID], (short)positions[spritePos].x, (short)positions[spritePos].y, windowID, Gui.DRAWSPRITE_FLAG_WIDGETPOS);

						counter = 8;
						stage++;
						spriteID++;
						if (++spritePos > 5) spritePos = 0;
						break;

					case 3: /* 3 : fade from black */
						if (counter < 8) Gfx.GFX_SetPalette(Gfx.g_palette1[(256 * 3 * counter)..]);

						if (counter-- == 0)
						{
							stage++;
							counter = 20;
						}
						break;

					case 5: /* 5 : fade to black */
						if (counter > 0) Gfx.GFX_SetPalette(Gfx.g_palette1[(256 * 3 * counter)..]);

						if (counter++ >= 8) stage = 0;
						break;

					default: break;
				}

				/* copy sprite (image) to back buffer */
				Gui.GUI_Screen_Copy((short)CWidget.g_curWidgetXBase, (short)CWidget.g_curWidgetYBase, (short)CWidget.g_curWidgetXBase, (short)CWidget.g_curWidgetYBase, (short)CWidget.g_curWidgetWidth, (short)CWidget.g_curWidgetHeight, spriteScreenID, backScreenID);

				/* draw all strings on back buffer and scroll them 1 pixel up */
				for (i = 0; i < stringCount; i++)
				{
					if (strings[i].y < CWidget.g_curWidgetHeight)
					{
						Gfx.GFX_Screen_SetActive(backScreenID);

						CFont.Font_Select(CFont.g_fontNew8p);

						if (strings[i].charHeight != CFont.g_fontCurrent.height) CFont.Font_Select(CFont.g_fontNew6p);

						Gui.GUI_DrawText(strings[i].text, (short)strings[i].x, (short)(strings[i].y + CWidget.g_curWidgetYBase), 255, 0);

						Gfx.GFX_Screen_SetActive(Screen.SCREEN_0);
					}

					strings[i].y--;
				}

				/* display what we just draw on back buffer */
				GameCredits_SwapScreen((ushort)(CWidget.g_curWidgetYBase + 24), CWidget.g_curWidgetHeight, backScreenID, Screen.SCREEN_3);

				if (strings[0].y < -10)
				{
					/* remove 1st string and shift the other */
					//strings[0].text += strlen(strings[0].text);
					strings[0].text = ((char)strings[0].separator).ToString();
					stringCount--;
					Array.Copy(strings, 1, strings, 0, stringCount); //memmove(&strings[0], &strings[1], stringCount * sizeof(*strings));
				}

				if ((CWidget.g_curWidgetHeight / 6 + 2) > stringCount)
				{
					if (strings[stringCount - 1].y + strings[stringCount - 1].charHeight < CWidget.g_curWidgetYBase + CWidget.g_curWidgetHeight) textEnd = true;
				}
			}

			/* fade to black */
			Gui.GUI_SetPaletteAnimated(Gfx.g_palette2, 120);

			Gui.GUI_ClearScreen(Screen.SCREEN_0);
			Gui.GUI_ClearScreen(spriteScreenID);
			Gui.GUI_ClearScreen(backScreenID);
		}

		static void GameCredits_SwapScreen(ushort top, ushort height, Screen srcScreenID, Screen dstScreenID)
		{
			var bPointer = 0;
			var screen1Pointer = 0;
			var screen2Pointer = 0;

			var b = Gfx.GFX_Screen_Get_ByIndex(dstScreenID);   /* destination */
			var screen1 = Gfx.GFX_Screen_Get_ByIndex(srcScreenID).AsSpan(top * Gfx.SCREEN_WIDTH / 2);  /* source */
			var screen2 = Gfx.GFX_Screen_Get_ByIndex(Screen.SCREEN_0).AsSpan(top * Gfx.SCREEN_WIDTH / 2);   /* secondary destination : Video RAM*/

			for (var count = height * Gfx.SCREEN_WIDTH / 2; count > 0; count--)
			{
				if (b[bPointer] != screen1[screen1Pointer] || b[bPointer + 1] != screen1[screen1Pointer + 1])
				{
					b[bPointer] = screen1[screen1Pointer];
					b[bPointer + 1] = screen1[screen1Pointer + 1];
					screen2[screen2Pointer] = screen1[screen1Pointer];
					screen2[screen2Pointer + 1] = screen1[screen1Pointer + 1];
				}
				bPointer += 2;
				screen1Pointer += 2;
				screen2Pointer += 2;
			}
		}
	}
}
