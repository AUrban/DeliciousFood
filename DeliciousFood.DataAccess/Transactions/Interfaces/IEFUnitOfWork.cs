using DeliciousFood.DataAccess.Repositories;
using Microsoft.EntityFrameworkCore;

namespace DeliciousFood.DataAccess.Transactions
{
    /// <summary>
    /// Unit of work extension interface in terms of Entity Framework
    /// </summary>
    public interface IEFUnitOfWork : IUnitOfWork
    {
        DbContext DbContext { get; }
    }
}
