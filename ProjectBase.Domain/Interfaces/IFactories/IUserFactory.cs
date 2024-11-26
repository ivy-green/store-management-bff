using ProjectBase.Domain.Abstractions;
using ProjectBase.Domain.DTOs.Requests;
using ProjectBase.Domain.Entities;

namespace ProjectBase.Domain.Interfaces.IFactories
{
    public interface IUserFactory
    {
        Task<Result> CreateUser(User user, UserCreateDTO dataCreate);
        Task<Result> UpdateUser(User user, UserCreateDTO dataUpdate);
    }
}