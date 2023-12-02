using MobileWeb.Models.DTOs;
using MobileWeb.Models.Entities;

namespace MobileWeb.Services.UserService;

public interface IUserService
{
	Task<bool> Order(string uid, OrderDTO orderDTO);

	Task<IList<User>> GetAllUsersAsync();

	Task<User> FindUserByIdAsync(string uid);

	Task<User> FindUserByNameAsync(string uname);


    Task<User> FindUserByEmailAsync(string email);

    Task<bool> UpdateUserAsync(string uid, UserDTO userDTO);

	Task<bool> VerifyEmailAsync(User user);

	Task<bool> DeleteUserAsync(string uid);

	Task<string> GeneratePasswordChangeTokenAsync(User user);

	Task<List<Order>> GetProccessingOrdersAsync(string uid);

	Task<List<Order>> GetShippingOrdersAsync(string uid);

	Task<List<Order>> GetFinishedOrdersAsync(string uid);
	
	Task<bool> ChangePasswordAsync(string uname, string currentPassword, string newPassword);
}
