using System;
using System.IO;
using System.Collections.Generic;
using System.Text;
using ICSheetCore.Data;
using System.Runtime.Serialization;
using Foundation;

namespace ICSheetIOS.Utility
{
    static public class FileManager
    {
        static string itemResource = @"BaseItems.xml";
        static string weaponResource = @"BaseWeapons.xml";
        static string armorResource = @"BaseArmors.xml";
        static string spellListResource = @"SpellList.xml";
        static string spellDetailResource = @"SpellDetails.xml";
        static string raceFeatureResource = @"RacialFeatures.xml";
        static string classFeatureResource = @"ClassFeatures.xml";

        static private string GetPathForCharacter(string name)
        {
            var libraryPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), "Characters");
            return Path.Combine(libraryPath, name + ".dnd5e");
        }

        public static IEnumerable<string> GetAllCharacterNames()
        {
            var libraryPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), "Characters");
            var output = new List<string>();
            if (!Directory.Exists(libraryPath))
            {
                Directory.CreateDirectory(libraryPath);
                return output;
            }
            var files = Directory.EnumerateFiles(libraryPath, "*.dnd5e");
            foreach (var f in files)
            {
                output.Add(Path.GetFileNameWithoutExtension(f));
            }
            return output;
        }

        public static CharacterData ReadCharacterWith(string name)
        {
            string path = GetPathForCharacter(name);
            var serializer = new DataContractSerializer(typeof(CharacterData));
            FileStream reader = new FileStream(path, FileMode.Open);
            var cData = (CharacterData)serializer.ReadObject(reader);
            reader.Close();
            return cData;
        }

        public static void SaveCharacterWith(string name, CharacterData pc)
        {
            string path = GetPathForCharacter(name);
            FileStream writer = new FileStream(path, FileMode.Create);
            var serializer = new DataContractSerializer(typeof(CharacterData));
            serializer.WriteObject(writer, pc);
            writer.Close();
        }

        public static string GetResourceString(ResourceType type)
        {
            string path;
            switch(type)
            {
                case ResourceType.Item:
                    path = Path.Combine(NSBundle.MainBundle.BundlePath, itemResource);
                    break;
                case ResourceType.Weapon:
                    path = Path.Combine(NSBundle.MainBundle.BundlePath, weaponResource);
                    break;
                case ResourceType.Armor:
                    path = Path.Combine(NSBundle.MainBundle.BundlePath, armorResource);
                    break;
                case ResourceType.SpellList:
                    path = Path.Combine(NSBundle.MainBundle.BundlePath, spellListResource);
                    break;
                case ResourceType.SpellDetail:
                    path = Path.Combine(NSBundle.MainBundle.BundlePath, spellDetailResource);
                    break;
                case ResourceType.ClassFeatures:
                    path = Path.Combine(NSBundle.MainBundle.BundlePath, classFeatureResource);
                    break;
                case ResourceType.RaceFeatures:
                    path = Path.Combine(NSBundle.MainBundle.BundlePath, raceFeatureResource);
                    break;
                default:
                    throw new ArgumentException($"Not a valid resource type: {type}");
            }
            return File.ReadAllText(path);
        }
    }

    public enum ResourceType
    {
        Item,
        Weapon,
        Armor,
        SpellList,
        SpellDetail,
        RaceFeatures,
        ClassFeatures
    }
}
