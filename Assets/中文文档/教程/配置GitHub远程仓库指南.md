# 配置GitHub远程仓库指南

> 🔗 **目标**：将本地Git仓库连接到GitHub远程仓库并推送代码

---

## 📋 目录

1. [准备工作](#准备工作)
2. [方法一：在GitHub上创建新仓库](#方法一在github上创建新仓库)
3. [方法二：使用已存在的仓库](#方法二使用已存在的仓库)
4. [连接并推送代码](#连接并推送代码)
5. [验证配置](#验证配置)
6. [常见问题](#常见问题)

---

## 准备工作

### 1. 确认本地仓库状态

在配置远程仓库之前，先确认本地仓库已经准备好：

```bash
# 检查当前状态
git status

# 查看提交历史
git log --oneline
```

**应该看到**：
- ✅ 至少有一个提交（我们已经创建了初始提交）
- ✅ 工作区干净（没有未提交的更改）

### 2. 确认Git用户信息

```bash
# 查看Git配置的用户名
git config --global user.name

# 查看Git配置的邮箱
git config --global user.email
```

如果还没有配置，需要先设置：

```bash
# 设置用户名（替换为你的GitHub用户名）
git config --global user.name "你的GitHub用户名"

# 设置邮箱（替换为你的GitHub邮箱）
git config --global user.email "你的邮箱@example.com"
```

---

## 方法一：在GitHub上创建新仓库

### 步骤1：登录GitHub

1. 打开浏览器，访问 [GitHub.com](https://github.com)
2. 登录你的账号

### 步骤2：创建新仓库

1. 点击右上角的 **"+"** 按钮
2. 选择 **"New repository"（新建仓库）**

### 步骤3：填写仓库信息

在创建仓库页面填写：

- **Repository name（仓库名称）**：
  - 例如：`kind_evil`
  - 只能包含字母、数字、连字符(-)和下划线(_)
  
- **Description（描述）**（可选）：
  - 例如：`Unity 2D游戏项目`
  
- **Visibility（可见性）**：
  - **Public（公开）**：所有人都能看到
  - **Private（私有）**：只有你能看到（需要付费账号才能创建私有仓库）
  
- **重要**：**不要**勾选以下选项：
  - ❌ "Add a README file"（添加README文件）
  - ❌ "Add .gitignore"（添加.gitignore）
  - ❌ "Choose a license"（选择许可证）
  
  **原因**：本地已经有这些文件了，如果勾选会产生冲突。

### 步骤4：创建仓库

点击 **"Create repository"（创建仓库）** 按钮

### 步骤5：复制仓库URL

创建成功后，GitHub会显示仓库页面。你会看到类似这样的URL：

```
https://github.com/你的用户名/kind_evil.git
```

**复制这个URL**，稍后会用到。

---

## 方法二：使用已存在的仓库

如果你已经在GitHub上创建了仓库，直接使用仓库的URL即可。

### 获取仓库URL

1. 进入你的GitHub仓库页面
2. 点击绿色的 **"Code"** 按钮
3. 选择 **"HTTPS"** 标签
4. 复制显示的URL，例如：
   ```
   https://github.com/你的用户名/仓库名.git
   ```

---

## 连接并推送代码

### 步骤1：添加远程仓库

在项目根目录打开终端/命令行，运行：

```bash
# 替换 YOUR_USERNAME 和 REPO_NAME 为你的实际信息
git remote add origin https://github.com/YOUR_USERNAME/REPO_NAME.git
```

**示例**：
```bash
git remote add origin https://github.com/cheese/kind_evil.git
```

### 步骤2：验证远程仓库配置

```bash
# 查看远程仓库配置
git remote -v
```

**应该看到**：
```
origin  https://github.com/你的用户名/仓库名.git (fetch)
origin  https://github.com/你的用户名/仓库名.git (push)
```

### 步骤3：重命名分支（如果需要）

如果你的分支不是 `main`，需要重命名：

```bash
# 查看当前分支
git branch

# 如果当前分支是 master，重命名为 main
git branch -M main
```

### 步骤4：推送到GitHub

```bash
# 第一次推送，设置上游分支
git push -u origin main
```

**如果遇到认证问题**：

GitHub现在要求使用个人访问令牌（Personal Access Token）而不是密码。

#### 创建个人访问令牌：

1. 登录GitHub
2. 点击右上角头像 → **"Settings"（设置）**
3. 左侧菜单选择 **"Developer settings"（开发者设置）**
4. 选择 **"Personal access tokens" → "Tokens (classic)"**
5. 点击 **"Generate new token" → "Generate new token (classic)"**
6. 填写信息：
   - **Note（备注）**：例如 "本地Git推送"
   - **Expiration（过期时间）**：选择合适的时间
   - **Scopes（权限）**：勾选 **`repo`**（完整仓库权限）
7. 点击 **"Generate token"（生成令牌）**
8. **重要**：复制生成的令牌（只显示一次！）

#### 使用令牌推送：

当Git要求输入密码时，**输入个人访问令牌**而不是GitHub密码。

---

## 验证配置

### 检查推送结果

1. 刷新GitHub仓库页面
2. 你应该能看到所有文件已经上传

### 验证远程连接

```bash
# 查看远程仓库信息
git remote -v

# 查看远程分支
git branch -r

# 查看所有分支（本地+远程）
git branch -a
```

---

## 常见问题

### Q1: 提示 "remote origin already exists"

**原因**：已经配置过远程仓库了。

**解决方案**：

```bash
# 查看现有配置
git remote -v

# 如果需要更换远程仓库URL
git remote set-url origin https://github.com/新用户名/新仓库名.git

# 或者删除后重新添加
git remote remove origin
git remote add origin https://github.com/你的用户名/仓库名.git
```

### Q2: 提示 "Authentication failed" 或 "Permission denied"

**原因**：认证失败，可能是：
- 使用了错误的密码
- 需要使用个人访问令牌而不是密码
- 令牌权限不足

**解决方案**：
1. 确认使用个人访问令牌而不是密码
2. 确认令牌有 `repo` 权限
3. 如果使用Windows，可以配置Git凭据管理器

### Q3: 提示 "failed to push some refs"

**原因**：远程仓库有本地没有的提交（例如创建仓库时添加了README）。

**解决方案**：

```bash
# 方法1：先拉取远程更改，然后合并
git pull origin main --allow-unrelated-histories
git push -u origin main

# 方法2：强制推送（谨慎使用，会覆盖远程更改）
git push -u origin main --force
```

### Q4: 如何更改远程仓库URL？

```bash
# 查看当前URL
git remote -v

# 更改URL
git remote set-url origin https://github.com/新用户名/新仓库名.git

# 验证更改
git remote -v
```

### Q5: 如何删除远程仓库配置？

```bash
# 删除远程仓库
git remote remove origin

# 验证已删除
git remote -v
```

---

## 📝 完整操作示例

假设你的GitHub用户名是 `cheese`，仓库名是 `kind_evil`：

```bash
# 1. 添加远程仓库
git remote add origin https://github.com/cheese/kind_evil.git

# 2. 验证配置
git remote -v

# 3. 确保分支名为main
git branch -M main

# 4. 推送到GitHub
git push -u origin main
```

---

## 🎯 快速检查清单

配置远程仓库前：
- [ ] 本地仓库已初始化（`git init`）
- [ ] 至少有一个提交（`git commit`）
- [ ] Git用户信息已配置（`git config user.name` 和 `user.email`）
- [ ] 在GitHub上创建了仓库（或已有仓库URL）

配置远程仓库：
- [ ] 添加远程仓库：`git remote add origin <URL>`
- [ ] 验证配置：`git remote -v`
- [ ] 推送代码：`git push -u origin main`

---

## 📝 总结

**基本流程**：
1. 在GitHub上创建仓库（或使用已有仓库）
2. 复制仓库URL
3. 添加远程仓库：`git remote add origin <URL>`
4. 推送代码：`git push -u origin main`

**重要提示**：
- 使用个人访问令牌而不是密码进行认证
- 确保令牌有 `repo` 权限
- 第一次推送使用 `-u` 参数设置上游分支

---

**配置完成后，你的代码就成功上传到GitHub了！** 🎉


