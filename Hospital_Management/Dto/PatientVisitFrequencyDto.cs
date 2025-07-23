using Microsoft.EntityFrameworkCore;

namespace Hospital_Management.Dto
{
    [Keyless]
    public class PatientVisitFrequencyDto
    {
        public int PatientId { get; set; }
        public int NumberOfVisit { get; set; }
    }
}
