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
    public class Transmitter : ITransmitter
    {
        public SynchronizedCollection<Client> clients = new SynchronizedCollection<Client>();

        public bool Register(string myName)
        {
            if (!clients.Any(c => c.Name == myName))
            {
                Client newClient = new Client(OperationContext.Current.GetCallbackChannel<ICallback>(), myName);
                newClient.Fault += FaultHandler;
                clients.Add(newClient);
                return true;
            }
            else return false;
        }

        private void FaultHandler(Client sender)
        {
            clients.Remove(sender);
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
    }
}
