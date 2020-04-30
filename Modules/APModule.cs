using CertusView.OP.WebClient.Interfaces;
using CertusView.OP.WebClient.Orchestrators.AccountPayable;
using Ninject.Modules;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CertusView.OP.WebClient.Modules
{
    /// <summary>
    /// Handles setting up the dependencies for AP
    /// </summary>
    public class APModule : NinjectModule
    {
        public override void Load()
        {
            this.Bind<IAccountPayableOrchestrator>().To<AccountPayableOrchestrator>();
        }
    }
}