using FunEvents.Application.Abstractions.Persistence;
using FunEvents.Application.Common.Results;
using FunEvents.Application.Users.Queries.GetUserById;
using FunEvents.Domain.Entities;
using MediatR;

namespace FunEvents.Application.Users.Commands.CreateUser
{
    public class CreateUserCommandHandler : IRequestHandler<CreateUserCommand, Result<UserResponse>>
    {
        private readonly IUserRepository _userRepository;
        private readonly IUnitOfWork _unitOfWork;

        public CreateUserCommandHandler(IUserRepository userRepository, IUnitOfWork unitOfWork)
        {
            _userRepository = userRepository;
            _unitOfWork = unitOfWork;
        }

        private static ResultError Error(string code, string message) => new ResultError(code, message);
        public async Task<Result<UserResponse>> Handle(CreateUserCommand request, CancellationToken cancellationToken)
        {

            var existingUser = await _userRepository.GetByEmailAsync(request.Email, cancellationToken);

            if (existingUser is not null)
            {
                return Result<UserResponse>.Failure(Error(
                        "User.EmailAlreadyExists",
                        "Ya existe un usuario registrado con ese email.")
                );
            }

            var user = new User
            {
                Id = Guid.NewGuid(),
                Name = request.Name,
                Email = request.Email
            };

            await _userRepository.AddAsync(user, cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            var response = new UserResponse(user.Id, user.Name, user.Email);

            return Result<UserResponse>.Success(response);
        }
    }
}
