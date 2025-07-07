using MinecraftLaunch.Base.Models.Authentication;
using MinecraftLaunch.Base.Models.Authentication.Microsoft;
using MinecraftLaunch.Components.Authenticator;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace WonderLab.Services.Authentication;

public sealed class AuthenticationService {
    private const string CLIENT_ID = "9fd44410-8ed7-4eb3-a160-9f1cc62c824c";

    private YggdrasilAuthenticator _yggdrasilAuthenticator;
    private readonly OfflineAuthenticator _offlineAuthenticator = new();
    private readonly MicrosoftAuthenticator _microsoftAuthenticator = new(CLIENT_ID);

    public OfflineAccount CreateOfflineAccount(string name, Guid uuid = default) {
        return _offlineAuthenticator.Authenticate(name, Guid.NewGuid());
    }

    public Task<IEnumerable<YggdrasilAccount>> LoginYggdrasilAccountsAsync(string email, string password, string url, CancellationToken cancellationToken = default) {
        _yggdrasilAuthenticator = new YggdrasilAuthenticator(url, email, password);
        return _yggdrasilAuthenticator.AuthenticateAsync(cancellationToken);
    }

    public async Task<MicrosoftAccount> LoginMicrosoftAccountAsync(Action<DeviceCodeResponse> action, CancellationToken cancellationToken = default) {
        var resultOAuth2 = await _microsoftAuthenticator.DeviceFlowAuthAsync(action, cancellationToken);
        return await _microsoftAuthenticator.AuthenticateAsync(resultOAuth2, cancellationToken);
    }

    public async Task<MicrosoftAccount> RefreshMicrosoftAccountAsync(MicrosoftAccount account, CancellationToken cancellationToken = default) {
        return await _microsoftAuthenticator.RefreshAsync(account, cancellationToken);
    }

    public async Task<YggdrasilAccount> RefreshYggdrasilAccountAsync(YggdrasilAccount account, CancellationToken cancellationToken = default) {
        _yggdrasilAuthenticator ??= 
            new(account.YggdrasilServerUrl, string.Empty, string.Empty); //刷新不需要邮箱及密码

        return await _yggdrasilAuthenticator.RefreshAsync(account, cancellationToken);
    }
}