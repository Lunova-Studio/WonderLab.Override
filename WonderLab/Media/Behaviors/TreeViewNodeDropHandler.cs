using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.VisualTree;
using Avalonia.Xaml.Interactions.DragAndDrop;
using MinecraftLaunch.Components.Parser;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using WonderLab.Models;
using WonderLab.ViewModels.Pages;

namespace WonderLab.Media.Behaviors;

public sealed class TreeViewNodeDropHandler : BaseTreeViewDropHandler {
    protected override (bool Valid, bool WillSourceItemBeMovedToDifferentParent) Validate(
        TreeView treeView,
        DragEventArgs e,
        object sourceContext,
        object targetContext,
        bool execute) {
        // 校验源和目标
        if (sourceContext is not ITreeViewNode sourceNode
            || treeView.GetVisualAt(e.GetPosition(treeView)) is not Control targetControl
            || targetControl.DataContext is not ITreeViewNode targetNode 
            || sourceNode == targetNode) {
            return (false, false);
        }
        var areSourceNodesDifferentThanTargetNodes = sourceNode != targetNode;

        // 一级节点不能拖动
        if (sourceNode is FavoritesModel) {
            return (false, false);
        }

        // 只能拖动二级节点
        if (sourceNode is not MinecraftModel sourceB) {
            return (false, false);
        }

        // 目标必须是父节点 A
        if (targetNode is not FavoritesModel targetA) {
            return (false, false);
        }

        // 找到源父节点（通过 Id 匹配 Title）
        if(targetContext is not MinecraftPageViewModel vm)
            return (false, false);

        var sourceParent = vm.Minecrafts.FirstOrDefault(a => a.Title == sourceB.FavoritesId);
        if (sourceParent == null) {
            return (false, false);
        }

        // 禁止同父节点内移动
        if (sourceParent == targetA) {
            return (false, false);
        }

        switch (e.DragEffects) {
            case DragDropEffects.Move:
                if (execute) {
                    sourceParent.Favorites.Remove(sourceB);
                    sourceB.FavoritesId = targetA.Title;
                    targetA.Favorites.Add(sourceB);
                    var sorted = targetA.Favorites.OrderBy(n => n.FavoritesId).ToList();
                    targetA.Favorites.Clear();
                    foreach (var node in sorted)
                        targetA.Favorites.Add(node);

                    _ = MinecraftParser.DataProcessors["Favorites"].SaveAsync();
                }

                return (true, areSourceNodesDifferentThanTargetNodes);
        }

        return (false, false);
    }
}
