using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Totality.CommonClasses;
using Totality.Model;

namespace Totality.Processors
{
    public interface ITransmitter
    {
        void InitializeNukeDialogs();

        void UpdateNukeDialogs(List<NukeRocket> rockets);

        void UpdateNews();

        void SendDip(DipMsg msg);

    }
}
