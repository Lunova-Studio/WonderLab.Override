; *** Inno Setup version 6.1.0+ 繁體中文訊息 ***
;
; 欲下載使用者貢獻的此檔案翻譯，請造訪：
;   https://jrsoftware.org/files/istrans/
;
; 注意：翻譯此文字時，請勿在原本無句號的
; 訊息尾端加入句號（。），因為 Inno Setup
; 會自動為此類訊息加上句號，重複加入將導致
; 畫面顯示兩個句號。
;
; 維護者：楊正瀚
; 電子郵件：847320916@QQ.com
; 翻譯基於網路資源
; 最新翻譯請見：https://github.com/kira-96/Inno-Setup-Chinese-Simplified-Translation
;

[LangOptions]
; 以下三個項目極為重要，請務必閱讀並
; 理解說明檔中「[LangOptions] 區段」的主題。
LanguageName=繁體中文
; 若語言名稱顯示錯誤，請取消下一行的註解
; LanguageName=<7E41><4F53><4E2D><6587>
; 關於 LanguageID，請參考連結：
; https://docs.microsoft.com/en-us/openspecs/windows_protocols/ms-lcid/a9eac961-e77d-41a6-90a5-ce1a8b0cdb9c
LanguageID=$0404
LanguageCodePage=950
; 若您所翻譯的語言需要特殊字型或
; 字號，請取消下列任一項目的註解並對應修改。
DialogFontName=微軟正黑體
;DialogFontSize=8
WelcomeFontName=微軟正黑體
;WelcomeFontSize=12
TitleFontName=微軟正黑體
;TitleFontSize=29
;CopyrightFontName=Arial
;CopyrightFontSize=8

[Messages]

; *** 應用程式標題
SetupAppTitle=安裝程式
SetupWindowTitle=安裝精靈 - %1
UninstallAppTitle=解除安裝
UninstallAppFullTitle=%1 解除安裝程式

; *** 一般常用
InformationTitle=資訊
ConfirmTitle=確認
ErrorTitle=錯誤

; *** SetupLdr 訊息
SetupLdrStartupMessage=即將安裝 %1，是否繼續？
LdrCannotCreateTemp=無法建立暫存檔，安裝作業已中止。
LdrCannotExecTemp=無法執行暫存目錄中的檔案，安裝作業已中止。
HelpTextNote=

; *** 啟動錯誤訊息
LastErrorMessage=%1.%n%n錯誤 %2：%3
SetupFileMissing=安裝目錄中的檔案 %1 遺失，請修正此問題或取得程式的新版本。
SetupFileCorrupt=安裝檔已損毀，請取得程式的新版本。
SetupFileCorruptOrWrongVer=安裝檔已損毀，或與此安裝程式的版本不相容，請修正此問題或取得程式的新版本。
InvalidParameter=無效的命令列參數：%n%n%1
SetupAlreadyRunning=安裝程式正在執行中。
WindowsVersionNotSupported=此程式不支援目前電腦執行的 Windows 版本。
WindowsServicePackRequired=此程式需要 %1 Service Pack %2 或更新版本。
NotOnThisPlatform=此程式無法在 %1 上執行。
OnlyOnThisPlatform=此程式僅能在 %1 上執行。
OnlyOnTheseArchitectures=此程式僅能安裝在下列處理器架構的 Windows 版本中：%n%n%1
WinVersionTooLowError=此程式需要 %1 版本 %2 或更新版本。
WinVersionTooHighError=此程式無法安裝在 %1 版本 %2 或更新版本中。
AdminPrivilegesRequired=安裝此程式必須以系統管理員身分登入。
PowerUserPrivilegesRequired=安裝此程式必須以系統管理員或具備權限的使用者群組身分登入。
SetupAppRunningError=安裝程式偵測到 %1 目前正在執行。%n%n請先關閉所有相關視窗，再按「確定」繼續，或按「取消」結束。
UninstallAppRunningError=解除安裝程式偵測到 %1 目前正在執行。%n%n請先關閉所有相關視窗，再按「確定」繼續，或按「取消」結束。

