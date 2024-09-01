using AutoMapper;
using DocumentFormat.OpenXml.Spreadsheet;
using Moq;
using Profunion.Anexs.Dto.EventDto;
using Profunion.Anexs.Dto.ReservationDto;
using Profunion.Core.Entitities.Applications;
using Profunion.Core.Entitities.Events;
using Profunion.Core.Entitities.Reservations;
using Profunion.Core.Entitities.UserModels;
using Profunion.Core.Interfaces.IServices;
using Profunion.Core.Interfaces.Repositories;
using Profunion.Core.Services.Reservation;

namespace UnitTests
{
    public class ReservationServiceTests
    {
        private readonly Mock<IReservationList> _reservationList;
        private readonly Mock<IUserRepository> _userRepository;
        private readonly Mock<IEventRepository> _eventRepository;
        private readonly Mock<IMapper> _mapper;

        private readonly ReservationService _reservationService;
        public ReservationServiceTests()
        {
            _reservationList = new Mock<IReservationList>();
            _userRepository = new Mock<IUserRepository>();
            _eventRepository = new Mock<IEventRepository>();
            _mapper = new Mock<IMapper>();

            _reservationService = new ReservationService(_reservationList.Object, _userRepository.Object, _eventRepository.Object, _mapper.Object);
        }

        [Fact]
        public async Task CreateReservationUnitTests()
        {
            var application = new Application
            {
                EventId = "1",
                UserId = "1",
                ticketsCount = 1,

            };

            var currentEvent = new Event
            {
                eventId = application.EventId,
                totalTickets = 5
            };

            _eventRepository.Setup(repo => repo.GetEventsByID(application.EventId))
                .ReturnsAsync(currentEvent);

            _mapper.Setup(mapper => mapper.Map<ReservationList>(It.IsAny<CreateReservationDto>))
                .Returns<CreateReservationDto>(dto => new ReservationList
                {
                    EventId = dto.eventId,
                    UserId = dto.userId,
                    ticketsCount = dto.ticketsCount,
                    createdAt = dto.createdAt
                });

            _reservationList.Setup(repo => repo.CreateReservation(It.IsAny<ReservationList>()))
                .ReturnsAsync(true);

            var result = await _reservationService.CreateReservation(application);

            Assert.True(result);

            _eventRepository.Verify(repo => repo.GetEventsByID(application.EventId), Times.Once);
            _eventRepository.Verify(repo => repo.UpdateEvents(It.IsAny<Event>()), Times.Once);
            _reservationList.Verify(repo => repo.CreateReservation(It.IsAny<ReservationList>()), Times.Once);
        }

        [Fact]
        public async Task GetUserReservationTests()
        {
            // Arrange
            var user = new User()
            {
                userId = "1"
            };

            var currentEvent = new GetEventDto
            {
                eventId = "1",
                totalTickets = 5
            };



            var reservations = new List<ReservationList>
            {
                new ReservationList
                {
                    Id = "1",
                    UserId = user.userId,
                    EventId = currentEvent.eventId,
                    ticketsCount = 5,
                    createdAt = DateTime.UtcNow
                }
            };
            var reservationDtos = new List<GetReservationDto>
            {
                new GetReservationDto
                {
                    eventId = currentEvent.eventId,
                    userId = user.userId,
                    ticketsCount = 5,
                    createdAt = DateTime.UtcNow
                }
            };


            _reservationList.Setup(repo => repo.GetAllReservation())
                .ReturnsAsync(reservations);

            _eventRepository.Setup(repo => repo.GetEvents())
                .ReturnsAsync(new List<GetEventDto> { currentEvent });

            _mapper.Setup(mapper => mapper.Map<List<GetReservationDto>>(reservations))
                .Returns(reservationDtos);

            // ACT

            var result = await _reservationService.GetUserReservation(user.userId);

            // ASSERT

            Assert.Equal(reservationDtos, result.reservations);

            Assert.Equal(reservationDtos.Sum(r => r.ticketsCount), result.totalTickets);

            _reservationList.Verify(repo => repo.GetAllReservation(), Times.Once);
            _eventRepository.Verify(repo => repo.GetEvents(), Times.Once);
            _mapper.Verify(mapper => mapper.Map<List<GetReservationDto>>(reservations), Times.Once);

            Console.WriteLine($"Reservations: {string.Join(", ", result.reservations.Select(r => r.eventId))}");
            Console.WriteLine($"Total Tickets: {result.totalTickets}");

        }

    }
}
