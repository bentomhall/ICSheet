using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ICSheet5e.EventCoordinator
{
    class MulticastEventCoordinator
    {
        private MulticastEventCoordinator instance;
        private List<IChangeNotificationReceiver> listeners = new List<IChangeNotificationReceiver>();

        private MulticastEventCoordinator() { }

        public MulticastEventCoordinator SharedInstance 
        { 
            get 
            { 
                if (instance == null)
                {
                    instance = new MulticastEventCoordinator();
                    return instance;
                }
                else
                {
                    return instance;
                }
            } 
        }

        public void Subscribe(IChangeNotificationReceiver receiver)
        {
            listeners.Add(receiver);
        }

        public void Unsubscribe(IChangeNotificationReceiver receiver)
        {
            listeners.Remove(receiver);
        }

        public void Notify(object sender, EventArgs e)
        {
            foreach (var r in listeners)
            {
                r.OnChangeNotificationReceived(sender, e);
            }
        }
    }
}