; *** 啟動相關問題
PrivilegesRequiredOverrideTitle=選擇安裝程式模式
PrivilegesRequiredOverrideInstruction=選擇安裝模式
PrivilegesRequiredOverrideText1=%1 可安裝給所有使用者使用（需要系統管理員權限），或僅安裝給目前使用者。
PrivilegesRequiredOverrideText2=%1 僅能安裝給目前使用者，或安裝給所有使用者（需要系統管理員權限）。
PrivilegesRequiredOverrideAllUsers=安裝給所有使用者(&A)
PrivilegesRequiredOverrideAllUsersRecommended=安裝給所有使用者(&A)（建議選項）
PrivilegesRequiredOverrideCurrentUser=僅安裝給我(&M)
PrivilegesRequiredOverrideCurrentUserRecommended=僅安裝給我(&M)（建議選項）

; *** 其他錯誤
ErrorCreatingDir=安裝程式無法建立目錄「%1」。
ErrorTooManyFilesInDir=無法在目錄「%1」中建立檔案，因該目錄內的檔案過多。

; *** 安裝程式共用訊息
ExitSetupTitle=結束安裝程式
ExitSetupMessage=安裝程式尚未完成安裝，若此時結束，程式將無法安裝。%n%n您可稍後重新執行安裝程式以完成安裝。%n%n是否立即結束安裝程式？
AboutSetupMenuItem=關於安裝程式(&A)...
AboutSetupTitle=關於安裝程式
AboutSetupMessage=%1 版本 %2%n%3%n%n%1 官方網站：%n%4
AboutSetupNote=
TranslatorNote=翻譯：PandamanAF

; *** 按鈕
ButtonBack=< 上一步(&B)
ButtonNext=下一步(&N) >
ButtonInstall=安裝(&I)
ButtonOK=確定
ButtonCancel=取消
ButtonYes=是(&Y)
ButtonYesToAll=全部皆是(&A)
ButtonNo=否(&N)
ButtonNoToAll=全部皆否(&O)
ButtonFinish=完成(&F)
ButtonBrowse=瀏覽(&B)...
ButtonWizardBrowse=瀏覽(&R)...
ButtonNewFolder=新增資料夾(&M)

; *** 「選擇語言」對話框訊息
SelectLanguageTitle=選擇安裝語言
SelectLanguageLabel=選擇安裝時要使用的語言。

; *** 共用精靈文字
ClickNext=按「下一步」繼續，或按「取消」結束安裝程式。
BeveledLabel=
BrowseDialogTitle=瀏覽資料夾
BrowseDialogLabel=請在下方清單中選擇資料夾，再按「確定」。
NewFolderName=新增資料夾

; *** 「歡迎」精靈頁
WelcomeLabel1=歡迎使用 [name] 安裝精靈
WelcomeLabel2=即將在您的電腦中安裝 [name/ver]。%n%n建議您在繼續安裝前關閉所有其他應用程式。

; *** 「密碼」精靈頁
WizardPassword=密碼
PasswordLabel1=此安裝程式受密碼保護。
PasswordLabel3=請輸入密碼，再按「下一步」繼續，密碼區分大小寫。
PasswordEditLabel=密碼(&P)：
IncorrectPassword=您輸入的密碼錯誤，請重新嘗試。

; *** 「授權合約」精靈頁
WizardLicense=授權合約
LicenseLabel=請在繼續安裝前閱讀下列重要資訊。
LicenseLabel3=請仔細閱讀下列授權合約，您必須同意合約條款才能繼續安裝。
LicenseAccepted=我同意此合約(&A)
LicenseNotAccepted=我拒絕此合約(&D)

; *** 「資訊」精靈頁
WizardInfoBefore=資訊
InfoBeforeLabel=請在繼續安裝前閱讀下列重要資訊。
InfoBeforeClickLabel=若要繼續安裝，請按「下一步」。
WizardInfoAfter=資訊
InfoAfterLabel=請在繼續安裝前閱讀下列重要資訊。
InfoAfterClickLabel=若要繼續安裝，請按「下一步」。

