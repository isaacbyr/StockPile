using Caliburn.Micro;
using DesktopUI.Library.Api;
using DesktopUI.Library.EventModels;
using DesktopUI.Library.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Threading;

namespace DesktopUI.ViewModels
{
    public class SocialViewModel: Screen
    {
        private readonly IFriendsEndpoint _friendEndpoint;
        private readonly IFriendRequestEndpoint _friendRequestEndpoint;
        private readonly IUserEndpoint _userEndpoint;
        private readonly IRealizedProfitLossEndpoint _realizedPLEndpoint;
        private readonly ITransactionEndoint _transactionEndpoint;
        private readonly IApiHelper _apiHelper;
        private readonly IEventAggregator _events;

        public SocialViewModel(IFriendsEndpoint friendEndpoint, IFriendRequestEndpoint friendRequestEndpoint,
            IUserEndpoint userEndpoint, IRealizedProfitLossEndpoint realizedPLEndpoint, ITransactionEndoint transactionEndpoint,
            IApiHelper apiHelper, IEventAggregator events)
        {
            _friendEndpoint = friendEndpoint;
            _friendRequestEndpoint = friendRequestEndpoint;
            _userEndpoint = userEndpoint;
            _realizedPLEndpoint = realizedPLEndpoint;
            _transactionEndpoint = transactionEndpoint;
            _apiHelper = apiHelper;
            _events = events;
        }

        protected override async void OnViewLoaded(object view)
        {
            await LoadFriends();
            await LoadRequests();
            await LoadLeaderboard();
            await LoadDashboard();
            StartClock();
        }

        private void StartClock()
        {
            DispatcherTimer timer = new DispatcherTimer();
            timer.Interval = TimeSpan.FromSeconds(30);
            timer.Tick += tickevent;
            timer.Start();
        }

        private void tickevent(object sender, EventArgs e)
        {
            CurrentTime = DateTime.Now.ToString("t");
        }


        private async Task LoadDashboard()
        {
            if (Friends == null || Friends.Count == 0)
            {
                return;
            }

            Dashboard = new List<TransactionDisplayModel>();

            foreach (var f in Friends)
            {
                var friendTransactions = await _transactionEndpoint.LoadTransactionsById(f.Id);

                var convertedTransactions = ConvertToDashboardDisplay(friendTransactions);

                Dashboard.AddRange(convertedTransactions);
            }
            Dashboard = Dashboard.OrderByDescending(x => x.Date).ToList();
        }

        private List<TransactionDisplayModel> ConvertToDashboardDisplay(List<SocialDashboardDataModel> friendTransactions)
        {
            var userTransactions = new List<TransactionDisplayModel>();

            foreach(var t in friendTransactions)
            {
                if(t.Buy == true)
                {
                    var transaction = new TransactionDisplayModel
                    {
                        Date = t.Date,
                        Transaction = $"{t.FullName} bought {t.Shares} shares of {t.Ticker} at {Math.Round(t.Price,2)}"
                    };

                    userTransactions.Add(transaction);
                }
                else
                {
                    var transaction = new TransactionDisplayModel
                    {
                        Date = t.Date,
                        Transaction = $"{t.FullName} sold {t.Shares} shares of {t.Ticker} at {Math.Round(t.Price, 2)}"
                    };

                    userTransactions.Add(transaction);
                }
            }
            return userTransactions;
        }

        private async Task LoadLeaderboard()
        {
            if(Friends == null || Friends.Count == 0)
            {
                return;
            }

            Leaderboard = new BindingList<LeaderboardDisplayModel>();

            foreach (var f in Friends)
            {
                var friendStats = await _realizedPLEndpoint.LoadRealizedPL(f.Id);

                var leaderboadFriend = ConvertToLeaderboardDisplay(friendStats);

                Leaderboard.Add(leaderboadFriend);
            }

        }

        private LeaderboardDisplayModel ConvertToLeaderboardDisplay(List<LeaderboardModel> friendStats)
        {
            int wins = 0;
            int losses = 0;
            decimal winnningPercentage = 0;

            foreach(var r in friendStats)
            {
                if(r.ProfitLoss > 0)
                {
                    wins++;
                }
                else
                {
                    losses++;
                }
            }

            if(wins != 0 && losses != 0)
            {
                winnningPercentage = Math.Round(((decimal)wins) / (wins + losses), 2);
            }
            else
            {
                winnningPercentage = 0;
            }


            var leaderboardFriend = new LeaderboardDisplayModel
            {
                Id = friendStats[0].Id,
                FullName = friendStats[0].FullName,
                ProfitLoss = friendStats.OrderByDescending(f => f.Date).First().TotalRealized,
                WinningPercentage = winnningPercentage  
            };

            return leaderboardFriend;
        }

        private async Task LoadRequests()
        {
            var requests = await _friendRequestEndpoint.LoadFriendRequests();

            if(requests.Count > 0)
            {
                FriendRequests = new BindingList<FriendDisplayModel>();

                foreach (var r in requests)
                {
                    var req = new FriendDisplayModel
                    {
                        FullName = r.FirstName + " " + r.LastName,
                        Id = r.Id
                    };

                    FriendRequests.Add(req);
                }
            }
            else
            {
                FriendRequests = new BindingList<FriendDisplayModel>();
            }
        }

