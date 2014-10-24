using System.Collections.Generic;
using System.Configuration;
using System.Data.Services.Client;
using System.Web.Caching;
using System.Web.Hosting;
using System.Web.Profile;
using System.Web.Security;
using VeraWAF.AzureTableStorage;

namespace VeraWAF.WebPages.Dal {
    public class VoteCache
    {
        public const string CacheKey = "Votes";
        private readonly string _applicationName;

        public VoteCache()
        {
            _applicationName = ConfigurationManager.AppSettings["ApplicationName"];
        }

        public VoteCache(string applicationName) {
            _applicationName = applicationName;
        }

        private void AddVotesToCache(Dictionary<string, Dictionary<string, VoteEntity>> votes)
        {
            HostingEnvironment.Cache.Add(CacheKey, votes, null,
                                         Cache.NoAbsoluteExpiration,
                                         Cache.NoSlidingExpiration,
                                         CacheItemPriority.Default, null);
        }

        private Dictionary<string, Dictionary<string, VoteEntity>> GetVotesFromCache()
        {
            return (Dictionary<string, Dictionary<string, VoteEntity>>)HostingEnvironment.Cache.Get(CacheKey);
        }

        private IEnumerable<VoteEntity> GetVotesFromStore()
        {
            var datasource = new AzureTableStorageDataSource();
            var applicationName = ConfigurationManager.AppSettings["ApplicationName"];
            return datasource.GetVotes(applicationName);
        }

        public Dictionary<string, Dictionary<string, VoteEntity>> GetVotesIndexedByItemId()
        {
            var votes = GetVotesFromCache();
            if (votes == null)
            {
                votes = new Dictionary<string, Dictionary<string, VoteEntity>>();

                try
                {
                    var storedVotes = GetVotesFromStore();
                    foreach (var vote in storedVotes)
                        if (votes.ContainsKey(vote.PartitionKey))
                            votes[vote.PartitionKey].Add(vote.RowKey, vote);
                        else
                            votes.Add(vote.PartitionKey, new Dictionary<string, VoteEntity> { { vote.RowKey, vote } });
                }
                catch (DataServiceQueryException)
                {
                    // Exception caught if there are no votes in the table
                }

                AddVotesToCache(votes);
            }
            return votes;
        }

        public Dictionary<string, VoteEntity> GetVotes(string voteItemId)
        {
            var votes = GetVotesIndexedByItemId();
            return votes.ContainsKey(voteItemId) ? votes[voteItemId] : null;
        }

        public void Clear()
        {
            HostingEnvironment.Cache.Remove(CacheKey);
        }

        void AddToUsersSocialScore(object userGettingVote, int value)
        {
            var user = Membership.GetUser(userGettingVote);
            if (user != null)
            {
                var profile = ProfileBase.Create(user.UserName);
                var socialScore = (int)profile.GetPropertyValue("SocialPoints") + value;
                profile.SetPropertyValue("SocialPoints", socialScore);
                profile.Save();

                Membership.UpdateUser(user);
            }
        }

        /// <summary>
        /// Add a new up or downvote
        /// </summary>
        /// <param name="voteItemId">Vote ID. Partition key identifing the vote item in the VeraVotes table</param>
        /// <param name="votingUser">Partition key or ASP.NET membership proovider key that identifies the user giving the vote</param>
        /// <param name="userGettingVote">Partition key or ASP.NET membership proovider key that identifies the user recieving the vote</param>
        /// <param name="value">Number of points that the vote counts for.</param>
        public void AddVote(string voteItemId, string votingUser, string userGettingVote, int value)
        {
            // Make sure people are not voting for themselves
            if (votingUser == null || userGettingVote == null 
                || votingUser as string == userGettingVote as string) return;

            var votingUserString = (string) votingUser;
            var dataSource = new AzureTableStorageDataSource();

            var votes = GetVotesIndexedByItemId();
            if (votes.ContainsKey(voteItemId))
            {
                if (votes[voteItemId].ContainsKey(votingUserString))
                {
                    // User has already voted
                    var oldVote = votes[voteItemId][votingUserString].Value;
                    var newVote = oldVote + value;

                    const int minVote = -5;
                    const int maxVote = 5;

                    // Make sure the user is not voting more than he/she is allowed to
                    if (newVote >= minVote && newVote <= maxVote)
                    {
                        votes[voteItemId][votingUserString].Value = newVote;
                        dataSource.Update(votes[voteItemId][votingUserString]);

                        AddToUsersSocialScore(userGettingVote, value);
                    }
                }
                else
                {
                    // A new vote
                    var vote = new VoteEntity(voteItemId, votingUserString)
                                   {
                                       ApplicationName = _applicationName,
                                       Value = value
                                   };
                    
                    votes[voteItemId].Add(votingUserString, vote);
                    dataSource.Insert(vote);

                    AddToUsersSocialScore(userGettingVote, value);
                }
            }
            else
            {
                var vote = new VoteEntity(voteItemId, votingUserString)
                                {
                                    ApplicationName = _applicationName,
                                    Value = value
                                };

                votes.Add(vote.PartitionKey, new Dictionary<string, VoteEntity> { {vote.RowKey, vote} });
                dataSource.Insert(vote);

                AddToUsersSocialScore(userGettingVote, value);
            }

            AddVotesToCache(votes);
        }
    
    }
}