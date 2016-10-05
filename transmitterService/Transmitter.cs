using CommonClasses;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;

namespace transmitterService
{
    [ServiceBehavior(InstanceContextMode = InstanceContextMode.Single)]
    public class Transmitter : ITransmitter
    {
        public class Client
        {
            public string name;
            public ICallback transmitter;
            public delegate void faultHandler(Client sender);
            public event faultHandler fault;

            public Client(ICallback channel, string name)
            {
                this.name = name;
                this.transmitter = channel;
                IContextChannel channelHandle = OperationContext.Current.Channel;
                channelHandle.Faulted += ChannelHandle_Faulted;
            }

            private void ChannelHandle_Faulted(object sender, EventArgs e)
            {
                fault.Invoke(this);
            }
        }

        public SynchronizedCollection<Client> clients = new SynchronizedCollection<Client>();

        public bool register(string myName)
        {
            if (!clients.Any(c => c.name == myName))
            {
                Client newClient = new Client(OperationContext.Current.GetCallbackChannel<ICallback>(), myName);
                newClient.fault += faultHandler;
                clients.Add(newClient);
                return true;
            }
            else return false;
        }

        private void faultHandler(Client sender)
        {
            clients.Remove(sender);
        }

        public bool addOrders(List<Order> orders)
        {
            return false;
        }

        public bool shootDownNuke()
        {
            return false;
        }

        public bool dipMsg(DipMsg msg)
        {
            return false;
        }
    }
}
