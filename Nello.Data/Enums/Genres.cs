using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System.Runtime.Serialization;

namespace Nello.Data.Enums
{
    [JsonConverter(typeof(StringEnumConverter))]
    public enum Genres
    {
        [EnumMember(Value = "Action")]
        Action,
        [EnumMember(Value = "Adventure")]
        Adventure,
        [EnumMember(Value = "Animation")]
        Animation,
        [EnumMember(Value = "Biography")]
        Biography,
        [EnumMember(Value = "Comedy")]
        Comedy,
        [EnumMember(Value = "Crime")]
        Crime,
        [EnumMember(Value = "Documentary")]
        Documentary,
        [EnumMember(Value = "Drama")]
        Drama,
        [EnumMember(Value = "Family")]
        Family,
        [EnumMember(Value = "Fantasy")]
        Fantasy,
        [EnumMember(Value = "Film-Noir")]
        FilmNoir,
        [EnumMember(Value = "History")]
        History,
        [EnumMember(Value = "Horror")]
        Horror,
        [EnumMember(Value = "Music")]
        Music,
        [EnumMember(Value = "Musical")]
        Musical,
        [EnumMember(Value = "News")]
        News,
        [EnumMember(Value = "Reality-TV")]
        RealityTV,
        [EnumMember(Value = "Romance")]
        Romance,
        [EnumMember(Value = "Sci-Fi")]
        SciFi,
        [EnumMember(Value = "Short")]
        Short,
        [EnumMember(Value = "Sport")]
        Sport,
        [EnumMember(Value = "Thriller")]
        Thriller,
        [EnumMember(Value = "War")]
        War,
        [EnumMember(Value = "Western")]
        Western,
    }
}
