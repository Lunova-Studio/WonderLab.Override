using System.Runtime.InteropServices;
using Nuke.Common.Tooling;

public static class HachimiPackagingAppImageSettingsExtensions {
    #region SourceDirectory

    [Builder(
        Type = typeof(PackagingAppImageSettings),
        Property = nameof(PackagingAppImageSettings.SourceDirectory))]
    public static T SetSourceDirectory<T>(this T o, string v) where T : PackagingAppImageSettings => 
        o.Modify(b => b.Set(() => o.SourceDirectory, v));

    [Builder(
        Type = typeof(PackagingAppImageSettings),
        Property = nameof(PackagingAppImageSettings.SourceDirectory))]
    public static T ResetSourceDirectory<T>(this T o) where T : PackagingAppImageSettings => 
        o.Modify(b => b.Remove(() => o.SourceDirectory));

    #endregion
    
    #region OutputFile

    [Builder(
        Type = typeof(PackagingAppImageSettings),
        Property = nameof(PackagingAppImageSettings.OutputFile))]
    public static T SetOutputFile<T>(this T o, string v) where T : PackagingAppImageSettings => 
        o.Modify(b => b.Set(() => o.OutputFile, v));

    [Builder(
        Type = typeof(PackagingAppImageSettings),
        Property = nameof(PackagingAppImageSettings.OutputFile))]
    public static T ResetOutputFile<T>(this T o) where T : PackagingAppImageSettings => 
        o.Modify(b => b.Remove(() => o.OutputFile));

    #endregion

    #region AppName

    [Builder(
        Type = typeof(PackagingAppImageSettings),
        Property = nameof(PackagingAppImageSettings.AppName))]
    public static T SetAppName<T>(this T o, string v) where T : PackagingAppImageSettings => 
        o.Modify(b => b.Set(() => o.AppName, v));

    [Builder(
        Type = typeof(PackagingAppImageSettings),
        Property = nameof(PackagingAppImageSettings.AppName))]
    public static T ResetAppName<T>(this T o) where T : PackagingAppImageSettings => 
        o.Modify(b => b.Remove(() => o.AppName));

    #endregion

    #region DisplayName

    [Builder(
        Type = typeof(PackagingAppImageSettings),
        Property = nameof(PackagingAppImageSettings.DisplayName))]
    public static T SetDisplayName<T>(this T o, string v) where T : PackagingAppImageSettings => 
        o.Modify(b => b.Set(() => o.DisplayName, v));

    [Builder(
        Type = typeof(PackagingAppImageSettings),
        Property = nameof(PackagingAppImageSettings.DisplayName))]
    public static T ResetDisplayName<T>(this T o) where T : PackagingAppImageSettings => 
        o.Modify(b => b.Remove(() => o.DisplayName));

    #endregion

    #region Architecture

    [Builder(
        Type = typeof(PackagingAppImageSettings),
        Property = nameof(PackagingAppImageSettings.Architecture))]
    public static T SetArchitecture<T>(this T o, Architecture v) where T : PackagingAppImageSettings => 
        o.Modify(b => b.Set(() => o.Architecture, v));

    [Builder(
        Type = typeof(PackagingAppImageSettings),
        Property = nameof(PackagingAppImageSettings.Architecture))]
    public static T ResetArchitecture<T>(this T o) where T : PackagingAppImageSettings => 
        o.Modify(b => b.Remove(() => o.Architecture));

    #endregion

    #region Description

    [Builder(
        Type = typeof(PackagingAppImageSettings),
        Property = nameof(PackagingAppImageSettings.Description))]
    public static T SetDescription<T>(this T o, string v) where T : PackagingAppImageSettings => 
        o.Modify(b => b.Set(() => o.Description, v));

    [Builder(
        Type = typeof(PackagingAppImageSettings),
        Property = nameof(PackagingAppImageSettings.Description))]
    public static T ResetDescription<T>(this T o) where T : PackagingAppImageSettings => 
        o.Modify(b => b.Remove(() => o.Description));

    #endregion

    #region Icon

    [Builder(
        Type = typeof(PackagingAppImageSettings),
        Property = nameof(PackagingAppImageSettings.Icon))]
    public static T SetIcon<T>(this T o, string v) where T : PackagingAppImageSettings => 
        o.Modify(b => b.Set(() => o.Icon, v));

    [Builder(
        Type = typeof(PackagingAppImageSettings),
        Property = nameof(PackagingAppImageSettings.Icon))]
    public static T ResetIcon<T>(this T o) where T : PackagingAppImageSettings => 
        o.Modify(b => b.Remove(() => o.Icon));

    #endregion

    #region IsTerminal

    [Builder(
        Type = typeof(PackagingAppImageSettings),
        Property = nameof(PackagingAppImageSettings.IsTerminal))]
    public static T EnableTerminal<T>(this T o) where T : PackagingAppImageSettings => 
        o.Modify(b => b.Set(() => o.IsTerminal, true));

    [Builder(
        Type = typeof(PackagingAppImageSettings),
        Property = nameof(PackagingAppImageSettings.IsTerminal))]
    public static T DisableTerminal<T>(this T o) where T : PackagingAppImageSettings => 
        o.Modify(b => b.Set(() => o.IsTerminal, false));


    #endregion
}