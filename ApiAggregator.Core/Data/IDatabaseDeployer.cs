
namespace ApiAggregator.Core.Data
{
    public interface IDatabaseDeployer
    {
        bool DatabaseExists();
        void DeploySchema();
        void Seed();
    }
}
