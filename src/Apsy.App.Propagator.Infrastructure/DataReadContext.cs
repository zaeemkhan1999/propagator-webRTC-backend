namespace Apsy.App.Propagator.Infrastructure
{
    public class DataReadContext : DataContext
    {
        public DataReadContext(DbContextOptions options)
      : base(options)
        {
        }
    }
}
