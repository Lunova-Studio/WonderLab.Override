using Nuke.Common.Tooling;

public static class HachimiPackagingPortableSettingsExtensions {
    #region OutputFile
    
    [Builder(
        Type = typeof(PackagingPortableSettings),
        Property = nameof(PackagingPortableSettings.OutputFile))]
    public static T SetOutputFile<T>(this T o, string v) where T : PackagingPortableSettings => 
        o.Modify(b => b.Set(() => o.OutputFile, v));

    [Builder(
        Type = typeof(PackagingPortableSettings),
        Property = nameof(PackagingPortableSettings.OutputFile))]
    public static T ResetOutputFile<T>(this T o) where T : PackagingPortableSettings => 
        o.Modify(b => b.Remove(() => o.OutputFile));
    
    #endregion

    #region SourceDirectory

    [Builder(
        Type = typeof(PackagingPortableSettings),
        Property = nameof(PackagingPortableSettings.SourceDirectory))]
    public static T SetSourceDirectory<T>(this T o, string v) where T : PackagingPortableSettings => 
        o.Modify(b => b.Set(() => o.SourceDirectory, v));

    [Builder(
        Type = typeof(PackagingPortableSettings),
        Property = nameof(PackagingPortableSettings.SourceDirectory))]
    public static T ResetSourceDirectory<T>(this T o) where T : PackagingPortableSettings => 
        o.Modify(b => b.Remove(() => o.SourceDirectory));

    #endregion

    #region AppName

    [Builder(
        Type = typeof(PackagingPortableSettings),
        Property = nameof(PackagingPortableSettings.AppName))]
    public static T SetAppName<T>(this T o, string v) where T : PackagingPortableSettings => 
        o.Modify(b => b.Set(() => o.AppName, v));

    [Builder(
        Type = typeof(PackagingPortableSettings),
        Property = nameof(PackagingPortableSettings.AppName))]
    public static T ResetAppName<T>(this T o) where T : PackagingPortableSettings => 
        o.Modify(b => b.Remove(() => o.AppName));

    #endregion

    #region Runtime

    [Builder(
        Type = typeof(PackagingPortableSettings),
        Property = nameof(PackagingPortableSettings.Runtime))]
    public static T SetRuntime<T>(this T o, string v) where T : PackagingPortableSettings => 
        o.Modify(b => b.Set(() => o.Runtime, v));

    [Builder(
        Type = typeof(PackagingPortableSettings),
        Property = nameof(PackagingPortableSettings.Runtime))]
    public static T ResetRuntime<T>(this T o) where T : PackagingPortableSettings => 
        o.Modify(b => b.Remove(() => o.Runtime));

    #endregion
}