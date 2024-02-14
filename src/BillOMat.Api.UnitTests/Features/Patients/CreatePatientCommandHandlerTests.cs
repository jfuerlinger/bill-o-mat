using BillOMat.Api.Data;
using BillOMat.Api.Data.Repositories;
using BillOMat.Api.Features.Patients;
using FluentAssertions;
using FluentValidation;
using FluentValidation.Results;
using Microsoft.EntityFrameworkCore;
using Moq;

namespace BillOMat.Api.Test.Features.Patients
{
    public class CreatePatientCommandHandlerTests
    {
        private readonly Mock<ApplicationDbContext> _dbContextMock;
        private readonly Mock<IUnitOfWork> _unitOfWorkMock;
        private readonly Mock<IPatientRepository> _patientRepositoryMock;
        private readonly Mock<IValidator<CreatePatient.Command>> _validatorMock;

        public CreatePatientCommandHandlerTests()
        {
            _dbContextMock = new Mock<ApplicationDbContext>(MockBehavior.Strict, [new Mock<DbContextOptions>().Object]);
            _patientRepositoryMock = new Mock<IPatientRepository>();
            _unitOfWorkMock = new Mock<IUnitOfWork>();
            _validatorMock = new Mock<IValidator<CreatePatient.Command>>();
        }

        [Fact]
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
                failures => failures.Should().ContainEquivalentOf(new ValidationFailure("Email", "Email is already in use"))
            );
        }
    }
}