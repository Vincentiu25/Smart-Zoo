using System;

namespace MobyLabWebProgramming.Core.DataTransferObjects
{
    public class SpeciesUpdateDTO
    {
        public Guid Id { get; set; } // ID-ul speciei care se va actualiza

        public string? CommonName { get; set; }

        public string? ScientificName { get; set; }

        public string? Habitat { get; set; }

        public string? Diet { get; set; }
    }
}
