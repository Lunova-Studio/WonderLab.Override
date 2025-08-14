# 贡献指南

## 构建本项目

### 准备

- dotnet-sdk 8.0
- git

### 快速构建

```bash
# 克隆仓库
git clone https://github.com/Lunova-Studio/WonderLab.Override
cd WonderLab.Override

# 拉取子模块
git submodule init
git submodule update

# 构建 (Release)
dotnet build --configuration Release

# 构建 (Debug)
dotnet build --configuration Debug
```
