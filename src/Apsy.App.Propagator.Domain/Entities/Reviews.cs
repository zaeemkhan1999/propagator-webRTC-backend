namespace Apsy.App.Propagator.Domain.Entities
{
    public class Reviews : EntityDef
    {
        public int ProductId { get; set; }

        public string Description { get; set; }

        public int Rating { get; set; }

        public int UserId { get; set; }

        //public Product Product { get; set; } 

        public User User { get; set; }
    }
   
}