/* HouseInfo file table */

namespace SharpDune.Table;

static class TableHouseInfo
{
    internal static HouseInfo[] g_table_houseInfo = [ //[HOUSE_MAX]
        new() {
            name = "Harkonnen",
            toughness = 200,
            degradingChance = 85,
            degradingAmount = 3,
            minimapColor = 144,
            specialCountDown = 600,
            starportDeliveryTime = 10,
            prefixChar = 'H',
            specialWeapon = 1,
            musicWin = 6,
            musicLose = 3,
            musicBriefing = 24,
            voiceFilename = "nhark.voc"
        },

        new() {
            name = "Atreides",
            toughness = 77,
            degradingChance = 0,
            degradingAmount = 1,
            minimapColor = 160,
            specialCountDown = 300,
            starportDeliveryTime = 10,
            prefixChar = 'A',
            specialWeapon = 2,
            musicWin = 7,
            musicLose = 4,
            musicBriefing = 25,
            voiceFilename = "nattr.voc"
        },

        new() {
            name = "Ordos",
            toughness = 128,
            degradingChance = 10,
            degradingAmount = 2,
            minimapColor = 176,
            specialCountDown = 300,
            starportDeliveryTime = 10,
            prefixChar = 'O',
            specialWeapon = 3,
            musicWin = 5,
            musicLose = 2,
            musicBriefing = 26,
            voiceFilename = "nordo.voc"
        },

        new() {
            name = "Fremen",
            toughness = 10,
            degradingChance = 0,
            degradingAmount = 1,
            minimapColor = 192,
            specialCountDown = 300,
            starportDeliveryTime = 0,
            prefixChar = 'O',
            specialWeapon = 2,
            musicWin = 5,
            musicLose = 2,
            musicBriefing = 65535,
            voiceFilename = "afremen.voc"
        },

        new() {
            name = "Sardaukar",
            toughness = 10,
            degradingChance = 0,
            degradingAmount = 1,
            minimapColor = 208,
            specialCountDown = 600,
            starportDeliveryTime = 0,
            prefixChar = 'H',
            specialWeapon = 1,
            musicWin = 6,
            musicLose = 3,
            musicBriefing = 65535,
            voiceFilename = "asard.voc"
        },

        new() {
            name = "Mercenary",
            toughness = 0,
            degradingChance = 0,
            degradingAmount = 1,
            minimapColor = 224,
            specialCountDown = 300,
            starportDeliveryTime = 0,
            prefixChar = 'M',
            specialWeapon = 3,
            musicWin = 7,
            musicLose = 4,
            musicBriefing = 65535,
            voiceFilename = "amerc.voc"
        }
    ];
}
