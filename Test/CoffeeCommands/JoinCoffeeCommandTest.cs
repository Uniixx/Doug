using Doug;
using Doug.Commands;
using Doug.Models;
using Doug.Repositories;
using Doug.Services;
using Doug.Slack;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System;
using System.Threading.Tasks;

namespace Test
{
    [TestClass]
    public class JoinCoffeeCommandTest
    {
        private const string CommandText = "<@otherUserid|username>";
        private const string Channel = "testchannel";
        private const string User = "testuser";

        private readonly Command command = new Command()
        {
            ChannelId = Channel,
            Text = CommandText,
            UserId = User
        };

        private CoffeeCommands _coffeeCommands;

        private readonly Mock<IChannelRepository> _channelRepository = new Mock<IChannelRepository>();
        private readonly Mock<IUserRepository> _userRepository = new Mock<IUserRepository>();
        private readonly Mock<ISlackWebApi> _slack = new Mock<ISlackWebApi>();
        private readonly Mock<IAdminValidator> _adminValidator = new Mock<IAdminValidator>();
        private readonly Mock<ICoffeeBreakService> _coffeeBreakService = new Mock<ICoffeeBreakService>();

        [TestInitialize]
        public void Setup()
        {
            _coffeeCommands = new CoffeeCommands(_channelRepository.Object, _userRepository.Object, _slack.Object, _adminValidator.Object, _coffeeBreakService.Object);
        }

        [TestMethod]
        public void WhenJoiningCoffee_UserIsAddedToRoster()
        {
            _coffeeCommands.JoinCoffee(command);

            _channelRepository.Verify(channelRepo => channelRepo.AddToRoster(User));
        }

        [TestMethod]
        public void WhenJoiningCoffee_UserIsAddedToDatabase()
        {
            _coffeeCommands.JoinCoffee(command);

            _userRepository.Verify(userRepo => userRepo.AddUser(User));
        }

        [TestMethod]
        public void WhenJoiningCoffee_BroadcastIsSentToChannel()
        {
            _coffeeCommands.JoinCoffee(command);

            _slack.Verify(slack => slack.SendMessage(It.IsAny<string>(), Channel));
        }
    }
}