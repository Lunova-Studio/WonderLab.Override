using System.Threading.Tasks;
using WonderLab.Classes.Models;

namespace WonderLab.Classes.Interfaces;

/// <summary>
/// 第三方启动器设置导入器接口
/// </summary>
public interface ISettingImporter {
    string Type { get; }

    /// <summary>
    /// 启动器路径
    /// </summary>
    string LauncherPath { get; set; }

    /// <summary>
    /// 导入设置
    /// </summary>
    Task<SettingModel> ImportAsync();
}