; *** 「使用者資訊」精靈頁
WizardUserInfo=使用者資訊
UserInfoDesc=請輸入您的相關資訊。
UserInfoName=使用者名稱(&U)：
UserInfoOrg=組織(&O)：
UserInfoSerial=序號(&S)：
UserInfoNameRequired=您必須輸入使用者名稱。

; *** 「選擇目標目錄」精靈頁
WizardSelectDir=選擇目標位置
SelectDirDesc=您要將 [name] 安裝至何處？
SelectDirLabel3=安裝程式將把 [name] 安裝至下列資料夾。
SelectDirBrowseLabel=按「下一步」繼續，若要選擇其他資料夾，請按「瀏覽」。
DiskSpaceGBLabel=至少需要 [gb] GB 的可用磁碟空間。
DiskSpaceMBLabel=至少需要 [mb] MB 的可用磁碟空間。
CannotInstallToNetworkDrive=安裝程式無法安裝至網路磁碟機。
CannotInstallToUNCPath=安裝程式無法安裝至 UNC 路徑。
InvalidPath=您必須輸入含磁碟機代號的完整路徑，例如：%n%nC:\APP%n%n或下列格式的 UNC 路徑：%n%n\\server\share
InvalidDrive=您選取的磁碟機或 UNC 共用資料夾不存在或無法存取，請選擇其他位置。
DiskSpaceWarningTitle=磁碟空間不足
DiskSpaceWarning=安裝程式至少需要 %1 KB 的可用空間才能安裝，但您選取的磁碟機僅有 %2 KB 的可用空間。%n%n是否仍要繼續？
DirNameTooLong=資料夾名稱或路徑過長。
InvalidDirName=無效的資料夾名稱。
BadDirName32=資料夾名稱不得包含下列任何字元：%n%n%1
DirExistsTitle=資料夾已存在
DirExists=資料夾：%n%n%1%n%n已存在，是否仍要安裝至此資料夾？
DirDoesntExistTitle=資料夾不存在
DirDoesntExist=資料夾：%n%n%1%n%n不存在，是否要建立此資料夾？

; *** 「選擇元件」精靈頁
WizardSelectComponents=選擇元件
SelectComponentsDesc=您要安裝此程式的哪些元件？
SelectComponentsLabel2=勾選您要安裝的元件，取消勾選您不要安裝的元件，再按「下一步」繼續。
FullInstallation=完整安裝
; 若可能，請勿將「Compact」翻譯為「最小安裝」
CompactInstallation=精簡安裝
CustomInstallation=自訂安裝
NoUninstallWarningTitle=元件已存在
NoUninstallWarning=安裝程式偵測到下列元件已安裝在您的電腦中：%n%n%1%n%n取消勾選這些元件將無法解除其安裝。%n%n是否仍要繼續？
ComponentSize1=%1 KB
ComponentSize2=%1 MB
ComponentsDiskSpaceGBLabel=目前選取的元件至少需要 [gb] GB 的磁碟空間。
ComponentsDiskSpaceMBLabel=目前選取的元件至少需要 [mb] MB 的磁碟空間。

; *** 「選擇額外工作」精靈頁
WizardSelectTasks=選擇額外工作
SelectTasksDesc=您要讓安裝程式執行哪些額外工作？
SelectTasksLabel2=勾選您要讓安裝程式在安裝 [name] 時執行的額外工作，再按「下一步」。

; *** 「選擇開始功能表資料夾」精靈頁
WizardSelectProgramGroup=選擇開始功能表資料夾
SelectStartMenuFolderDesc=安裝程式應將程式的捷徑放置於何處？
SelectStartMenuFolderLabel3=安裝程式即將在下列開始功能表資料夾中建立程式的捷徑。
SelectStartMenuFolderBrowseLabel=按「下一步」繼續，若要選擇其他資料夾，請按「瀏覽」。
MustEnterGroupName=您必須輸入資料夾名稱。
GroupNameTooLong=資料夾名稱或路徑過長。
InvalidGroupName=無效的資料夾名稱。
BadGroupName=資料夾名稱不得包含下列任何字元：%n%n%1
NoProgramGroupCheck2=不建立開始功能表資料夾(&D)

