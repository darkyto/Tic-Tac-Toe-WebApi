namespace TicTacToe.Data
{
    using Microsoft.AspNet.Identity.EntityFramework;
    using System;
    using System.Collections.Generic;
    using System.Data.Entity;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using TicTacToe.Data.Migrations;
    using TicTacToe.Models;


    public class TicTacToeDbContext : IdentityDbContext<User>
    {
        public TicTacToeDbContext()
            : base("TicTacToe")
        {
            Database.SetInitializer(new MigrateDatabaseToLatestVersion<TicTacToeDbContext, Configuration>());
        }

        public IDbSet<Game> Games { get; set; }

        public static TicTacToeDbContext Create()
        {
            return new TicTacToeDbContext();
        }
    }
}
