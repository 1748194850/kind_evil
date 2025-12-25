# GitHub 上传指南 - Unity 项目

> 📦 **目标**：正确地将 Unity 项目上传到 GitHub，避免上传不必要的文件

---

## 📋 目录

1. [应该上传的文件](#1-应该上传的文件)
2. [不应该上传的文件](#2-不应该上传的文件)
3. [如何使用 .gitignore](#3-如何使用-gitignore)
4. [上传步骤](#4-上传步骤)
5. [常见问题](#5-常见问题)

---

## 1. 应该上传的文件 ✅

### 1.1 必须上传的核心文件

```
kind_evil/
├── Assets/                    ✅ 必须上传（所有资源文件）
│   ├── Scenes/               ✅ 场景文件
│   ├── Scripts/              ✅ 脚本文件
│   ├── Materials/            ✅ 材质文件
│   ├── Prefabs/              ✅ 预制体
│   ├── Textures/             ✅ 贴图
│   ├── 中文文档/             ✅ 文档
│   └── *.meta                ✅ 必须上传（Unity需要这些文件）
│
├── ProjectSettings/           ✅ 必须上传（项目设置）
│   ├── ProjectVersion.txt    ✅ Unity版本信息
│   ├── InputManager.asset    ✅ 输入设置
│   ├── Physics2DSettings.asset ✅ 物理设置
│   └── ...                    ✅ 其他设置文件
│
├── Packages/                  ✅ 必须上传（包配置）
│   ├── manifest.json         ✅ 包清单
│   └── packages-lock.json    ✅ 包锁定文件
│
└── .gitignore                 ✅ 必须上传（忽略规则）
```

### 1.2 为什么这些文件必须上传？

| 文件/文件夹 | 为什么必须上传 |
|------------|---------------|
| **Assets/** | 包含所有游戏资源（场景、脚本、材质等），这是项目的核心 |
| **Assets/*.meta** | Unity需要这些文件来正确识别和引用资源 |
| **ProjectSettings/** | 包含项目配置（输入、物理、渲染等），其他人需要这些设置才能正确打开项目 |
| **Packages/** | 定义项目使用的Unity包，确保所有人使用相同的包版本 |
| **.gitignore** | 告诉Git哪些文件不应该上传 |

---

## 2. 不应该上传的文件 ❌

### 2.1 自动生成的文件（不要上传）

```
kind_evil/
├── Library/                  ❌ 不要上传（Unity自动生成）
│   ├── Artifacts/            ❌ 编译缓存
│   ├── ScriptAssemblies/     ❌ 编译后的程序集
│   ├── ShaderCache/          ❌ 着色器缓存
│   └── ...                   ❌ 所有内容
│
├── Temp/                     ❌ 不要上传（临时文件）
│   └── ...                   ❌ 所有内容
│
├── Logs/                     ❌ 不要上传（日志文件）
│   └── *.log                 ❌ 所有日志
│
├── UserSettings/             ❌ 不要上传（个人设置）
│   ├── EditorUserSettings.asset ❌ 编辑器个人设置
│   └── Layouts/              ❌ 布局设置
│
├── obj/                      ❌ 不要上传（编译输出）
├── Build/                    ❌ 不要上传（构建输出）
├── Builds/                   ❌ 不要上传（构建输出）
│
├── *.csproj                  ❌ 可选（Visual Studio项目文件）
├── *.sln                     ❌ 可选（Visual Studio解决方案文件）
│
└── .vs/                      ❌ 不要上传（Visual Studio缓存）
```

### 2.2 为什么这些文件不应该上传？

| 文件/文件夹 | 为什么不应该上传 |
|------------|----------------|
| **Library/** | Unity会自动重新生成，文件很大（可能几GB），上传浪费时间 |
| **Temp/** | 临时文件，每次打开Unity都会重新生成 |
| **Logs/** | 日志文件，个人调试信息，对其他人没用 |
| **UserSettings/** | 个人编辑器设置（窗口布局等），每个人不同 |
| **Build/** | 构建输出，文件很大，可以重新构建 |
| ***.csproj, *.sln** | Visual Studio项目文件，Unity会自动生成 |

---

## 3. 如何使用 .gitignore

### 3.1 什么是 .gitignore？

**简单理解**：`.gitignore` 是一个"黑名单"，告诉Git哪些文件不要上传。

**类比**：就像告诉快递员"这些包裹不要送"。

### 3.2 项目中的 .gitignore

我已经为你创建了标准的Unity `.gitignore` 文件，放在项目根目录。

**它会自动忽略**：
- `Library/` 文件夹
- `Temp/` 文件夹
- `Logs/` 文件夹
- `UserSettings/` 文件夹
- `Build/` 文件夹
- `*.csproj` 和 `*.sln` 文件
- 其他临时和缓存文件

### 3.3 验证 .gitignore 是否生效

在Git中，你可以使用以下命令查看哪些文件会被忽略：

```bash
git status --ignored
```

---

## 4. 上传步骤

### 4.1 第一次上传到GitHub

#### 步骤1：初始化Git仓库

```bash
# 在项目根目录（kind_evil文件夹）打开终端/命令行
cd D:\Users\Desktop\mygame\project\kind_evil

# 初始化Git仓库
git init
```

#### 步骤2：添加 .gitignore

```bash
# 确保 .gitignore 文件存在（我已经为你创建了）
# 如果没有，创建它（参考上面的内容）
```

#### 步骤3：添加文件到Git

```bash
# 添加所有文件（.gitignore会自动排除不需要的文件）
git add .

# 或者只添加特定文件
git add Assets/
git add ProjectSettings/
git add Packages/
git add .gitignore
```

#### 步骤4：提交

```bash
# 创建第一次提交
git commit -m "Initial commit: Unity project setup"
```

#### 步骤5：在GitHub创建仓库

1. 登录 GitHub
2. 点击右上角 "+" → "New repository"
3. 填写仓库名称（例如：`kind_evil`）
4. **不要**勾选 "Initialize this repository with a README"（因为本地已有文件）
5. 点击 "Create repository"

#### 步骤6：连接并推送

```bash
# 添加远程仓库（替换 YOUR_USERNAME 和 REPO_NAME）
git remote add origin https://github.com/YOUR_USERNAME/REPO_NAME.git

# 推送代码
git branch -M main
git push -u origin main
```

### 4.2 后续更新

```bash
# 1. 查看更改
git status

# 2. 添加更改的文件
git add .

# 3. 提交
git commit -m "描述你的更改"

# 4. 推送到GitHub
git push
```

---

## 5. 常见问题

### Q1: 我已经上传了 Library/ 文件夹，怎么办？

**A**: 可以删除它：

```bash
# 1. 从Git中删除（但保留本地文件）
git rm -r --cached Library/

# 2. 提交删除
git commit -m "Remove Library folder from Git"

# 3. 推送到GitHub
git push
```

**注意**：`Library/` 文件夹会保留在你的电脑上，只是不再被Git跟踪。

### Q2: .meta 文件需要上传吗？

**A**: **必须上传！**

`.meta` 文件是Unity用来识别和引用资源的。如果不上传：
- 其他人打开项目时，资源引用会丢失
- 预制体、材质等会显示为"Missing"
- 场景中的对象引用会断开

### Q3: 可以上传 .csproj 和 .sln 文件吗？

**A**: **建议不上传**，但也可以上传。

**不上传的理由**：
- Unity会自动生成这些文件
- 不同版本的Visual Studio可能生成不同格式
- 文件可能包含个人路径信息

**上传的理由**：
- 如果团队都使用Visual Studio，可以方便打开项目
- 可以保留一些项目配置

**建议**：如果只是个人项目或小团队，可以上传。如果是公开项目，建议不上传。

### Q4: 如何检查哪些文件会被上传？

**A**: 使用以下命令：

```bash
# 查看会被添加的文件
git status

# 查看会被忽略的文件
git status --ignored

# 查看详细的文件列表
git ls-files
```

### Q5: 项目文件太大，上传很慢怎么办？

**A**: 检查是否有大文件：

```bash
# 查看大文件（可能需要安装git-lfs）
find . -type f -size +10M -not -path "./Library/*" -not -path "./Temp/*"
```

**解决方案**：
1. 使用 Git LFS（Large File Storage）上传大文件
2. 将大文件放在外部存储（如云盘），在README中说明
3. 压缩资源文件

### Q6: 可以上传中文文档吗？

**A**: **完全可以！**

中文文档（`Assets/中文文档/`）应该上传，因为：
- 它们是项目的一部分
- 帮助其他开发者理解项目
- 文件很小，不会影响上传速度

---

## 6. 推荐的文件结构

上传后，GitHub仓库应该看起来像这样：

```
kind_evil/
├── .gitignore                 ✅
├── Assets/                    ✅
│   ├── Scenes/                ✅
│   ├── Scripts/               ✅
│   ├── 中文文档/              ✅
│   └── ...                    ✅
├── ProjectSettings/            ✅
├── Packages/                   ✅
│   ├── manifest.json          ✅
│   └── packages-lock.json     ✅
└── README.md                   ✅（建议添加）
```

**不应该看到**：
- `Library/` 文件夹
- `Temp/` 文件夹
- `Logs/` 文件夹
- `UserSettings/` 文件夹

---

## 7. 最佳实践

### ✅ 推荐做法

1. **使用 .gitignore**：确保自动忽略不需要的文件
2. **定期提交**：经常提交更改，写清晰的提交信息
3. **添加 README.md**：在项目根目录添加README，说明项目
4. **使用分支**：为不同功能创建分支
5. **检查文件大小**：上传前检查是否有意外的大文件

### ❌ 避免的做法

1. **不要上传 Library/**：文件太大，浪费时间
2. **不要上传个人设置**：UserSettings 是个人偏好
3. **不要上传构建输出**：Build 文件夹可以重新构建
4. **不要忽略 .meta 文件**：会导致资源引用丢失
5. **不要提交敏感信息**：API密钥、密码等

---

## 8. 快速检查清单

上传前，检查以下内容：

- [ ] `.gitignore` 文件已创建
- [ ] `Library/` 文件夹在 .gitignore 中
- [ ] `Temp/` 文件夹在 .gitignore 中
- [ ] `Logs/` 文件夹在 .gitignore 中
- [ ] `UserSettings/` 文件夹在 .gitignore 中
- [ ] `Assets/` 文件夹会被上传
- [ ] `ProjectSettings/` 文件夹会被上传
- [ ] `Packages/` 文件夹会被上传
- [ ] 所有 `.meta` 文件会被上传
- [ ] 没有敏感信息（API密钥、密码等）

---

## 📝 总结

**应该上传**：
- ✅ `Assets/`（包括所有 `.meta` 文件）
- ✅ `ProjectSettings/`
- ✅ `Packages/`
- ✅ `.gitignore`

**不应该上传**：
- ❌ `Library/`
- ❌ `Temp/`
- ❌ `Logs/`
- ❌ `UserSettings/`
- ❌ `Build/` 和 `Builds/`

**可选**：
- ⚠️ `*.csproj` 和 `*.sln`（建议不上传）

使用我创建的 `.gitignore` 文件，Git会自动处理这些规则，你只需要正常使用 `git add .` 和 `git commit` 即可！

---

**祝你上传顺利！** 🚀