; *** 「準備安裝」精靈頁
WizardReady=準備安裝
ReadyLabel1=安裝程式已準備好在您的電腦中安裝 [name]。
ReadyLabel2a=按「安裝」繼續此安裝程式，若要檢閱或修改設定，請按「上一步」。
ReadyLabel2b=按「安裝」繼續此安裝程式？
ReadyMemoUserInfo=使用者資訊：
ReadyMemoDir=目標位置：
ReadyMemoType=安裝類型：
ReadyMemoComponents=已選取元件：
ReadyMemoGroup=開始功能表資料夾：
ReadyMemoTasks=額外工作：

; *** TDownloadWizardPage 精靈頁與 DownloadTemporaryFile
DownloadingLabel=正在下載額外檔案...
ButtonStopDownload=停止下載(&S)
StopDownload=您確定要停止下載嗎？
ErrorDownloadAborted=下載已中止
ErrorDownloadFailed=下載失敗：%1 %2
ErrorDownloadSizeFailed=取得下載檔案大小失敗：%1 %2
ErrorFileHash1=驗證檔案雜湊值失敗：%1
ErrorFileHash2=無效的檔案雜湊值：預期為 %1，實際為 %2
ErrorProgress=無效的進度：%1，總共%2
ErrorFileSize=檔案大小錯誤：預期為 %1，實際為 %2

; *** TExtractionWizardPage 介面與 Extract7ZipArchive
ExtractionLabel=正在解壓其他檔案...
ButtonStopExtraction=&停止解壓
StopExtraction=您確定要停止解壓嗎？
ErrorExtractionAborted=解壓已中止
ErrorExtractionFailed=解壓失敗：%1

; *** 「正在準備安裝」精靈頁
WizardPreparing=正在準備安裝
PreparingDesc=安裝程式正在準備在您的電腦中安裝 [name]。
PreviousInstallNotCompleted=先前的程式安裝/解除安裝尚未完成，您需要重新啟動電腦才能完成安裝。%n%n重新啟動電腦後，請再次執行安裝程式以完成 [name] 的安裝。
CannotContinue=安裝程式無法繼續，請按「取消」結束。
ApplicationsFound=下列應用程式正在使用需要更新的檔案，建議您允許安裝程式自動關閉這些應用程式。
ApplicationsFound2=下列應用程式正在使用需要更新的檔案，建議您允許安裝程式自動關閉這些應用程式，安裝完成後將嘗試重新啟動這些應用程式。
CloseApplications=自動關閉這些應用程式(&A)
DontCloseApplications=不關閉這些應用程式(&D)
ErrorCloseApplications=安裝程式無法自動關閉所有應用程式，建議您在繼續前，關閉所有使用安裝程式更新檔案的應用程式。
PrepareToInstallNeedsRestart=安裝程式必須重新啟動電腦，重新啟動後請再次執行安裝程式以完成 [name] 的安裝。%n%n是否立即重新啟動？

; *** 「正在安裝」精靈頁
WizardInstalling=正在安裝
InstallingLabel=安裝程式正在您的電腦中安裝 [name]，請稍候。

; *** 「安裝完成」精靈頁
FinishedHeadingLabel=[name] 安裝完成
FinishedLabelNoIcons=安裝程式已在您的電腦中安裝 [name]。
FinishedLabel=安裝程式已在您的電腦中安裝 [name]，您可透過安裝的捷徑執行此應用程式。
ClickFinish=按「完成」結束安裝程式。
FinishedRestartLabel=要完成 [name] 的安裝，安裝程式必須重新啟動您的電腦，是否立即重新啟動？
FinishedRestartMessage=要完成 [name] 的安裝，安裝程式必須重新啟動您的電腦。%n%n是否立即重新啟動？
ShowReadmeCheck=是，我要檢視自述檔
YesRadio=是，立即重新啟動電腦(&Y)
NoRadio=否，稍後再重新啟動電腦(&N)
; 範例用法：「執行 MyProg.exe」
RunEntryExec=執行 %1
; 範例用法：「檢視 Readme.txt」
RunEntryShellExec=檢視 %1

