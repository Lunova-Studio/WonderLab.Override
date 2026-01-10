using Avalonia.Media.Imaging;
using System;
using WonderLab.Extensions;

namespace WonderLab.Services.UI;

public sealed class ThemeService {
    public static readonly Lazy<Bitmap> Minecraft = new("avares://WonderLab/Assets/Images/minecraft.png".ToBitmap);
    public static readonly Lazy<Bitmap> LoadingIcon = new("avares://WonderLab/Assets/Images/doro_loading.jpg".ToBitmap);
    public static readonly Lazy<Bitmap> PCLIcon = new("avares://WonderLab/Assets/Images/Icons/launcher_PCL2.png".ToBitmap);
    public static readonly Lazy<Bitmap> HMCLIcon = new("avares://WonderLab/Assets/Images/Icons/launcher_HMCL.ico".ToBitmap);
    public static readonly Lazy<Bitmap> LoaderQuiltIcon = new("avares://WonderLab/Assets/Images/Icons/loader_quilt.png".ToBitmap);
    public static readonly Lazy<Bitmap> LoaderForgeIcon = new("avares://WonderLab/Assets/Images/Icons/loader_forge.png".ToBitmap);
    public static readonly Lazy<Bitmap> LoaderFabricIcon = new("avares://WonderLab/Assets/Images/Icons/loader_fabric.png".ToBitmap);
    public static readonly Lazy<Bitmap> LoaderOptifineIcon = new("avares://WonderLab/Assets/Images/Icons/loader_optifine.png".ToBitmap);
    public static readonly Lazy<Bitmap> LoaderNeoforgeIcon = new("avares://WonderLab/Assets/Images/Icons/loader_neoforge.png".ToBitmap);
    public static readonly Lazy<Bitmap> OldMinecraftIcon = new("avares://WonderLab/Assets/Images/Icons/old_minecraft.png".ToBitmap);
    public static readonly Lazy<Bitmap> LoaderMinecraftIcon = new("avares://WonderLab/Assets/Images/Icons/loader_minecraft.png".ToBitmap);
    public static readonly Lazy<Bitmap> ReleaseMinecraftIcon = new("avares://WonderLab/Assets/Images/Icons/release_minecraft.png".ToBitmap);
    public static readonly Lazy<Bitmap> SnapshotMinecraftIcon = new("avares://WonderLab/Assets/Images/Icons/snapshot_minecraft.png".ToBitmap);
}