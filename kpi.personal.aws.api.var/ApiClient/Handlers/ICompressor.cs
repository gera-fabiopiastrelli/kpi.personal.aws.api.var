using System.IO;
using System.Threading.Tasks;

namespace kpi.personal.aws.api.var.ApiClient.Handlers
{
    public interface ICompressor
    {
        string EncodingType { get; }
        Task Compress(Stream source, Stream destination);
        Task Decompress(Stream source, Stream destination);
    }
}