namespace MobyLabWebProgramming.Core.DataTransferObjects
{
    public class SpeciesAddDTO
    {
        public string CommonName { get; set; } = string.Empty;        // ex: Leu
        public string ScientificName { get; set; } = string.Empty;    // ex: Panthera leo
        public string Habitat { get; set; } = string.Empty;           // ex: Savană, Junglă
        public string Diet { get; set; } = string.Empty;              // ex: Carnivor, Erbivor
    }
}
