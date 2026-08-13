using FunEvents.Application.Abstractions.Persistence;
using FunEvents.Application.Common.Results;
using MediatR;

namespace FunEvents.Application.Users.Queries.GetUserById
{
    public class GetUserByIdHandler : IRequestHandler<GetUserByIdQuery, Result<UserResponse>>
    {
        private readonly IUserRepository _userRepository;

        public GetUserByIdHandler(
            IUserRepository userRepository)
        {
            _userRepository = userRepository;
        }

        private static ResultError Error(string code, string message) => new ResultError(code, message);

        public async Task<Result<UserResponse>> Handle(GetUserByIdQuery request, CancellationToken cancellationToken)
        {
            var user = await _userRepository.GetByIdAsync(request.Id, cancellationToken);

            if (user is null)
            {
                return Result<UserResponse>.Failure(Error(
                    "User.NotFound",
                    $"No se encontró el usuario con Id '{request.Id}'.")
                );
            }

            var response = new UserResponse(user.Id, user.Name, user.Email);

            return Result<UserResponse>.Success(response);
        }

    }
}
