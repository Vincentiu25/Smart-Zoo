using MobyLabWebProgramming.Core.Entities;

namespace MobyLabWebProgramming.Core.Entities
{
    
    public class Species : BaseEntity
    {
        public string CommonName { get; set; } = string.Empty;         
        public string ScientificName { get; set; } = string.Empty;     
        public string Habitat { get; set; } = string.Empty;            
        public string Diet { get; set; } = string.Empty; 

        // Relație: o specie are mai multe animale
        public ICollection<ZooAnimal>? Animals { get; set; }
    }
}
