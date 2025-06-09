namespace Apsy.App.Propagator.Infrastructure
{
    public class DataWriteContext:DataContext
    {
        public DataWriteContext(DbContextOptions options):base(options) { }
    }   
}
