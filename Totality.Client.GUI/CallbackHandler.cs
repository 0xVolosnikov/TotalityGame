using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Totality.Client.GUI.ReferenceToServer;

namespace Totality.Client.GUI
{
    class CallbackHandler : ITransmitterServiceCallback
    {
        public delegate void CountryUpdatedHandler(Country country);
        public event CountryUpdatedHandler CountryUpdated;

        public CallbackHandler()
        {
        }

        public void InitializeNukeDialog()
        {
            //throw new NotImplementedException();
        }

        public void SendContracts(DipContract[] contracts)
        {
            //throw new NotImplementedException();
        }

        public void SendDip(DipMsg msg)
        {
            //throw new NotImplementedException();
        }

        public void SendNews(News[] newsList)
        {
            //throw new NotImplementedException();
        }

        public void UpdateClient(Country country)
        {
            CountryUpdated.Invoke(country);
        }

        public void UpdateNukeDialog(NukeRocket[] rockets)
        {
            //throw new NotImplementedException();
        }
    }
}
