using Nello.Data.Models;

namespace Nello.Data.Interfaces
{
    public interface IUnogsRepo
    {
        MovieRestResultModel ConvertJsonToMovieResult(string MovieJson);
        string GetMovieJson(int offset = 0);
    }
}