; *** 「安裝程式需要下一張磁片」提示
ChangeDiskTitle=安裝程式需要下一張磁片
SelectDiskLabel2=請插入磁片 %1 並按「確定」。%n%n若此磁片中的檔案可在下列資料夾以外的位置找到，請輸入正確路徑或按「瀏覽」。
PathLabel=路徑(&P)：
FileNotInDir2=無法在「%2」中找到檔案「%1」，請插入正確的磁片或選擇其他資料夾。
SelectDirectoryLabel=請指定下一張磁片的位置。

; *** 安裝狀態訊息
SetupAborted=安裝程式尚未完成安裝。%n%n請修正問題並重新執行安裝程式。
AbortRetryIgnoreSelectAction=選擇動作
AbortRetryIgnoreRetry=重試(&T)
AbortRetryIgnoreIgnore=忽略錯誤並繼續(&I)
AbortRetryIgnoreCancel=關閉安裝程式

; *** 安裝狀態訊息
StatusClosingApplications=正在關閉應用程式...
StatusCreateDirs=正在建立目錄...
StatusExtractFiles=正在解壓縮檔案...
StatusCreateIcons=正在建立捷徑...
StatusCreateIniEntries=正在建立 INI 項目...
StatusCreateRegistryEntries=正在建立登錄檔項目...
StatusRegisterFiles=正在註冊檔案...
StatusSavingUninstall=正在儲存解除安裝資訊...
StatusRunProgram=正在完成安裝...
StatusRestartingApplications=正在重新啟動應用程式...
StatusRollback=正在復原變更...

; *** 其他錯誤
ErrorInternal2=內部錯誤：%1
ErrorFunctionFailedNoCode=%1 執行失敗
ErrorFunctionFailed=%1 執行失敗；錯誤代碼 %2
ErrorFunctionFailedWithMessage=%1 執行失敗；錯誤代碼 %2.%n%3
ErrorExecutingProgram=無法執行檔案：%n%1

; *** 登錄檔錯誤
ErrorRegOpenKey=開啟登錄檔機碼時發生錯誤：%n%1\%2
ErrorRegCreateKey=建立登錄檔機碼時發生錯誤：%n%1\%2
ErrorRegWriteKey=寫入登錄檔機碼時發生錯誤：%n%1\%2

; *** INI 錯誤
ErrorIniEntry=在檔案「%1」中建立 INI 項目時發生錯誤。

; *** 檔案複製錯誤
FileAbortRetryIgnoreSkipNotRecommended=略過此檔案(&S)（不建議）
FileAbortRetryIgnoreIgnoreNotRecommended=忽略錯誤並繼續(&I)（不建議）
SourceIsCorrupted=來源檔案已損毀
SourceDoesntExist=來源檔案「%1」不存在
ExistingFileReadOnly2=無法取代現有檔案，因其為唯讀屬性。
ExistingFileReadOnlyRetry=移除唯讀屬性並重試(&R)
ExistingFileReadOnlyKeepExisting=保留現有檔案(&K)
ErrorReadingExistingDest=嘗試讀取現有檔案時發生錯誤：
FileExistsSelectAction=選擇動作
FileExists2=檔案已存在。
FileExistsOverwriteExisting=覆寫現有檔案(&O)
FileExistsKeepExisting=保留現有檔案(&K)
FileExistsOverwriteOrKeepAll=對所有衝突檔案執行此動作(&D)
ExistingFileNewerSelectAction=選擇動作
ExistingFileNewer2=現有檔案的版本比安裝程式將要安裝的版本更新。
ExistingFileNewerOverwriteExisting=覆寫現有檔案(&O)
ExistingFileNewerKeepExisting=保留現有檔案(&K)（建議）
ExistingFileNewerOverwriteOrKeepAll=對所有衝突檔案執行此動作(&D)
ErrorChangingAttr=嘗試變更下列現有檔案的屬性時發生錯誤：
ErrorCreatingTemp=嘗試在目標目錄建立檔案時發生錯誤：
ErrorReadingSource=嘗試讀取下列來源檔案時發生錯誤：
ErrorCopying=嘗試複製下列檔案時發生錯誤：
ErrorReplacingExistingFile=嘗試取代現有檔案時發生錯誤：
ErrorRestartReplace=重新啟動後取代檔案失敗：
ErrorRenamingTemp=嘗試重新命名下列目標目錄中的檔案時發生錯誤：
ErrorRegisterServer=無法註冊 DLL/OCX：%1
ErrorRegSvr32Failed=RegSvr32 執行失敗；結束代碼 %1
ErrorRegisterTypeLib=無法註冊類型程式庫：%1

