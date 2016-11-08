using System;
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
            DipContract contract;
            switch(msg.Type)
            {
                case DipMsg.Types.Trade:
                    contract = new ContractTrade(msg.Id, msg.From, msg.To)
                    {
                        Res = msg.Resource,
                        Price = msg.Price,
                        Time = msg.Time,
                        Count = (int)msg.Count,
                        Text = msg.Text
                    };
                    _dataLayer.AddContract(contract);
                    break;

                case DipMsg.Types.Peace:
                    contract = new ContractPeace(msg.Id, msg.From, msg.To)
                    {
                        Text = msg.Text
                    };
                    _dataLayer.AddContract(contract);
                    // заключить мир
                    break;

                case DipMsg.Types.Alliance:
                    contract = new ContractAlliance(msg.Id, msg.From, msg.To)
                    {
                        Text = msg.Text
                    };
                    _dataLayer.AddContract(contract);
                    // добавить в альянс
                    break;

                case DipMsg.Types.CurrencyAlliance:
                    contract = new ContractCurrencyAlliance(msg.Id, msg.From, msg.To)
                    {
                        Text = msg.Text
                    };
                    _dataLayer.AddContract(contract);
                    // добавить в валютный альянс
                    break;

                case DipMsg.Types.Transfer:
                    contract = new ContractTransfer(msg.Id, msg.From, msg.To)
                    {
                        Count = msg.Count,
                        Time = msg.Time,
                        Text = msg.Text
                    };
                    _dataLayer.AddContract(contract);
                    break;

                case DipMsg.Types.MilitaryTraining:
                    contract = new ContractMilitaryTraining(msg.Id, msg.From, msg.To)
                    {
                        Text = msg.Text
                    };
                    _dataLayer.AddContract(contract);
                    // провести учения
                    break;

                case DipMsg.Types.Other:
                    contract = new ContractOther(msg.Id, msg.From, msg.To)
                    {
                        Text = msg.Text
                    };
                    _dataLayer.AddContract(contract);
                    break;
            }
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
