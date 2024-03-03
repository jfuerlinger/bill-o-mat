using BillOMat.Api.Entities;

namespace BillOMat.Api.Data.Specifications.Patients
{
    public class PatientByIdSpecification(int id) : Specification<Patient>(patient => patient.Id == id)
    {
    }
}
