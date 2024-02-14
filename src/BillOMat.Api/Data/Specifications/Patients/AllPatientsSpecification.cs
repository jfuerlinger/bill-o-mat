using BillOMat.Api.Entities;    

namespace BillOMat.Api.Data.Specifications.Patients
{
    public class AllPatientsSpecification : Specification<Patient>
    {
        public AllPatientsSpecification() : base()
        {
            AddOrderBy(p => p.LastName);
        }


    }
}
