using System;

namespace MobyLabWebProgramming.Core.DataTransferObjects
{
    public class ZooAnimalUpdateDTO
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public Guid SpeciesId { get; set; }
        public int Age { get; set; }
        public bool IsEndangered { get; set; }
    }
}
