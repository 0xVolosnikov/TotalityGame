using System;
using System.Linq;
using Totality.Handlers.News;
using Totality.Model.Diplomatical;
using Totality.Model.Interfaces;

namespace Totality.Handlers.Diplomatical
{
    public class DiplomaticalHandler : AbstractHandler
    {
        private ITransmitter _transmitter;

        public DiplomaticalHandler(NewsHandler newsHandler, ITransmitter transmitter, IDataLayer dataLayer, ILogger logger) : base(newsHandler, dataLayer, logger)
        {
            _transmitter = transmitter;
        }

        public void ProcessDipMessage(DipMsg msg)
        {
            if (!msg.Applied)
            {
                _transmitter.SendDip(msg);
            }
            else
                addToDatabase(msg);
        }

        private void addToDatabase(DipMsg msg)
        {
            DipContract contract = new DipContract(msg.Type, msg.From, msg.To);
            contract.Text = msg.Text;
            contract.Res = msg.Resource;
            contract.Price = msg.Price;
            contract.Id = msg.Id;
            contract.Time = msg.Time;
            contract.Count = (int)msg.Count;
            contract.Description = msg.Description;

            _dataLayer.AddContract(contract);
        }

        public void BreakContract(Guid id)
        {
            _dataLayer.BreakContract(id);
        }

        public void SendContractsToAll()
        {
            _transmitter.SendContractsToAll(_dataLayer.GetContractList());
        }

    }
}