        private async Task LoadFriends()
        {
            var friends = await _friendEndpoint.LoadFriends();

            if (friends.Count > 0)
            {
                Friends = new BindingList<FriendDisplayModel>();

                foreach (var f in friends)
                {
                    var friend = new FriendDisplayModel
                    {
                        FullName = f.FirstName + " " + f.LastName,
                        Id = f.Id
                    };

                    Friends.Add(friend);
                }
            }
        }

        public async Task Search()
        {
            var results = await _userEndpoint.FriendSearch(SearchInput);

            if(results.Count > 0)
            {
                SearchResults = new BindingList<FriendDisplayModel>();
                
                foreach (var r in results)
                {
                    var user = new FriendDisplayModel
                    {
                        FullName = r.FirstName + " " + r.LastName,
                        Id = r.Id
                    };

                    SearchResults.Add(user);
                }
            }
            else
            {
                SearchResults = new BindingList<FriendDisplayModel>();
            }
        }

        private string _currentTime = DateTime.Now.ToString("t");

        public string CurrentTime
        {
            get { return _currentTime; }
            set
            {
                _currentTime = value;
                NotifyOfPropertyChange(() => CurrentTime);
            }
        }

        private string _searchInput;

        public string SearchInput
        {
            get { return _searchInput; }
            set 
            {
                _searchInput = value;
                NotifyOfPropertyChange(() => SearchInput);
            }
        }

        private BindingList<FriendDisplayModel> _searchResults;

        public BindingList<FriendDisplayModel> SearchResults
        {
            get { return _searchResults; }
            set 
            {
                _searchResults = value;
                NotifyOfPropertyChange(() => SearchResults);
            }
        }

        private FriendDisplayModel _selectedSearchResult;

        public FriendDisplayModel SelectedSearchResult
        {
            get { return _selectedSearchResult; }
            set 
            { 
                _selectedSearchResult = value;
                NotifyOfPropertyChange(() => SelectedSearchResult);
            }
        }


        private FriendDisplayModel _selectedFriendRequest;

        public FriendDisplayModel SelectedFriendRequest
        {
            get { return _selectedFriendRequest; }
            set 
            { 
                _selectedFriendRequest = value;
                NotifyOfPropertyChange(() => SelectedFriendRequest);
            }
        }


        private BindingList<FriendDisplayModel> _friendRequests;

        public BindingList<FriendDisplayModel> FriendRequests
        {
            get { return _friendRequests; }
            set 
            { 
                _friendRequests = value;
                NotifyOfPropertyChange(() => FriendRequests);
            }
        }


        private BindingList<FriendDisplayModel> friends;

        public BindingList<FriendDisplayModel> Friends
        {
            get { return friends; }
            set 
            {
                friends = value;
                NotifyOfPropertyChange(() => Friends);
            }
        }

        private FriendDisplayModel _selectedFriend;

        public FriendDisplayModel SelectedFriend
        {
            get { return _selectedFriend; }
            set 
            {
                _selectedFriend = value;
                NotifyOfPropertyChange(() => SelectedFriend);
            }
        }

        private BindingList<LeaderboardDisplayModel> _leaderboard;

        public BindingList<LeaderboardDisplayModel> Leaderboard
        {
            get { return _leaderboard; }
            set 
            { 
                _leaderboard = value;
                NotifyOfPropertyChange(() => Leaderboard);
            }
        }

        private List<TransactionDisplayModel> _dashboard;

        public List<TransactionDisplayModel> Dashboard
        {
            get { return _dashboard; }
            set
            { 
                _dashboard = value;
                NotifyOfPropertyChange(() => Dashboard);
            }
        }


        public async Task Accept()
        {
            if(SelectedFriendRequest != null)
            {
                // add user to friends table
                await _friendEndpoint.PostFriendship(SelectedFriendRequest.Id);

                // delete user from request table
                await _friendRequestEndpoint.DeleteRequest(SelectedFriendRequest.Id);

                await LoadFriends();
                await LoadRequests();
            }
            else
            {
                return;
            }
        }

        public async Task Delete()
        {
            if (SelectedFriendRequest != null)
            {
                // delete user from request table
                await _friendRequestEndpoint.DeleteRequest(SelectedFriendRequest.Id);

                await LoadRequests();
            }
            else
            {
                return;
            }
        }

        public async Task SendRequest()
        {
            // TODO: Figure out to remove yourself/exisitng friend from search results and/or remove people who are pending approval of your request

            if (SelectedSearchResult != null)
            {
                var request = new FriendRequestModel
                {
                    FolloweeId = SelectedSearchResult.Id
                };
                await _friendRequestEndpoint.PostRequest(request);

                await LoadRequests();
            }
            else
            {
                return;
            }
        }

        public void Performance()
        {
            _events.PublishOnUIThread(new OpenPortfolioSummaryView());
        }


        public void BuyStocks()
        {
            _events.PublishOnUIThread(new OpenPortfolioStockView("AAPL"));
        }


        public void Home()
        {
            _events.PublishOnUIThread(new ReturnHomeEvent());
        }


        public void Logout()
        {
            _apiHelper.Logout();
            _events.PublishOnUIThread(new LogOffEvent());
        }

        public void Exit()
        {
            _events.PublishOnUIThread(new ExitAppEvent());
        }
    }
}
