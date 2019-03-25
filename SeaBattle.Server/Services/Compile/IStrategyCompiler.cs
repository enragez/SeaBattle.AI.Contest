namespace SeaBattle.Server.Services.Compile
{
    using System.IO;
    using System.Threading.Tasks;

    public interface IStrategyCompiler
    {
        Task<byte[]> Compile(Stream zipStream);
    }
}