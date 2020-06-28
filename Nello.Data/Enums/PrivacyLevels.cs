using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System.Runtime.Serialization;

namespace Nello.Data.Enums
{
    public enum PrivacyLevels
    {
        [EnumMember(Value = "Public")]
        Public, // Owned by a user but visible to community
        [EnumMember(Value = "Private")]
        Private, // Owned by a user but only visable to them
        [EnumMember(Value = "System")]
        System // Owned by app and visible to all users
    }
}