using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProxyProviderLibrary
{
    public class BaseObserver<T> : IObservable<T>
    {
        protected ConcurrentBag<IObserver<T>> _observers = new ConcurrentBag<IObserver<T>>();

    
        protected void RaiseOnNext(T instance)
        {
            foreach (var observer in _observers)
            {
                try
                {
                    observer.OnNext(instance);
                }
                catch (Exception)
                {
                }
            }
        }

        protected void RaiseOnCompleted()
        {
            foreach (var observer in _observers)
            {
                try
                {
                    observer.OnCompleted();
                }
                catch (Exception)
                {
                }
            }
        }

        protected void RaiseOnException(Exception ex)
        {
            foreach (var observer in _observers)
            {
                try
                {
                    observer.OnError(ex);
                }
                catch (Exception)
                {
                }
            }
        }

        public IDisposable Subscribe(IObserver<T> observer)
        {
            _observers.Add(observer);
            return new Subscription(this, observer);
        }


        private void RemoveSubscription(IObserver<T> observer)
        {
            if (!_observers.TryTake(out observer))
                throw new ObjectDisposedException("Unable to dispose the subscription.");
        }


        private class Subscription : IDisposable
        {
            private BaseObserver<T> _instance;
            private IObserver<T> _observer;

            public Subscription(BaseObserver<T> instance, IObserver<T> observer)
            {
                _instance = instance;
                _observer = observer;
            }

            public void Dispose()
            {
                _instance.RemoveSubscription(_observer);
            }
        }
    }
}
