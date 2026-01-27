# GitHub 使用完整指南

> 📦 **目标**：完整的 GitHub 使用指南，包括上传、配置、清理和删除

---

## 📋 目录

1. [快速开始](#快速开始)
2. [上传项目到 GitHub](#上传项目到github)
3. [配置远程仓库](#配置远程仓库)
4. [清理已上传的文件](#清理已上传的文件)
5. [删除 GitHub 仓库](#删除-github-仓库)
6. [常见问题](#常见问题)

---

## 快速开始

### 第一次使用 GitHub？

1. **创建 GitHub 账号**：访问 [GitHub.com](https://github.com) 注册
2. **安装 Git**：下载并安装 [Git](https://git-scm.com/)
3. **配置 Git**：
   ```bash
   git config --global user.name "你的GitHub用户名"
   git config --global user.email "你的邮箱@example.com"
   ```

---

## 上传项目到 GitHub

### 应该上传的文件 ✅

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
├── Packages/                  ✅ 必须上传（包配置）
└── .gitignore                 ✅ 必须上传（忽略规则）
```

### 不应该上传的文件 ❌

```
kind_evil/
├── Library/                  ❌ 不要上传（Unity自动生成，几GB大小）
├── Temp/                     ❌ 不要上传（临时文件）
├── Logs/                     ❌ 不要上传（日志文件）
├── UserSettings/             ❌ 不要上传（个人设置）
├── Build/                    ❌ 不要上传（构建输出）
└── *.csproj, *.sln           ❌ 可选（Visual Studio项目文件）
```

### 上传步骤

#### 步骤 1：初始化 Git 仓库

```bash
# 在项目根目录打开终端
cd D:\Users\Desktop\mygame\project\kind_evil

# 初始化Git仓库
git init
```

#### 步骤 2：添加文件

```bash
# 添加所有文件（.gitignore会自动排除不需要的文件）
git add .

# 或者只添加特定文件
git add Assets/
git add ProjectSettings/
git add Packages/
git add .gitignore
```

#### 步骤 3：提交

```bash
git commit -m "Initial commit: Unity project setup"
```

#### 步骤 4：在 GitHub 创建仓库

1. 登录 GitHub
2. 点击右上角 "+" → "New repository"
3. 填写仓库名称（例如：`kind_evil`）
4. **不要**勾选 "Initialize this repository with a README"
5. 点击 "Create repository"

#### 步骤 5：连接并推送

```bash
# 添加远程仓库（替换 YOUR_USERNAME 和 REPO_NAME）
git remote add origin https://github.com/YOUR_USERNAME/REPO_NAME.git

# 推送代码
git branch -M main
git push -u origin main
```

**注意**：GitHub 现在要求使用个人访问令牌（Personal Access Token）而不是密码。创建令牌：
1. GitHub → Settings → Developer settings → Personal access tokens → Tokens (classic)
2. Generate new token → 勾选 `repo` 权限
3. 复制令牌，推送时作为密码使用

---

## 配置远程仓库

### 方法一：在 GitHub 上创建新仓库

1. 登录 GitHub
2. 点击右上角 "+" → "New repository"
3. 填写仓库信息：
   - **Repository name**：例如 `kind_evil`
   - **Visibility**：Public 或 Private
   - **不要勾选**：Add a README、Add .gitignore、Choose a license
4. 点击 "Create repository"
5. 复制仓库 URL

### 方法二：使用已存在的仓库

1. 进入 GitHub 仓库页面
2. 点击 "Code" 按钮
3. 选择 "HTTPS" 标签
4. 复制 URL

### 连接本地仓库

```bash
# 添加远程仓库
git remote add origin https://github.com/YOUR_USERNAME/REPO_NAME.git

# 验证配置
git remote -v

# 重命名分支（如果需要）
git branch -M main

# 推送代码
git push -u origin main
```

### 常见问题

**Q: 提示 "remote origin already exists"**
```bash
# 查看现有配置
git remote -v

# 更换远程仓库URL
git remote set-url origin https://github.com/新用户名/新仓库名.git

# 或删除后重新添加
git remote remove origin
git remote add origin https://github.com/你的用户名/仓库名.git
```

**Q: 提示 "Authentication failed"**
- 确认使用个人访问令牌而不是密码
- 确认令牌有 `repo` 权限

---

## 清理已上传的文件

### 情况 1：还没有推送到 GitHub（推荐）

直接提交并推送即可，`.gitignore` 会自动排除不需要的文件。

### 情况 2：已经推送了不必要的文件

#### 步骤 1：从 Git 中删除文件

```bash
# 从Git中删除这些文件夹（但保留本地文件）
git rm -r --cached Library/
git rm -r --cached Temp/
git rm -r --cached Logs/
git rm -r --cached UserSettings/

# 删除不需要的文件
git rm --cached *.csproj
git rm --cached *.sln
```

#### 步骤 2：提交删除操作

```bash
git commit -m "Remove unnecessary files (Library, Temp, Logs, etc.)"
```

#### 步骤 3：推送到 GitHub

```bash
git push origin main
```

**注意**：这会从 GitHub 删除这些文件，但本地文件会保留。

### 验证清理结果

```bash
# 查看会被跟踪的文件
git ls-files

# 查看会被忽略的文件
git status --ignored
```

**应该看到**：
- ✅ `Assets/` 下的所有文件
- ✅ `ProjectSettings/` 下的所有文件
- ✅ `Packages/` 下的所有文件

**不应该看到**：
- ❌ `Library/`
- ❌ `Temp/`
- ❌ `Logs/`
- ❌ `UserSettings/`

---

## 删除 GitHub 仓库

### 方法一：完全删除仓库

1. 登录 GitHub
2. 进入仓库页面
3. 点击 "Settings"（设置）标签
4. 向下滚动到 "Danger Zone"（危险区域）
5. 点击 "Delete this repository"
6. 输入仓库名称确认删除
7. 点击 "I understand the consequences, delete this repository"

**⚠️ 警告**：删除后无法恢复！请确保你真的想删除这个仓库。

### 方法二：清理文件（保留仓库）

参考 [清理已上传的文件](#清理已上传的文件) 部分。

---

## 常见问题

### Q1: .meta 文件需要上传吗？

**A**: **必须上传！**

`.meta` 文件是 Unity 用来识别和引用资源的。如果不上传：
- 其他人打开项目时，资源引用会丢失
- 预制体、材质等会显示为 "Missing"
- 场景中的对象引用会断开

### Q2: 可以上传 .csproj 和 .sln 文件吗？

**A**: **建议不上传**，但也可以上传。

**不上传的理由**：
- Unity 会自动生成这些文件
- 不同版本的 Visual Studio 可能生成不同格式
- 文件可能包含个人路径信息

**上传的理由**：
- 如果团队都使用 Visual Studio，可以方便打开项目
- 可以保留一些项目配置

### Q3: 如何检查哪些文件会被上传？

```bash
# 查看会被添加的文件
git status

# 查看会被忽略的文件
git status --ignored

# 查看详细的文件列表
git ls-files
```

### Q4: 项目文件太大，上传很慢怎么办？

**解决方案**：
1. 使用 Git LFS（Large File Storage）上传大文件
2. 将大文件放在外部存储（如云盘），在 README 中说明
3. 压缩资源文件

### Q5: 提示 "failed to push some refs"

**原因**：远程仓库有本地没有的提交（例如创建仓库时添加了 README）。

**解决方案**：
```bash
# 方法1：先拉取远程更改，然后合并
git pull origin main --allow-unrelated-histories
git push -u origin main

# 方法2：强制推送（谨慎使用，会覆盖远程更改）
git push -u origin main --force
```

---

## 📝 快速检查清单

### 上传前
- [ ] `.gitignore` 文件已创建
- [ ] `Library/` 文件夹在 .gitignore 中
- [ ] `Temp/` 文件夹在 .gitignore 中
- [ ] `Assets/` 文件夹会被上传
- [ ] `ProjectSettings/` 文件夹会被上传
- [ ] `Packages/` 文件夹会被上传
- [ ] 所有 `.meta` 文件会被上传

### 配置远程仓库
- [ ] 在 GitHub 上创建了仓库（或已有仓库 URL）
- [ ] 添加了远程仓库：`git remote add origin <URL>`
- [ ] 验证了配置：`git remote -v`
- [ ] 推送了代码：`git push -u origin main`

---

## 📊 文件大小对比

### 清理前（包含 Library）
- 总大小：可能几 GB（主要是 Library 文件夹）
- 上传时间：非常慢（可能需要几小时）

### 清理后（只包含必要文件）
- 总大小：通常几十 MB 到几百 MB
- 上传时间：几分钟

---

## ⚠️ 重要提示

1. **Library 文件夹**：删除后，Unity 会在下次打开项目时自动重新生成，不影响项目功能
2. **本地文件**：使用 `git rm --cached` 只会从 Git 中删除，本地文件会保留
3. **.gitignore**：确保 `.gitignore` 文件已正确配置，避免再次上传这些文件
4. **个人访问令牌**：GitHub 现在要求使用令牌而不是密码进行认证

---

**祝你使用 GitHub 顺利！** 🚀
