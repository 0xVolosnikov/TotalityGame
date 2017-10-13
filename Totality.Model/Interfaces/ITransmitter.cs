using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Totality.CommonClasses;
using Totality.Model;
using Totality.Model.Diplomatical;

namespace Totality.Model.Interfaces
{
    public interface ITransmitter
    {
        void InitializeNukeDialogs();

        void UpdateNukeDialogs(List<NukeRocket> rockets);

        void SendNews(Dictionary<string, List<Model.News>> newsBase);

        void UpdateClients(Dictionary<string, Country> countries);

        void UpdateClient(Country country);

        void SendDip(DipMsg msg);

        void SendContractsToAll(List<DipContract> contracts);

        void SendResults(Dictionary<string, List<OrderResult>> orderResults);
    }
}
