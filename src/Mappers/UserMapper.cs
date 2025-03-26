using BrainThrust.src.Dtos.UserDtos;
using BrainThrust.src.Models.Entities;

namespace BrainThrust.src.Mappers
{
    public static class UserMapper
    {
        // Maps User entity to UserDto
        public static GetUserDto ToUserDto(this User user)
        {
            return new GetUserDto
            {
                FirstName = user.FirstName,
                LastName = user.LastName,
                Email = user.Email,
                PhoneNumber = user.PhoneNumber,
                Address = user.Address
            };
        }

        // Maps CreateUserDto to User entity
        public static User ToUser(this CreateUserDto createUserDto, byte[] passwordHash, byte[] passwordSalt, bool isAdmin)
        {
            return new User
            {
                FirstName = createUserDto.FirstName,
                LastName = createUserDto.LastName,
                Email = createUserDto.Email.ToLower(),
                PasswordHash = passwordHash,
                PasswordSalt = passwordSalt,
                Role = isAdmin ? "User" : "Admin"
            };
        }

        // Maps UpdateUserDto to existing User entity
        public static User UpdateUserFromDto(this User user, UpdateUserDto updateUserDto)
        {            
            if (!string.IsNullOrEmpty(updateUserDto.FirstName)) user.FirstName = updateUserDto.FirstName;
            if (!string.IsNullOrEmpty(updateUserDto.LastName)) user.LastName = updateUserDto.LastName;
            if (!string.IsNullOrEmpty(updateUserDto.PhoneNumber)) user.PhoneNumber = updateUserDto.PhoneNumber;
            if (!string.IsNullOrEmpty(updateUserDto.Address)) user.Address = updateUserDto.Address;

            return user;
        }

        // Maps ChangePasswordDto to existing User entity
        public static User ChangePasswordFromDto(this User user, byte[] newPasswordHash, byte[] newPasswordSalt)
        {
            user.PasswordHash = newPasswordHash;
            user.PasswordSalt = newPasswordSalt;

            return user;
        }
        // Maps ResetPasswordDto to existing User entity
        public static User ResetPasswordFromDto(this User user, byte[] newPasswordHash, byte[] newPasswordSalt)
        {
            user.PasswordHash = newPasswordHash;
            user.PasswordSalt = newPasswordSalt;
            user.PasswordResetToken = null;
            user.TokenExpiry = null;

            return user;
        }
    }
}
