using Microsoft.Owin;
using Ninject;
using Ninject.Web.Common.OwinHost;
using Ninject.Web.WebApi.OwinHost;
using Owin;
using System.Reflection;
using System.Web.Http;
using TicTacToe.Data;
using TicTacToe.Data.UnitOfWork;
using TicTacToe.GameLogic;

[assembly: OwinStartup(typeof(TicTacToe.WebApi.Startup))]

namespace TicTacToe.WebApi
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);

            app.UseNinjectMiddleware(CreateKernel).UseNinjectWebApi(GlobalConfiguration.Configuration);
        }

        private static StandardKernel CreateKernel()
        {
            var kernel = new StandardKernel();
            kernel.Load(Assembly.GetExecutingAssembly());

            RegisterMappings(kernel);

            return kernel;
        }

        public static void RegisterMappings(StandardKernel kernel)
        {
            kernel
                .Bind<ITicTacToeData>()
                .To<TicTacToeData>()
                .WithConstructorArgument("context", c => new TicTacToeDbContext());

            kernel
                .Bind<IGameResultValidator>()
                .To<GameResultValidator>();
        }
    }
}
