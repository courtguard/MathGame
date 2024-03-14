
using Microsoft.AspNetCore.SignalR;
using System.Text;

namespace MathGame_Server
{
    public class ServerTimeMathProblemGenerator : BackgroundService
    {
        private static readonly TimeSpan Period = TimeSpan.FromSeconds(10);
        private readonly ILogger<ServerTimeMathProblemGenerator> _logger;
        private readonly IHubContext<GameHub, IGameClient> _context;

        public ServerTimeMathProblemGenerator(ILogger<ServerTimeMathProblemGenerator> logger, IHubContext<GameHub, IGameClient> context)
        {
            _logger = logger;
            _context = context;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            using var timer = new PeriodicTimer(Period);

            while (!stoppingToken.IsCancellationRequested && await timer.WaitForNextTickAsync(stoppingToken))
            {
                var problem = GenerateMathProblem();
                _logger.LogInformation("Executing {Service} {Time}", nameof(ServerTimeMathProblemGenerator), problem);

                await _context.Clients.All.ReceiveMathProblem($"{problem}");
            }
        }

        private string GenerateMathProblem()
        {
            Random rnd = new Random();

            int firstOperand = rnd.Next(1, 10);
            int secondOperand = rnd.Next(1, 10);
            int operation = rnd.Next(1, 4);
            char op = ' ';
            switch (operation)
            {
                case 1:
                    op = '+';
                    break;
                case 2:
                    op = '-';
                    if(firstOperand<secondOperand)
                    {
                        var pom = secondOperand;
                        secondOperand = firstOperand;
                        firstOperand = pom;
                    }
                    break;
                case 3:
                    op = '*';
                    break;
                case 4:
                    op = '/';
                    if (firstOperand < secondOperand)
                    {
                        var pom = secondOperand;
                        secondOperand = firstOperand;
                        firstOperand = pom;
                    }
                    break;
            }
            StringBuilder sb = new StringBuilder();
            sb.Append(firstOperand.ToString());
            sb.Append(op.ToString());
            sb.Append(secondOperand.ToString());

            return sb.ToString();
        }
    }
}
