# GitHub 删除项目指南

> 🗑️ **目标**：删除GitHub上的项目，或清理已上传的不必要文件

---

## 📋 目录

1. [方法一：完全删除GitHub仓库](#方法一完全删除github仓库)
2. [方法二：清理已上传的文件（保留仓库）](#方法二清理已上传的文件保留仓库)
3. [重新正确上传项目](#重新正确上传项目)

---

## 方法一：完全删除GitHub仓库

如果你想完全删除GitHub上的项目，可以按照以下步骤操作：

### 步骤1：登录GitHub

1. 打开浏览器，访问 [GitHub.com](https://github.com)
2. 登录你的账号

### 步骤2：进入仓库设置

1. 进入你的项目仓库页面
2. 点击仓库页面顶部的 **"Settings"（设置）** 标签

### 步骤3：删除仓库

1. 在设置页面，向下滚动到最底部
2. 找到 **"Danger Zone"（危险区域）** 部分
3. 点击 **"Delete this repository"（删除此仓库）** 按钮
4. 在弹出的对话框中：
   - 输入仓库名称以确认删除（例如：`kind_evil`）
   - 点击 **"I understand the consequences, delete this repository"（我了解后果，删除此仓库）**

### 步骤4：确认删除

- GitHub会要求你再次确认
- 确认后，仓库将被永久删除

**⚠️ 警告**：删除后无法恢复！请确保你真的想删除这个仓库。

---

## 方法二：清理已上传的文件（保留仓库）

如果你只是想清理已上传的不必要文件（如 `Library/`、`Temp/` 等），但保留仓库，可以按照以下步骤操作：

### 步骤1：检查已上传的文件

在项目根目录打开终端/命令行，运行：

```bash
# 查看Git跟踪的所有文件
git ls-files
```

### 步骤2：从Git中删除不需要的文件

```bash
# 从Git中删除Library文件夹（但保留本地文件）
git rm -r --cached Library/

# 从Git中删除Temp文件夹
git rm -r --cached Temp/

# 从Git中删除Logs文件夹
git rm -r --cached Logs/

# 从Git中删除UserSettings文件夹
git rm -r --cached UserSettings/

# 从Git中删除Build文件夹（如果有）
git rm -r --cached Build/
git rm -r --cached Builds/

# 从Git中删除.csproj和.sln文件（如果有）
git rm --cached *.csproj
git rm --cached *.sln
```

### 步骤3：确保.gitignore正确配置

检查 `.gitignore` 文件是否包含这些规则（应该已经有了）：

```bash
# 查看.gitignore内容
cat .gitignore
```

如果 `.gitignore` 文件正确，这些文件夹应该已经被忽略了。

### 步骤4：提交更改

```bash
# 提交删除操作
git commit -m "Remove unnecessary files (Library, Temp, Logs, etc.)"
```

### 步骤5：推送到GitHub

```bash
# 推送到GitHub（这会从远程仓库删除这些文件）
git push origin main
```

或者如果你的主分支是 `master`：

```bash
git push origin master
```

### 步骤6：清理Git历史（可选，但推荐）

如果你想让仓库更干净，可以清理Git历史记录。**注意**：这会重写历史，如果其他人也在使用这个仓库，需要谨慎操作。

```bash
# 使用git filter-branch清理历史（不推荐，复杂）
# 或者使用BFG Repo-Cleaner（更简单）

# 更简单的方法：创建一个新的初始提交
# 1. 删除.git文件夹（备份重要数据！）
# 2. 重新初始化Git仓库
# 3. 重新添加文件并提交
```

**更安全的方法**：如果你只是想清理当前的文件，步骤1-5就足够了。Git历史中的大文件虽然还在，但不会影响仓库的日常使用。

---

## 重新正确上传项目

删除或清理后，如果你想重新正确上传项目，可以按照以下步骤：

### 步骤1：检查.gitignore

确保 `.gitignore` 文件存在且正确配置（应该已经有了）。

### 步骤2：检查要上传的文件

```bash
# 查看哪些文件会被添加
git status
```

**应该看到**：
- ✅ `Assets/` 文件夹
- ✅ `ProjectSettings/` 文件夹
- ✅ `Packages/` 文件夹
- ✅ `.gitignore` 文件

**不应该看到**：
- ❌ `Library/` 文件夹
- ❌ `Temp/` 文件夹
- ❌ `Logs/` 文件夹
- ❌ `UserSettings/` 文件夹

### 步骤3：添加文件

```bash
# 添加所有应该上传的文件
git add .

# 或者只添加特定文件夹
git add Assets/
git add ProjectSettings/
git add Packages/
git add .gitignore
```

### 步骤4：提交

```bash
git commit -m "Initial commit: Unity project (cleaned)"
```

### 步骤5：推送到GitHub

如果仓库已删除，需要重新创建：

1. 在GitHub上创建新仓库
2. 连接并推送：

```bash
# 添加远程仓库（替换 YOUR_USERNAME 和 REPO_NAME）
git remote add origin https://github.com/YOUR_USERNAME/REPO_NAME.git

# 推送代码
git branch -M main
git push -u origin main
```

如果仓库还存在（只是清理了文件）：

```bash
# 直接推送
git push origin main
```

---

## 📊 文件大小对比

### 上传前应该检查

```bash
# 查看项目总大小（不包括Library等）
du -sh Assets/ ProjectSettings/ Packages/

# 查看Library文件夹大小（不应该上传）
du -sh Library/
```

**典型情况**：
- `Assets/` + `ProjectSettings/` + `Packages/`：通常几十MB到几百MB
- `Library/`：可能几GB（这就是为什么不应该上传）

---

## ⚠️ 重要提示

1. **删除仓库前**：确保你已经备份了重要数据
2. **清理文件前**：确保 `.gitignore` 正确配置，避免再次上传
3. **推送前**：使用 `git status` 检查要上传的文件
4. **大文件**：如果项目中有大文件（>100MB），考虑使用 Git LFS

---

## 🎯 快速操作清单

### 完全删除仓库
- [ ] 登录GitHub
- [ ] 进入仓库设置
- [ ] 找到"危险区域"
- [ ] 输入仓库名称确认删除
- [ ] 确认删除操作

### 清理文件（保留仓库）
- [ ] 检查已上传的文件：`git ls-files`
- [ ] 从Git删除不需要的文件：`git rm -r --cached Library/` 等
- [ ] 确认 `.gitignore` 正确
- [ ] 提交更改：`git commit -m "Remove unnecessary files"`
- [ ] 推送到GitHub：`git push origin main`

### 重新上传
- [ ] 检查 `.gitignore` 文件
- [ ] 检查要上传的文件：`git status`
- [ ] 添加文件：`git add .`
- [ ] 提交：`git commit -m "Initial commit"`
- [ ] 推送：`git push origin main`

---

## 📝 总结

**完全删除仓库**：
- 适合：想彻底删除项目，或重新开始
- 操作：GitHub网页 → Settings → Delete repository

**清理文件（保留仓库）**：
- 适合：想保留仓库但清理不必要的文件
- 操作：使用 `git rm -r --cached` 删除文件，然后提交推送

**推荐做法**：
1. 先清理文件（方法二），看看效果
2. 如果还是不满意，再完全删除（方法一）
3. 重新创建仓库并正确上传

---

**祝你操作顺利！** 🚀

