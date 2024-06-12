﻿/* Widget file table */

namespace SharpDune.Table;

static class TableWidget
{
    internal static CWidget[] g_table_windowWidgets = [
        new() {
            next = null,
            index = 30,
            shortcut = 0,
            shortcut2 = 0,
            drawModeNormal = 1,
            drawModeSelected = 1,
            drawModeDown = 1,
            flags = new WidgetFlags { /* flags */
	    		requiresClick = true,
                notused1 = true,
                clickAsHover = false,
                invisible = false,
                greyWhenInvisible = false,
                noClickCascade = false,
                loseSelect = true,
                notused2 = false,
                buttonFilterLeft = 4,
                buttonFilterRight = 4
            },
            drawParameterNormal = new WidgetDrawParameter(), //{ 0x0 },
	    	drawParameterSelected = new WidgetDrawParameter(), //{ 0x0 },
	    	drawParameterDown = new WidgetDrawParameter(), //{ 0x0 },
	    	parentID = 0,
            offsetX = 0,
            offsetY = 0,
            width = 0,
            height = 0,
            fgColourNormal = 15,
            bgColourNormal = 12,
            fgColourSelected = 15,
            bgColourSelected = 12,
            fgColourDown = 15,
            bgColourDown = 12,
            state = new WidgetState { /* state */
	    		selected = false,
                hover1 = false,
                hover2 = false,
                selectedLast = false,
                hover1Last = false,
                hover2Last = false,
                notused = false,
                keySelected = false,
                buttonState = 0
            },
            clickProc = null,
            data = null,
            stringID = (ushort)Text.STR_NULL
        },
        new() {
            next = null,
            index = 31,
            shortcut = 0,
            shortcut2 = 0,
            drawModeNormal = 1,
            drawModeSelected = 1,
            drawModeDown = 1,
            flags = new WidgetFlags { /* flags */
	    		requiresClick = true,
                notused1 = true,
                clickAsHover = false,
                invisible = false,
                greyWhenInvisible = false,
                noClickCascade = false,
                loseSelect = true,
                notused2 = false,
                buttonFilterLeft = 4,
                buttonFilterRight = 4
            },
            drawParameterNormal = new WidgetDrawParameter(), //{ 0x0 },
			drawParameterSelected = new WidgetDrawParameter(), //{ 0x0 },
			drawParameterDown = new WidgetDrawParameter(), //{ 0x0 },
			parentID = 0,
            offsetX = 0,
            offsetY = 0,
            width = 0,
            height = 0,
            fgColourNormal = 15,
            bgColourNormal = 12,
            fgColourSelected = 15,
            bgColourSelected = 12,
            fgColourDown = 15,
            bgColourDown = 12,
            state = new WidgetState { /* state */
				selected = false,
                hover1 = false,
                hover2 = false,
                selectedLast = false,
                hover1Last = false,
                hover2Last = false,
                notused = false,
                keySelected = false,
                buttonState = 0
            },
            clickProc = null,
            data = null,
            stringID = (ushort)Text.STR_NULL
        },
        new() {
            next = null,
            index = 32,
            shortcut = 0,
            shortcut2 = 0,
            drawModeNormal = 1,
            drawModeSelected = 1,
            drawModeDown = 1,
            flags = new WidgetFlags { /* flags */
				requiresClick = true,
                notused1 = true,
                clickAsHover = false,
                invisible = false,
                greyWhenInvisible = false,
                noClickCascade = false,
                loseSelect = true,
                notused2 = false,
                buttonFilterLeft = 4,
                buttonFilterRight = 4
            },
            drawParameterNormal = new WidgetDrawParameter(), //{ 0x0 },
			drawParameterSelected = new WidgetDrawParameter(), //{ 0x0 },
			drawParameterDown = new WidgetDrawParameter(), //{ 0x0 },
			parentID = 0,
            offsetX = 0,
            offsetY = 0,
            width = 0,
            height = 0,
            fgColourNormal = 15,
            bgColourNormal = 12,
            fgColourSelected = 15,
            bgColourSelected = 12,
            fgColourDown = 15,
            bgColourDown = 12,
            state = new WidgetState { /* state */
				selected = false,
                hover1 = false,
                hover2 = false,
                selectedLast = false,
                hover1Last = false,
                hover2Last = false,
                notused = false,
                keySelected = false,
                buttonState = 0
            },
            clickProc = null,
            data = null,
            stringID = (ushort)Text.STR_NULL
        },
        new() {
            next = null,
            index = 33,
            shortcut = 0,
            shortcut2 = 0,
            drawModeNormal = 1,
            drawModeSelected = 1,
            drawModeDown = 1,
            flags = new WidgetFlags { /* flags */
				requiresClick = true,
                notused1 = true,
                clickAsHover = false,
                invisible = false,
                greyWhenInvisible = false,
                noClickCascade = false,
                loseSelect = true,
                notused2 = false,
                buttonFilterLeft = 4,
                buttonFilterRight = 4
            },
            drawParameterNormal = new WidgetDrawParameter(), //{ 0x0 },
			drawParameterSelected = new WidgetDrawParameter(), //{ 0x0 },
			drawParameterDown = new WidgetDrawParameter(), //{ 0x0 },
			parentID = 0,
            offsetX = 0,
            offsetY = 0,
            width = 0,
            height = 0,
            fgColourNormal = 15,
            bgColourNormal = 12,
            fgColourSelected = 15,
            bgColourSelected = 12,
            fgColourDown = 15,
            bgColourDown = 12,
            state = new WidgetState { /* state */
				selected = false,
                hover1 = false,
                hover2 = false,
                selectedLast = false,
                hover1Last = false,
                hover2Last = false,
                notused = false,
                keySelected = false,
                buttonState = 0
            },
            clickProc = null,
            data = null,
            stringID = (ushort)Text.STR_NULL
        },
        new() {
            next = null,
            index = 34,
            shortcut = 0,
            shortcut2 = 0,
            drawModeNormal = 1,
            drawModeSelected = 1,
            drawModeDown = 1,
            flags = new WidgetFlags { /* flags */
				requiresClick = true,
                notused1 = true,
                clickAsHover = false,
                invisible = false,
                greyWhenInvisible = false,
                noClickCascade = false,
                loseSelect = true,
                notused2 = false,
                buttonFilterLeft = 4,
                buttonFilterRight = 4
            },
            drawParameterNormal = new WidgetDrawParameter(), //{ 0x0 },
			drawParameterSelected = new WidgetDrawParameter(), //{ 0x0 },
			drawParameterDown = new WidgetDrawParameter(), //{ 0x0 },
			parentID = 0,
            offsetX = 0,
            offsetY = 0,
            width = 0,
            height = 0,
            fgColourNormal = 15,
            bgColourNormal = 12,
            fgColourSelected = 15,
            bgColourSelected = 12,
            fgColourDown = 15,
            bgColourDown = 12,
            state = new WidgetState { /* state */
				selected = false,
                hover1 = false,
                hover2 = false,
                selectedLast = false,
                hover1Last = false,
                hover2Last = false,
                notused = false,
                keySelected = false,
                buttonState = 0
            },
            clickProc = null,
            data = null,
            stringID = (ushort)Text.STR_NULL
        },
        new() {
            next = null,
            index = 35,
            shortcut = 0,
            shortcut2 = 0,
            drawModeNormal = 1,
            drawModeSelected = 1,
            drawModeDown = 1,
            flags = new WidgetFlags { /* flags */
				requiresClick = true,
                notused1 = true,
                clickAsHover = false,
                invisible = false,
                greyWhenInvisible = false,
                noClickCascade = false,
                loseSelect = true,
                notused2 = false,
                buttonFilterLeft = 4,
                buttonFilterRight = 4
            },
            drawParameterNormal = new WidgetDrawParameter(), //{ 0x0 },
			drawParameterSelected = new WidgetDrawParameter(), //{ 0x0 },
			drawParameterDown = new WidgetDrawParameter(), //{ 0x0 },
			parentID = 0,
            offsetX = 0,
            offsetY = 0,
            width = 0,
            height = 0,
            fgColourNormal = 15,
            bgColourNormal = 12,
            fgColourSelected = 15,
            bgColourSelected = 12,
            fgColourDown = 15,
            bgColourDown = 12,
            state = new WidgetState { /* state */
				selected = false,
                hover1 = false,
                hover2 = false,
                selectedLast = false,
                hover1Last = false,
                hover2Last = false,
                notused = false,
                keySelected = false,
                buttonState = 0
            },
            clickProc = null,
            data = null,
            stringID = (ushort)Text.STR_NULL
        },
        new() {
            next = null,
            index = 36,
            shortcut = 0,
            shortcut2 = 0,
            drawModeNormal = 1,
            drawModeSelected = 1,
            drawModeDown = 1,
            flags = new WidgetFlags { /* flags */
				requiresClick = true,
                notused1 = true,
                clickAsHover = false,
                invisible = false,
                greyWhenInvisible = false,
                noClickCascade = false,
                loseSelect = true,
                notused2 = false,
                buttonFilterLeft = 4,
                buttonFilterRight = 4
            },
            drawParameterNormal = new WidgetDrawParameter(), //{ 0x0 },
			drawParameterSelected = new WidgetDrawParameter(), //{ 0x0 },
			drawParameterDown = new WidgetDrawParameter(), //{ 0x0 },
			parentID = 0,
            offsetX = 0,
            offsetY = 0,
            width = 0,
            height = 0,
            fgColourNormal = 15,
            bgColourNormal = 12,
            fgColourSelected = 15,
            bgColourSelected = 12,
            fgColourDown = 15,
            bgColourDown = 12,
            state = new WidgetState { /* state */
				selected = false,
                hover1 = false,
                hover2 = false,
                selectedLast = false,
                hover1Last = false,
                hover2Last = false,
                notused = false,
                keySelected = false,
                buttonState = 0
            },
            clickProc = null,
            data = null,
            stringID = (ushort)Text.STR_NULL
        },
        new() {
            next = null,
            index = 37,
            shortcut = 96,
            shortcut2 = 0,
            drawModeNormal = 1,
            drawModeSelected = 1,
            drawModeDown = 1,
            flags = new WidgetFlags { /* flags */
				requiresClick = true,
                notused1 = true,
                clickAsHover = true,
                invisible = false,
                greyWhenInvisible = false,
                noClickCascade = false,
                loseSelect = false,
                notused2 = true,
                buttonFilterLeft = 4,
                buttonFilterRight = 4
            },
            drawParameterNormal = new WidgetDrawParameter(), //{ 0x0 },
			drawParameterSelected = new WidgetDrawParameter(), //{ 0x0 },
			drawParameterDown = new WidgetDrawParameter(), //{ 0x0 },
			parentID = 17,
            offsetX = 128,
            offsetY = 20,
            width = 25,
            height = 16,
            fgColourNormal = 15,
            bgColourNormal = 12,
            fgColourSelected = 15,
            bgColourSelected = 12,
            fgColourDown = 15,
            bgColourDown = 12,
            state = new WidgetState { /* state */
				selected = false,
                hover1 = false,
                hover2 = false,
                selectedLast = false,
                hover1Last = false,
                hover2Last = false,
                notused = false,
                keySelected = false,
                buttonState = 0
            },
            clickProc = null,
            data = null,
            stringID = (ushort)Text.STR_NULL
        },
        new() {
            next = null,
            index = 38,
            shortcut = 98,
            shortcut2 = 0,
            drawModeNormal = 1,
            drawModeSelected = 1,
            drawModeDown = 1,
            flags = new WidgetFlags { /* flags */
				requiresClick = true,
                notused1 = true,
                clickAsHover = true,
                invisible = false,
                greyWhenInvisible = false,
                noClickCascade = false,
                loseSelect = false,
                notused2 = true,
                buttonFilterLeft = 4,
                buttonFilterRight = 4
            },
            drawParameterNormal = new WidgetDrawParameter(), //{ 0x0 },
			drawParameterSelected = new WidgetDrawParameter(), //{ 0x0 },
			drawParameterDown = new WidgetDrawParameter(), //{ 0x0 },
			parentID = 17,
            offsetX = 128,
            offsetY = 126,
            width = 25,
            height = 16,
            fgColourNormal = 15,
            bgColourNormal = 12,
            fgColourSelected = 15,
            bgColourSelected = 12,
            fgColourDown = 15,
            bgColourDown = 12,
            state = new WidgetState { /* state */
				selected = false,
                hover1 = false,
                hover2 = false,
                selectedLast = false,
                hover1Last = false,
                hover2Last = false,
                notused = false,
                keySelected = false,
                buttonState = 0
            },
            clickProc = null,
            data = null,
            stringID = (ushort)Text.STR_NULL
        }
    ];
}
