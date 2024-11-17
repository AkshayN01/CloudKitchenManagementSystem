using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CKMS.Interfaces.Repository
{
    public interface IUnitOfWork : IDisposable
    {
        Task<int> CompleteAsync();
    }

    public interface IOrderUnitOfWork : IUnitOfWork
    {
        IOrderRepository OrderRepository { get; }
    }
    public interface IAdminUserUnitOfWork : IUnitOfWork
    {
        IAdminUserRepository AdminUserRepository { get; }
        IKitchenRepository KitchenRepository { get; }
    }
}
