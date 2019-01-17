using System.Threading.Tasks;

namespace Funda.Shared
{
    public interface IApiGetter
    {
        Task<string> GetMakelaarInformation();
    }
}