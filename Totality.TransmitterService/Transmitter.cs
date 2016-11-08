using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;
using Totality.Model;
using Totality.Model.Interfaces;
using Totality.LoggingSystem;
using Totality.Model.Diplomatical;
using Totality.Handlers.Diplomatical;
using Totality.Handlers.Nuke;
using Totality.Handlers.Main;

namespace Totality.TransmitterService
{
    [ServiceBehavior(InstanceContextMode = InstanceContextMode.Single)]
    public class Transmitter : AbstractLoggable, ITransmitterService
    {
        public SynchronizedCollection<Client> Clients = new SynchronizedCollection<Client>();
        public DiplomaticalHandler DipHandler { get; set; }
        public NukeHandler NukeHandler { get; set; }
        public MainHandler MainHandler { get; set; }

        public Transmitter(ILogger log) : base(log)
        {
        }

        public Transmitter() : base(null)
        {
        }

        public bool Register(string myName)
        {
            if (!Clients.Any(c => c.Name == myName))
            {
                Client newClient = new Client(OperationContext.Current.GetCallbackChannel<ICallbackService>(), myName);
                newClient.Fault += FaultHandler;
                Clients.Add(newClient);
                _log.Info("Client " + myName + " registered!");
                return true;
            }
            else
            {
                _log.Warn("Client " + myName + " tried to register twice!");
                return false;
            }
        }

        private void FaultHandler(Client sender)
        {
            _log.Info("Client " + sender.Name + " disconnected!");
            Clients.Remove(sender);
        }

        public bool AddOrders(List<Order> orders)
        {
            MainHandler.AddOrders(orders);
            return true;
        }

        public bool ShootDownNuke(string defender, Guid rocketId)
        {
            NukeHandler.TryToShootdown(defender, rocketId);
            return true;
        }

        public bool DipMsg(DipMsg msg)
        {
            _log.Trace("Got a diplomatical message from " + msg.From);
            DipHandler.ProcessDipMessage(msg);
            return true;
        }

        public void InitializeNukeDialogs()
        {
            _log.Trace("Starting nuke attack...");
            foreach (Client client in Clients)
            {
                client.Transmitter.InitializeNukeDialog();
            }
        }

        public void UpdateNukeDialogs(List<NukeRocket> rockets)
        {
            foreach (Client client in Clients)
            {
                client.Transmitter.UpdateNukeDialog(rockets);
            }
        }

        public void SendNews(Dictionary<string, List<Model.News>> newsBase)
        {
            _log.Trace("Sending news...");
            foreach (Client client in Clients)
            {
                if(newsBase.ContainsKey(client.Name))
                client.Transmitter.SendNews(newsBase[client.Name]);
            }
        }

        public void UpdateClients(Dictionary<string, Country> countries)
        {
            _log.Trace("Updating countries...");
            foreach (Client client in Clients)
            {
                client.Transmitter.UpdateClient(countries[client.Name]);
            }
        }

        public void SendDip(DipMsg msg)
        {
            _log.Trace("Client " + msg.From + " sended a diplomatical message to " + msg.To);
            Clients.First(x => x.Name == msg.To).Transmitter.SendDip(msg);
        }

        public void SendContractsToAll(List<DipContract> contracts)
        {
            _log.Trace("Sending contracts to all...");
            foreach (Client client in Clients)
            {
                client.Transmitter.SendContracts( contracts.Where(x => x.From == client.Name || x.To == client.Name).ToList<DipContract>() );
            }
        }

    }
}
