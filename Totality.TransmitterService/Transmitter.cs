using Totality.CommonClasses;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;
using Totality.Model;

namespace Totality.TransmitterService
{
    [ServiceBehavior(InstanceContextMode = InstanceContextMode.Single)]
    public class Transmitter : ITransmitterService
    {
        public SynchronizedCollection<Client> Clients = new SynchronizedCollection<Client>();

        public bool Register(string myName)
        {
            if (!Clients.Any(c => c.Name == myName))
            {
                Client newClient = new Client(OperationContext.Current.GetCallbackChannel<ICallbackService>(), myName);
                newClient.Fault += FaultHandler;
                Clients.Add(newClient);
                return true;
            }
            else return false;
        }

        private void FaultHandler(Client sender)
        {
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
            Clients.First(x => x.Name == msg.To).Transmitter.SendDip(msg);
        }
    }
}