; *** 解除安裝顯示名稱標記
; 範例用法：「我的程式 (32 位元)」
UninstallDisplayNameMark=%1 (%2)
; 範例用法：「我的程式 (32 位元，所有使用者)」
UninstallDisplayNameMarks=%1 (%2, %3)
UninstallDisplayNameMark32Bit=32 位元
UninstallDisplayNameMark64Bit=64 位元
UninstallDisplayNameMarkAllUsers=所有使用者
UninstallDisplayNameMarkCurrentUser=目前使用者

; *** 安裝後錯誤
ErrorOpeningReadme=嘗試開啟自述檔時發生錯誤。
ErrorRestartingComputer=精靈無法自動重新啟動電腦，請手動重新啟動。

; *** 解除安裝訊息
UninstallNotFound=「%1」不存在，無法解除安裝。
UninstallOpenError=無法開啟「%1」，無法解除安裝。
UninstallUnsupportedVer=此版本的解除安裝程式無法解析解除安裝記錄檔「%1」的格式，無法解除安裝。
UninstallUnknownEntry=在解除安裝記錄檔中偵測到未知項目 (%1)
ConfirmUninstall=您確定要完全移除 %1 及其所有元件嗎？
UninstallOnlyOnWin64=此解除安裝程式僅能在 64 位元 Windows 中執行。
OnlyAdminCanUninstall=此安裝的程式僅能由具備系統管理員權限的使用者解除安裝。
UninstallStatusLabel=正在從您的電腦移除 %1，請稍候。
UninstalledAll=%1 已成功從您的電腦中移除。
UninstalledMost=%1 解除安裝完成。%n%n部分項目無法刪除，您可手動移除。
UninstalledAndNeedsRestart=要完成 %1 的解除安裝，必須重新啟動您的電腦。%n%n是否立即重新啟動電腦？
UninstallDataCorrupted=檔案「%1」已損毀，無法解除安裝。

; *** 解除安裝狀態訊息
ConfirmDeleteSharedFileTitle=是否刪除共用檔案？
ConfirmDeleteSharedFile2=系統中的下列共用檔案已無其他程式使用，您要讓解除安裝程式刪除這些檔案嗎？%n%n若刪除後仍有程式使用這些檔案，該程式可能無法正常執行。若您不確定，請選擇「否」，將這些檔案保留在系統中以避免問題。
SharedFileNameLabel=檔案名稱：
SharedFileLocationLabel=所在位置：
WizardUninstalling=正在解除安裝
StatusUninstalling=正在解除安裝 %1...

; *** 關機封鎖原因
ShutdownBlockReasonInstallingApp=正在安裝 %1。
ShutdownBlockReasonUninstallingApp=正在解除安裝 %1。

; 以下自訂訊息並非由安裝程式本身使用，但若您在
; 指令碼中使用這些訊息，請務必進行翻譯。

[CustomMessages]

NameAndVersion=%1 版本 %2
AdditionalIcons=額外捷徑：
CreateDesktopIcon=建立桌面捷徑(&D)
CreateQuickLaunchIcon=建立快速啟動列捷徑(&Q)
ProgramOnTheWeb=%1 官方網站
UninstallProgram=解除安裝 %1
LaunchProgram=啟動 %1
AssocFileExtension=將 %2 檔案副檔名與 %1 建立關聯(&A)
AssocingFileExtension=正在將 %2 檔案副檔名與 %1 建立關聯...
AutoStartProgramGroupDescription=啟動群組：
AutoStartProgram=自動啟動 %1
AddonHostProgramNotFound=%1 無法找到您所選擇的資料夾。%n%n是否仍要繼續？
