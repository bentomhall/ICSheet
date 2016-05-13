using System.Collections.Generic;
using System.Runtime.Serialization;

namespace ICSheetCore
{
    /// <summary>
    /// 
    /// </summary>
    [DataContract]
    public class CharacterRPInformation
    {
        [DataMember]
        private List<string> _languages;

        [DataMember]
        private List<string> _tools;

        [DataMember]
        private List<string> _contacts;

        /// <summary>
        /// 
        /// </summary>
        [DataMember]
        public string Deity { get; private set; }

        /// <summary>
        /// 
        /// </summary>
        [DataMember]
        public string Name { get; private set; }

        /// <summary>
        /// 
        /// </summary>
        [DataMember]
        public string Alignment { get; private set; }

        /// <summary>
        /// 
        /// </summary>
        [DataMember]
        public string Background { get; private set; }

        /// <summary>
        /// 
        /// </summary>
        [DataMember]
        public int BaseWeight { get; private set; }

        /// <summary>
        /// 
        /// </summary>
        [DataMember]
        public string Notes { get; set; }

        /// <summary>
        /// 
        /// </summary>
        [DataMember]
        public string Height { get; private set; }

        /// <summary>
        /// 
        /// </summary>
        public IEnumerable<string> Languages { get { return _languages; } }

        /// <summary>
        /// 
        /// </summary>
        public IEnumerable<string> Tools { get { return _tools; } }

        /// <summary>
        /// 
        /// </summary>
        public IEnumerable<string> Contacts { get { return _contacts; } }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="language"></param>
        public void AddLanguage(string language) { _languages.Add(language); }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="tool"></param>
        public void AddTool(string tool) { _tools.Add(tool); }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="contactName"></param>
        public void AddContact(string contactName) { _contacts.Add(contactName); }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="name"></param>
        /// <param name="alignment"></param>
        /// <param name="baseWeight"></param>
        /// <param name="background"></param>
        /// <param name="deity"></param>
        /// <param name="height"></param>
        public CharacterRPInformation(string name, string alignment, int baseWeight, string height, string background, string deity)
        {
            _contacts = new List<string>();
            _tools = new List<string>();
            _languages = new List<string>() { "Common" }; //all character start out with common
            BaseWeight = BaseWeight;
            Name = name;
            Alignment = alignment;
            Height = height;
            Background = background;
            Deity = deity;
        }
    }
}
