using BillOMat.Api.Data;
using BillOMat.Api.Data.Repositories;
using BillOMat.Api.Data.Specifications.Patients;
using BillOMat.Api.Entities;
using BillOMat.Api.Features.Patients;
using FluentAssertions;
using Moq;
using Xunit.Categories;

namespace BillOMat.Api.Test.Features.Patients
{
    public class GetPatientQueryHandlerTests
    {
        private readonly Mock<IUnitOfWork> _unitOfWorkMock;
        private readonly Mock<IPatientRepository> _patientRepositoryMock;

        public GetPatientQueryHandlerTests()
        {
            _patientRepositoryMock = new Mock<IPatientRepository>();
            _unitOfWorkMock = new Mock<IUnitOfWork>();
        }

        [Fact]
        [UnitTest]
        public async Task Handle_ShouldReturnPatient_WhenExistingPatientIdIsProvided()
        {
            // Arrange
            int existingPatientId = 73;
            var existingPatient = new Patient()
            {
                Id = existingPatientId,
                FirstName = "Josef",
                LastName = "Fürlinger",
                Email = "josef.fuerlinge@gmail.com",
                Nickname = "Joe"
            };

            var query = new GetPatient.Query(existingPatientId);

            _patientRepositoryMock
                .Setup(
                    x => x.GetEntitiesAsync(
                        It.IsAny<PatientByIdSpecification>(),
                        It.IsAny<CancellationToken>()))
                .ReturnsAsync([existingPatient]);

            _unitOfWorkMock
                .Setup(x => x.PatientRepository)
                .Returns(_patientRepositoryMock.Object);


            var handler = new GetPatient.Handler(
                               unitOfWork: _unitOfWorkMock.Object,
                               validator: new GetPatient.QueryValidator());

            // Act
            var result = await handler.Handle(query, default);

            // Assert
            result.Match<object>(
                patient => patient.Id.Should().Be(existingPatientId),
                failures => failures.Should().BeEmpty(),
                notfound => throw new Exception("NotFound was reported!")
            );
        }


        [Fact]
        [UnitTest]
        public async Task Handle_ShouldReturnValidationFailure_WhenIdIsNotGreaterThanZero()
        {
            // Arrange
            var query = new GetPatient.Query(0);


            var handler = new GetPatient.Handler(
                               unitOfWork: _unitOfWorkMock.Object,
                               validator: new GetPatient.QueryValidator());

            // Act
            var result = await handler.Handle(query, default);

            // Assert
            result.Match<object>(
                patientId => throw new Exception("A validation error was expected!"),
                failures => failures.Should().NotBeEmpty(),
                notfound => throw new Exception("NotFound was reported!")
            );
        }
    }
}