﻿using System;
using System.Threading.Tasks;
using Doug.Models;
using Doug.Monsters.Seagulls;
using Doug.Repositories;
using Doug.Slack;

namespace Doug.Services
{
    public interface IMonsterService
    {
        void RollMonsterSpawn();
        Task HandleMonsterDeathByUser(User user, SpawnedMonster spawnedMonster, string channel);
    }

    public class MonsterService : IMonsterService
    {
        private const double SpawnChance = 0.2;
        private const string PvpChannel = "CL2TYGE1E";

        private readonly IMonsterRepository _monsterRepository;
        private readonly ISlackWebApi _slack;
        private readonly IUserService _userService;
        private readonly IUserRepository _userRepository;

        public MonsterService(IMonsterRepository monsterRepository, ISlackWebApi slack, IUserService userService, IUserRepository userRepository)
        {
            _monsterRepository = monsterRepository;
            _slack = slack;
            _userService = userService;
            _userRepository = userRepository;
        }

        public void RollMonsterSpawn()
        {
            if (new Random().NextDouble() >= SpawnChance)
            {
                return;
            }

            var monster = new Seagull(); //TODO add more monster variety and pick them randomly (or based on present players levels)

            _monsterRepository.SpawnMonster(monster, PvpChannel); 
            _slack.BroadcastMessage(string.Format(DougMessages.MonsterSpawned, monster.Name), PvpChannel);
        }

        public async Task HandleMonsterDeathByUser(User user, SpawnedMonster spawnedMonster, string channel)
        {
            var monster = spawnedMonster.Monster;

            var userIds = await _slack.GetUsersInChannel(channel);
            var users = _userRepository.GetUsers(userIds);

            _monsterRepository.RemoveMonster(spawnedMonster.Id);

            await _slack.BroadcastMessage(string.Format(DougMessages.MonsterDied, monster.Name), channel);

            await _userService.AddBulkExperience(users, monster.ExperienceValue, channel);
        }
    }
}
