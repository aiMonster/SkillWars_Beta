using Common.Interfaces.Services;
using DataAccessLayer.Context;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Common.Helpers;

namespace Services.TimeredFunctionsService
{
    public class TimeredFunctionsService : ITimeredFunctionsService
    {
        private readonly MSContext _db;
        private readonly IConfigurationRoot _config;        
        private readonly IServiceProvider _provider;
        private readonly ILogger<TimeredFunctionsService> _logger;


        public TimeredFunctionsService(MSContext db, IConfigurationRoot config,
             IServiceProvider provider, ILogger<TimeredFunctionsService> logger)
        {
            _logger = logger;
            _db = db;
            _config = config;
            _provider = provider;
        }

        public async Task<bool> Setup()
        {
            var switcher = TimerConfigurator.GetConfiguredTimer(1 * 30, Switcher);
            switcher.Start();

            return true;
        }

        private async Task Switcher()
        {
            _logger.LogInformation("Timered Functions Service started");
            var now = DateTime.UtcNow;

            if (now.Day == 1 && now.Hour < 1)
            {
                //await SendMonthlyReport();
            }

            if (now.Hour < 1)
            {
                //await ParseConferencesFromDou();
                //await RemoveOldConferences();
                await CheckEndsTokenExpirates();
                //await LoadFacebookEvents();
                //await RemoveOldChatFiles();
            }
            //await CheckBanEnds();
            //await CheckDurationIsEnd();
            //await CheckEndsPetitionVoting();
            //await CheckVotingStarting();
            _logger.LogInformation("Timered Functions Service stopped");
        }


        public async Task CheckEndsTokenExpirates()
        {
            var now = DateTime.UtcNow;
            var oldTokens = await _db.Tokens.Where(p => p.ExpirationDate < now).ToListAsync();
            _db.Tokens.RemoveRange(oldTokens);
        }
    }
}
