using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System.Runtime.Serialization;

namespace Nello.Data.Models.Enums
{
    [JsonConverter(typeof(StringEnumConverter))]
    public enum Genres
    {
        [EnumMember(Value = "Comedy")]
        Comedy,
        [EnumMember(Value = "Documentary")]
        Documentary
    }
}
