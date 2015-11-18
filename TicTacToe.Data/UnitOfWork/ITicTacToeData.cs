namespace TicTacToe.Data.UnitOfWork
{
    using Repositories;
    using Models;

    public interface ITicTacToeData
    {
        IRepository<User> Users { get; }

        IRepository<Game> Games { get; }

        int SaveChanges();
    }
}
