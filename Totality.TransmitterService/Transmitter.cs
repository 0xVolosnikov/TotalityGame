using Totality.CommonClasses;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;
using Totality.Model;
using Totality.Model.Interfaces;
using Totality.LoggingSystem;

namespace Totality.TransmitterService
{
    [ServiceBehavior(InstanceContextMode = InstanceContextMode.Single)]
    public class Transmitter : AbstractLoggable, ITransmitterService
    {
        public SynchronizedCollection<Client> Clients = new SynchronizedCollection<Client>();

        public Transmitter(ILogger log) : base(log)
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
            return false;
        }

        public bool ShootDownNuke()
        {
            return false;
        }

        public bool DipMsg(DipMsg msg)
        {
            return false;
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

        public void UpdateNews()
        {
            throw new NotImplementedException();
        }

        public void SendDip(DipMsg msg)
        {
            _log.Trace("Client " + msg.From + " sended a diplomatical message to " + msg.To);
            Clients.First(x => x.Name == msg.To).Transmitter.SendDip(msg);
        }
    }
}
