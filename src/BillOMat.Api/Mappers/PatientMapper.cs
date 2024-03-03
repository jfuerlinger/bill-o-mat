using BillOMat.Api.Entities;
using BillOMat.Api.Features.Patients;
using Riok.Mapperly.Abstractions;

namespace BillOMat.Api.Mappers
{
    [Mapper]
    public partial class PatientMapper
    {
        public partial GetPatientDto PatientToGetPatientDto(Patient patient);
        public partial ListPatientsDto PatientToListPatientsDto(Patient patient);
    }
}
