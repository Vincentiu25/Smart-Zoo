namespace MobyLabWebProgramming.Core.DataTransferObjects
{
    public class SpeciesDTO
    {
        public Guid Id { get; set; }
        public string CommonName { get; set; } = string.Empty;
        public string ScientificName { get; set; } = string.Empty;
        public string Habitat { get; set; } = string.Empty;
        public string Diet { get; set; } = string.Empty;
    }
}
