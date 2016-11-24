using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Totality.Client.ClientComponents.ServiceReference1;
using Totality.Model;
using Totality.Model.Diplomatical;

namespace Totality.Client.GUI
{
    class CallbackHandler : ITransmitterServiceCallback
    {
        public delegate void CountryUpdatedHandler(Country country);
        public event CountryUpdatedHandler CountryUpdated;

        public delegate void InitializeNukes();
        public event InitializeNukes NukesInitialized;

        public delegate void UpdateNukes(NukeRocket[] rockets);
        public event UpdateNukes NukesUpdated;

        public delegate void NewsReceivedDelegate(News[] news);
        public event NewsReceivedDelegate NewsReceived;

        public delegate void ContractsReceivedDelegate(DipContract[] contracts);
        public event ContractsReceivedDelegate ContractsReceived;

        public delegate void ContractsReceivedMessage(DipMsg msg);
        public event ContractsReceivedMessage MessageReceived;

        public CallbackHandler()
        {
        }

        public void InitializeNukeDialog()
        {
            NukesInitialized.Invoke();
        }

        public void SendContracts(DipContract[] contracts)
        {
            ContractsReceived.Invoke(contracts);
        }

        public void SendDip(DipMsg msg)
        {
            MessageReceived.Invoke(msg);
        }

        public void SendNews(News[] newsList)
        {
            NewsReceived.Invoke(newsList);
        }

        public void UpdateClient(Country country)
        {
            CountryUpdated.Invoke(country);
        }

        public void UpdateNukeDialog(NukeRocket[] rockets)
        {
            NukesUpdated.Invoke(rockets);
        }
    }
}
