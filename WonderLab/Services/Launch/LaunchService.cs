using WonderLab.Services.Account;
using WonderLab.Services.UI;

namespace WonderLab.Services.Launch;

public sealed class LaunchService {
    private readonly TaskService _taskService;
    private readonly GameService _gameService;
    private readonly ConfigService _configService;
    private readonly AccountService _accountService;
    private readonly NotificationService _notificationService;

    public LaunchService(
        GameService gameService,
        TaskService taskService,
        AccountService accountService,
        ConfigService configService,
        NotificationService notificationService) {
        _taskService = taskService;
        _gameService = gameService;
        _configService = configService;
        _accountService = accountService;
        _notificationService = notificationService;
    }


}