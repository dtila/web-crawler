using MovieCrawler.Core;
using MovieCrawler.Domain.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace MovieCrawler.Domain.Collections
{
    public abstract class SyncronizedEnumerator : IEnumerator<Task<ICollection<BasicMovieInfo>>>, IPageSet
    {
        private int _currentPage;
        private int _totalPages;
        private ManualResetEventSlim _initializationEvent;

        public SyncronizedEnumerator(int? seedPage = null)
        {
            _initializationEvent = new ManualResetEventSlim();
            this.Reset();

            if (seedPage.HasValue)
            {
                _currentPage = seedPage.Value;
                if (_currentPage <= 0)
                    throw new ArgumentOutOfRangeException("The seedPage parameter must be a positive number");
            }
        }

        #region IPageSet interface 

        public int CurrentPage
        {
            get { return Interlocked.CompareExchange(ref _currentPage, 0, 0); }
        }

        public int TotalPages
        {
            get
            {
                var value = Interlocked.CompareExchange(ref _totalPages, 0, 0);
                if (value == -1)
                    throw new InvalidOperationException("Unable to get the number of pages because the enumerator has not been started");
                return value;
            }
        }

        public bool HasTotalPages
        {
            get { return Interlocked.CompareExchange(ref _totalPages, 0, 0) > 0; }
        }

        #endregion

        public Task<ICollection<BasicMovieInfo>> Current { get; private set; }

        protected abstract Task<ICollection<BasicMovieInfo>> ParseFirstPage();
        
        /// <summary>
        /// Issue a page parse for a certain page number.
        /// </summary>
        /// <param name="page"></param>
        /// <returns></returns>
        protected abstract Task<ICollection<BasicMovieInfo>> ParsePage(int page);

        public bool MoveNext()
        {
            if (Interlocked.CompareExchange(ref _totalPages, 0, 0) == -1)
            {
                Current = InternalInitialize();
                return true;
            }

            _initializationEvent.Wait();

            var page = Interlocked.Increment(ref _currentPage);
            if (page <= _totalPages)
            {
                Current = ParsePage(page);
                return true;
            }
            return false;
        }

        public void Reset()
        {
            Interlocked.Exchange(ref _currentPage, -1);
            Interlocked.Exchange(ref _totalPages, -1);
            Current = null;
            _initializationEvent.Reset();
        }

        public virtual void Dispose()
        {
            if (Current != null)
            {
                Current.Dispose();
                Current = null;
            }

            if (_initializationEvent != null)
            {
                _initializationEvent.Dispose();
                _initializationEvent = null;
            }
        }

        private async Task<ICollection<BasicMovieInfo>> InternalInitialize()
        {
            var results = await ParseFirstPage();
            if (Interlocked.CompareExchange(ref _totalPages, 0, 0) < 0)
                throw new InvalidOperationException("Initialization parse event must set the total number of pages. This must be done by calling the SetTotalPages function");
            Interlocked.CompareExchange(ref _currentPage, 1, -1);
            _initializationEvent.Set();
            return results;
        }

        protected void SetTotalPages(int pages)
        {
            if (pages < 0)
                throw new InvalidOperationException("The total number of pages must be a positive number");
            Interlocked.Exchange(ref _totalPages, pages);
        }

        object System.Collections.IEnumerator.Current
        {
            get { return this.Current; }
        }

        public IEnumerator<Task<ICollection<BasicMovieInfo>>> GetEnumerator()
        {
            return this;
        }

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return this;
        }
    }
}
