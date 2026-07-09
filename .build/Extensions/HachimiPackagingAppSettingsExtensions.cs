using Nuke.Common.Tooling;

public static class HachimiPackagingAppSettingsExtensions {
    #region OutputFile

    [Builder(
        Type = typeof(PackagingAppBundleSettings),
        Property = nameof(PackagingAppBundleSettings.OutputFile))]
    public static T SetOutputFile<T>(this T o, string v) where T : PackagingAppBundleSettings => 
        o.Modify(b => b.Set(() => o.OutputFile, v));

    [Builder(
        Type = typeof(PackagingAppBundleSettings),
        Property = nameof(PackagingAppBundleSettings.OutputFile))]
    public static T ResetOutputFile<T>(this T o) where T : PackagingAppBundleSettings => 
        o.Modify(b => b.Remove(() => o.OutputFile));

    #endregion

    #region SourceDirectory

    [Builder(
        Type = typeof(PackagingAppBundleSettings),
        Property = nameof(PackagingAppBundleSettings.SourceDirectory))]
    public static T SetSourceDirectory<T>(this T o, string v) where T : PackagingAppBundleSettings => 
        o.Modify(b => b.Set(() => o.SourceDirectory, v));

    [Builder(
        Type = typeof(PackagingAppBundleSettings),
        Property = nameof(PackagingAppBundleSettings.SourceDirectory))]
    public static T ResetSourceDirectory<T>(this T o) where T : PackagingAppBundleSettings => 
        o.Modify(b => b.Remove(() => o.SourceDirectory));

    #endregion

    #region AppName

    [Builder(
        Type = typeof(PackagingAppBundleSettings),
        Property = nameof(PackagingAppBundleSettings.AppName))]
    public static T SetAppName<T>(this T o, string v) where T : PackagingAppBundleSettings => 
        o.Modify(b => b.Set(() => o.AppName, v));

    [Builder(
        Type = typeof(PackagingAppBundleSettings),
        Property = nameof(PackagingAppBundleSettings.AppName))]
    public static T ResetAppName<T>(this T o) where T : PackagingAppBundleSettings => 
        o.Modify(b => b.Remove(() => o.AppName));

    #endregion

    #region Icon

    [Builder(
        Type = typeof(PackagingAppBundleSettings),
        Property = nameof(PackagingAppBundleSettings.Icon))]
    public static T SetIcon<T>(this T o, string v) where T : PackagingAppBundleSettings => 
        o.Modify(b => b.Set(() => o.Icon, v));

    [Builder(
        Type = typeof(PackagingAppBundleSettings),
        Property = nameof(PackagingAppBundleSettings.Icon))]
    public static T ResetIcon<T>(this T o) where T : PackagingAppBundleSettings => 
        o.Modify(b => b.Remove(() => o.Icon));

    #endregion

    #region Version

    [Builder(
        Type = typeof(PackagingAppBundleSettings),
        Property = nameof(PackagingAppBundleSettings.Version))]
    public static T SetVersion<T>(this T o, string v) where T : PackagingAppBundleSettings => 
        o.Modify(b => b.Set(() => o.Version, v));

    [Builder(
        Type = typeof(PackagingAppBundleSettings),
        Property = nameof(PackagingAppBundleSettings.Version))]
    public static T ResetVersion<T>(this T o) where T : PackagingAppBundleSettings => 
        o.Modify(b => b.Remove(() => o.Version));

    #endregion

    #region Identifier

    [Builder(
        Type = typeof(PackagingAppBundleSettings),
        Property = nameof(PackagingAppBundleSettings.Identifier))]
    public static T SetIdentifier<T>(this T o, string v) where T : PackagingAppBundleSettings => 
        o.Modify(b => b.Set(() => o.Identifier, v));

    [Builder(
        Type = typeof(PackagingAppBundleSettings),
        Property = nameof(PackagingAppBundleSettings.Identifier))]
    public static T ResetIdentifier<T>(this T o) where T : PackagingAppBundleSettings => 
        o.Modify(b => b.Remove(() => o.Identifier));

    #endregion

    #region DisplayName

    [Builder(
        Type = typeof(PackagingAppBundleSettings),
        Property = nameof(PackagingAppBundleSettings.DisplayName))]
    public static T SetDisplayName<T>(this T o, string v) where T : PackagingAppBundleSettings => 
        o.Modify(b => b.Set(() => o.DisplayName, v));

    [Builder(
        Type = typeof(PackagingAppBundleSettings),
        Property = nameof(PackagingAppBundleSettings.DisplayName))]
    public static T ResetDisplayName<T>(this T o) where T : PackagingAppBundleSettings => 
        o.Modify(b => b.Remove(() => o.DisplayName));

    #endregion

    #region PrincipalClass

    [Builder(
        Type = typeof(PackagingAppBundleSettings),
        Property = nameof(PackagingAppBundleSettings.PrincipalClass))]
    public static T SetPrincipalClass<T>(this T o, string v) where T : PackagingAppBundleSettings => 
        o.Modify(b => b.Set(() => o.PrincipalClass, v));

    [Builder(
        Type = typeof(PackagingAppBundleSettings),
        Property = nameof(PackagingAppBundleSettings.PrincipalClass))]
    public static T ResetPrincipalClass<T>(this T o) where T : PackagingAppBundleSettings => 
        o.Modify(b => b.Remove(() => o.PrincipalClass));

    #endregion

    #region IsHighResolutionCapable
    
    [Builder(
        Type = typeof(PackagingAppBundleSettings),
        Property = nameof(PackagingAppBundleSettings.IsHighResolutionCapable))]
    public static T EnableHighResolutionCapable<T>(this T o) where T : PackagingAppBundleSettings => 
        o.Modify(b => b.Set(() => o.IsHighResolutionCapable, true));

    [Builder(
        Type = typeof(PackagingAppBundleSettings),
        Property = nameof(PackagingAppBundleSettings.IsHighResolutionCapable))]
    public static T DisableHighResolutionCapable<T>(this T o) where T : PackagingAppBundleSettings => 
        o.Modify(b => b.Set(() => o.IsHighResolutionCapable, false));
    
    #endregion
}