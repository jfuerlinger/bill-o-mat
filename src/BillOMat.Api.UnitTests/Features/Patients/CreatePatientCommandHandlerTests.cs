using BillOMat.Api.Data;
using BillOMat.Api.Data.Repositories;
using BillOMat.Api.Features.Patients;
using FluentAssertions;
using FluentValidation.Results;
using Moq;
using Xunit.Categories;

namespace BillOMat.Api.Test.Features.Patients
{
    public class CreatePatientCommandHandlerTests
    {
        private readonly Mock<IUnitOfWork> _unitOfWorkMock;
        private readonly Mock<IPatientRepository> _patientRepositoryMock;

        public CreatePatientCommandHandlerTests()
        {
            _patientRepositoryMock = new Mock<IPatientRepository>();
            _unitOfWorkMock = new Mock<IUnitOfWork>();
        }

        [Fact]
        [UnitTest]
        public async Task Handle_Should_ReturnValidationFailure_WhenEmailIsNotUniqe()
        {
            // Arrange
            var command = new CreatePatient.Command()
            {
                FirstName = "Josef",
                LastName = "Fürlinger",
                Nickname = "Joe",
                Email = "josef.fuerlinger@gmail.com"
            };

            _patientRepositoryMock
                .Setup(
                    x => x.IsEmailUniqueAsync(
                        It.IsAny<string>(),
                        It.IsAny<CancellationToken>()))
                .ReturnsAsync(false);

            _unitOfWorkMock
                .Setup(x => x.PatientRepository)
                .Returns(_patientRepositoryMock.Object);
            

            var handler = new CreatePatient.Handler(
                               unitOfWork: _unitOfWorkMock.Object,
                               validator: new CreatePatient.CommandValidator());

            // Act
            var result = await handler.Handle(command, default);

            // Assert
            result.Match(
                patientId => throw new Exception("A patient was created with a duplicate email address!"),
                failures => failures.Should().ContainEquivalentOf(new ValidationFailure("Email", "Email is already in use!"))
            );
        }
    }
}