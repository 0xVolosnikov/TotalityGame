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
using System.Windows;

namespace Totality.TransmitterService
{
    [ServiceBehavior(InstanceContextMode = InstanceContextMode.Single)]
    public class Transmitter : AbstractLoggable, ITransmitterService
    {
        public delegate void ClientRegister(string name);
        public event ClientRegister ClientRegistered;

        public delegate void ClientFault(string name);
        public event ClientFault ClientDisconnected;

        public delegate void ClientData(string name);
        public event ClientData ClientSendedData;

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
            ClientRegistered?.Invoke(myName);

            if (!Clients.Any(c => c.Name == myName))
            {
                Client newClient = new Client(OperationContext.Current.GetCallbackChannel<ICallbackService>(), myName);
                newClient.Fault += FaultHandler;
                Clients.Add(newClient);
                MainHandler.AddCountry(myName);
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
            ClientDisconnected?.Invoke(sender.Name);
            Clients.Remove(sender);
        }

        public bool AddOrders(List<Order> orders, string name)
        {
            MainHandler.AddOrders(orders);
            ClientSendedData?.Invoke(name);
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

        public Country GetCountryData(string name)
        {
            try
            {
                return MainHandler.GetCountry(name);
            }
            catch (Exception e)
            {
                _log.Error(e.Message);
                MessageBox.Show(e.Message);
                return null;
            }

        }

        public Dictionary<string, long> GetCurrencyStock()
        {
            try
            {
                return MainHandler.GetCurrencyStock();
            }

            catch (Exception e)
            {
                _log.Error(e.Message);
                MessageBox.Show(e.Message);
                return null;
            }

        }

        public Dictionary<string, long> GetCurrencyDemands()
        {
            try
            {
                return MainHandler.GetCurrencyDemands();
            }

            catch (Exception e)
            {
                _log.Error(e.Message);
                MessageBox.Show(e.Message);
                return null;
            }
}
    }
}
